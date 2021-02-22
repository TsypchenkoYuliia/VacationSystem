using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.DomainModel
{
    public class Request
    {
        public Request()
        {
            Reviews = new List<Review>();
            ReviewsIds = new List<string>();
        }
        public int Id { get; set; }
        public VacationType Type { get; set; }
        [NotMapped]
        public int TypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Review> Reviews { get; set; }
        [NotMapped]
        public ICollection<string> ReviewsIds { get; set; }
        public string Comment { get; set; }
        public VacationState State { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public string? ModifiedByUserId { get; set; }
        public int? ParentRequestId { get; set; }
        [NotMapped]
        public bool IsDateIntersectionAllowed { get; set; }
    }
}
