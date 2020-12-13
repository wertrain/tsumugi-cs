using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tsumugi
{
    public class Parser
    {
        public Commands.CommandQueue CommandQueue { get; }

        public Parser()
        {
            CommandQueue = new Commands.CommandQueue();
        }

        public void Parse(string script)
        {
            var progressingText = new StringBuilder();

            using (var reader = new StringReader(script))
            {
                char c = (char)reader.Peek();

                switch (c)
                {
                    case '[':
                        parseTag(reader, progressingText);
                        break;

                    default:
                        progressingText.Append(c);
                        break;
                }
            }
        }

        private bool parseTag(StringReader reader, StringBuilder progressingText)
        {
            var tag = new StringBuilder();

            int c = -1;
            while ((c = reader.Peek()) >= 0)
            {
                switch (c)
                {
                    case ']':
                        break;

                    default:
                        tag.Append(c);
                        break;
                }
            }

            switch(tag.ToString())
            {
                case "l":
                    CommandQueue.Enqueue(new Commands.PrintTextCommand() { Text = progressingText.ToString() });
                    progressingText.Clear();
                    break;

                case "r":
                    CommandQueue.Enqueue(new Commands.PrintTextCommand() { Text = progressingText.ToString() });
                    progressingText.Clear();
                    CommandQueue.Enqueue(new Commands.NewLineCommand());
                    break;
            }

            return true;
        }
    }
}
