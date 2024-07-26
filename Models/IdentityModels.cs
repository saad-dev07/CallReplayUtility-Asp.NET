using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CallBackUtility.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public string UserId { get; set; }
        
        
        [Display(Name = "Upload Profile Image")]
        public string UserProfileImagePath { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Contact No")]
        [RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Enter valid contact no.!")]
        public string ContactNo { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
                [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public int LoginCounter { get; internal set; }
        public bool IsActive { get; internal set; }
        public string FullName { get; internal set; }

      
        [Required]
        public string RoleId { get;  set; }
        //public int DesignationId { get; set; }
        //public virtual Designation Designation { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("CS", throwIfV1Schema: false)
        {
        }

     //   public  DbSet<logsActivity> logsActivities { get; set; }
        //public DbSet<Recording> Recordings { get; set; }
        //public DbSet<session> sessions { get; set; }
        //public DbSet<File> Files { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<CallBackUtility.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<CallBackUtility.Models.ApplicationUser> ApplicationUsers { get; set; }

        // public System.Data.Entity.DbSet<CallBackUtility.Models.ApplicationUser> Users { get; set; }

        //  public System.Data.Entity.DbSet<CallBackUtility.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}