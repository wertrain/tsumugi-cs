using System;
using System.Collections.Generic;

namespace Tsumugi.Log
{
    /// <summary>
    /// 標準のロガー兼ベースクラス
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// ログのカテゴリ
        /// 
        /// </summary>
        public enum Categories
        {
            Information,
            Warning,
            Error
        }

        /// <summary>
        /// 履歴
        /// </summary>
        private struct History
        {
            public Categories Category { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static Logger()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;
            Localize.Localizer.SetStringLocalizer(new Localize.EmbeddedResourceStringLocalizer());
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Logger()
        {
            histories = new List<List<History>>();
            foreach (Categories value in Enum.GetValues(typeof(Categories)))
            {
                histories.Add(new List<History>());
            }
        }

        /// <summary>
        /// Categories 定義から文字列に変換するテーブル
        /// （enum Attribute に変更できるが数が少ないのでテーブルに）
        /// </summary>
        private static readonly List<string> CategoryToString = new List<string>()
        {
            "Information", "Warning", "Error"
        };

        /// <summary>
        /// 指定されたカテゴリのログ数を取得
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns>カテゴリのログ数</returns>
        public int Count(Categories category)
        {
            return histories[(int)category].Count;
        }

        /// <summary>
        /// すべてのカテゴリのログの総数を取得
        /// </summary>
        /// <returns>ログの総数</returns>
        public int Count()
        {
            int count = 0;
            foreach (var history in histories)
            {
                count += history.Count;
            }
            return count;
        }

        /// <summary>
        /// ログを表示
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <param name="message">表示するログ</param>
        public virtual void Log(Categories category, string message)
        {
            Logging(category, message);
            System.Console.WriteLine("[{0}] {1}", CategoryToString[(int)category], message);
        }

        /// <summary>
        /// ログを記録
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <param name="message">記録するログ</param>
        public void Logging(Categories category, string message)
        {
            histories[(int)category].Add(new History
            {
                Category = category,
                Message = message
            });
        }

        /// <summary>
        /// 指定されたカテゴリの履歴を一行にして取得
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns>改行で区切られたログ履歴</returns>
        public string GetHistory(Categories category)
        {
            var messages = new List<string>();
            foreach(var history in histories[(int)category])
            {
                messages.Add(string.Format("[{0}] {1}", CategoryToString[(int)history.Category], history.Message));
            }
            return string.Join(System.Environment.NewLine, messages.ToArray());
        }

        /// <summary>
        /// すべてのカテゴリの履歴を取得
        /// </summary>
        /// <returns>改行で区切られたログ履歴</returns>
        public string GetHistory()
        {
            var messages = new List<string>();
            foreach (Categories category in Enum.GetValues(typeof(Categories)))
            {
                foreach (var history in histories[(int)category])
                {
                    messages.Add(string.Format("[{0}] {1}", CategoryToString[(int)history.Category], history.Message));
                }
            }
            return string.Join(System.Environment.NewLine, messages.ToArray());
        }

        /// <summary>
        /// 指定されたカテゴリの履歴を取得
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns>カテゴリの履歴</returns>
        public IEnumerable<string> GetHistories(Categories category)
        {
            foreach (var history in histories[(int)category])
            {
                yield return string.Format("[{0}] {1}", CategoryToString[(int)history.Category], history.Message);
            }
        }

        /// <summary>
        /// すべてのカテゴリの履歴を取得
        /// </summary>
        /// <returns>履歴</returns>
        public IEnumerable<string> GetHistories()
        {
            foreach (Categories category in Enum.GetValues(typeof(Categories)))
            {
                foreach (var history in histories[(int)category])
                {
                    yield return string.Format("[{0}] {1}", CategoryToString[(int)history.Category], history.Message);
                }
            }
        }

        /// <summary>
        /// カテゴリごとの履歴
        /// </summary>
        private List<List<History>> histories { get; set; }
    }
}
