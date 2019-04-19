using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace Company.Function
{
    [Serializable]
    public class CommentCard
    {
        [Required]
        public string responseCode { get; set; }
        
        [Required]
        [StringLength(70, MinimumLength = 4)]
        [RegularExpression(@"^(?:\b\w+\b[\s'""`~\-]*){2,5}$", ErrorMessage="Please enter a valid full name")]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid US phone number or leave it null")]
        public string phone { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 20)]
        [RegularExpression(@"^(?:\b\w+\b[\s\r\n\['"".,\/#!$%\^&\*;:\-_`~()\]]*){10,50}$", ErrorMessage = "Comment should be between 10 and 50 words.")]
        public string comment { get; set; }

    }
}