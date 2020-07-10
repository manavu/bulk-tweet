namespace BulkTweet
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// セッション拡張メソッド
    /// </summary>
    /// <remarks>
    /// 直接オブジェクトが保存できないためJSONに変換しています
    /// </remarks>
    public static class ISessionExtensions
    {
        /// <summary>
        /// セッションにオブジェクトを保存
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="session">セッション</param>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public static void Put<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// セッションからオブジェクトを取得
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="session">セッション</param>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        public static T Get<T>(this ISession session, string key)
        {
            if (!session.Keys.Contains(key))
            {
                return default(T);
            }

            var obj = session.GetString(key);
            return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}
