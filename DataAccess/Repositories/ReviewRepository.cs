using DataAccess.Context;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public class ReviewRepository : BaseRepository<Review, int>
    {
        public ReviewRepository(VacationSystemContext context) : base(context) { }
    }
}
