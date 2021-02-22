using Domain.DomainModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    public class RequestUpdatedNotification : INotification
    {
        public Request Request { get; set; }
    }
}
