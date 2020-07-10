namespace BulkTweet.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>
    /// HTMLタグに出力条件の属性を付与するタグヘルパー
    /// </summary>
    [HtmlTargetElement(Attributes = nameof(Condition))]
    public class ConditionTagHelper : TagHelper
    {
        /// <summary>
        /// 条件
        /// </summary>
        public bool Condition { get; set; }

        /// <summary>
        /// コンテキストと出力で処理を同期的に実行する
        /// </summary>
        /// <param name="context">HTMLタグに関連付けられたコンテキスト</param>
        /// <param name="output">HTMLタグ生成オブジェクト</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!this.Condition)
            {
                output.SuppressOutput();
            }
        }
    }
}