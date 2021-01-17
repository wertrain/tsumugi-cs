﻿using System;
using System.Threading.Tasks;

namespace ConsoleSample
{
    class Program
    {
        /// <summary>
        /// Tsumugi コマンド実行クラスのコンソール版
        /// </summary>
        private class CommandExecutor : Tsumugi.Text.Executing.CommandExecutor
        {
            public override void PrintText(string text)
            {
                Console.Write("{0}", text);
            }

            public override void StartNewLine()
            {
                Console.WriteLine();
            }

            public override void WaitAnyKey()
            {
                Console.ReadKey(true);
            }

            public override void StartNewPage()
            {
                Console.Clear();
            }

            public override void WaitTime(int millisec)
            {
                Task.Delay(millisec).Wait();
            }

            public override void Indent(int count)
            {
                Console.Write("\t");
            }

            public void DebugPring(Tsumugi.Text.Commanding.CommandQueue queue)
            {
                queue.Each((int index, Tsumugi.Text.Commanding.CommandBase command) => { Console.WriteLine("[{0}] {1}", index, command.GetType().ToString()); });
            }

        }

        static void Main(string[] args)
        {
            var script = "" +
            ":start|開始位置" +
            "[var wtime=1000][jump target=notfound]" +
            "こんにちは[r]" +
            "これは Tsumugi のテスト[wait time=wtime]です。[l][cm]" +
            "ページをクリアしました。[l][r][cm][jump target=start]" +
            "[l]";
            var interpreter = new Tsumugi.Interpreter();
            interpreter.Executor = new CommandExecutor();
            interpreter.Execute(script);
        }
    }
}
