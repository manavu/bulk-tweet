namespace BulkTweet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using BulkTweet.Models;
    using BulkTweet.ViewModels;
    using Newtonsoft.Json;
    using Tweetinvi;
    using Tweetinvi.Models;
    using Tweetinvi.Parameters;

    [Authorize]
    public class TwitterController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public TwitterController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //var tweets = Timeline.GetUserTimeline(user, 40);

            /*
            var consumerKey = "";
            var consumerSecret = "";
            var accessToken = "";
            var accessTokenSecret = "";

            Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
            // これでツイートはできる
            // Tweet.PublishTweet("Hello World!");

            // ユーザーのタイムラインを取得する。最新の40件
            // var tweets = Timeline.GetUserTimeline("tkintheglobe");

            {
                // Get more control over the request with a UserTimelineParameters
                var userTimelineParameters = new UserTimelineParameters();
                var tweets = Timeline.GetUserTimeline("tkintheglobe", userTimelineParameters);
            }

            // 認証されているユーザーの詳細情報を取得
            var authenticatedUser = Tweetinvi.User.GetAuthenticatedUser();

            {
                // 自身のタイムラインを取得
                var tweets = Timeline.GetHomeTimeline();
            }*/

            return View();
        }

        [HttpPost]
        public IActionResult Index(TweetForm form)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Index", "Home");
            }

            // 認証情報から Twitter の資格情報を取得
            var rawUserCreds = this.User.Claims.Single(m => m.Type == ClaimTypes.UserData).Value;
            var userCreds = JsonConvert.DeserializeObject<TwitterCredentials>(rawUserCreds);

            // そのままではツイートできないので資格情報をセットする
            Auth.SetCredentials(userCreds);

            var data = new List<(DateTime Shot, byte[] Binaries)>();

            // アップロードされたファイルを扱いやすい形式に変換
            foreach (var uploadFile in form.UploadFiles)
            {
                using var fs = uploadFile.OpenReadStream();
                using var ms = new MemoryStream();
                fs.CopyTo(ms);

                fs.Seek(0, SeekOrigin.Begin);
                using var bmp = Bitmap.FromStream(fs);
                var shot = bmp.GetShotDateTime() ?? DateTime.Now;

                data.Add((shot, ms.ToArray()));
            }

            // 撮影日順にデータを並べてからツイート
            foreach (var datum in data.OrderBy(m => m.Shot))
            {
                if (this.HttpContext.RequestAborted.IsCancellationRequested)
                {
                    break;
                }

                var publishOptions = new PublishTweetOptionalParameters();
                publishOptions.MediaBinaries.Add(datum.Binaries);

                var tweet = string.IsNullOrEmpty(form.Text) ? "[date]" : form.Text;
                tweet = tweet.Replace("[date]", datum.Shot.ToString("d"));

                Tweet.PublishTweet(tweet, publishOptions);
            }

            this.Notice("ツイートが完了しました");
            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Thumb()
        {
            // 認証情報から Twitter の資格情報を取得
            var rawUserCreds = this.User.Claims.Single(m => m.Type == ClaimTypes.UserData).Value;
            var userCreds = JsonConvert.DeserializeObject<TwitterCredentials>(rawUserCreds);

            // そのままではツイートできないので資格情報をセットする
            Auth.SetCredentials(userCreds);
            var user = Tweetinvi.User.GetAuthenticatedUser();

            return this.File(user.GetProfileImageStream(ImageSize.mini), "image/jpg");
        }

        public IActionResult Test()
        {
            var data = new List<(DateTime Shot, byte[] Binaries, string FilePath)>();

            var path = @"D:\ftproot2\1024600";
            foreach (var filePath in Directory.EnumerateFiles(path, "*.png"))
            {
                var fileInfo = new System.IO.FileInfo(filePath);

                using var fs = System.IO.File.OpenRead(filePath);
                using var ms = new MemoryStream();
                fs.CopyTo(ms);

                data.Add((fileInfo.LastWriteTime, ms.ToArray(), filePath));
            }

            // そのままではツイートできないので資格情報をセットする
            var userCreds = this.HttpContext.Session.Get<TwitterCredentials>("UserCreds");
            Auth.SetCredentials(userCreds);

            // 撮影日順にデータを並べてからツイート
            foreach (var datum in data.OrderBy(m => m.Shot))
            {
                if (this.HttpContext.RequestAborted.IsCancellationRequested)
                {
                    break;
                }

                var publishOptions = new PublishTweetOptionalParameters();
                publishOptions.MediaBinaries.Add(datum.Binaries);

                // ツイートが成功するとそのオブジェクトが返ってくる
                var tweet = Tweet.PublishTweet(datum.Shot.ToString("d"), publishOptions);
                if (tweet != null)
                {
                    System.IO.File.Delete(datum.FilePath);
                }
                else
                {
                    return this.Content("failed.");
                }
            }

            return this.Content("succeeded");
        }
    }
}
