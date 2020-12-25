﻿using Domain.DomainModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Notification
{
     class RequestApprovedNotification : INotification
    {
        public Request Request { get; set; }
    }
}
