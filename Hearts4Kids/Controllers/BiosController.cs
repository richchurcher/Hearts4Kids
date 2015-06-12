using Hearts4Kids.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Hearts4Kids.Models;

namespace Hearts4Kids.Controllers
{
    [Authorize]
    public class BiosController : Controller
    {
        // GET: Bios
        public ActionResult CreateEditBio(int id=0)
        {
            if (id == 0) { id = User.Identity.GetUserId<int>(); }
            else if (!IsAuthorised())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }
            
            return View(MemberDetailService.GetBioDetails(id));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateEditBio(BiosViewModel model)
        {
            if (!IsAuthorised())
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return RedirectToAction("Error");
            }
            if (ModelState.IsValid)
            {
                MemberDetailService.UpdateBios(model, User.Identity.GetUserId<int>(), ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            return View(model);
        }
        public ActionResult UpdateDetails(int id=0)
        {
            var usr = GetUser(id);
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
            if (!IsAuthorised())
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return RedirectToAction("Error");
            }
            if (ModelState.IsValid)
            {
                MemberDetailService.UpdateMemberDetails(model, User.Identity.GetUserId<int>(), ModelState);
                //to do allow phone number update
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
            return View(model);
        }
        bool IsAuthorised(int id = 0)
        {
            return id == 0 || id == User.Identity.GetUserId<int>() || User.IsInRole(Domain.Admin);
        }
        ApplicationUser GetUser(int id=0)
        {
            using (var um = AccountController.GetApplicationUserManager())
            {
                ApplicationUser usr;
                if (id == 0)
                {
                    usr = um.FindByName(User.Identity.Name);
                }
                else
                {
                    usr = um.FindById(id);
                }
                
                return usr;
            }
        }
    }
}