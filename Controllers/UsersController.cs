using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CallBackUtility.Models;
using CallBackUtility.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CallBackUtility.Controllers
{
    [Authorize(Roles = AppRoles.SYSTEMADMIN)]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index(string id = null)
        {
            // string sysAdmin = ConfigurationManager.AppSettings["SystemAdminEmailId"].ToString();
            string adminUserId = ConfigurationManager.AppSettings["SystemAdminEmailId"].ToString();
            var all_users = db.Users.Where(u => u.Email != adminUserId && u.IsActive == true).ToList();
            //var all_users = db.Users.Where(u =>  u.IsActive == true).ToList();
            if (!string.IsNullOrEmpty(id))
            {
                all_users = all_users.Where(m => m.Roles.Any(r => r.RoleId == id)).ToList();
            }
            var appusers = from u in all_users.OrderBy(u => u.FirstName) select new RegisterViewModel { RoleName = getRoleByRoleId(u.RoleId), Id = u.Id, ContactNo = u.ContactNo, FirstName = DataHelper.ToPascalConvention(u.FirstName + " " + u.LastName), Email = u.Email, UserProfileImagePath = loadUserImage(u.UserProfileImagePath) };
            ViewBag.RoleId = new SelectList(db.Roles.Where(u => u.Name.ToLower() != "admin").ToList(), "Id", "Name");
            return View(Json(appusers.ToList()));
        }
        public static string loadUserImage(string userImage)
        {
            string imagePath = ConfigurationManager.AppSettings["UserImageUplaodPath"].ToString();
            if (!string.IsNullOrEmpty(userImage) && System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(imagePath + userImage)))
            {
                return imagePath + userImage;
            }
            else
            {
                return imagePath + "ProfileImage.jpg";
            }
        }

        internal static string getRoleByRoleId(string RoleId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string role_name = db.Roles.Find(RoleId).Name;
            return role_name;
        }

        [HttpPost]
        public virtual ActionResult UploadUserProfileImage()
        {
            string userid = User.Identity.GetUserId();
            string FileFolderPath = ConfigurationManager.AppSettings["UserImageUplaodPath"].ToString();
            string pathForSaving = string.Empty;
            HttpPostedFileBase myFile = Request.Files["ImageToUpload"];
            string filename = Path.GetFileName(myFile.FileName);
            string ImagePath = FileFolderPath + filename;
            bool isUploaded = false;
            string message = "Failed to upload image!";
            if (myFile != null && myFile.ContentLength != 0)
            {
                pathForSaving = Server.MapPath(FileFolderPath);
                if (DataHelper.CreateFolderIfNeededWithPhysicalPath(pathForSaving))
                {
                    try
                    {
                        myFile.SaveAs(Path.Combine(pathForSaving, filename));
                        isUploaded = true;
                        message = "Image uploaded successfully!";
                    }
                    catch (Exception ex)
                    {
                        message = string.Format("Image uploading failed: {0}", ex.Message);
                    }
                }
            }
            return Json(new { isUploaded = isUploaded, message = message, filepath = ImagePath });
        }
        //Emails
        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserProfileImagePath,FirstName,ContactNo,LastName,Password,ConfirmPassword,LoginCounter,IsActive,FullName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = AppRoles.SYSTEMADMIN)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var store = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser applicationUser = userManager.FindById(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            string role_name = userManager.GetRoles(id)[0];
            applicationUser.FirstName = DataHelper.ToPascalConvention(applicationUser.FirstName);
            applicationUser.LastName = DataHelper.ToPascalConvention(applicationUser.LastName);
            applicationUser.UserProfileImagePath = ConfigurationManager.AppSettings["UserImageUplaodPath"].ToString() + (string.IsNullOrEmpty(applicationUser.UserProfileImagePath) ? "ProfileImage.jpg" : applicationUser.UserProfileImagePath);
            applicationUser.RoleId = role_name;
            return View(applicationUser);
        }

        [Authorize(Roles = AppRoles.SYSTEMADMIN)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var store = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser applicationUser = userManager.FindById(id);
            applicationUser.IsActive = false;
            db.Entry(applicationUser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}