using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DomainModel
{
    public class User
    {
        public ICollection<Request> Requests { get; set; }

        public User()
        {
            Requests = new List<Request>();
        }

    }
}
