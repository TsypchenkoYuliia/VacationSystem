using BusinessLogic.Exceptions;
using BusinessLogic.NotificationHandlers;
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

namespace BusinessLogic.Services
{
    public class ReviewService: IReviewService
    {
        IRepository<Review, int> _reviewRepository;
        IRepository<Request, int> _requestRepository;
        IRepository<User, string> _userRepository;
        IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public ReviewService(IRepository<Review, int> reviewRepository, IRepository<Request, int> requestRepository, IRepository<User, string> userRepository, IMediator mediator, UserManager<User> userManager)
        {
            _requestRepository = requestRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task CreateAsync(Review obj)
        {            
             await _reviewRepository.CreateAsync(obj);            
        }

        public async Task DeleteAsync(int id)
        {
            if (await _reviewRepository.FindAsync(x => x.Id == id) != null)
                await _reviewRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Review>> GetAllAsync(string reviewerId = null, int? requestId = null, int? stateId = null, DateTime? startDate = null, DateTime? endDate = null, string name = null, int? typeId = null)
        {
            User userName = null;

            if (name != null)
                userName = (await _userRepository.FilterAsync(x => ((x.FirstName + " " + x.LastName).ToLower()).Contains(name.ToLower()))).FirstOrDefault();

            if (name != null && userName == null)
                throw new ConflictException("Name not found", 409);

            //selection of all reviews or by filters
            Expression<Func<Review, bool>> condition = review =>
                    (reviewerId == null || review.ReviewerId == reviewerId)
                    && (requestId == null || review.RequestId == requestId)
                    && (stateId == null || (int)review.Request.State == stateId)
                    && (startDate == null || review.Request.StartDate.Date >= startDate)
                    && (endDate == null || review.Request.EndDate.Date <= endDate)
                    && (name == null || review.Request.UserId == userName.Id)
                    && (typeId == null || (int)review.Request.Type == typeId)
                    && !(review.Request.State == VacationState.Rejected && review.IsApproved == null);

            var res = await _reviewRepository.FilterAsync(condition);

            return res.Where(x => x.Request.Reviews.OrderBy(x => x.Id).TakeWhile(x => x.ReviewerId != reviewerId).All(x => x.IsApproved != null));
        }

        public async Task<Review> GetByIdAsync(int reviewId)
        {
            return await _reviewRepository.FindAsync(x => x.Id == reviewId);
        }

        public async Task UpdateAsync(int reviewId, Review newModel, string userId)
        {
            var reviewfromDb = await _reviewRepository.FindAsync(x => x.Id == reviewId);

            if (reviewfromDb != null)
            {
                var reviwer = await _userRepository.FindAsync(x=>x.Id== userId);

                reviwer.Role = (await _userManager.GetRolesAsync(reviwer)).FirstOrDefault();

               //check id and role
                if (reviewfromDb.ReviewerId != userId || (reviwer.Role != RoleName.Accountant.ToString() && reviwer.Role != RoleName.Manager.ToString()))
                    throw new ConflictException("The request is not actual!", 409);

                //check of all previous reviews 
                if (!((reviewfromDb.Request.Reviews.OrderBy(x => x.Id)).TakeWhile(x => x.ReviewerId != userId)).All(x => x.IsApproved != null))
                    throw new ConflictException("No previous review!", 409);

                //check status
                if (reviewfromDb.Request.State == VacationState.Rejected)
                    throw new ConflictException("The request has already been rejected!", 409);

                //check Ids reviews
                if ((await _requestRepository.FilterAsync(r => r.Id == reviewfromDb.RequestId && r.Reviews
                    .Select(x => x.ReviewerId).Contains(userId)))
                    .Any() == false)
                    throw new ConflictException("The request is not actual!", 409);

                //check reviers approved/rejected
                if (IsReviewPassed(reviewfromDb, userId))
                    throw new ConflictException("The request has already been <approved/rejected>!", 409);

                reviewfromDb.IsApproved = newModel.IsApproved;
                reviewfromDb.Comment = newModel.Comment;
                reviewfromDb.UpdatedAt = DateTime.Now.Date;

                await _reviewRepository.UpdateAsync(reviewfromDb);

                //for creating email
                var notification = new ReviewUpdateNotification { Request = await _requestRepository.FindAsync(x=>x.Id==reviewfromDb.RequestId) };
                await _mediator.Publish(notification);
            }
            else
                throw new ConflictException("The request is not actual!", 409);
        }

        private bool IsReviewPassed(Review review, string reviewerId)
        {
            return review.Request.Reviews.Where(x => x.ReviewerId == reviewerId && x.IsApproved != null).FirstOrDefault() != null ? true : false;
        }
    }
}
