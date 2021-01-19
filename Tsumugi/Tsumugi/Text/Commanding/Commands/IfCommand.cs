using System.Collections.Generic;

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
        /// 関連するコマンド（if/elif/else/endif）
        /// </summary>
        public List<IfBranchCommandBase> RelatedCommands { get; set; }

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
        public static bool RelateCommand(IfCommand command, CommandQueue queue)
        {
            command.RelatedCommands.Clear();

            if (RecursiveRelateCommand(command, queue, queue.IndexOf(command) + 1))
            {
                return true;
            }

            return false;
        }

        private static bool RecursiveRelateCommand(IfCommand command, CommandQueue queue, int index)
        {
            var current = queue.GetCommand(index);

            switch (current)
            {
                case IfCommand ifCommand:
                    ifCommand.RelatedCommands.Clear();
                    if (!RecursiveRelateCommand(ifCommand, queue, ++index))
                        return false;
                    break;

                case ElseCommand elseCommand:
                    command.RelatedCommands.Add(elseCommand);
                    RecursiveRelateCommand(command, queue, ++index);
                    break;

                case ElifCommand elifCommand:
                    command.RelatedCommands.Add(elifCommand);
                    RecursiveRelateCommand(command, queue, ++index);
                    break;

                case EndIfCommand endIfCommand:
                    command.RelatedCommands.Add(endIfCommand);
                    return true;
            }

            return false;
        }
    }

}
