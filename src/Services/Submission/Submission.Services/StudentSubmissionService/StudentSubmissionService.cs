using Submission.Repositories.Repositories;
using Submission.Services.DTOs;

namespace Submission.Services.StudentSubmissionService
{
    public class StudentSubmissionService : IStudentSubmissionService
    {
        private readonly IStudentSubmissionRepository _repo;

        public StudentSubmissionService(IStudentSubmissionRepository repo)
        {
            _repo = repo;
        }

        public async Task<DTOs.PaginatedResponse<StudentSubmissionDto>> GetSubmissionsAsync(int page, int size)
        {
            var (entities, totalItems) = await _repo.GetSubmissionsAsync(page, size);

            var dtos = entities.Select(s => new StudentSubmissionDto
            {
                Id = s.Id,
                StudentId = s.StudentId,
                SolutionUrl = s.SolutionUrl,
                IsValid = s.IsValid,
                Status = s.Status,
                Note = s.Note,
                UploadAt = s.UploadAt
            });

            return new DTOs.PaginatedResponse<StudentSubmissionDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = size
            };
        }
        public async Task<StudentSubmissionDto> GetSubmissionByIdAsync(Guid submissionId)
        {
            var entity = await _repo.GetSubmissionByIdAsync(submissionId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Submission with ID {submissionId} not found.");
            }
            return new StudentSubmissionDto
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                SolutionUrl = entity.SolutionUrl,
                IsValid = entity.IsValid,
                Status = entity.Status,
                Note = entity.Note,
                UploadAt = entity.UploadAt
            };
        }
    }
}
