using AutoMapper;
using Repository.Models;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class ExaminerService : IExaminerService
    {
        private readonly IExaminerRepository _examinerRepository;
        private readonly IMapper _mapper;

        public ExaminerService(IExaminerRepository examinerRepository, IMapper mapper)
        {
            _examinerRepository = examinerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExaminerDto>> GetAllExaminersAsync()
        {
            var examiners = await _examinerRepository.GetAllExaminersAsync();
            return _mapper.Map<IEnumerable<ExaminerDto>>(examiners);
        }

        public async Task<ExaminerDto> CreateExaminerAsync(CreateExaminerDto createExaminerDto)
        {
            var examiner = _mapper.Map<Examiner>(createExaminerDto);
            var createdExaminer = await _examinerRepository.CreateExaminerAsync(examiner);
            return _mapper.Map<ExaminerDto>(createdExaminer);
        }
    }
}
