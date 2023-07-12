using Hangfire.Dashboard;

namespace SimpleERP.GoogleDriveIntegration.Filters
{
    public class AuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
