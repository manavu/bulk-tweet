namespace BulkTweet.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class BaseController : Controller
    {
        /// <summary>
        /// アクション間で共有するメッセージを設定
        /// </summary>
        /// <param name="message">共有したいメッセージ</param>
        protected void Notice(string message)
        {
            this.TempData["TempDataNotice"] = message;
        }

        /// <summary>
        /// アクションが実行される前に呼ばれる
        /// </summary>
        /// <param name="context">コンテキスト</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as BaseController;

            controller.ViewBag.Notice = controller.TempData["TempDataNotice"];
        }
    }
}