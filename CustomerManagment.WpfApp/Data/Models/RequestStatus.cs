namespace CustomerManagment.WpfApp.Data.Models
{
    public enum RequestStatus
    {
        Pending = 0,      // Default - request needs attention
        InProgress = 1,   // Request is being worked on
        Solved = 2,       // Request has been resolved
        OnHold = 3,       // Request is waiting for something
        Cancelled = 4,    // Request was cancelled
        CallLater = 5,    // Need to call back later
        FollowUp = 6,     // Requires follow-up
        Escalated = 7,    // Request has been escalated
        Reopened = 8      // Request was solved but reopened
    }
}