using System.Collections.Generic;

namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// If コマンド
    /// </summary>
    public class IfCommand : CommandBase
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
    /// If 分岐に関するコマンドの基底クラス
    /// </summary>
    public class IfBranchCommandBase : CommandBase
    {
        /// <summary>
        /// 紐づく If コマンド
        /// </summary>
        public IfCommand IfCommand { get; set; }

        /// <summary>
        /// If コマンドを保持しているか
        /// </summary>
        public bool HasIfCommand { get { return IfCommand != null; } }

        /// <summary>
        /// 次の分岐コマンドを取得
        /// </summary>
        /// <returns></returns>
        public IfBranchCommandBase GetNextCommand()
        {
            int nextIndex = IfCommand.RelatedCommands.IndexOf(this) + 1;
            if (nextIndex >= IfCommand.RelatedCommands.Count) return null;
            return IfCommand.RelatedCommands[nextIndex];
        }
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
        /// 評価オブジェクトの真偽を判定
        /// </summary>
        /// <param name="evaluated"></param>
        /// <returns></returns>
        public static bool IsTrue(Script.Objects.IObject evaluated)
        {
            // BooleanObject 以外はすべて Flase
            // Null や 0 などの扱いは検討すべき

            if (evaluated is Script.Objects.BooleanObject)
            {
                return (evaluated as Script.Objects.BooleanObject).Value;

            }
            return false;
        }

        /// <summary>
        /// If コマンドの並びをチェック
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queue"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool InspectSequence(IfCommand command, CommandQueue queue, out IfCommand error)
        {
            if (command.RelatedCommands.Count > 0)
            {
                error = null;
                return true;
            }

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
                        if (elseCommand.HasIfCommand) break;
                        elseCommand.IfCommand = command;
                        command.RelatedCommands.Add(elseCommand);
                        break;

                    case ElifCommand elifCommand:
                        if (elifCommand.HasIfCommand) break;
                        elifCommand.IfCommand = command;
                        command.RelatedCommands.Add(elifCommand);
                        break;

                    case EndIfCommand endIfCommand:
                        if (endIfCommand.HasIfCommand) break;
                        endIfCommand.IfCommand = command;
                        command.RelatedCommands.Add(endIfCommand);
                        return null;
                }

                current = queue.GetCommand(++index);
            }

            return command;
        }
    }

}
