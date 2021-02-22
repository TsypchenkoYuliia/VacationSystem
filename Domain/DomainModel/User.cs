using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace Domain.DomainModel
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Request> Requests { get; set; }

        public User()
        {
            Requests = new List<Request>();
        }

        public string Role { get; set; }

    }
}
