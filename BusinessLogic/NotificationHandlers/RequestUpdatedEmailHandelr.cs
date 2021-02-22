using BusinessLogic.Services.Intarfaces;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using EmailTemplateRender.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using BusinessLogic.Settings;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;
using EmailModels.Models;
using System.Linq;
using EmailTemplateRender.Views.Emails;

namespace BusinessLogic.NotificationHandlers
{
    class RequestUpdatedEmailHandelr : INotificationHandler<RequestUpdatedNotification>
    {
        IRepository<Review, int> _reviewRepository;
        UserManager<User> _userManager;
        IRazorViewToStringRenderer _razorViewToStringRenderer;
        IEmailService _mailer;
        UrlConfig _uiConfig;

        public RequestUpdatedEmailHandelr(
            IRepository<Review, int>
            revRepository, UserManager<User> userManager,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            IEmailService mailer,
            IOptions<UrlConfig> uiConfig)
        {
            _reviewRepository = revRepository;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _userManager = userManager;
            _mailer = mailer;
            _uiConfig = uiConfig.Value;
        }

        public async Task Handle(RequestUpdatedNotification notification, CancellationToken cancellationToken)
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

            var approvedPeopleNames = reviews.Where(r => r.IsApproved == true).Select(r => $"{r.Reviewer.FirstName} {r.Reviewer.LastName}".Trim()).ToList();
            model.ApprovedFullNames = string.Join(", ", approvedPeopleNames);

            var curReview = reviews.Where(r => r.IsApproved == null).FirstOrDefault();
            string address = curReview.Reviewer.Email;
            string theme = string.Format(
                    model.AuthorFullName,
                    model.StartDate,
                    model.EndDate
                    );

            string reference = _uiConfig.Url + $"/reviews";

            var dataForViewModel = new RequestEmailViewModel(model, reference);

            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/RequestUpdate/RequestUpdate.cshtml", dataForViewModel);

            await _mailer.SendEmailAsync(address, theme, body);
        }
    }
}
