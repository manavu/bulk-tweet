namespace BulkTweet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using BulkTweet.Models;
    using Tweetinvi;
    using Tweetinvi.Models;
    using Tweetinvi.Parameters;
    using Newtonsoft.Json;

    public class AccountController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public AccountController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var consumerKey = _configuration["Twitter:ConsumerApiKey"];
            var consumerSecret = _configuration["Twitter:ConsumerApiSecret"];

            var appCreds = new ConsumerCredentials(consumerKey, consumerSecret);

            // Twitter で認証後にリダイレクトされるURL
            var redirectURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Account/ValidateAuth";
            // Twitter にリクエストを送信して認証画面のURLを取得
            var authenticationContext = AuthFlow.InitAuthentication(appCreds, redirectURL);

            // Twitter の認証画面へのリダイレクト
            return this.Redirect(authenticationContext.AuthorizationURL);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateAuth()
        {
            // 認証後のリダイレクトURLにパラメータがついているのでそれを保持
            if (!(Request.Query.ContainsKey("oauth_verifier") && Request.Query.ContainsKey("authorization_id")))
            {
                return this.RedirectToAction("Login");
            }

            var verifierCode = Request.Query["oauth_verifier"];
            var authorizationId = Request.Query["authorization_id"];

            // 承認IDと検証者コードから資格情報を取得
            var userCreds = AuthFlow.CreateCredentialsFromVerifierCode(verifierCode, authorizationId);
            var user = Tweetinvi.User.GetAuthenticatedUser(userCreds);

            // セッションに資格情報を保持
            this.HttpContext.Session.Put("UserCreds", userCreds);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("FullName", user.ScreenName),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userCreds)),   // 資格情報をクレーム
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                // 認証クッキーの有効時間。CookieAuthenticationOptions.ExpireTimeSpan をオーバーライドする
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15),

                // false の場合はブラウザを閉じた時点でクッキーは消える
                IsPersistent = true,

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return this.RedirectToAction("Index", "Home");
        }
    }
}
