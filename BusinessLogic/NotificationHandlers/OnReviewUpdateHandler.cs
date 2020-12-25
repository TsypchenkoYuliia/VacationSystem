using BusinessLogic.Notification;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.NotificationHandlers
{
    class OnReviewUpdateHandler : INotificationHandler<ReviewUpdateNotification>
    {
        IRepository<Request, int> _requestRepository;
        IMediator _mediator;
        public OnReviewUpdateHandler(IRepository<Request, int> requestRepository, IMediator mediator)
        {
            _requestRepository = requestRepository;
            _mediator = mediator;
        }

        public async Task Handle(ReviewUpdateNotification notification, CancellationToken cancellationToken)
        {
            var requestfromDb = await _requestRepository.FindAsync(x => x.Id == notification.Request.Id);

            INotification new_notification = null;

            if (requestfromDb.Reviews.Any(x => x.IsApproved == false))
            {
                requestfromDb.State = VacationState.Rejected;
                new_notification = new RequestRejectedNotification { Request = requestfromDb };
            }
            else if (requestfromDb.Reviews.All(x => x.IsApproved == true))
            {
                requestfromDb.State = VacationState.Approved;
                new_notification = new RequestApprovedNotification { Request = requestfromDb };
            }
            else if (requestfromDb.Reviews.Any(x => x.IsApproved == null))
            {
                requestfromDb.State = VacationState.InProgress;
                new_notification = new RequestUpdatedNotification { Request = requestfromDb };
            }

            await _requestRepository.UpdateAsync(requestfromDb);
            await _mediator.Publish(new_notification);
        }
    }
}
