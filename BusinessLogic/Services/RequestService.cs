using BusinessLogic.Exceptions;
using BusinessLogic.Notification;
using BusinessLogic.Services.Intarfaces;
using DataAccess.Context.Enum;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessLogic.Services
{
    public class RequestService : IRequestService
    {
        IRepository<Request, int> _repository;
        IUserService _userService;
        IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public RequestService(IRepository<Request, int> repository, IUserService userService, IMediator mediator, UserManager<User> userManager)
        {
            _repository = repository;
            _userService = userService;
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<Request> AddAsync(Request obj)
        {
            if (await IsRequestCorrect(obj))
            {
                //IDs of the reviewers come from the UI, the collection of reviewers needs to be filled
                FillReviewers(obj);

                //when creating an request, the default status is new
                obj.State = VacationState.New;
                await _repository.CreateAsync(obj);

                //for creating email
                var notification = new RequestCreatedNotification { Request = obj };
                await _mediator.Publish(notification);

                return obj;
            }
            else
                return null;
        }

        public async Task<IReadOnlyCollection<Request>> GetAllAsync(string userId, DateTime? start = null, DateTime? end = null, int? stateId = null, int? typeId = null)
       {

            //user can get all requests or filter
            Expression<Func<Request, bool>> condition = request =>
                (request.UserId == userId)
                && (start == null || request.StartDate.Date >= start)
                && (end == null || request.EndDate.Date <= end)
                && (stateId == null || (int)request.State == stateId)
                && (typeId == null || (int)request.Type == typeId);

            return await _repository.FilterAsync(condition);

        }
        public async Task<Request> GetByIdAsync(int requestId)
        {
            return await _repository.FindAsync(x => x.Id == requestId);
        }
        public async Task<Request> GetByIdAsync(string uderId, int requestId)
        {
            return await _repository.FindAsync(x => x.UserId == uderId && x.Id == requestId);
        }

        public async Task UpdateAsync(int requestId, Request newModel)
        {
            var requestFromDb = await _repository.FindAsync(r => r.Id == requestId); 
            
            if (requestFromDb == null)
                throw new RequestNotFoundException($"Request not found: RequestId={requestId}", 409);

            if (newModel.UserId != requestFromDb.UserId)      
                throw new ConflictException($"Current user is not the author of the request (userId: {newModel.UserId}, requestId: {requestId})", 409);

            bool needToNotify = false;

            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                switch (requestFromDb.State)
                {
                    case VacationState.New:
                        //newModel.ParentRequestId = requestFromDb.Id;
                        if (await IsRequestCorrect(newModel))
                        {
                            newModel.ParentRequestId = null;
                            newModel.State = VacationState.New;
                            FillReviewers(newModel);
                            needToNotify = true;
                        }
                        break;

                    case VacationState.InProgress:
                        needToNotify = await ChangeAsync(requestFromDb, newModel);
                        break;

                    case VacationState.Approved:
                        await Duplicate(requestFromDb, newModel);
                        break;

                    case VacationState.Rejected:
                        throw new StateException("It is forbidden to change the rejected request", 409);

                    default:
                        throw new StateException("Request status is not allowed or does not exist", 409);
                }

                await _repository.UpdateAsync(requestFromDb);
                transactionScope.Complete();
            }

            if (needToNotify)
            {
                var notification = new RequestUpdatedNotification { Request = requestFromDb };
                await _mediator.Publish(notification);
            }
        }

        public async Task DeleteAsync(int requestId)
        {
            if (await _repository.FindAsync(x => x.Id == requestId) != null)
                await _repository.DeleteAsync(requestId);
        }

        public async Task RejectedByOwnerAsync(string userId, int requestId)
        {
            var requestFromDb = await _repository.FindAsync(x => x.Id == requestId);

            if (requestFromDb == null)
                throw new RequestNotFoundException($"Request not found: RequestId={requestId}", 409);
            if (requestFromDb.UserId != userId)
                throw new ConflictException($"Current user is not the author of the request (userId: {userId}, requestId: {requestId})", 409);

            switch (requestFromDb.State)
            {
                //if new - can be deleted
                case VacationState.New:
                    await _repository.DeleteAsync(requestId);
                    break;
                //otherwise - status Rejected
                case VacationState.InProgress:
                case VacationState.Approved:
                    if (requestFromDb.EndDate <= DateTime.Now.Date)
                        throw new ConflictException("End date of the request must be later than the current date", 409);

                    requestFromDb.State = VacationState.Rejected;
                    requestFromDb.ModifiedByUserId = requestFromDb.UserId;
                    await _repository.UpdateAsync(requestFromDb);

                    await _mediator.Publish(new RequestRejectedNotification { Request = requestFromDb });
                    await _mediator.Publish(new StatisticUpdateHandler { Request = requestFromDb });
                    break;
            }

        }

        private async Task<bool> ChangeAsync(Request sourceRequest, Request changedModel)
        {
            await IsReviewsCorrect(changedModel);

            var approvedReviews = sourceRequest.Reviews.TakeWhile(r => r.IsApproved == true);

            if (!Enumerable.SequenceEqual(approvedReviews.Select(r => r.ReviewerId), changedModel.ReviewsIds.Take(approvedReviews.Count())))
                throw new ConflictException("Approved reviews can't be changed", 409);   


            //update reviewers
            var replacedReviews = sourceRequest.Reviews.Skip(approvedReviews.Count()).ToList();
            var newReviewers = changedModel.ReviewsIds.Skip(approvedReviews.Count()).ToList();

            if (!newReviewers.Any())
                throw new ConflictException("Last review can't be already approved", 409);

            bool isActiveReviewReplace = replacedReviews.First().ReviewerId != newReviewers.First();

            if (!isActiveReviewReplace)
            {
                replacedReviews.RemoveAt(0);
                newReviewers.RemoveAt(0);
            }

            foreach (Review review in replacedReviews)    
                sourceRequest.Reviews.Remove(review);

            foreach (var reviewerId in newReviewers)                    
                sourceRequest.Reviews.Add(new Review() { ReviewerId = reviewerId, RequestId = sourceRequest.Id });

            return isActiveReviewReplace;
        }

        private async Task Duplicate(Request parentRequest, Request duplicateModel)
        {
            if (parentRequest.EndDate <= DateTime.Now.Date)                     
                throw new ConflictException("End date of the request must be later than the current date", 409);

            var safeModel = new Request();
            safeModel.EndDate = duplicateModel.EndDate;
            safeModel.StartDate = duplicateModel.StartDate;
            safeModel.Comment = duplicateModel.Comment;
            safeModel.ReviewsIds = duplicateModel.ReviewsIds;
            safeModel.Reviews.Clear();
            safeModel.ParentRequestId = duplicateModel.Id;

            await AddAsync(safeModel);                                         

            parentRequest.State = VacationState.Rejected;
            parentRequest.ModifiedByUserId = parentRequest.UserId;

            await _mediator.Publish(new StatisticUpdateHandler { Request = parentRequest });
        }

        private async Task<bool> ValidateAccounting(IEnumerable<string> reviewerId)
        {
            //check first reviewer, must be Accountant
            var accountantReview = await _userService.GetUser(reviewerId.FirstOrDefault());
            accountantReview.Role = (await _userManager.GetRolesAsync(accountantReview)).FirstOrDefault();
            return (accountantReview.Role == RoleName.Accountant.ToString());
        }

        private async Task<bool> ValidateManagers(ICollection<string> reviewerIds)
        {
            //check the rest of the reviewers, there should only be managers
            reviewerIds = reviewerIds.Skip(1).ToList();

            var managerReviews = reviewerIds.Select(rId => _userService.GetUser(rId).Result).ToList();

            foreach(var item in managerReviews)
                item.Role = (await _userManager.GetRolesAsync(item)).FirstOrDefault();

            return managerReviews.Any() && managerReviews.All(r => r.Role == RoleName.Manager.ToString());
        }

        //reviewers should not be repeated
        private bool IsNoDuplicate(ICollection<string> ids) => (ids != null && ids.Count() == ids.Distinct().Count());

        private async Task<bool> IsRequestCorrect(Request request)
        {
            //comment is required field
            if (String.IsNullOrEmpty(request.Comment))
                    throw new RequiredArgumentNullException("Comment field is empty", 409);

            //check reviewers
            await IsReviewsCorrect(request);

            //checking vacation dates
            if (await IntersectionDates(request))
                throw new ConflictException("Dates intersection", 409);

            return true;
        }

        private async Task IsReviewsCorrect(Request request)
        {
            if (!await ValidateAccounting(request.ReviewsIds))
                throw new NoReviewerException("Not defined accounting", 409);

            //reviewers must have managers
            if (request.Type == VacationType.Administrative || request.Type == VacationType.Study || request.Type == VacationType.Annual || request.Type == VacationType.Sick)
            {
                if (!await ValidateManagers(request.ReviewsIds))
                    throw new NoReviewerException("Not all managers defined", 409);
            }
            else if (request.ReviewsIds.Count() > 1)
                throw new NoReviewerException("Not all managers defined", 409);

            if (!IsNoDuplicate(request.ReviewsIds))
                throw new ConflictException("Any manager cannot be specified more than once", 409);
        }

        private void FillReviewers(Request request)
        {
            foreach (var item in request.ReviewsIds)
            {
                var rewiew = new Review()
                {
                    ReviewerId = item,
                    RequestId = request.Id
                };

                request.Reviews.Add(rewiew);
            }
        }

        private async Task<bool> IntersectionDates(Request obj)
        {
            if (obj.EndDate < obj.StartDate)
                throw new ConflictException("End date can't be earlier than start date", 409);

            if (obj.StartDate < DateTime.Now.Date)
                throw new ConflictException("The dates are in the past", 409);

            return !obj.IsDateIntersectionAllowed &&
                (await _repository.FilterAsync(u => u.UserId == obj.UserId
                    && u.State != VacationState.Rejected
                    && (obj.ParentRequestId == null || u.Id != obj.ParentRequestId)
                    && ((obj.StartDate >= u.StartDate && obj.StartDate <= u.EndDate)
                        || (obj.EndDate <= u.EndDate && obj.EndDate >= u.StartDate)
                        || (obj.StartDate < u.StartDate && obj.EndDate > u.EndDate)))
                ).Any();
        }
    }
}
