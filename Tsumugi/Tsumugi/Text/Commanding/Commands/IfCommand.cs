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
        public List<IfBranchCommandBase> RelatedCommands { get; set; } = new List<IfBranchCommandBase>();

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
    /// If コマンドユーティリティ
    /// </summary>
    public static class IfCommandUtility
    {
        /// <summary>
        /// If コマンドの並びをチェック
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queue"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool InspectSequence(IfCommand command, CommandQueue queue, out IfCommand error)
        {
            command.RelatedCommands.Clear();

            if ((error = RecursiveSequenceCommands(command, queue, queue.IndexOf(command) + 1)) == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// If コマンドの対応が正しいかチェック
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool InspectRelated(IfCommand command)
        {
            if (command.RelatedCommands.Count == 0)
            {
                return false;
            }

            int elseCount = 0;
            int lastIndex = command.RelatedCommands.Count - 1;

            for (int index = 0; index < command.RelatedCommands.Count; ++index)
            {
                if (command.RelatedCommands[index] is ElseCommand)
                {
                    // else は一つしかない
                    if (++elseCount > 1)
                    {
                        return false;
                    }
                    // else が置かれるのは endif の一つ前だけ
                    if (index != lastIndex - 1)
                    {
                        return false;
                    }
                }
            }

            return command.RelatedCommands[lastIndex] is EndIfCommand;
        }

        /// <summary>
        /// If コマンドの並びを再帰的にチェック
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static IfCommand RecursiveSequenceCommands(IfCommand command, CommandQueue queue, int index)
        {
            var current = queue.GetCommand(index);

            while (current != null)
            {
                switch (current)
                {
                    case IfCommand ifCommand:
                        ifCommand.RelatedCommands.Clear();
                        if (RecursiveSequenceCommands(ifCommand, queue, ++index) != null)
                            return ifCommand;
                        break;

                    case ElseCommand elseCommand:
                        command.RelatedCommands.Add(elseCommand);
                        break;

                    case ElifCommand elifCommand:
                        command.RelatedCommands.Add(elifCommand);
                        break;

                    case EndIfCommand endIfCommand:
                        command.RelatedCommands.Add(endIfCommand);
                        return null;
                }

                current = queue.GetCommand(++index);
            }

            return command;
        }
    }

}
