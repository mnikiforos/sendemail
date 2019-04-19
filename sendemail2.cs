using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Company.Function
{
    public static class sendemail2
    {
        [FunctionName("sendemail2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");


            var body = await req.GetBodyAsync<CommentCard>();
            if (!(body?.IsValid ?? false))
                return new BadRequestObjectResult($"Model is invalid: {string.Join(", ", body.ValidationResults.Select(s => s.ErrorMessage).ToArray())}");

            var commentCard = body.Value;

            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);
            //var cstTime = UtcToCst(DateTime.UtcNow).ToLongTimeString();
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("test@chicagotailor.com", "Chicago Tailor"),
                Subject = $"{commentCard.name} has a question/comment!",
                PlainTextContent = "Hello, Email!",
                HtmlContent = $@"Hello,<br/><strong>{commentCard.name}</strong> has a question/comment:<br/>{commentCard.comment}.<br/><br/><br/>Below is contact info for {commentCard.name}.<br/>Emai: {commentCard.email}<br/>Phone: {commentCard.phone}<br/>",
                ReplyTo = new EmailAddress(commentCard.email, commentCard.name)
            };
            msg.AddTo(new EmailAddress("mnn414@gmail.com", "Test User"));
            var response = await client.SendEmailAsync(msg);

            return response?.StatusCode == HttpStatusCode.Accepted ?
                (ActionResult)new OkObjectResult($"Email sent") :
                new BadRequestObjectResult("Bad request");

        }
        // public static DateTime UtcToCst(DateTime timeUtc)
        // {
        //     TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        //     return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
        // }
    }
}