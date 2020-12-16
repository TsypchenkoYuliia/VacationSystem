using DataAccess.Context;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User, int>
    {
        public UserRepository(VacationSystemContext context) : base(context) { }
        
    }
}
