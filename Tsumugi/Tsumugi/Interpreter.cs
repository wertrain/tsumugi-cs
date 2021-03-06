using System;
using System.Collections.Generic;

namespace Tsumugi
{
    /// <summary>
    /// Tsumugi インタプリター
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// コマンド実行クラス
        /// </summary>
        public Text.Executing.ICommandExecutor Executor { set { Executors.Add(value); } }

        /// <summary>
        /// コマンド実行クラス
        /// </summary>
        public List<Text.Executing.ICommandExecutor> Executors { get; set; }

        /// <summary>
        /// 警告表示イベント
        /// </summary>
        public event EventHandler<string> OnPrintWarning;

        /// <summary>
        /// エラー表示イベント
        /// </summary>
        public event EventHandler<string> OnPrintError;

        /// <summary>
        /// 実行環境
        /// </summary>
        private Script.Evaluating.Enviroment Enviroment { get; set; }

        /// <summary>
        /// 評価器
        /// </summary>
        private Script.Evaluating.Evaluator Evaluator { get; set; }

        /// <summary>
        /// ロガー
        /// </summary>
        private Log.Logger Logger { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Interpreter()
        {
            Enviroment = new Script.Evaluating.Enviroment();
            Evaluator = new Script.Evaluating.Evaluator();
            Logger = new Log.Logger();
            Executors = new List<Text.Executing.ICommandExecutor>();
            ConditionalSeekStack = new Stack<ConditionalSeekParameter>();
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public bool Execute(string script)
        {
            var lexer = new Text.Lexing.Lexer(script);
            var parser = new Text.Parsing.Parser(lexer);
            parser.Logger = Logger;

            var commandQueue = parser.ParseProgram();

            if (Logger.Count() > 0)
            {
                foreach (var warning in parser.Logger.GetHistories(Log.Logger.Categories.Warning))
                {
                    OnPrintWarning(this, warning);
                }

                bool hasError = true;
                foreach (var error in parser.Logger.GetHistories(Log.Logger.Categories.Error))
                {
                    OnPrintError(this, error);
                    hasError = true;
                }
                if (hasError) return false;
            }

            ExecuteCommands(commandQueue);

            return true;
        }

        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private int ExecuteCommands(Text.Commanding.CommandQueue queue)
        {
            Text.Commanding.CommandBase command = null;

            while ((command = queue.Dequeue()) != null)
            {
                if (ConditionalCommandSeek(queue, command)) continue;

                foreach (var executor in Executors)
                {
                    switch (command)
                    {
                        case Text.Commanding.Commands.PrintTextCommand cmd:
                            executor.PrintText(cmd.Text);
                            break;

                        case Text.Commanding.Commands.NewLineCommand cmd:
                            executor.StartNewLine();
                            break;

                        case Text.Commanding.Commands.WaitKeyCommand cmd:
                            executor.WaitAnyKey();
                            break;

                        case Text.Commanding.Commands.NewPageCommand cmd:
                            executor.StartNewPage();
                            break;

                        case Text.Commanding.Commands.DefineVariablesCommand cmd:
                            DefineVariables(cmd);
                            break;

                        case Text.Commanding.Commands.WaitTimeCommand cmd:
                            if (ResolveVariableReferences<int>(cmd.Time))
                            {
                                executor.WaitTime(cmd.Time.GetValueOrDefault());
                            }
                            break;

                        case Text.Commanding.Commands.JumpCommand cmd:
                            var labels = queue.FindCommands<Text.Commanding.Commands.LabelCommand>();
                            var label = labels.Find(c => c.Name == cmd.Target);
                            queue.Seek(label);
                            break;

                        case Text.Commanding.Commands.IfCommand cmd:
                            if (Text.Commanding.Commands.IfCommandUtility.IsTrue(Eval(cmd.Expression)))
                            {
                                var endIfCommand = cmd.RelatedCommands[cmd.RelatedCommands.Count - 1];
                                ConditionalSeekStack.Push(new ConditionalSeekParameter(cmd.RelatedCommands, endIfCommand));
                            }
                            else
                            {
                                for (int index = 0; index < cmd.RelatedCommands.Count; ++index)
                                {
                                    var next = cmd.RelatedCommands[index];
                                    switch (next)
                                    {
                                        case Text.Commanding.Commands.ElifCommand elifCmd:
                                            if (Text.Commanding.Commands.IfCommandUtility.IsTrue(Eval(elifCmd.Expression)))
                                            {
                                                var endIfCommand = cmd.RelatedCommands[cmd.RelatedCommands.Count - 1];
                                                ConditionalSeekStack.Push(new ConditionalSeekParameter(cmd.RelatedCommands, endIfCommand));
                                                break;
                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        case Text.Commanding.Commands.ElseCommand elseCmd:
                                            {
                                                var endIfCommand = cmd.RelatedCommands[cmd.RelatedCommands.Count - 1];
                                                ConditionalSeekStack.Push(new ConditionalSeekParameter(cmd.RelatedCommands, endIfCommand));
                                            }
                                            break;
                                    }

                                    if (queue.Seek(next))
                                    {
                                        queue.Dequeue();
                                    }
                                    break;
                                }
                            }
                            break;

                        case Text.Commanding.Commands.ElifCommand cmd:
                            if (Text.Commanding.Commands.IfCommandUtility.IsTrue(Eval(cmd.Expression)))
                            {
                                var siblings = cmd.IfCommand.RelatedCommands;
                                var endIfCommand = siblings[siblings.Count - 1];
                                ConditionalSeekStack.Push(new ConditionalSeekParameter(cmd.IfCommand.RelatedCommands, endIfCommand));
                            }
                            else
                            {
                                var siblings = cmd.IfCommand.RelatedCommands;
                                var next = siblings[siblings.IndexOf(cmd) + 1];
                                switch (next)
                                {
                                    case Text.Commanding.Commands.ElifCommand elifCmd:
                                        if (Text.Commanding.Commands.IfCommandUtility.IsTrue(Eval(elifCmd.Expression)))
                                        {
                                            var endIfCommand = siblings[siblings.Count - 1];
                                            ConditionalSeekStack.Push(new ConditionalSeekParameter(siblings, endIfCommand));
                                        }
                                        break;

                                    case Text.Commanding.Commands.ElseCommand elseCmd:
                                        {
                                            var endIfCommand = siblings[siblings.Count - 1];
                                            ConditionalSeekStack.Push(new ConditionalSeekParameter(siblings, endIfCommand));
                                        }
                                        break;
                                }
                                queue.Seek(next);
                            }
                            break;

                        case Text.Commanding.Commands.EndIfCommand cmd:
                            break;

                        case Text.Commanding.Commands.EvalCommand cmd:
                            Eval(cmd.Expression);
                            break;

                        case Text.Commanding.Commands.EmbedCommand cmd:
                            executor.PrintText(Eval(cmd.Expression).Inspect());
                            break;

                        case Text.Commanding.Commands.DefaultFontCommand cmd:
                            executor.SetDefaultFont(new Text.Executing.Font
                            {
                                Size = cmd.Size,
                                Face = cmd.Face,
                                Color = cmd.Color,
                                RubySize = cmd.RubySize,
                                RubyOffset = cmd.RubyOffset,
                                RubyFace = cmd.RubyFace,
                                Shadow = cmd.Shadow,
                                ShadowColor = cmd.ShadowColor,
                                Edge = cmd.Edge,
                                EdgeColor = cmd.EdgeColor,
                                Bold = cmd.Bold,
                            });
                            break;

                        case Text.Commanding.Commands.FontCommand cmd:
                            executor.SetFont(new Text.Executing.Font
                            {
                                Size = cmd.Size,
                                Face = cmd.Face,
                                Color = cmd.Color,
                                RubySize = cmd.RubySize,
                                RubyOffset = cmd.RubyOffset,
                                RubyFace = cmd.RubyFace,
                                Shadow = cmd.Shadow,
                                ShadowColor = cmd.ShadowColor,
                                Edge = cmd.Edge,
                                EdgeColor = cmd.EdgeColor,
                                Bold = cmd.Bold,
                            });
                            break;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Tsumugi スクリプトの評価
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        private Script.Objects.IObject Eval(string script)
        {
            var lexer = new Script.Lexing.Lexer(script);
            var parser = new Script.Parsing.Parser(lexer);
            parser.Logger = Logger;
            var root = parser.ParseProgram();
            return Evaluator.Eval(root, Enviroment);
        }

        /// <summary>
        /// 自動型判定変数定義
        /// </summary>
        /// <param name="command"></param>
        private void DefineVariables(Text.Commanding.Commands.DefineVariablesCommand command)
        {
            foreach (var variable in command.Variables)
            {
                Script.AbstractSyntaxTree.IExpression right = null;

                if (variable.Value.IndexOf(".") >= 0 && double.TryParse(variable.Value, out var d))
                {
                    right = new Script.AbstractSyntaxTree.Expressions.DoubleLiteral() { Value = d };
                }
                else if (int.TryParse(variable.Value, out var n))
                {
                    right = new Script.AbstractSyntaxTree.Expressions.IntegerLiteral() { Value = n };
                }
                else
                {
                    right = new Script.AbstractSyntaxTree.Expressions.StringLiteral() { Value = variable.Value };
                }

                var let = new Script.AbstractSyntaxTree.Statements.LetStatement()
                {
                    Name = new Script.AbstractSyntaxTree.Expressions.Identifier(
                        new Script.Lexing.Token(Script.Lexing.TokenType.Integer, variable.Name), variable.Name
                    ),
                    Value = right
                };

                Evaluator.Eval(let, Enviroment);
            }
        }

        /// <summary>
        /// 変数参照型の参照を解決
        /// </summary>
        /// <typeparam name="T">期待する変数の型</typeparam>
        /// <param name="variable">参照を解決する変数参照型</param>
        /// <returns>参照を解決できれば true</returns>
        private bool ResolveVariableReferences<T>(object variable) where T : struct
        {
            if ((variable as Text.Commanding.ReferenceVariable<T>).HasValue)
                return true;

            switch (variable)
            {
                case Text.Commanding.ReferenceVariable<int> refv:
                    {
                        var (obj, ok) = Enviroment.Get(refv.Name);
                        if (ok && (obj is Script.Objects.IntegerObject))
                        {
                            var var = obj as Script.Objects.IntegerObject;
                            refv.SetValue(var.Value);
                            return true;
                        }
                        break;
                    }

                case Text.Commanding.ReferenceVariable<double> refv:
                    {
                        var (obj, ok) = Enviroment.Get(refv.Name);
                        if (ok && (obj is Script.Objects.DoubleObject))
                        {
                            var var = obj as Script.Objects.DoubleObject;
                            refv.SetValue(var.Value);
                            return true;
                        }
                        break;
                    }

                case Text.Commanding.ReferenceVariable<string> refv:
                    {
                        var (obj, ok) = Enviroment.Get(refv.Name);
                        if (ok && (obj is Script.Objects.StringObject))
                        {
                            var var = obj as Script.Objects.StringObject;
                            refv.SetValue(var.Value);
                            return true;
                        }
                        break;
                    }
            }

            return false;
        }

        /// <summary>
        /// 条件付きシーク
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private bool ConditionalCommandSeek(Text.Commanding.CommandQueue queue, Text.Commanding.CommandBase command)
        {
            if (ConditionalSeekStack.Count == 0) return false;

            var seek = ConditionalSeekStack.Peek();

            foreach (var targetCommand in seek?.TargetCommands)
            {
                if (targetCommand == command)
                {
                    ConditionalSeekStack.Pop();

                    return queue.Seek(seek?.JumpCommand);
                }
            }

            return false;
        }

        /// <summary>
        /// 条件付きシーク用のパラメータ
        /// </summary>
        private class ConditionalSeekParameter
        {
            /// <summary>
            /// 
            /// </summary>
            public List<Text.Commanding.Commands.IfBranchCommandBase> TargetCommands;

            /// <summary>
            /// 
            /// </summary>
            public Text.Commanding.CommandBase JumpCommand;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="commands"></param>
            /// <param name="command"></param>
            public ConditionalSeekParameter(List<Text.Commanding.Commands.IfBranchCommandBase> commands, Text.Commanding.CommandBase command)
            {
                TargetCommands = commands;
                JumpCommand = command;
            }
        }

        /// <summary>
        /// 条件付きシークを保持するスタック
        /// </summary>
        private Stack<ConditionalSeekParameter> ConditionalSeekStack;
    }
}
