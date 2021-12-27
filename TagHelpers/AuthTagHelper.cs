namespace BulkTweet.TagHelpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>
    /// HTMLタグに出力条件の属性を付与するタグヘルパー
    /// </summary>
    [HtmlTargetElement(Attributes = AspAuthAttributeName)]
    public class AuthTagHelper : TagHelper
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public AuthTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 認証必須
        /// </summary>
        [HtmlAttributeName(AspAuthAttributeName)]
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// HttpContext Accessor
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// タグの属性名
        /// </summary>
        private const string AspAuthAttributeName = "asp-auth";

        /// <summary>
        /// コンテキストと出力で処理を同期的に実行する
        /// </summary>
        /// <param name="context">HTMLタグに関連付けられたコンテキスト</param>
        /// <param name="output">HTMLタグ生成オブジェクト</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (this.IsAuthenticated != _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                output.SuppressOutput();
            }
        }
    }
}