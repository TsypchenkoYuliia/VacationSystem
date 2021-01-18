using Domain.DomainModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Notification
{
    public class StatisticUpdateHandler : INotification
    {
        public Request Request { get; set; }
    }
}
