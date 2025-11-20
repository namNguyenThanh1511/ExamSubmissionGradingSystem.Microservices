using AutoMapper;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IExaminerRepository _examinerRepository;
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            IExaminerRepository examinerRepository,
            IExamRepository examRepository,
            IMapper mapper)
        {
            _submissionRepository = submissionRepository;
            _examinerRepository = examinerRepository;
            _examRepository = examRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubmissionDto>> GetSubmissionsByExamIdAsync(long examId)
        {
            var submissions = await _submissionRepository.GetSubmissionsByExamIdAsync(examId);
            return _mapper.Map<IEnumerable<SubmissionDto>>(submissions);
        }

        public async Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto createSubmissionDto)
        {
            // Validate exam exists
            var exam = await _examRepository.GetExamByIdAsync(createSubmissionDto.ExamId);
            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {createSubmissionDto.ExamId} not found");
            }

            // Check if submission with this SubmissionId already exists for this exam
            var existingSubmissions = await _submissionRepository.GetSubmissionsByExamIdAsync(createSubmissionDto.ExamId);
            if (existingSubmissions.Any(s => s.SubmissionId == createSubmissionDto.SubmissionId))
            {
                throw new InvalidOperationException($"Submission with SubmissionId {createSubmissionDto.SubmissionId} already exists for exam {createSubmissionDto.ExamId}");
            }

            // Map DTO to entity
            var submission = _mapper.Map<Repository.Models.Submission>(createSubmissionDto);

            // Create the submission
            var createdSubmission = await _submissionRepository.CreateSubmissionAsync(submission);

            // Map back to DTO
            return _mapper.Map<SubmissionDto>(createdSubmission);
        }

        public async Task<SubmissionDto?> AssignExaminerAsync(long submissionId, long examinerId)
        {
            // Get submission
            var submission = await _submissionRepository.GetSubmissionByIdAsync(submissionId);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with ID {submissionId} not found");
            }

            // Validate examiner exists
            var examiner = await _examinerRepository.GetExaminerByIdAsync(examinerId);
            if (examiner == null)
            {
                throw new KeyNotFoundException($"Examiner with ID {examinerId} not found");
            }

            // Update examiner
            submission.ExaminerId = examinerId;
            var updatedSubmission = await _submissionRepository.UpdateSubmissionAsync(submission);

            return _mapper.Map<SubmissionDto>(updatedSubmission);
        }

        public async Task<SubmissionDto> GradeSubmissionAsync(long submissionId, double totalScore)
        {
            // Get submission
            var submission = await _submissionRepository.GetSubmissionByIdAsync(submissionId);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with ID {submissionId} not found");
            }

            // Update total score
            submission.TotalScore = totalScore;
            var updatedSubmission = await _submissionRepository.UpdateSubmissionAsync(submission);

            return _mapper.Map<SubmissionDto>(updatedSubmission);
        }
    }
}
