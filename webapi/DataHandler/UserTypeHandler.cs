using Microsoft.AspNetCore.Authorization;

namespace webapi.DataHandler
{
    public static class UserTypes
    {
        public const string Admin = "Admin";
        public const string Worker = "Worker";
    }

    public class UserTypeHandler : IAuthorizationRequirement
    {
        public UserTypeHandler(IEnumerable<string> userTypes)
        {
            Users = userTypes;
        }

        public IEnumerable<string> Users { get; }

        private static readonly string[] ValidUserRoles =
        {
            UserTypes.Admin,
            UserTypes.Worker
        };

        public static bool IsValidUserRole(string userRole)
        {
            return ValidUserRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase);
        }
    }
}
