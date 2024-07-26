using CallBackUtility.Models;
using CallBackUtility.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR.Hosting;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.Mvc;
namespace CallBackUtility
{
    public class AppUserManager
    {
        public static string adminEmailId = ConfigurationManager.AppSettings["SystemAdminEmailId"].ToString();
        public static string adminPWD = ConfigurationManager.AppSettings["SystemAdminPW"].ToString();
        public static string calls_listener_user_emailId = ConfigurationManager.AppSettings["calls_listener_user_emailId"].ToString();
        public static string calls_listener_user_pwd = ConfigurationManager.AppSettings["calls_listener_user_pwd"].ToString();
        internal static void SeedUser()
        {
            string CALLSLISTENERONLY_RoleId = string.Empty;
            string SYSTEMADMIN_RoleId = string.Empty;
            string SUPERUSER_RoleId = string.Empty;
            ApplicationDbContext context = new ApplicationDbContext();

            if ((!context.Roles.Any(r => r.Name == AppRoles.SYSTEMADMIN)) || (!context.Roles.Any(r => r.Name == AppRoles.SUPERUSER)) || (!context.Roles.Any(r => r.Name == AppRoles.CALLSLISTENERONLY)))
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                if (!context.Roles.Any(r => r.Name == AppRoles.CALLSLISTENERONLY))
                {
                    roleManager.Create(new IdentityRole { Name = AppRoles.CALLSLISTENERONLY });
                    var _role = roleManager.FindByName(AppRoles.CALLSLISTENERONLY);
                    CALLSLISTENERONLY_RoleId = _role.Id;
                }
                if (!context.Roles.Any(r => r.Name == AppRoles.SYSTEMADMIN))
                {
                    roleManager.Create(new IdentityRole { Name = AppRoles.SYSTEMADMIN });
                    var _role = roleManager.FindByName(AppRoles.SYSTEMADMIN);
                    SYSTEMADMIN_RoleId = _role.Id;
                }
                if (!context.Roles.Any(r => r.Name == AppRoles.SUPERUSER))
                {
                    roleManager.Create(new IdentityRole { Name = AppRoles.SUPERUSER });
                    var _role = roleManager.FindByName(AppRoles.SUPERUSER);
                    SUPERUSER_RoleId = _role.Id;
                }
            }
            if (!context.Users.Any(u => u.UserName == adminEmailId))
            {
                var manager = GetUserManager();
                // var roleStore = new RoleStore<IdentityRole>(context);
                //var roleManager = new RoleManager<IdentityRole>(roleStore);               
                //roleManager.Create(new IdentityRole { Name = AppRoles.SYSTEMADMIN });
                var user = new ApplicationUser { UserName = adminEmailId, FullName = "System Administrator", RoleId = SYSTEMADMIN_RoleId, FirstName = "System", LastName = "Administrator", IsActive = true, Password = adminPWD, ConfirmPassword = adminPWD, Email = adminEmailId, PhoneNumber = "04434343432", ContactNo = "04434343432", PasswordHash = adminPWD, LoginCounter = 0, UserProfileImagePath = "ProfileImage.jpg" };
                var result = manager.Create(user, user.Password);
                manager.AddToRole(user.Id, AppRoles.SYSTEMADMIN);
            }
            if (!context.Users.Any(u => u.UserName == calls_listener_user_emailId))
            {
                var manager = GetUserManager();
                // var roleStore = new RoleStore<IdentityRole>(context);
                //var roleManager = new RoleManager<IdentityRole>(roleStore);
                CALLSLISTENERONLY_RoleId = string.IsNullOrEmpty(CALLSLISTENERONLY_RoleId) ? context.Roles.Where(r => r.Name == AppRoles.CALLSLISTENERONLY).ToList()[0].Id : CALLSLISTENERONLY_RoleId;
                //roleManager.Create(new IdentityRole { Name = AppRoles.SYSTEMADMIN });
                var user = new ApplicationUser { UserName = calls_listener_user_emailId, RoleId = CALLSLISTENERONLY_RoleId, FullName = "Calls Listener", FirstName = "Calls", LastName = "Listener", IsActive = true, Password = calls_listener_user_pwd, ConfirmPassword = calls_listener_user_pwd, Email = calls_listener_user_emailId, PhoneNumber = "04434343432", ContactNo = "04434343432", PasswordHash = calls_listener_user_pwd, LoginCounter = 1, UserProfileImagePath = "ProfileImage.jpg" };
                var result = manager.Create(user, user.Password);
                manager.AddToRole(user.Id, AppRoles.CALLSLISTENERONLY);
            }
        }

        public static ApplicationUserManager GetUserManager()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            return manager;
        }
    }
}