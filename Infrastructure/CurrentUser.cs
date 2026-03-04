using SyntroVaccPApp.Infrastructure;

namespace SyntroVaccPApp.Infrastructure
{
    public static class CurrentUser
    { 
        public static string UserName => "AdminUser"; 
        public static string Role => AppRoles.Admin;
    }
}
