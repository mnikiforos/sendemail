using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Company.Function.ReCaptchav2
{
    public enum RecaptchaProcessCodes
    {
        None = 0,
        NoResponse = 1,
        TimeoutOrDuplicate = 2,
        OtherGoogleError = 3,
        Error = 4,
        Success = 5

    }
}