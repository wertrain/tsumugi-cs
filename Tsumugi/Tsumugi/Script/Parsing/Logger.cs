﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Parsing
{
    /// <summary>
    /// 標準のロガー兼ベースクラス
    /// </summary>
    public class Logger
    {
        public enum Categories
        {
            Information,
            Warning,
            Error
        }

        private struct History
        {
            public Categories Category { get; set; }
            public string Message { get; set; }
        }

        static Logger()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;
            Localize.Localizer.SetStringLocalizer(new Localize.EmbeddedResourceStringLocalizer());
        }

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
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int Count(Categories category)
        {
            return histories[(int)category].Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        public virtual void Log(Categories category, string message)
        {
            Logging(category, message);
            System.Console.WriteLine("[{0}] {1}", CategoryToString[(int)category], message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        public void Logging(Categories category, string message)
        {
            histories[(int)category].Add(new History
            {
                Category = category,
                Message = message
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        private List<List<History>> histories { get; set; }
    }
}
