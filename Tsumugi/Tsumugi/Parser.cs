using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tsumugi
{
    public class Parser
    {
        public Commands.CommandQueue CommandQueue { get; }

        public Parser()
        {
            CommandQueue = new Commands.CommandQueue();
            TemporaryVariables = new Dictionary<string, string>();
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="script"></param>
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

        /// <summary>
        /// タグをパース
        /// タグの仕様としては [ で開始し、] で終了
        /// [ の次には一文字以上の命令があり、さらに 0 個以上の命令に関する属性が含まれる
        /// Ex. [命令 属性1=属性1の値 ...]
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Tag parseTag(StringReader reader)
        {
            var tag = new StringBuilder();

            int c = -1;
            while ((c = reader.Read()) >= 0)
            {
                switch (c)
                {
                    case ']':
                        return new Tag()
                        {
                            Name = tag.ToString(),
                            Attributes = new List<Tag.Attribute>()
                        };

                    case ' ':
                        return new Tag()
                        {
                            Name = tag.ToString(),
                            Attributes = parseAttributes(reader)
                        };

                    default:
                        tag.Append((char)c);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// 属性をパース
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<Tag.Attribute> parseAttributes(StringReader reader)
        {
            var name = new StringBuilder();
            var attributes = new List<Tag.Attribute>();

            int c = -1;
            while ((c = reader.Read()) >= 0)
            {
                switch (c)
                {
                    case ']':
                        if (name.Length > 0)
                        {
                            attributes.Add(new Tag.Attribute()
                            {
                                Name = name.ToString(),
                                Value = string.Empty
                            });
                        }
                        return attributes;

                    case '=':
                        attributes.Add(new Tag.Attribute()
                        {
                            Name = name.ToString(),
                            Value = parseAttributeValue(reader)
                        });
                        name.Clear();
                        break;

                    default:
                        name.Append((char)c);
                        break;
                }
            }

            return attributes;

        }

        /// <summary>
        /// 属性の値をパース
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string parseAttributeValue(StringReader reader)
        {
            var value = new StringBuilder();

            int c = -1;
            while ((c = reader.Peek()) >= 0)
            {
                switch (c)
                {
                    case ']':
                        return value.ToString();

                    case ' ':
                        reader.Read();
                        return value.ToString();

                    default:
                        value.Append((char)c);
                        break;
                }
                reader.Read();
            }

            return null;
        }

        /// <summary>
        /// タグの処理
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="progressingText"></param>
        /// <returns></returns>
        private bool processTag(StringReader reader, StringBuilder progressingText)
        {
            var tag = parseTag(reader);

            switch (tag.Name)
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

                case "wait":
                    {
                        var attr = tag.Attributes.FirstOrDefault(s => s.Name == "time");
                        int time = 0;
                        if (!int.TryParse(attr?.Value, out time))
                        {
                            time = 1000;
                            if (attr != null && TemporaryVariables.ContainsKey(attr?.Value))
                            {
                                var value = TemporaryVariables[attr?.Value];
                                int.TryParse(value, out time);
                            }
                        }
                        addPrintTextCommand(progressingText.ToString());
                        progressingText.Clear();
                        CommandQueue.Enqueue(new Commands.WaitTimeCommand(time));
                    }
                    break;

                case "var":
                    foreach (var attr in tag.Attributes)
                    {
                        if (TemporaryVariables.ContainsKey(attr.Name))
                        {
                            TemporaryVariables[attr.Name] = attr.Value;
                        }
                        else
                        {
                            TemporaryVariables.Add(attr.Name, attr.Value);
                        }
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 文字列が空でない場合に、文字表示コマンドを追加
        /// </summary>
        /// <param name="text"></param>
        private void addPrintTextCommand(string text)
        {
            if (text.Length > 0)
            {
                CommandQueue.Enqueue(new Commands.PrintTextCommand() { Text = text });
            }
        }

        /// <summary>
        /// タグ
        /// </summary>
        public class Tag
        {
            /// <summary>
            /// 属性
            /// </summary>
            public class Attribute
            {
                /// <summary>
                /// 属性名
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// 属性の値
                /// </summary>
                public string Value { get; set; }
            }

            /// <summary>
            /// タグの名前
            /// </summary>
            public string Name;
            
            /// <summary>
            /// 
            /// </summary>
            public List<Tag.Attribute> Attributes;
        }

        /// <summary>
        /// 一時変数の配列
        /// </summary>
        private Dictionary<string, string> TemporaryVariables;
    }
}
