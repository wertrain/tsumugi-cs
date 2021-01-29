namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// 評価埋め込みコマンド
    /// </summary>
    public class EmbedCommand : CommandBase
    {
        /// <summary>
        /// 評価する式
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="expression"></param>
        public EmbedCommand(string expression) => Expression = expression;
    }
}
