using AutoMapper;
using Repository.Models;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetAllSubjectsAsync();
            return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(long id)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(id);
            
            if (subject == null)
                return null;

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto)
        {
            // Check if subject code already exists
            var codeExists = await _subjectRepository.IsSubjectCodeExistsAsync(createSubjectDto.Code);
            if (codeExists)
            {
                throw new InvalidOperationException($"Subject with code '{createSubjectDto.Code}' already exists");
            }

            // Map DTO to entity
            var subject = _mapper.Map<Subject>(createSubjectDto);

            // Create the subject
            var createdSubject = await _subjectRepository.CreateSubjectAsync(subject);

            // Map back to DTO
            return _mapper.Map<SubjectDto>(createdSubject);
        }
    }
}
