using DataAccess.Context;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public class RequestRepository : BaseRepository<Request, int>
    {
        public RequestRepository(VacationSystemContext context) : base(context) { }
    }
}
