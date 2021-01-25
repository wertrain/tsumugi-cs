namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// 評価コマンド
    /// </summary>
    public class EvalCommand : CommandBase
    {
        /// <summary>
        /// 評価する式
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="expression"></param>
        public EvalCommand(string expression) => Expression = expression;
    }
}
