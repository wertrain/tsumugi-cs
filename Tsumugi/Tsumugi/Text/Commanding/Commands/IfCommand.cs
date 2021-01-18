namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// If 分岐に関するコマンドの基底クラス
    /// </summary>
    public class IfBranchCommandBase : CommandBase
    {

    }

    /// <summary>
    /// If コマンド
    /// </summary>
    public class IfCommand : IfBranchCommandBase
    {
        /// <summary>
        /// 条件式
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
    public class ElseCommand : IfBranchCommandBase
    {
    }

    /// <summary>
    /// Elif コマンド
    /// </summary>
    public class ElifCommand : IfBranchCommandBase
    {
        /// <summary>
        /// 条件式
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
    public class EndIfCommand : IfBranchCommandBase
    {
    }

    /// <summary>
    /// コマンドユーティリティ
    /// </summary>
    public static class IfCommandUtility
    {
        public static T GetPairCommand<T>(IfBranchCommandBase command, CommandQueue queue) where T : IfBranchCommandBase
        {
            switch (command)
            {
                case IfCommand ifCommand:

                    break;
            }
        }

        private static T RecursiveGetPair<T>(CommandQueue queue)
        {

        }
    }

}
