using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CallBackUtility.Models;
using System.Web.Security;
using System.Configuration;
using CallBackUtility.Utility;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using System.Net;
using System.Web.Helpers;
using Microsoft.AspNet;
using System.Diagnostics;
using System.Data;

namespace CallBackUtility.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string id = null)
        {
            LoginViewModel objLoginViewModel = new LoginViewModel();
            ViewBag.ReturnUrl = returnUrl;
            ApplicationDbContext db = new ApplicationDbContext();
            
            if (id != null)
            {
                var currentUser = db.Users.Find(id);
                if (User.Identity.IsAuthenticated)
                {

                    string userRole = db.Roles.Find(currentUser.RoleId).Name;
                    return RedirectToLocal(userDefaultLoginPage(userRole));
                }
                else
                {

                    objLoginViewModel.Email = currentUser.Email;
                    objLoginViewModel.Password = "";
                    return View(objLoginViewModel);
                }
            }
            else
            {
                return View(objLoginViewModel);
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                ApplicationUser user = null;
                if (ModelState.IsValid)
                {
                    user = await UserManager.FindAsync(model.Email, model.Password);
                   
                    if (user != null && user.IsActive)
                    {
                        var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                ApplicationDbContext db = new ApplicationDbContext();
                              string userRole= db.Roles.Find(user.RoleId).Name;
                                if ( user.LoginCounter==0)
                                {
                                    
                                       
                                        return RedirectToLocal(userDefaultLoginPage(userRole));
                                  

                                }
                                else
                                {
                                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                                    return RedirectToAction("ResetPassword", new { code = code, userId = user.Id, resetPw = "true" });
                                   // return RedirectToAction("ResetPassword", "Account");
                                }
                                
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Invalid login attempt.");
                                return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found!.");
                        return View(model);
                    }
                }
                else
                {

                    ModelState.AddModelError("", "User not found!.");
                    return View(model);
                }
                
               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Invalid login attempt." + ex.Message);
                return View(model);
            }
        }

        private string userDefaultLoginPage(string userRole)
        {
            string url = string.Empty;
            switch (userRole)
            {
                case AppRoles.SUPERUSER:
                    url = "/Recordings/Index";
                    //url = "/";
                    break;
                case  AppRoles.CALLSLISTENERONLY:
                    url = "/Recordings/Index";
                    //url = "/";
                    break;
                default:
                    url= "/Users/Index"; 
                    break;
            }
            return url;
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        [Authorize(Roles = AppRoles.SYSTEMADMIN)]
        public ActionResult Edit(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser applicationUser = db.Users.Find(id);
            if (id == null || !applicationUser.IsActive)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var u_manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string role_name = u_manager.GetRoles(id).FirstOrDefault() == null ? "" : u_manager.GetRoles(id).FirstOrDefault();
          

            applicationUser.FirstName = DataHelper.ToPascalConvention(applicationUser.FirstName);
            applicationUser.LastName = DataHelper.ToPascalConvention(applicationUser.LastName);


            applicationUser.UserProfileImagePath = ConfigurationManager.AppSettings["UserImageUplaodPath"].ToString()+( string.IsNullOrEmpty(applicationUser.UserProfileImagePath) ? "ProfileImage.jpg" : applicationUser.UserProfileImagePath);

            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            

            ViewBag.Roles = new SelectList(db.Roles.Where(u => u.Name.ToLower() != "supervisor" && u.Name.ToLower() != "admin").ToList(), "Id", "Name");
            return View(applicationUser);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.SYSTEMADMIN)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicationUser model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                try
                {
                    var u_manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var currentUser = u_manager.FindById(model.Id);
                    if (currentUser != null)
                    {
                        string role_name = u_manager.GetRoles(model.Id).FirstOrDefault() == null ? "" : u_manager.GetRoles(model.Id).FirstOrDefault();
                        //currentUser.Active = model.Active;
                        currentUser.FullName = DataHelper.ToPascalConvention(model.FirstName + " " + model.LastName);
                        currentUser.UserName = model.Email;
                        currentUser.FirstName = model.FirstName;
                        currentUser.LastName = model.LastName;
                        currentUser.PhoneNumber = model.ContactNo;
                        currentUser.UserProfileImagePath = model.UserProfileImagePath.Split('/').Last();
                        currentUser.Email = model.Email;
                        

                        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                        //    var role = roleManager.FindById(roleId);
                        //    u_manager.RemoveFromRole(model.Id, role_name);
                        //    u_manager.AddToRole(model.Id, role.Name);
                        //    db.SaveChanges();
                        if (currentUser.RoleId!=model.RoleId)
                        {
                            string newRoleName = db.Roles.Find(model.RoleId).Name;
                            u_manager.RemoveFromRole(currentUser.Id, role_name);
                            u_manager.AddToRole(model.Id, newRoleName);
                            currentUser.RoleId = model.RoleId;
                        }
                        
                        if (currentUser.Password != model.Password)
                        {
                            currentUser.Password = model.Password;
                            currentUser.ConfirmPassword = model.ConfirmPassword;
                            u_manager.RemovePassword(model.Id);
                            u_manager.AddPassword(model.Id, model.Password);
                        }

                        db.Entry(currentUser).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Users");
                    }
                }
                catch (Exception ex)
                {
                    string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
                    LogsManager.Logs(LogPath,ex.ToString());
                    ModelState.AddModelError("", "User already exist with the email address!"+ex.Message);
                }
            }

            ViewBag.Roles = new SelectList(db.Roles.Where(u => u.Name.ToLower() != "supervisor" && u.Name.ToLower() != "admin").ToList(), "Id", "Name");

            return View(model);
        }
        //
        // GET: /Account/Register
        [Authorize(Roles = AppRoles.SYSTEMADMIN)]
        public ActionResult Register()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ViewBag.Roles = new SelectList(db.Roles.Where(u => u.Name.ToLower() != "supervisor" && u.Name.ToLower() != "admin").ToList(), "Id", "Name");
            ApplicationUser objRegisterViewModel = new ApplicationUser();
            return View(objRegisterViewModel);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = AppRoles.SYSTEMADMIN)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(ApplicationUser model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, FullName = DataHelper.ToPascalConvention(model.FirstName + " " + model.LastName), RoleId = model.RoleId, FirstName = model.FirstName, LastName = model.LastName, IsActive = true, Password = model.Password, ConfirmPassword = model.ConfirmPassword, Email = model.Email, PhoneNumber = model.ContactNo, ContactNo = model.ContactNo, PasswordHash = model.Password, LoginCounter = 1, UserProfileImagePath = string.IsNullOrEmpty(model.UserProfileImagePath) ? "ProfileImage.jpg" : model.UserProfileImagePath.Split('/').Last() };               
                string newRoleName = db.Roles.Find(model.RoleId).Name;
                List<ApplicationUser> _user = db.Users.Where(x => x.Email == model.Email && x.IsActive== true && x.Email != AppRoles.SYSTEMADMIN).ToList();
                if (_user.Count() > 0)
                {
                    var u_manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var user_updated = u_manager.FindById(_user[0].Id);
                    user_updated.PhoneNumber = model.ContactNo;
                    user_updated.UserName = model.Email;
                    user_updated.FullName = DataHelper.ToPascalConvention(model.FirstName + " " + model.LastName);
                    user_updated.FirstName = model.FirstName;
                    user_updated.Password = model.Password;
                    user_updated.LastName = model.LastName;
                    user_updated.Email = model.Email;
                    user_updated.UserProfileImagePath = string.IsNullOrEmpty(model.UserProfileImagePath) ? "ProfileImage.jpg" : model.UserProfileImagePath.Split('/').Last();
                    if (user_updated.Password != model.Password)
                    {
                        u_manager.RemovePassword(user_updated.Id);
                        u_manager.AddPassword(user_updated.Id, model.Password);
                    }
                    db.Entry(user_updated).State = EntityState.Modified;
                    db.SaveChanges();
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                    var role = roleManager.FindById(model.RoleId);
                    string role_name = u_manager.GetRoles(_user[0].Id).FirstOrDefault() == null ? "" : u_manager.GetRoles(_user[0].Id).FirstOrDefault();
                    if (role != null)
                    {
                        if (!string.IsNullOrEmpty(role_name))
                        {
                            u_manager.RemoveFromRole(_user[0].Id, role_name);
                        }
                        u_manager.AddToRole(_user[0].Id, role.Name);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    var manager = AppUserManager.GetUserManager();
                    var result = manager.Create(user, user.Password);
                    if (result.Succeeded)
                    { manager.AddToRole(user.Id, newRoleName); return RedirectToAction("Index", "Users"); }
                

                        //var result = await UserManager.CreateAsync(user, model.Password);
                        //if (result.Succeeded)
                        //{
                        //    UserManager.AddToRole(user.Id, newRoleName);
                        //    return RedirectToAction("Index", "Users");
                        //}
                        AddErrors(result); 
                }
            }
            ViewBag.RoleId = new SelectList(db.Roles.ToList(), "Id", "Name", model.RoleId);
            model.UserProfileImagePath = ConfigurationManager.AppSettings["UserImageUplaodPath"].ToString() + model.UserProfileImagePath;
            return View(model);
        }
        
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userId = null, string resetPw = null)
        {
            ResetPasswordViewModel objResetPasswordViewModel = new ResetPasswordViewModel();
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.Find(userId);
            try
            {
                if (user != null)
                {
                    if (resetPw == "true")
                    {
                        if (Session["CurrentUser"] == null)
                        {
                            Session["CurrentUser"] = User.Identity.Name;
                        }
                    }
                    objResetPasswordViewModel.UserId= user.Id;
                    objResetPasswordViewModel.Password = user.Password;
                    objResetPasswordViewModel.NewPassword = string.Empty;
                    objResetPasswordViewModel.Email = user.Email;
                    return code == null ? View("Error") : View(objResetPasswordViewModel);
                }
                return View("Error");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                // User not found, handle the error
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            // Check if the user's password reset is required
            if (user.LoginCounter==1)
            {
                // Reset the user's password
               // var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.NewPassword);

                if (result.Succeeded)
                {
                    // Update the IsPasswordResetRequired flag and save changes
                    user.LoginCounter = 0;

                    user.Password =model.NewPassword;
                    user.ConfirmPassword = model.ConfirmPassword;
                   // user.PasswordHash= model.NewPassword;
                    user.LoginCounter = 0;
                    var updateResult = await UserManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        //var code = await UserManager.UpdateSecurityStampAsync(user.Id);
                       // if (code.Succeeded)
                        //{

                            //    Session.Remove("CurrentUser"); 
                            //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            //    //added by uzma start to resolve authentication token error
                            Session.Abandon();
                            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                            cookie1.Expires = DateTime.Now.AddYears(-1);
                            Response.Cookies.Add(cookie1);
                            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
                            cookie2.Expires = DateTime.Now.AddYears(-1);
                            Response.Cookies.Add(cookie2);
                            // Password reset successful, redirect to login page or another page
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            return RedirectToAction("ResetPasswordConfirmation", "Account", user);
                        //}
                    }
                    else
                    {
                        // Failed to update user, handle the error
                        ModelState.AddModelError("", "Failed to update user");
                    }
                }
                else
                {
                    // Failed to reset password, handle the error
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            else
            {
                // Password reset is not required, redirect to another page
                return RedirectToAction("Login", "Account");
            }

            // If validation fails or there's an error, redisplay the form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation(ApplicationUser user)
        {
            if (user != null)
            {
                ViewBag.Email = user.Email;
                return View(user);
            }
            return View();
        }


        //
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }
            if (ModelState.IsValid)
            {
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());

            try
            {
                clearUserSpecifiedFilesAndFolders(LogPath);
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                //added by uzma start to resolve authentication token error
                Session.Abandon();
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie1);
                // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
                cookie2.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie2);

                
                
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
               
                LogsManager.Logs(LogPath,ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }

        private void clearUserSpecifiedFilesAndFolders(string LogPath)
        {
            
            string uploadedfolderName =ConfigurationManager.AppSettings["ProductionAudioFilesPhysicalPath"].ToString();
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()))
            {
                uploadedfolderName = ConfigurationManager.AppSettings["LabAudioFilesPhysicalPath"].ToString();
            }
           
           // uploadedfolderName = uploadedfolderName.Replace("\\", "/");
            string userid = User.Identity.GetUserId();
     
            string FileFolderPath =uploadedfolderName + userid;

            
            if (Directory.Exists(FileFolderPath))
            {
                foreach (var _process in Process.GetProcessesByName("ffmpeg"))
                {
                    _process.Kill();
                }
                DirectoryInfo dr = new DirectoryInfo(FileFolderPath);

                //foreach (FileInfo fi in dr.GetFiles())
                //{

                //    GC.Collect();
                //    GC.WaitForPendingFinalizers();                    
                //    fi.IsReadOnly = false;
                //    fi.Delete();
                //}
                dr.Refresh();
                dr.Delete(true);
            }
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
      
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Recordings");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}