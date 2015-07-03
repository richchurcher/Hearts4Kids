using Hearts4Kids.Domain;
using Hearts4Kids.Extensions;
using Hearts4Kids.Models;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using static Hearts4Kids.Domain.DomainConstants;

namespace Hearts4Kids.Services
{
    public static class ReceiptServices
    {
        public static async Task CreateReceipt(ReceiptModel receipt, ModelStateDictionary modelState, Func<int, Guid, SubscriptionTypes, string> getLink)
        {
            using (var db = new Hearts4KidsEntities())
            {
                if (await db.Receipts.AnyAsync(r=>r.DateReceived==receipt.DateReceived && r.NewsletterSubscriber.Email == receipt.Email))
                {
                    modelState.AddModelError("",string.Format("A receipt already exists for {0} on {1:dd/MM/yy}",receipt.Email, receipt.DateReceived));
                    return;
                }
                var s = await db.NewsletterSubscribers.FirstOrDefaultAsync(n => n.Email == receipt.Email);
                if (s == null)
                {
                    s = new NewsletterSubscriber { Email = receipt.Email, UnsubscribeToken = Guid.NewGuid(), Subscription=SubscriptionTypes.FullSubscription };
                    db.NewsletterSubscribers.Add(s);
                }
                if (receipt.Name != null) { s.Name = receipt.Name; }
                if (receipt.Address != null)
                {
                    s.Address = receipt.Address;
                }
                Receipt rcpt = new Receipt
                {
                    DateReceived = receipt.DateReceived.Value,
                    Amount = receipt.Amount,
                    DateSent = DateTime.Now,
                    TransferMethod = receipt.TransferMethodId.Value,
                    NewsletterSubscriber = s,
                    Description = receipt.Description
                };
                db.Receipts.Add(rcpt);
                if (receipt.IsOrganisation)
                {
                    CorporateSponsor c=null;
                    if (s.Id != 0)
                    {
                        c = await db.CorporateSponsors.FindAsync(s.Id);
                    }
                    if (c == null)
                    {
                        c = new CorporateSponsor();
                        c.NewsletterSubscriber = s;
                        db.CorporateSponsors.Add(c);
                    }
                    c.WebUrl = receipt.WebUrl;
                    c.LogoUrl = receipt.LogoSrc;
                }
                await db.SaveChangesAsync();
                string author = await (from u in db.AspNetUsers
                                      where u.UserName == HttpContext.Current.User.Identity.Name
                                      select u.UserBio==null?u.UserName:(u.UserBio.FirstName + " " + u.UserBio.Surname)).FirstAsync();
                string baseUr = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                Thread emailThread = new Thread(() => SendReceipt(rcpt, baseUr,author));
                emailThread.Start();
            }
        }
        public static void SendReceipt(Receipt receipt, string author, string baseUr)
        {
            var m = new MailMessage
            {
                Subject = "Thank you & receipt for your generous donation to Hearts4Kids",
                Body = "<p>Thank you very much for your donation.</p>"
                + "<p>We appreciate there are many other organisations deserving of your generosity. Hearts4Kids is a dedicated team "
                + "that has the privilege to use our specialist expertise and time to reach "
                + "some of the poorest children in the world and give them the opportunity to lead active and full lives. "
                + "Thank you once again for helping us make it happen.</p><p>Please see attached receipt.</p>"
                + "<p>Regards</p><p>The Hearts4Kids Team.</p>"
                + "<hr/>" + SubscriberServices.getUnsubscribeDetails(receipt.NewletterSubscriberId, receipt.NewsletterSubscriber.UnsubscribeToken, receipt.NewsletterSubscriber.Subscription, baseUr),
                IsBodyHtml = true
            };
            m.To.Add(receipt.NewsletterSubscriber.Email);
            using (var stream = new MemoryStream())
            {
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                Document doc = renderer.Document = CreatePdf(receipt, baseUr, author);
                renderer.RenderDocument();
                var security = renderer.PdfDocument.SecuritySettings;
                security.PermitExtractContent = security.PermitModifyDocument = security.PermitFormsFill = false;
                renderer.Save(stream, false);
                stream.Position = 0;
                Attachment pdf = new Attachment(stream, new ContentType(MediaTypeNames.Application.Pdf)) { Name = doc.Info.Title };

                m.Attachments.Add(pdf);
                using (var client = new SmtpClient())
                {
                    client.Send(m);
                }
            }
                
        }

