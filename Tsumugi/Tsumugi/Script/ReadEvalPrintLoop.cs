using System;

namespace Tsumugi.Script
{
    public class ReadEvalPrintLoop
    {
        const string PROMPT = ">> ";

        public void Start()
        {
            Console.WriteLine("Hello Tsumugi Script!");

            var enviroment = new Evaluating.Enviroment();

            while (true)
            {
                Console.Write(PROMPT);

                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) return;

                var lexer = new Lexing.Lexer(input);
                var parser = new Parsing.Parser(lexer);
                var root = parser.ParseProgram();

                if (parser.Logger.Count() > 0)
                {
                    foreach (var error in parser.Logger.GetHistories())
                    {
                        Console.WriteLine("{0}{1}", "\t", error);
                    }
                    continue;
                }

                var evaluator = new Evaluating.Evaluator();

                var evaluated = evaluator.Eval(root, enviroment);
                if (evaluated != null)
                {
                    Console.WriteLine(evaluated.Inspect());
                }
            }
        }
    }
}
