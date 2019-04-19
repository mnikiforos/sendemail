using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Collections.Generic;

namespace Company.Function
{
    public static class HttpRequestExtensions
    {
        public static async Task<HttpResponseBody<T>> GetBodyAsync<T>(this HttpRequest request)
        {
            var body = new HttpResponseBody<T>();
            var bodyString = await request.ReadAsStringAsync();
            var results = new List<ValidationResult>();
            try
            {
                body.Value = JsonConvert.DeserializeObject<T>(bodyString);
                body.IsValid = Validator.TryValidateObject(body.Value, new ValidationContext(body.Value, null, null), results, true);
            }
            catch (System.Exception ex)
            {
                results.Add(new ValidationResult($"Invalid Json was sent: {ex.Message}"));
            }
            body.ValidationResults = results;

            return body;
        }

        public static string GetRemoteIPAddress(this HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}