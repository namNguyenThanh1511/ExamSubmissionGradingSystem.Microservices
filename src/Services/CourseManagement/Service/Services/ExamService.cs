using AutoMapper;
using Repository.Models;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public ExamService(IExamRepository examRepository, IMapper mapper)
        {
            _examRepository = examRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetAllExamsAsync();
            return _mapper.Map<IEnumerable<ExamDto>>(exams);
        }

        public async Task<IEnumerable<ExamDto>> GetExamsAsync(long? semesterId = null, long? subjectId = null)
        {
            var exams = await _examRepository.GetExamsAsync(semesterId, subjectId);
            return _mapper.Map<IEnumerable<ExamDto>>(exams);
        }

        public async Task<ExamDto?> GetExamByIdAsync(long id)
        {
            var exam = await _examRepository.GetExamByIdAsync(id);
            
            if (exam == null)
                return null;

            return _mapper.Map<ExamDto>(exam);
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createExamDto)
        {
            // Validate time range
            if (createExamDto.EndTime <= createExamDto.StartTime)
            {
                throw new InvalidOperationException("End time must be after start time");
            }

            // Check if subject exists
            var subjectExists = await _examRepository.IsSubjectExistsAsync(createExamDto.SubjectId);
            if (!subjectExists)
            {
                throw new InvalidOperationException($"Subject with ID {createExamDto.SubjectId} does not exist");
            }

            // Check if semester exists
            var semesterExists = await _examRepository.IsSemesterExistsAsync(createExamDto.SemesterId);
            if (!semesterExists)
            {
                throw new InvalidOperationException($"Semester with ID {createExamDto.SemesterId} does not exist");
            }

            // Map DTO to entity
            var exam = _mapper.Map<Exam>(createExamDto);

            // Create the exam
            var createdExam = await _examRepository.CreateExamAsync(exam);

            // Map back to DTO
            return _mapper.Map<ExamDto>(createdExam);
        }
    }
}
