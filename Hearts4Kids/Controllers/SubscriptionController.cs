using Hearts4Kids.Helpers;
using Hearts4Kids.Models;
using Hearts4Kids.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Hearts4Kids.Domain.DomainConstants;

namespace Hearts4Kids.Controllers
{
    public class SubscriptionController : BaseUserController
    {
        // GET: Subscription

        public async Task<ActionResult> Unsubscribe(int subscriberId, Guid code, SubscriptionTypes subscribeType)
        {
            var model = new ChangeSubscriptionModel
            {
                Success = await SubscriberServices.Unsubscribe(subscriberId, code, subscribeType),
                CurrentSubscription = subscribeType
            };
            
            return View(model);
        }
        [Authorize(Roles=Admin)]
        public ActionResult CreateReceipt()
        {
            return View(GetNewReceiptModel());
        }
        [Authorize(Roles = Admin), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateReceipt(ReceiptModel model, HttpPostedFileBase logoImg=null)
        {
            if (ModelState.IsValid)
            {
                if (logoImg != null)
                {
                    model.LogoSrc = PhotoServices.processLogo(logoImg);
                }
                await ReceiptServices.CreateReceipt(model, ModelState, (subscriberId, code, subscriptionType) =>
                     Url.Action("Unsubscribe", "Subscription", new { subscriberId = subscriberId, code = code, subscriptionType = subscriptionType }, protocol: Request.Url.Scheme));
                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    return View(GetNewReceiptModel());
                }
            }
            return View(model);
        }
        [Authorize(Roles = Admin),HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> GetEmails(string startsWith)
        {
            return new JsonResult { Data = await SubscriberServices.GetEmails(startsWith) };
        }

        [Authorize(Roles = Admin), HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> GetDonorInfo(string email)
        {
            return new JsonResult { Data = await SubscriberServices.GetDonorInfo(email) };
        }
        static ReceiptModel GetNewReceiptModel()
        {
            var returnVar = new ReceiptModel();
            //returnVar.Countries = EnumHelpers.AsListItem<Countries>();
            //returnVar.TransferMethods = EnumHelpers.AsListItem<DonationTypes>();
            return returnVar;
        }
    }
}