namespace Tsumugi.Text.Executing
{
    /// <summary>
    /// コマンド実行の基底クラス
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// 文字列の表示
        /// </summary>
        /// <param name="text"></param>
        void PrintText(string text);

        /// <summary>
        /// 改行
        /// </summary>
        void StartNewLine();

        /// <summary>
        /// キー入力待ち
        /// </summary>
        void WaitAnyKey();

        /// <summary>
        /// 改ページ
        /// </summary>
        void StartNewPage();

        /// <summary>
        /// 指定時間の待ち
        /// </summary>
        void WaitTime(int millisec);

        /// <summary>
        /// フォント設定
        /// </summary>
        /// <param name="font"></param>
        void SetFont(Font font);

        /// <summary>
        /// デフォルトフォント設定
        /// </summary>
        /// <param name="font"></param>
        void SetDefaultFont(Font font);
    }
}
