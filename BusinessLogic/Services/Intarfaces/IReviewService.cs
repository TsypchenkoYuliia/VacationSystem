using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Intarfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllAsync(string reviewerId = null, int? requestId = null, int? stateId = null, DateTime? startDate = null, DateTime? endDate = null, string name = null, int? typeId = null);
        Task<Review> GetByIdAsync(int reviewId);
        Task CreateAsync(Review obj);
        Task DeleteAsync(int id);
        Task UpdateAsync(int reviewId, Review newModel, string userId);
    }
}
