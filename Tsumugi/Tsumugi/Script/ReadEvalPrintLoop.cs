using System;
using Tsumugi.Script.Lexing;

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
                for (var token = lexer.NextToken(); token.Type != TokenType.EOF; token = lexer.NextToken())
                {
                    Console.WriteLine("{{ Type: {0}, Literal: {1} }}", token.Type.ToString(), token.Literal.ToString());
                }
            }
        }
    }
}
