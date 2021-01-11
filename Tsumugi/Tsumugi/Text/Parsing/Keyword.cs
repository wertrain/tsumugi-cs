namespace Tsumugi.Text.Parsing
{
    /// <summary>
    /// Tsumugi がシステムで利用するキーワード
    /// </summary>
    class TsumugiKeyword
    {
        /// <summary>
        /// ラベルの開始文字
        /// </summary>
        public const char LabelPrefix = ':';

        /// <summary>
        /// タグの開始文字
        /// </summary>
        public const char TagStart = '[';

        /// <summary>
        /// タグの終了文字
        /// </summary>
        public const char TagEnd = ']';

        /// <summary>
        /// タグ行の開始
        /// </summary>
        public const char TagLineStart = '@';

        /// <summary>
        /// ラベルと見出しの分けるセパレータ
        /// </summary>
        public const char HeadlineSeparator = '|';

        /// <summary>
        /// タグと属性のセパレータ
        /// </summary>
        public const char TagAttributeSeparator = ' ';

        /// <summary>
        /// 割り当て記号
        /// </summary>
        public const char Assignment = '=';
    }

    /// <summary>
    /// Tsumugi のタグ名
    /// </summary>
    class TsumugiTag
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
}
