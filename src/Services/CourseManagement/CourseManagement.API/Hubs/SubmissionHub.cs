using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CourseManagement.API.Hubs
{
    /// <summary>
    /// SignalR Hub for submission grading notifications
    /// </summary>
    [Authorize]
    public class SubmissionHub : Hub
    {
        /// <summary>
        /// Join a group for specific exam to receive notifications
        /// </summary>
        public async Task JoinExamGroup(long examId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"exam-{examId}");
        }

        /// <summary>
        /// Leave exam group
        /// </summary>
        public async Task LeaveExamGroup(long examId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"exam-{examId}");
        }

        /// <summary>
        /// Join a group for specific submission to receive notifications
        /// </summary>
        public async Task JoinSubmissionGroup(long submissionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"submission-{submissionId}");
        }

        /// <summary>
        /// Leave submission group
        /// </summary>
        public async Task LeaveSubmissionGroup(long submissionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"submission-{submissionId}");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}

