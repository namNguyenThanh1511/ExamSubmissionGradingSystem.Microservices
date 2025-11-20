namespace Submission.Services.CourseManagementClient
{
    public interface ICourseManagementClient
    {
        Task<bool> LinkSubmissionToExamAsync(Guid submissionId, long examId, string studentCode);
    }
}

