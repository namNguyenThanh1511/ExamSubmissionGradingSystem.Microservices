using AutoMapper;
using Repository.Models;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class ViolationService : IViolationService
    {
        private readonly IViolationRepository _violationRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IMapper _mapper;

        public ViolationService(
            IViolationRepository violationRepository,
            ISubmissionRepository submissionRepository,
            IMapper mapper)
        {
            _violationRepository = violationRepository;
            _submissionRepository = submissionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViolationDto>> GetViolationsByStatusAsync(bool? status)
        {
            var violations = await _violationRepository.GetViolationsByStatusAsync(status);
            return _mapper.Map<IEnumerable<ViolationDto>>(violations);
        }

        public async Task<ViolationDto> CreateViolationAsync(CreateViolationDto createViolationDto)
        {
            // Validate submission exists
            var submission = await _submissionRepository.GetSubmissionByIdAsync(createViolationDto.SubmissionId);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with ID {createViolationDto.SubmissionId} not found");
            }

            var violation = _mapper.Map<Violation>(createViolationDto);
            // Automatically set Verified to false when creating a new violation
            violation.Verified = false;
            
            var createdViolation = await _violationRepository.CreateViolationAsync(violation);

            // Reload with includes for DTO mapping
            var violations = await _violationRepository.GetViolationsByStatusAsync(null);
            var reloadedViolation = violations.FirstOrDefault(v => v.Id == createdViolation.Id);

            return _mapper.Map<ViolationDto>(reloadedViolation);
        }

        public async Task<ViolationDto> VerifyViolationAsync(long violationId, bool verified)
        {
            // Get violation
            var violation = await _violationRepository.GetViolationByIdAsync(violationId);
            if (violation == null)
            {
                throw new KeyNotFoundException($"Violation with ID {violationId} not found");
            }

            // Update verified status
            violation.Verified = verified;
            var updatedViolation = await _violationRepository.UpdateViolationAsync(violation);

            return _mapper.Map<ViolationDto>(updatedViolation);
        }
    }
}
