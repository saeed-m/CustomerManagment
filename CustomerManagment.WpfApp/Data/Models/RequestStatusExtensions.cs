using System.ComponentModel;

namespace CustomerManagment.WpfApp.Data.Models
{
    public static class RequestStatusExtensions
    {
        private static readonly Dictionary<RequestStatus, string> _statusDisplayNames = new()
        {
            { RequestStatus.Pending, "⏳ Pending" },
            { RequestStatus.InProgress, "🚀 In Progress" },
            { RequestStatus.Solved, "✅ Solved" },
            { RequestStatus.OnHold, "⏸️ On Hold" },
            { RequestStatus.Cancelled, "❌ Cancelled" },
            { RequestStatus.CallLater, "📞 Call Later" },
            { RequestStatus.FollowUp, "🔔 Follow Up" },
            { RequestStatus.Escalated, "⚠️ Escalated" },
            { RequestStatus.Reopened, "🔄 Reopened" }
        };

        private static readonly Dictionary<RequestStatus, string> _statusColors = new()
        {
            { RequestStatus.Pending, "#FF9800" },      // Orange
            { RequestStatus.InProgress, "#2196F3" },   // Blue
            { RequestStatus.Solved, "#4CAF50" },       // Green
            { RequestStatus.OnHold, "#9E9E9E" },       // Gray
            { RequestStatus.Cancelled, "#F44336" },    // Red
            { RequestStatus.CallLater, "#FFC107" },    // Amber
            { RequestStatus.FollowUp, "#9C27B0" },     // Purple
            { RequestStatus.Escalated, "#FF5722" },    // Deep Orange
            { RequestStatus.Reopened, "#795548" }      // Brown
        };

        public static string GetDisplayName(this RequestStatus status)
        {
            return _statusDisplayNames.TryGetValue(status, out var displayName)
                ? displayName
                : status.ToString();
        }

        public static string GetColor(this RequestStatus status)
        {
            return _statusColors.TryGetValue(status, out var color)
                ? color
                : "#9E9E9E";
        }

        public static string GetIcon(this RequestStatus status)
        {
            return _statusDisplayNames[status].Split(' ')[0];
        }

        public static string GetDescription(this RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "Request is awaiting attention",
                RequestStatus.InProgress => "Request is being actively worked on",
                RequestStatus.Solved => "Request has been resolved successfully",
                RequestStatus.OnHold => "Request is temporarily paused",
                RequestStatus.Cancelled => "Request has been cancelled",
                RequestStatus.CallLater => "Will call customer back later",
                RequestStatus.FollowUp => "Requires follow-up action",
                RequestStatus.Escalated => "Request has been escalated to higher level",
                RequestStatus.Reopened => "Previously solved request has been reopened",
                _ => "Unknown status"
            };
        }

        public static List<RequestStatus> GetAllStatuses()
        {
            return Enum.GetValues(typeof(RequestStatus))
                       .Cast<RequestStatus>()
                       .ToList();
        }
    }
}