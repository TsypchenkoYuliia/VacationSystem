using BusinessLogic.Notification;
using BusinessLogic.Services.Intarfaces;
using BusinessLogic.Settings;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using EmailModels.Models;
using EmailTemplateRender.Services;
using EmailTemplateRender.Views.Emails;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.NotificationHandlers
{
    class RequestEmailHandler : INotificationHandler<RequestCreatedNotification>
    {
        IRepository<Review, int> _reviewRepository;
        UserManager<User> _userManager;
        //IStringLocalizer<SharedEmailResources> _localizer;
        IRazorViewToStringRenderer _razorViewToStringRenderer;
        //IMapper _mapper;
        IEmailService _mailer;
        UrlConfig _uiConfig;
        //UIConfig _uiConfig;

        public RequestEmailHandler(IRepository<Review, int> revRepository, UserManager<User> userManager, IEmailService mailer, IOptions<UrlConfig> uiConfig, IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _reviewRepository = revRepository;
            _userManager = userManager;
            _mailer = mailer;
            _uiConfig = uiConfig.Value;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public async Task Handle(RequestCreatedNotification notification, CancellationToken cancellationToken)
        {
            Request request = notification.Request;

            RequestDataForEmail model = new RequestDataForEmail { RequestType = request.Type.ToString(),
                StartDate = request.StartDate.Date.ToString("dd/MM/yyyy"),
                EndDate = request.EndDate.Date.ToString("dd/MM/yyyy"),
                Comment = request.Comment,
                Duration = request.EndDate.Date.Subtract(request.StartDate.Date).Days+1,        
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

            string theme = string.Format(model.AuthorFullName +", "+ model.StartDate + " - " + model.EndDate + ", " + "type: " + model.RequestType);

            string reference = _uiConfig.Url + $"other_requests/actions?review={curReview.Id}&action=";

            var dataForViewModel = new RequestEmailViewModel(model, reference + "approve", reference + "reject");

            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/RequestUpdate/RequestUpdate.cshtml", dataForViewModel); ;

            await _mailer.SendEmailAsync(address, theme, body);
        }
    }
}
