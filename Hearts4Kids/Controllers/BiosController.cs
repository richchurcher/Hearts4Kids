﻿using Hearts4Kids.Services;
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
        [Authorize(Roles=Domain.DomainConstants.Admin)]
        public ActionResult Index()
        {
            var model = MemberDetailService.GetBioSummaries();
            return View(model);
        }

        public async Task<ActionResult> AllUserContacts()
        {
            return View(await MemberDetailService.GetAllUserContacts(User.Identity.Name));
        }
        // GET: Bios
        public ActionResult CreateEditBio(int id=0)
        {
            if (id == 0) { id = CurrentUser.Id; }
            else if (!IsAuthorised(CurrentUser.Id))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }
            var model = MemberDetailService.GetBioDetails(id) ?? new BiosViewModel() { UserId=id  };
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
                bool isAdmin = await IsAdminAsync();
                if (bioImg != null)
                {
                    model.BioPicUrl = PhotoServices.processBioImage(bioImg);
                }
                
                await MemberDetailService.UpdateBios(model, ModelState, IsAdmin);
                if (ModelState.IsValid)
                {
                    if (!isAdmin)
                    {
                        await SendEmailsToRoleAsync(Domain.DomainConstants.Admin, new IdentityMessage
                        {
                            Subject = "New H4K bio awaiting approval",
                            Body = string.Format("<p>A biography has been created or updated for <strong>{0}</strong> "
                            + "please go to <a href='{1}' >to approve</a></p>", model.Name,
                               Url.Action("CreateEditBio", "Bios",
                                   routeValues: model.UserId,
                                   protocol: Request.Url.Scheme /* This is the trick */))
                        });
                    }
                    return IsAdmin? RedirectToAction("Index")
                        : RedirectToAction("Index", "Manage", new { message = ManageController.ManageMessageId.UpdateBioSuccess });
                }

            }
            model.IsAdmin = IsAdmin;
            return View(model);
        }
        public ActionResult UpdateDetails(int id=0)
        {
            if (id == 0) { id = CurrentUser.Id; }
            var usr = UserManager.FindById(id);
            if (string.IsNullOrEmpty(usr.PasswordHash) && id==CurrentUser.Id)
            {
               return RedirectToAction("Register","Account");
            }
            if (!IsAuthorised(usr.Id))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }
            var returnView = MemberDetailService.GetMemberDetails(usr.Id) ?? new BioDetailsViewModel { UserId = usr.Id };
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
                    if (model.UserId == CurrentUser.Id && MemberDetailService.BioRequired(model.UserId))
                    {
                        return RedirectToAction("CreateEditBio", model.UserId);
                    }
                    return IsAdmin ? RedirectToAction("Index")
                        : RedirectToAction("Index", "Manage", new { message = ManageController.ManageMessageId.UpdateUserDetailsSuccess });
                }
                
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> UserBios(string name=null)
        {
            var model = await MemberDetailService.GetBiosForDisplay(isMainPage: false);
            return View(model);
        }
        bool IsAuthorised(int id)
        {
            return id == CurrentUser.Id || IsAdmin;
        }
    }
}