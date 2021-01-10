using System;
using Tsumugi.Script.Lexing;
using Tsumugi.Script.Parsing;

namespace Tsumugi.Script
{
    public class ReadEvalPrintLoop
    {
        const string PROMPT = ">> ";

        public void Start()
        {
            while (true)
            {
                Console.Write(PROMPT);

                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) return;

                var lexer = new Lexer(input);
                var parser = new Tsumugi.Script.Parsing.Parser(lexer);
                var root = parser.ParseProgram();

                if (parser.Logger.Count() > 0)
                {
                    foreach (var error in parser.Logger.GetHistories())
                    {
                        Console.WriteLine("{0}{1}", "\t", error);
                    }
                    continue;
                }

                Console.WriteLine(root.ToCode());
            }
        }
    }
}
