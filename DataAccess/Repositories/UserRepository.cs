using DataAccess.Context;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User, string>
    {
        public UserRepository(VacationSystemContext context) : base(context) { }
    }
}
