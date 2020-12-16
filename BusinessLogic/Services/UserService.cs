using BusinessLogic.Services.Intarfaces;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User, string> _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IRepository<User, string> repository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task CreateUser(User user)
        {
            await _repository.CreateAsync(user);
        }

        public async Task DeleteUser(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<User> GetUser(Expression<Func<User, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public async Task UpdateUser(User newUser)
        {
            await _repository.UpdateAsync(newUser);
        }
    }
}
