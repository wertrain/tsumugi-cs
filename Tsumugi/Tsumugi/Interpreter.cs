﻿using System;
using System.Collections.Generic;
using System.Text;

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
        public Text.Executing.ICommandExecutor Executor { get; set; }

        /// <summary>
        /// 実行環境
        /// </summary>
        private Script.Evaluating.Enviroment Enviroment { get; set; }

        /// <summary>
        /// 評価器
        /// </summary>
        private Script.Evaluating.Evaluator Evaluator { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Interpreter()
        {
            Enviroment = new Script.Evaluating.Enviroment();
            Evaluator = new Script.Evaluating.Evaluator();
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

            var commandQueue = parser.ParseProgram();
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
                switch (command)
                {
                    case Text.Commanding.Commands.PrintTextCommand cmd:
                        Executor.PrintText(cmd.Text);
                        break;

                    case Text.Commanding.Commands.NewLineCommand cmd:
                        Executor.StartNewLine();
                        break;

                    case Text.Commanding.Commands.WaitKeyCommand cmd:
                        Executor.WaitAnyKey();
                        break;

                    case Text.Commanding.Commands.NewPageCommand cmd:
                        Executor.StartNewPage();
                        break;

                    case Text.Commanding.Commands.DefineVariablesCommand cmd:
                        foreach(var variable in cmd.Variables)
                        {
                            var let = new Script.AbstractSyntaxTree.Statements.LetStatement()
                            {
                                Name = new Script.AbstractSyntaxTree.Expressions.Identifier(
                                    new Script.Lexing.Token(Script.Lexing.TokenType.Integer32, variable.Name), variable.Name
                                ),
                                Value = new Script.AbstractSyntaxTree.Expressions.IntegerLiteral()
                                {
                                    Value = int.Parse(variable.Value)
                                }
                            };
                            Evaluator.Eval(let, Enviroment);
                        }
                        break;

                    case Text.Commanding.Commands.WaitTimeCommand cmd:
                        if (ResolveVariableReferences<int>(cmd.Time))
                        {
                            Executor.WaitTime(cmd.Time.GetValueOrNull().Value);
                        }
                        break;

                    case Text.Commanding.Commands.InsertIndentCommand cmd:
                        //Executor.Indent(Environment.Indentation);
                        break;

                    case Text.Commanding.Commands.JumpCommand cmd:
                        var labels = queue.FindCommands<Text.Commanding.Commands.LabelCommand>();
                        var label = labels.Find(c => c.Name == cmd.Target);
                        queue.Seek(label);
                        break;

                }
            }

            return 0;
        }

        /// <summary>
        /// 変数参照型の参照を解決する
        /// </summary>
        /// <typeparam name="T">期待する変数の型</typeparam>
        /// <param name="variable">参照を解決する変数参照型</param>
        /// <returns>参照を解決できれば true</returns>
        private bool ResolveVariableReferences<T>(object variable) where T : struct
        {
            if ((variable as Text.Commanding.ReferenceVariable<T>).GetValueOrNull().HasValue)
                return true;

            switch (variable)
            {
                case Text.Commanding.ReferenceVariable<int> refv:
                    var (obj, ok) = Enviroment.Get(refv.Name);
                    if (ok && (obj is Script.Objects.IntegerObject))
                    {
                        var var = obj as Script.Objects.IntegerObject;
                        refv.SetValue(var.Value);
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
