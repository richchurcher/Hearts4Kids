using Hearts4Kids.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Hearts4Kids.Models;
using System.Threading.Tasks;

namespace Hearts4Kids.Controllers
{
    [Authorize]
    public class BiosController : BaseUserController
    {
        [Authorize(Roles=Domain.Admin)]
        public ActionResult Index()
        {
            return View(MemberDetailService.GetBioSumarries());
        }
        // GET: Bios
        public ActionResult CreateEditBio(int id=0)
        {
            if (id == 0) { id = CurrentUser.Id; }
            else if (!IsAuthorised(CurrentUser.Id))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }
            var model = MemberDetailService.GetBioDetails(id);
            model.IsAdmin = IsAdmin;
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEditBio([Bind(Exclude ="IsAdmin")]BiosViewModel model, HttpPostedFileBase bioImg)
        {
            if (!IsAuthorised(model.UserId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return RedirectToAction("Error");
            }
            if (ModelState.IsValid)
            {
                if (bioImg != null)
                {
                    model.BioPicUrl = PhotoServices.processBioImage(bioImg);
                }
                
                await MemberDetailService.UpdateBios(model, ModelState, IsAdmin);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            model.IsAdmin = IsAdmin;
            return View(model);
        }
        public ActionResult UpdateDetails(int id=0)
        {
            if (id == 0) { id = CurrentUser.Id; }
            var usr = UserManager.FindById(id);
            if (string.IsNullOrEmpty(usr.PasswordHash))
            {
                RedirectToAction("Register","Account");
            }
            if (!IsAuthorised(usr.Id))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }
            var returnView = MemberDetailService.GetMemberDetails(usr.Id) ?? new BioDetailsViewModel();
            returnView.PhoneNumber = usr.PhoneNumber;
            returnView.Email = usr.Email;
            returnView.UserName = usr.UserName;
            return View(returnView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateDetails(BioDetailsViewModel model)
        {
            if (!IsAuthorised(model.UserId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return RedirectToAction("Error");
            }
            if (ModelState.IsValid)
            {
                MemberDetailService.UpdateMemberDetails(model, ModelState);
                //to do allow phone number update
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> UserBios()
        {
            var model = await MemberDetailService.GetBiosForDisplay(isMainPage: false);
            return View(model);
        }
        bool? _isAdmin;
        bool IsAdmin
        {
            get
            {
                return _isAdmin.HasValue?_isAdmin.Value
                    :(_isAdmin = UserManager.IsInRole(CurrentUser.Id, Domain.Admin)).Value;
            }
        }
        bool IsAuthorised(int id)
        {
            return id == CurrentUser.Id || IsAdmin;
        }
    }
}