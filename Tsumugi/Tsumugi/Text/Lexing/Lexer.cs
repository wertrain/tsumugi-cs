using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tsumugi.Localize;
using Tsumugi.Text.Commanding;

namespace Tsumugi.Text.Lexing
{
    public class Lexer
    {
        public CommandQueue CommandQueue { get; }

        public Script.Logger Logger { get; set; }

        public Lexer()
        {
            CommandQueue = new CommandQueue();
            Logger = new Script.Logger();
            TemporaryVariables = new Dictionary<string, string>();
            Labels = new Dictionary<string, Label>();
            _enableIndent = false;
        }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="script"></param>
        public int Parse(string script)
        {
            var progressingText = new StringBuilder();

            using (var reader = new StringReader(script))
            {
                int c = -1;
                while ((c = reader.Read()) >= 0)
                {
                    switch (c)
                    {
                        case TsumugiKeyword.LabelPrefix:
                            processLabel(reader, progressingText);
                            break;

                        case TsumugiKeyword.TagStart:
                            processTag(reader, progressingText);
                            break;

                        case TsumugiKeyword.TagLineStart:
                            processTag(reader, progressingText);
                            break;

                        default:
                            progressingText.Append((char)c);
                            break;
                    }
                }

                if (progressingText.Length > 0)
                {
                    addPrintTextCommand(progressingText.ToString(), false);
                    progressingText.Clear();
                }
            }

            return 0;
        }

        /// <summary>
        /// ラベルの処理
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="progressingText"></param>
        private void processLabel(StringReader reader, StringBuilder progressingText)
        {
            var label = new StringBuilder();

            int c = -1;
            while ((c = reader.Read()) >= 0)
            {
                if (checkNameTerminated(c))
                {
                    if (Labels.ContainsKey(label.ToString()))
                    {
                        error(string.Format(LocalizationTexts.AlreadyUsedLabelName.Localize(), label.ToString()));
                    }
                    else
                    {
                        var headline = string.Empty;
                        Labels.Add(label.ToString(), new Label()
                        {
                            Name = label.ToString(),
                            Headline = headline
                        });
                        CommandQueue.Enqueue(new Commanding.Commands.LabelCommand(label.ToString(), headline));
                    }
                    progressingText.Clear();
                    return;
                }

                switch (c)
                {
                    case TsumugiKeyword.HeadlineSeparator:
                        if (Labels.ContainsKey(label.ToString()))
                        {
                            error(string.Format(LocalizationTexts.AlreadyUsedLabelName.Localize(), label.ToString()));
                        }
                        else
                        {
                            var headline = parseLabelHeadline(reader);
                            Labels.Add(label.ToString(), new Label()
                            {
                                Name = label.ToString(),
                                Headline = headline
                            });
                            CommandQueue.Enqueue(new Commanding.Commands.LabelCommand(label.ToString(), headline));
                        }
                        progressingText.Clear();
                        return;

                    default:
                        label.Append((char)c);
                        break;
                }
            }
        }

