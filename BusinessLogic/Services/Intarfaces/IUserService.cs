using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Intarfaces
{
    public interface IUserService
    {
        Task<User> GetUser(Expression<Func<User, bool>> predicate);
        Task<IReadOnlyCollection<User>> GetUsers(string name, string role);
        Task<User> CreateUser(User user, string password);
        Task UpdateUser(User newUser);
        Task DeleteUser(string id);
    }
}
