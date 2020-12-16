﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DomainModel
{
    public class Review
    {
        public int Id { get; set; }
        public User Reviewer { get; set; }
        public int ReviewerId { get; set; }
        public bool? IsApproved { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }
        public string Comment { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
