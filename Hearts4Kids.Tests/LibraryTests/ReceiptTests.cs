using Xunit;
using MigraDoc.DocumentObjectModel;
using Hearts4Kids.Services;
using MigraDoc.RtfRendering;
using System;
using System.IO;
using MigraDoc.Rendering;

namespace Hearts4Kids.Tests.LibraryTests
{
    public class ReceiptTests
    {
        [Fact]
        public void CreateRtf()
        {
            
            Document doc = ReceiptServices.CreatePdf(new Domain.Receipt
            {
                Amount = 0.0m,
                 DateReceived = DateTime.Now,
                 DateSent = DateTime.Now,
                 Description = "Pretend receipt/VOID",
                 Id=-1,
                 TransferMethod = Domain.DomainConstants.DonationTypes.DirectBankTransfer,
                 AspNetUser = new Domain.AspNetUser { Id=-1, Email="test@example.com", UserName="testing framework" },
                 NewsletterSubscriber = new Domain.NewsletterSubscriber { Email = "donor@example.com", Id = -1, Name="dummy donor"}
            }, "Hearts4Kids.org.nz", "Void -Auto Generated Test");
            string path = string.Empty;
            //Test Name:'C:\Users\OEM\Documents\Visual Studio 2015\Projects\Hearts4Kids\Hearts4Kids.Tests\bin\Debug\testreceipt.rtf'.
            RtfDocumentRenderer rtfRenderer = new RtfDocumentRenderer();
            rtfRenderer.Render(doc,Path.Combine(path,"testreceipt.rtf"),path);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true) { WorkingDirectory = path, Document = doc };
            pdfRenderer.RenderDocument();
            pdfRenderer.Save(Path.Combine(path, "testreceipt.pdf"));
        }
    }
}
