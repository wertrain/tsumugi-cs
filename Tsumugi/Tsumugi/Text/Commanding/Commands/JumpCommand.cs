namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// ジャンプコマンド
    /// </summary>
    public class JumpCommand : CommandBase
    {
        /// <summary>
        /// ジャンプ先のラベル名
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="target"></param>
        public JumpCommand(string target) => Target = target;
    }
}