        public static Document CreatePdf(Receipt receipt, string siteUrl, string author)
        {
            Document document = new Document();
            string date = receipt.DateReceived.ToString("dd MMM yyyy");
            document.Info.Title = "Hearts4Kids receipt " + date;
            document.Info.Subject = "Receipt for your donation on " + date;
            document.Info.Author = author;
            document.DefaultPageSetup.PageFormat = PageFormat.A4;

            Section section = DefineLetterHead(document, siteUrl);
            DefineReceipt(section, receipt);
            return document;
        }

        
        public static Section DefineLetterHead(Document document, string url)
        {
            Section section = document.AddSection();

            //create header
            Image image = section.Headers.Primary.AddImage(HostingEnvironment.MapPath("~/Content/Logos/Full2.png"));
            image.LockAspectRatio = true;
            image.Width = "10cm";
            image.Left = ShapePosition.Center;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.WrapFormat.Style = WrapStyle.Through;

            //create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph("page 1 of 1");
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            // Create the text frame for the address
            var addressFrame = section.AddTextFrame();
            addressFrame.Height = "2.0cm";
            addressFrame.Width = "5.0cm";
            addressFrame.Left = ShapePosition.Right;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Page;
            addressFrame.Top = "2.2cm";
            addressFrame.RelativeVertical = RelativeVertical.Margin;

            // Put sender in address frame
            paragraph = addressFrame.AddParagraph("Hearts4Kids Trust\n\n"
                +"PO Box 5522,\n"
                +"Wellesley Street,\n"
                +"Auckland 1010\n\n");
            var link = paragraph.AddHyperlink(url,HyperlinkType.Url);
            link.AddFormattedText(url.Substring(7)); //http://
            link.Font.Color = Colors.Blue;
            link.Font.Underline = Underline.Single;

            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddDateField("dd MMMM yyyy");
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.SpaceAfter = 3;
            return section;
        }
        const int GoodsInKindId = 3;
        public static void DefineReceipt(Section section, Receipt receipt)
        {
            Paragraph paragraph = section.AddParagraph();

            //paragraph.Style = "Reference";
            paragraph.Format.SpaceAfter = "1cm";
            paragraph.Format.SpaceBefore = "6cm";
            var subscriber = receipt.NewsletterSubscriber;
            if (!string.IsNullOrEmpty(subscriber.Name)) { paragraph.AddText(subscriber.Name + "\n\n"); }

            paragraph.AddText(subscriber.Address + '\n');

            if (!(new Countries[] { Countries.Unknown, Countries.Other }).Any(s=>s==subscriber.Country))
            {
                var country = subscriber.Country.ToString().SplitCamelCase();
                if (subscriber.Address.IndexOf(country, StringComparison.InvariantCultureIgnoreCase) == -1){
                    paragraph.AddText(country);
                }
            }
            paragraph.AddLineBreak();

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceAfter = "1cm";
            paragraph.AddFormattedText("RECEIPT", TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            paragraph = section.AddParagraph();
            string goodsDescription;
            switch (receipt.TransferMethod)
            {
                case DonationTypes.GoodsInKind:
                    goodsDescription = "goods in kind to the value of";
                    break;
                case DonationTypes.DiscountedGoods:
                    goodsDescription = "discounted goods to the value of";
                    break;
                default:
                    goodsDescription = "a donation of";
                    break;
            }
            paragraph.AddText(string.Format("Hearts4Kids trust has gratefully received {0} {1:c} on the {2:dd MMM yyyy}.",
                goodsDescription,
                receipt.Amount, receipt.DateReceived));

            paragraph = section.AddParagraph();
            paragraph.AddFormattedText("Receipt Number:\t", TextFormat.Bold);
            paragraph.AddText(receipt.Id.ToString());
            //paragraph.Format.Font.Size = 12;
            paragraph.Format.SpaceBefore = paragraph.Format.SpaceAfter = "1cm";
            paragraph.Format.TabStops.Clear();
            paragraph.Format.TabStops.AddTabStop(new TabStop("4cm"));
            var stops = paragraph.Format.TabStops.Clone();

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = paragraph.Format.SpaceAfter = "1cm";
            paragraph.Format.TabStops = stops;
            paragraph.AddFormattedText("Donor:\t", TextFormat.Bold);

            paragraph.AddText(string.IsNullOrEmpty(subscriber.Name) ? subscriber.Email : (subscriber.Name + " (" + subscriber.Email + ')'));
            if (!string.IsNullOrEmpty(subscriber.Address))
            {
                paragraph.AddText('\n' + string.Join("\n",
                    subscriber.Address
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s=>'\t'+s.Trim())));
            }
            if (!string.IsNullOrEmpty(receipt.Description))
            {
                paragraph.AddFormattedText("Description:\t", TextFormat.Bold);

                paragraph.AddText(string.Join("\n",
                    receipt.Description
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => '\t' + s.Trim())));
            }
            //add signature
            paragraph = section.AddParagraph("With Thanks,");
            paragraph.Format.SpaceBefore = paragraph.Format.SpaceAfter = "1cm";

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = paragraph.Format.SpaceAfter = "1cm";
            var image = paragraph.AddImage(HostingEnvironment.MapPath("~/Content/Photos/Kate signature.png"));
            image.LockAspectRatio = true;
            image.Width = "5cm";
            image.Left = ShapePosition.Left;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.WrapFormat.Style = WrapStyle.Through;

            paragraph = section.AddParagraph("Kate Farmer");
            paragraph = section.AddParagraph();
            paragraph.AddFormattedText("Hearts4Kids Trustee", TextFormat.Italic);
        }
    }
}