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
using Company.Function.ReCaptchav2;

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
            var captchaResponse = await ReCaptchav2Processor.ValidateReponseCode(commentCard.responseCode, req.GetRemoteIPAddress(), log);


            if (captchaResponse == null || captchaResponse.ProcessCode == RecaptchaProcessCodes.Error)
                return new BadRequestObjectResult($"Bad recaptcha request -- error: {captchaResponse.ErrorMessage}");

            else if (captchaResponse.ProcessCode == RecaptchaProcessCodes.TimeoutOrDuplicate)
                return new BadRequestObjectResult($"Bad recaptcha request --Timeout occurred. Please refresh page and try again.");
            else if (captchaResponse.ProcessCode != RecaptchaProcessCodes.Success)
                return new BadRequestObjectResult($"Bad recaptcha request other captcha error -- {string.Join(',', captchaResponse.errorcodes)}");


            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);
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
    }
}