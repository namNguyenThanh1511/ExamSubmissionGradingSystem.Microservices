using Service.DTO;

namespace Service.Services
{
    public interface IViolationService
    {
        Task<IEnumerable<ViolationDto>> GetViolationsByStatusAsync(bool? status);
        Task<ViolationDto> CreateViolationAsync(CreateViolationDto createViolationDto);
        Task<ViolationDto> VerifyViolationAsync(long violationId, bool verified);
    }
}
