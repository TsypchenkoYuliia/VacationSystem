using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DomainModel
{
    public class Request
    {
        public Request()
        {
            Reviews = new List<Review>();
        }
        public int Id { get; set; }
        public VacationType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public string Comment { get; set; }
        public VacationState State { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public int? ModifiedByUserId { get; set; }
        public int? ParentRequestId { get; set; }
    }
}
