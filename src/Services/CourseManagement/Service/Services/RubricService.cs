using AutoMapper;
using Repository.Models;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class RubricService : IRubricService
    {
        private readonly IRubricRepository _rubricRepository;
        private readonly IMapper _mapper;

        public RubricService(IRubricRepository rubricRepository, IMapper mapper)
        {
            _rubricRepository = rubricRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RubricDto>> GetAllRubricsAsync()
        {
            var rubrics = await _rubricRepository.GetAllRubricsAsync();
            return _mapper.Map<IEnumerable<RubricDto>>(rubrics);
        }

        public async Task<IEnumerable<RubricDto>> GetRubricsByExamIdAsync(long examId)
        {
            var rubrics = await _rubricRepository.GetRubricsByExamIdAsync(examId);
            return _mapper.Map<IEnumerable<RubricDto>>(rubrics);
        }

        public async Task<RubricDto?> GetRubricByIdAsync(long id)
        {
            var rubric = await _rubricRepository.GetRubricByIdAsync(id);
            
            if (rubric == null)
                return null;

            return _mapper.Map<RubricDto>(rubric);
        }

        public async Task<RubricDto> CreateRubricAsync(CreateRubricDto createRubricDto)
        {
            // Validate that exam exists
            var examExists = await _rubricRepository.IsExamExistsAsync(createRubricDto.ExamId);
            if (!examExists)
            {
                throw new InvalidOperationException($"Exam with ID {createRubricDto.ExamId} does not exist");
            }

            // Validate that at least one criterion is provided
            if (createRubricDto.RubricCriteria == null || !createRubricDto.RubricCriteria.Any())
            {
                throw new InvalidOperationException("At least one rubric criterion is required");
            }

            // Validate that all criteria have positive max scores
            var invalidCriteria = createRubricDto.RubricCriteria.Where(c => c.MaxScore <= 0).ToList();
            if (invalidCriteria.Any())
            {
                throw new InvalidOperationException("All criteria must have a positive max score");
            }

            // Map DTO to entity
            var rubric = _mapper.Map<Rubric>(createRubricDto);

            // Create the rubric with its criteria
            var createdRubric = await _rubricRepository.CreateRubricAsync(rubric);

            // Map back to DTO
            return _mapper.Map<RubricDto>(createdRubric);
        }
    }
}
