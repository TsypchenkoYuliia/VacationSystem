using Domain.DomainModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.NotificationHandlers
{
    public class ReviewUpdateNotification : INotification
    {
        public Request Request { get; set; }
    }
}
