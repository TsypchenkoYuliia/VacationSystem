using BusinessLogic.Services.Intarfaces;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public async Task<User> CreateUser(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                return user;
            else
                return null;
        }

        public async Task DeleteUser(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<User> GetUser(string id)
        {
            return await _repository.FindAsync(x=>x.Id==id);
        }

        public async Task<User> GetUser(Expression<Func<User, bool>> predicat)
        {
            return await _repository.FindAsync(predicat);
        }

        public async Task<IReadOnlyCollection<User>> GetUsers(string name, string role)
        {
            Expression<Func<User, bool>> condition = user =>
                (name == null || (user.FirstName + " " + user.LastName).ToLower().Contains(name.ToLower()))
                && (role == null || user.Role == role);

            return await _repository.FilterAsync(condition);
        }

        public async Task UpdateUser(User newUser)
        {
            await _repository.UpdateAsync(newUser);
        }
    }
}
