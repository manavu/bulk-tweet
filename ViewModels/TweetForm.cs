namespace BulkTweet.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class TweetForm
    {
        [Required]
        public string Text { get; set; } = "撮影日: [date]";

        public IEnumerable<IFormFile> UploadFiles { get; set; }
    }
}