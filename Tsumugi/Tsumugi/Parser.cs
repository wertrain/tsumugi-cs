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
                int c = -1;
                while((c = reader.Read()) >= 0)
                {
                    switch (c)
                    {
                        case '[':
                            processTag(reader, progressingText);
                            break;

                        default:
                            progressingText.Append((char)c);
                            break;
                    }
                }
            }
        }

        private string parseTag(StringReader reader)
        {
            var tag = new StringBuilder();

            int c = -1;
            while ((c = reader.Read()) >= 0)
            {
                switch (c)
                {
                    case ']':
                        return tag.ToString();

                    default:
                        tag.Append((char)c);
                        break;
                }
            }

            return string.Empty;
        }

        private bool processTag(StringReader reader, StringBuilder progressingText)
        {
            var tag = parseTag(reader);

            switch (tag)
            {
                case "l":
                    addPrintTextCommand(progressingText.ToString());
                    progressingText.Clear();
                    CommandQueue.Enqueue(new Commands.WaitKeyCommand());
                    break;

                case "r":
                    addPrintTextCommand(progressingText.ToString());
                    progressingText.Clear();
                    CommandQueue.Enqueue(new Commands.NewLineCommand());
                    break;

                case "cm":
                    CommandQueue.Enqueue(new Commands.NewPageCommand());
                    break;
            }

            return true;
        }

        private void addPrintTextCommand(string text)
        {
            if (text.Length > 0)
            {
                CommandQueue.Enqueue(new Commands.PrintTextCommand() { Text = text });
            }
        }
    }
}
