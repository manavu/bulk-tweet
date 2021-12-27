namespace BulkTweet
{
    using System;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// セッション拡張メソッド
    /// </summary>
    /// <remarks>
    /// 直接オブジェクトが保存できないためJSONに変換しています
    /// </remarks>
    public static class IImageExtensions
    {
        /// <summary>
        /// 画像のEXIFから撮影日を取得する
        /// </summary>
        /// <param name="self">Image オブジェクト自身</param>
        /// <returns>EXIFの撮影日</returns>
        public static DateTime? GetShotDateTime(this Image self)
        {
            DateTime? shot = null;

            // 撮影時間/変更時間を取得
            var infoShot = self.PropertyItems
                .Where(m => m.Type == 2 && m.Id == 0x9003)
                .Select(m => m.Value)
                .SingleOrDefault();
            var shotDate = GetDateTimeFromByte(infoShot);

            var infoModify = self.PropertyItems
                .Where(m => m.Type == 2 && m.Id == 0x0132)
                .Select(m => m.Value)
                .SingleOrDefault();
            var modifiedDate = GetDateTimeFromByte(infoModify);

            // 撮影日情報を取得できなかった場合、変更日時を撮影日とする
            shot = shotDate == null ? modifiedDate : shotDate;

            //// ※メモ
            //// EXIFに関する情報はここを参照
            //// http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/EXIF.html

            return shot;
        }

        /// <summary>
        /// Byte オブジェクトの配列を日付に変換する
        /// </summary>
        /// <param name="values">ASCII 形式でエンコードされた Byte オブジェクトの配列</param>
        /// <returns>日付</returns>
        private static DateTime? GetDateTimeFromByte(byte[] values)
        {
            DateTime? date = null;

            if (values == null)
            {
                return date;
            }

            var val = System.Text.Encoding.ASCII.GetString(values);
            val = val.Trim(new char[] { '\0' });

            //// ※注意
            //// 稀にフォーマット違いがあるため強制的に取得する文字数を限定させる

            val = val.Substring(0, 19);

            // 値が正常に入っていない場合のエラーを回避
            if (val != "0000:00:00 00:00:00")
            {
                date = DateTime.ParseExact(val, "yyyy:MM:dd HH:mm:ss", null);
            }

            return date;
        }
    }
}
