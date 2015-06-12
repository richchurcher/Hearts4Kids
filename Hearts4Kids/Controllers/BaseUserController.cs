using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Hearts4Kids.Models;
using System.Threading.Tasks;

namespace Hearts4Kids.Controllers
{
    public abstract class BaseUserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationUser _currentUser;
        private ApplicationSignInManager _signInManager;

        public BaseUserController()
        {
        }

        public BaseUserController(ApplicationUserManager userManager, ApplicationRoleManager roleManager,ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }
        // GET: BaseUser
        internal static ApplicationUserManager GetApplicationUserManager()
        {
            return System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
        }
        internal static ApplicationRoleManager GetApplicationRoleManager()
        {
            return System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
        }

        protected ApplicationSignInManager SignInManager
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
        protected ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? GetApplicationUserManager();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? GetApplicationRoleManager();
            }
            private set
            {
                _roleManager = value;
            }
        }
        protected async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var usr = await UserManager.FindByNameAsync(User.Identity.Name);
            return usr;
        }
        protected ApplicationUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    string name = User.Identity.Name;
                    _currentUser = UserManager.FindByName(name);
                }
                return _currentUser;
            }
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

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}