using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.Net.Http;

namespace Company.Function.ReCaptchav2
{
    public class ReCaptchav2Processor
    {


        public static async Task<ReCaptchav2Response> ValidateReponseCode(string clientresponse, string clientIP, ILogger log)
        {

            var url = System.Environment.GetEnvironmentVariable("reCaptchav2.Url");
            var secretkey = System.Environment.GetEnvironmentVariable("reCaptchav2.SecretKey");
            var response = new ReCaptchav2Response()
            {
                ProcessCode = RecaptchaProcessCodes.None
            };

            if (string.IsNullOrWhiteSpace(clientresponse))
                response.ProcessCode = RecaptchaProcessCodes.NoResponse;
            else
            {
                response.ClientResponse = clientresponse;
                var dict = new Dictionary<string, string>();
                dict.Add("secret", secretkey);
                dict.Add("response", clientresponse);
                dict.Add("remoteip", clientIP);

                try
                {

                    var client = new System.Net.Http.HttpClient();

                    var body = await client.PostAsync(url, new FormUrlEncodedContent(dict));
                    body.EnsureSuccessStatusCode();
                    var responseString = body.Content.ReadAsStringAsync().Result;

                    response = JsonConvert.DeserializeObject<ReCaptchav2Response>(responseString.Replace("error-codes", "errorcodes"));
                    response.ClientResponse = clientresponse;
                    if (!response.success)
                    {
                        response.errorcodes = response.errorcodes ?? new List<string>();
                        if (response.errorcodes.Any(x => x == "timeout-or-duplicate"))
                        {
                            response.ProcessCode = RecaptchaProcessCodes.TimeoutOrDuplicate;
                        }
                        else
                        {
                            response.ProcessCode = RecaptchaProcessCodes.OtherGoogleError;
                        }
                        var error = string.Format("reCaptchav2 - Response Error:{0}", responseString);
                        log.LogError(error);
                    }
                    else
                    {
                        response.ProcessCode = RecaptchaProcessCodes.Success;
                    }

                }
                catch (Exception e)
                {
                    response.ProcessCode = RecaptchaProcessCodes.Error;
                    response.ErrorMessage = string.Format("{0} - Inner Exception: {1}", e.Message, e.InnerException == null ? "none" : e.InnerException.Message);
                    var error = string.Format("reCaptchav2: Unknown Error :{0}", e.Message);
                    log.LogError(error);
                }
            }

            return response;
        }
    }
}