using System.Collections.Generic;

namespace Tsumugi.Text.Parsing
{
    /// <summary>
    /// タグ名
    /// </summary>
    class TagName
    {
        /// <summary>
        /// キー入力待ち
        /// </summary>
        public const string WaitKey = "l";

        /// <summary>
        /// 改行
        /// </summary>
        public const string NewLine = "r";

        /// <summary>
        /// 改ページ
        /// </summary>
        public const string NewPage = "cm";

        /// <summary>
        /// 指定時間待ち
        /// </summary>
        public const string WaitTime = "wait";

        /// <summary>
        /// 変数定義
        /// </summary>
        public const string DefineVariable = "var";

        /// <summary>
        /// インデント開始
        /// </summary>
        public const string IndentStart = "indent";

        /// <summary>
        /// インデント終了
        /// </summary>
        public const string IndentEnd = "endindent";

        /// <summary>
        /// ジャンプ
        /// </summary>
        public const string Jump = "jump";
    }

    /// <summary>
    /// タグ
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// 属性
        /// </summary>
        public class Attribute
        {
            /// <summary>
            /// 属性名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 属性の値
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// タグの名前
        /// </summary>
        public string Name;

        /// <summary>
        /// タグの属性
        /// </summary>
        public List<Tag.Attribute> Attributes;
    }
}