        /// <summary>
        /// ラベル見出しのパース
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string parseLabelHeadline(StringReader reader)
        {
            var headline = new StringBuilder();

            int c = -1;
            while ((c = reader.Peek()) >= 0)
            {
                if (checkNameTerminated(c))
                {
                    break;
                }
                else
                {
                    headline.Append((char)c);
                    reader.Read();
                }
            }

            return headline.ToString();
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
                    case TsumugiKeyword.TagEnd:
                        return new Tag()
                        {
                            Name = tag.ToString(),
                            Attributes = new List<Tag.Attribute>()
                        };

                    case TsumugiKeyword.TagAttributeSeparator:
                        return new Tag()
                        {
                            Name = tag.ToString(),
                            Attributes = parseTagAttributes(reader)
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
        private List<Tag.Attribute> parseTagAttributes(StringReader reader)
        {
            var name = new StringBuilder();
            var attributes = new List<Tag.Attribute>();

            int c = -1;
            while ((c = reader.Read()) >= 0)
            {
                switch (c)
                {
                    case TsumugiKeyword.TagEnd:
                        if (name.Length > 0)
                        {
                            attributes.Add(new Tag.Attribute()
                            {
                                Name = name.ToString(),
                                Value = string.Empty
                            });
                        }
                        return attributes;

                    case TsumugiKeyword.Assignment:
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
                    case TsumugiKeyword.TagEnd:
                        return value.ToString();

                    case TsumugiKeyword.TagAttributeSeparator:
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
                case TsumugiTag.WaitKey:
                    addPrintTextCommand(progressingText.ToString(), false);
                    progressingText.Clear();
                    CommandQueue.Enqueue(new Commanding.Commands.WaitKeyCommand());
                    break;

                case TsumugiTag.NewLine:
                    addPrintTextCommand(progressingText.ToString(), false);
                    progressingText.Clear();
                    CommandQueue.Enqueue(new Commanding.Commands.NewLineCommand());
                    break;

                case TsumugiTag.NewPage:
                    CommandQueue.Enqueue(new Commanding.Commands.NewPageCommand());
                    break;

                case TsumugiTag.WaitTime:
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
                            else
                            {
                                error(string.Format(LocalizationTexts.NotDefined.Localize(), attr?.Value));
                            }
                        }
                        addPrintTextCommand(progressingText.ToString(), true);
                        progressingText.Clear();
                        CommandQueue.Enqueue(new Commanding.Commands.WaitTimeCommand(time));
                    }
                    break;

                case TsumugiTag.DefineVariable:
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

                case TsumugiTag.IndentStart:
                    addPrintTextCommand(progressingText.ToString(), true);
                    progressingText.Clear();
                    _enableIndent = true;
                    break;

                case TsumugiTag.IndentEnd:
                    addPrintTextCommand(progressingText.ToString(), true);
                    progressingText.Clear();
                    _enableIndent = false;
                    break;

                case TsumugiTag.Jump:
                    {
                        addPrintTextCommand(progressingText.ToString(), true);
                        progressingText.Clear();
                        var attr = tag.Attributes.FirstOrDefault(s => s.Name == "target");

                        if (attr == null)
                        {
                            error(string.Format(LocalizationTexts.CannotFindAttributeRequiredTag.Localize(), "target", TsumugiTag.Jump));
                        }
                        else if (Labels.ContainsKey(attr?.Value))
                        {
                            CommandQueue.Enqueue(new Commanding.Commands.JumpCommand(Labels[attr?.Value].Name));
                        }
                        else
                        {
                            error(string.Format(LocalizationTexts.CannotFindJumpTarget.Localize(), attr?.Value));
                        }
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// PrintTextCommand 設定ユーティリティ
        /// </summary>
        /// <param name="text">設定するテキスト</param>
        /// <param name="fromCommand">コマンドによる文字出力かどうか</param>
        private void addPrintTextCommand(string text, bool fromCommand)
        {
            if (text.Length > 0)
            {
                if (fromCommand && _enableIndent)
                {
                    CommandQueue.Enqueue(new Commanding.Commands.InsertIndentCommand() { });
                }

                CommandQueue.Enqueue(new Commanding.Commands.PrintTextCommand() { Text = text });
            }
        }

        /// <summary>
        /// ラベル名などの終端を判定
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool checkNameTerminated(int c)
        {
            switch (c)
            {
                case ' ':
                case '[':
                case '\n':
                case '\r':
                case '\t':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// エラー出力
        /// </summary>
        /// <param name="message"></param>
        private void error(string message)
        {
            Logger.Log(Script.Logger.Categories.Error, message);
        }

        /// <summary>
        /// 一時変数の辞書
        /// </summary>
        private Dictionary<string, string> TemporaryVariables;

        /// <summary>
        /// ラベルの辞書
        /// </summary>
        private Dictionary<string, Label> Labels;

        /// <summary>
        /// インデントの有効
        /// </summary>
        private bool _enableIndent;
    }
}
