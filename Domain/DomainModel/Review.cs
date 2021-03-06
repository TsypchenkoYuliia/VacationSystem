﻿using System;

namespace Domain.DomainModel
{
    public class Review
    {
        public int Id { get; set; }
        public virtual User Reviewer { get; set; }
        public string ReviewerId { get; set; }
        public bool? IsApproved { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }
        public string Comment { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
