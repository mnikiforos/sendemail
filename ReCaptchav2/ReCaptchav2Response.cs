using System.Collections.Generic;
using System;

namespace Company.Function.ReCaptchav2
{
    public class ReCaptchav2Response
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        public List<string> errorcodes { get; set; }
        public RecaptchaProcessCodes ProcessCode { get; set; }
        public string ClientResponse { get; set; }
        public string ErrorMessage { get; set; }
    }
}