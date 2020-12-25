using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Intarfaces
{
    public interface IRequestService
    {
        Task<Request> AddAsync(Request obj);
        Task<IReadOnlyCollection<Request>> GetAllAsync(string userId, DateTime? startDate = null, DateTime? endDate = null, int? stateId = null, int? typeId = null);
        Task<Request> GetByIdAsync(int requestId);
        Task<Request> GetByIdAsync(string userId, int requestId);
        Task UpdateAsync(int userId, Request newModel);
        Task DeleteAsync(int requestId);
        Task RejectedByOwnerAsync(string userId, int requestId);
    }
}
