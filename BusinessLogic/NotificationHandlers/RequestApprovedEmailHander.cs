using BusinessLogic.Notification;
using BusinessLogic.Services.Intarfaces;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using EmailModels.Models;
using EmailTemplateRender.Services;
using EmailTemplateRender.Views.Emails;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.NotificationHandlers
{
   public class RequestApprovedEmailHander : INotificationHandler<RequestApprovedNotification>
    {
        IRepository<Review, int> _reviewRepository;
        UserManager<User> _userManager;
        IRazorViewToStringRenderer _razorViewToStringRenderer;
        IEmailService _mailer;

        public RequestApprovedEmailHander(
            IRepository<Review, int>
            revRepository, UserManager<User> userManager,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            IEmailService mailer)
        {
            _reviewRepository = revRepository;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _userManager = userManager;
            _mailer = mailer;
        }

        public async Task Handle(RequestApprovedNotification notification, CancellationToken cancellationToken)
        {
            Request request = notification.Request;

            RequestDataForEmail model = new RequestDataForEmail
            {
                AuthorFullName = $"{request.User.FirstName} {request.User.LastName}".Trim(),
                RequestType = request.Type.ToString(),
                StartDate = request.StartDate.Date.ToString("dd/MM/yyyy"),
                EndDate = request.EndDate.Date.ToString("dd/MM/yyyy"),
                Comment = request.Comment,
                Duration = request.EndDate.Date.Subtract(request.StartDate.Date).Days + 1,
            };


            User author = await _userManager.FindByIdAsync(request.UserId.ToString());
            model.AuthorFullName = $"{author.FirstName} {author.LastName}".Trim();

            IEnumerable<Review> reviews = await _reviewRepository.FilterAsync(rev => rev.RequestId == request.Id);

            foreach (var review in reviews)
                review.Reviewer = await _userManager.FindByIdAsync(review.ReviewerId.ToString());

            var approvedPeopleNames = reviews.Select(r => $"{r.Reviewer.FirstName} {r.Reviewer.LastName}".Trim()).ToList();
            model.ApprovedFullNames = string.Join(", ", approvedPeopleNames);

            var dataForViewModel = new RequestEmailViewModel(model);
            {   //Author mail
                string authorAddress = author.Email;
                string authorTheme = string.Format(
                        model.StartDate,
                        model.EndDate);

                string authorBody = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/RequestApprove/RequestApproveForAuthor.cshtml", dataForViewModel);

                await _mailer.SendEmailAsync(authorAddress, authorTheme, authorBody);
            }

            {   //Accountant mail
                string accountantAddress = reviews.FirstOrDefault().Reviewer.Email;
                string accountantTheme = string.Format(
                        model.AuthorFullName,
                        model.StartDate,
                        model.EndDate);

                string accountantBody = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/RequestApprove/RequestApprove.cshtml", dataForViewModel);

                await _mailer.SendEmailAsync(accountantAddress, accountantTheme, accountantBody);
            }
        }
    }
}
