using AutoMapper;
using Repository.Models;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMapper _mapper;

        public SemesterService(ISemesterRepository semesterRepository, IMapper mapper)
        {
            _semesterRepository = semesterRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SemesterDto>> GetAllSemestersAsync()
        {
            var semesters = await _semesterRepository.GetAllSemestersAsync();
            return _mapper.Map<IEnumerable<SemesterDto>>(semesters);
        }

        public async Task<SemesterDto?> GetSemesterByIdAsync(long id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            
            if (semester == null)
                return null;

            return _mapper.Map<SemesterDto>(semester);
        }

        public async Task<SemesterDto> CreateSemesterAsync(CreateSemesterDto createSemesterDto)
        {
            // Validate date range
            if (createSemesterDto.EndDate <= createSemesterDto.StartDate)
            {
                throw new InvalidOperationException("End date must be after start date");
            }

            // Check if semester name already exists
            var nameExists = await _semesterRepository.IsSemesterNameExistsAsync(createSemesterDto.Name);
            if (nameExists)
            {
                throw new InvalidOperationException($"Semester with name '{createSemesterDto.Name}' already exists");
            }

            // Map DTO to entity
            var semester = _mapper.Map<Semester>(createSemesterDto);

            // Create the semester
            var createdSemester = await _semesterRepository.CreateSemesterAsync(semester);

            // Map back to DTO
            return _mapper.Map<SemesterDto>(createdSemester);
        }
    }
}
