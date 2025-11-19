using Repository.Models;

namespace Repository.Repository
{
    public interface IViolationRepository
    {
        Task<IEnumerable<Violation>> GetViolationsByStatusAsync(bool? status);
        Task<Violation?> GetViolationByIdAsync(long id);
        Task<Violation> CreateViolationAsync(Violation violation);
        Task<Violation> UpdateViolationAsync(Violation violation);
    }
}
