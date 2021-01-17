namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// If コマンド
    /// </summary>
    public class IfCommand : CommandBase
    {
        /// <summary>
        /// 式
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="expression"></param>
        public IfCommand(string expression) => Expression = expression;
    }

    /// <summary>
    /// Else コマンド
    /// </summary>
    public class ElseCommand : CommandBase
    {
    }

    /// <summary>
    /// Elif コマンド
    /// </summary>
    public class ElifCommand : CommandBase
    {
        /// <summary>
        /// 式
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="expression"></param>
        public ElifCommand(string expression) => Expression = expression;
    }

    /// <summary>
    /// EndIf コマンド
    /// </summary>
    public class EndIfCommand : CommandBase
    {
    }
}
