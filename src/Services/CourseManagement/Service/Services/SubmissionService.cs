using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;
using Repository.Repository;
using Service.DTO;

namespace Service.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IExaminerRepository _examinerRepository;
        private readonly IExamRepository _examRepository;
        private readonly ICriterionScoreRepository _criterionScoreRepository;
        private readonly ExamManagementContext _context;
        private readonly IMapper _mapper;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            IExaminerRepository examinerRepository,
            IExamRepository examRepository,
            ICriterionScoreRepository criterionScoreRepository,
            ExamManagementContext context,
            IMapper mapper)
        {
            _submissionRepository = submissionRepository;
            _examinerRepository = examinerRepository;
            _examRepository = examRepository;
            _criterionScoreRepository = criterionScoreRepository;
            _context = context;
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

        public async Task<SubmissionDto> GradeSubmissionByCriteriaAsync(GradeSubmissionByCriteriaDto dto)
        {
            // Validate submission exists
            var submission = await _submissionRepository.GetSubmissionByIdAsync(dto.SubmissionId);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with ID {dto.SubmissionId} not found");
            }

            double totalScore = 0;

            // Process each criterion score
            foreach (var scoreDto in dto.Scores)
            {
                // Validate rubric criterion exists and get max score
                var rubricCriterion = await _context.RubricCriteria
                    .FirstOrDefaultAsync(rc => rc.Id == scoreDto.CriterionId);

                if (rubricCriterion == null)
                {
                    throw new KeyNotFoundException($"Rubric criterion with ID {scoreDto.CriterionId} not found");
                }

                // Validate score doesn't exceed max score
                if (rubricCriterion.MaxScore.HasValue && scoreDto.Score > rubricCriterion.MaxScore.Value)
                {
                    throw new InvalidOperationException(
                        $"Score {scoreDto.Score} exceeds maximum score {rubricCriterion.MaxScore.Value} for criterion {rubricCriterion.CriterionName}");
                }

                // Create or update criterion score
                var criterionScore = new CriterionScore
                {
                    SubmissionId = dto.SubmissionId,
                    RubricCriterionId = scoreDto.CriterionId,
                    Score = scoreDto.Score,
                    Comment = scoreDto.Comment
                };

                await _criterionScoreRepository.CreateOrUpdateCriterionScoreAsync(criterionScore);

                // Add to total score
                totalScore += scoreDto.Score;
            }

            // Update submission total score
            submission.TotalScore = totalScore;
            submission.Status = "Chờ admin xác nhận kết quả chấm điểm";
            var updatedSubmission = await _submissionRepository.UpdateSubmissionAsync(submission);

            return _mapper.Map<SubmissionDto>(updatedSubmission);
        }

        public async Task<SubmissionDetailDto?> GetSubmissionDetailAsync(long submissionId)
        {
            // Get submission with all related data including criterion scores
            var submission = await _submissionRepository.GetSubmissionDetailByIdAsync(submissionId);
            
            if (submission == null)
            {
                return null;
            }

            // Map to detail DTO using AutoMapper
            var detailDto = _mapper.Map<SubmissionDetailDto>(submission);
            return detailDto;
        }
        public async Task<SubmissionDto> ConfirmSubmissionAsync(long submissionId)
        {
            // Get submission
            var submission = await _submissionRepository.GetSubmissionByIdAsync(submissionId);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with ID {submissionId} not found");
            }
            submission.Status = "Đã xác nhận kết quả chấm điểm";
            var updatedSubmission = await _submissionRepository.UpdateSubmissionAsync(submission);
            return _mapper.Map<SubmissionDto>(updatedSubmission);
        }
    }
}
