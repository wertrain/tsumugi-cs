namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// テキスト表示コマンド
    /// </summary>
    public class PrintTextCommand : CommandBase
    {
        /// <summary>
        /// 表示するテキスト
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text"></param>
        public PrintTextCommand(string text) => Text = text;
    }
}
