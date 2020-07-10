namespace BulkTweet
{
    using System;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Newtonsoft.Json;

    /// <summary>
    /// TempData拡張メソッド
    /// </summary>
    /// <remarks>
    /// 直接オブジェクトが保存できないためJSONに変換しています
    /// </remarks>
    public static class TempDataDictionaryExtensions
    {
        /// <summary>
        /// オブジェクトをTempDataに保存
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="tempData">TempDataオブジェクト</param>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// オブジェクトをTempDataから取得
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="tempData">TempDataオブジェクト</param>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        public static T Get<T>(this ITempDataDictionary tempData, string key)
        {
            tempData.TryGetValue(key, out var obj);
            return obj == null ? default(T) : JsonConvert.DeserializeObject<T>(obj.ToString());
        }
    }
}
