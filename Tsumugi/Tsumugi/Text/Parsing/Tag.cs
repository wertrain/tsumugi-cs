using System;
using System.Collections.Generic;
using System.Linq;

namespace Tsumugi.Text.Parsing
{
    /// <summary>
    /// タグ名
    /// </summary>
    class TagName
    {
        /// <summary>
        /// キー入力待ち
        /// </summary>
        public const string WaitKey = "l";

        /// <summary>
        /// 改行
        /// </summary>
        public const string NewLine = "r";

        /// <summary>
        /// 改ページ
        /// </summary>
        public const string NewPage = "cm";

        /// <summary>
        /// 指定時間待ち
        /// </summary>
        public const string WaitTime = "wait";

        /// <summary>
        /// 変数定義
        /// </summary>
        public const string DefineVariable = "var";

        /// <summary>
        /// インデント開始
        /// </summary>
        public const string IndentStart = "indent";

        /// <summary>
        /// インデント終了
        /// </summary>
        public const string IndentEnd = "endindent";

        /// <summary>
        /// ジャンプ
        /// </summary>
        public const string Jump = "jump";

        /// <summary>
        /// If 
        /// </summary>
        public const string If = "if";

        /// <summary>
        /// Else 
        /// </summary>
        public const string Else = "else";

        /// <summary>
        /// Else If
        /// </summary>
        public const string Elif = "elif";

        /// <summary>
        /// EndIf 
        /// </summary>
        public const string Endif = "endif";

        /// <summary>
        /// 評価
        /// </summary>
        public const string Eval = "eval";

        /// <summary>
        /// 評価結果の埋め込み
        /// </summary>
        public const string Embed = "embed";

        /// <summary>
        /// フォント設定
        /// </summary>
        public const string Font = "font";

        /// <summary>
        /// デフォルトフォントの設定
        /// </summary>
        public const string DefaultFont = "deffont";
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
        /// タグの属性
        /// </summary>
        public List<Tag.Attribute> Attributes;
    }

    /// <summary>
    /// タグのコマンドを生成
    /// </summary>
    class TagCommandFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static Commanding.CommandBase CreateTagCommand(Tag tag)
        {
            switch (tag.Name)
            {
                case TagName.NewLine:
                    return new Commanding.Commands.NewLineCommand();

                case TagName.NewPage:
                    return new Commanding.Commands.NewPageCommand();

                case TagName.WaitKey:
                    return new Commanding.Commands.WaitKeyCommand();

                case TagName.WaitTime:
                    {
                        var attr = GetAttribute("time", tag);

                        if (int.TryParse(attr?.Value, out var time))
                        {
                            return new Commanding.Commands.WaitTimeCommand(time);
                        }
                        return new Commanding.Commands.WaitTimeCommand(attr?.Value);
                    }

                case TagName.DefineVariable:
                    {
                        var variables = new List<Commanding.Commands.DefineVariablesCommand.Variable>();

                        foreach (var attr in tag.Attributes)
                        {
                            variables.Add(new Commanding.Commands.DefineVariablesCommand.Variable() { Name = attr.Name, Value = attr.Value });
                        }

                        return new Commanding.Commands.DefineVariablesCommand(variables);
                    }

                case TagName.Jump:
                    {
                        var attr = GetAttribute("target", tag);

                        return new Commanding.Commands.JumpCommand(attr?.Value);
                    }

                case TagName.If:
                    {
                        var attr = GetAttribute("exp", tag);

                        return new Commanding.Commands.IfCommand(attr?.Value);
                    }

                case TagName.Else:
                    return new Commanding.Commands.ElseCommand();

                case TagName.Elif:
                    {
                        var attr = GetAttribute("exp", tag);

                        return new Commanding.Commands.ElifCommand(attr?.Value);
                    }

                case TagName.Endif:
                    return new Commanding.Commands.EndIfCommand();

                case TagName.Eval:
                    {
                        var attr = GetAttribute("exp", tag);

                        return new Commanding.Commands.EvalCommand(attr?.Value);
                    }

                case TagName.Embed:
                    {
                        var attr = GetAttribute("exp", tag);

                        return new Commanding.Commands.EmbedCommand(attr?.Value);
                    }

                case TagName.Font:
                    {
                        return new Commanding.Commands.FontCommand()
                        {
                            Size = GetAttributeValueOrDefault("size", tag, 10),
                            Face = GetAttributeValueOrDefault("face", tag, "MS UI Gothic"),
                            Color = GetAttributeValueOrDefault<uint>("color", tag, 0x000000ff),
                            RubySize = GetAttributeValueOrDefault("rubysize", tag, 10),
                            RubyOffset = GetAttributeValueOrDefault("rubyoffset", tag, 5),
                            RubyFace = GetAttributeValueOrDefault("rubyface", tag, "MS UI Gothic"),
                            Shadow = GetAttributeValueOrDefault("shadow", tag, false),
                            ShadowColor = GetAttributeValueOrDefault<uint>("shadowcolor", tag, 0x000000ff),
                            Edge = GetAttributeValueOrDefault("edge", tag, false),
                            EdgeColor = GetAttributeValueOrDefault<uint>("edgecolor", tag, 0x000000ff),
                            Bold = GetAttributeValueOrDefault("bold", tag, false),
                        };
                    }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static Tag.Attribute GetAttribute(string name, Tag tag)
        {
            var attr = tag.Attributes.FirstOrDefault(s => s.Name == name);

            if (attr == null || string.IsNullOrWhiteSpace(attr.Value))
            {
                throw new CannotFindAttributeException(name);
            }

            return attr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetAttributeValueOrDefault<T>(string name, Tag tag, T defaultValue)
        {
            var attr = tag.Attributes.FirstOrDefault(s => s.Name == name);

            if (attr == null || string.IsNullOrWhiteSpace(attr.Value))
            {
                return defaultValue;
            }
            
            // C# 7.0 だとタイプ判定を使用できない

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    return (T)Convert.ChangeType(int.Parse(attr.Value), typeof(T));

                case TypeCode.UInt32:
                    return (T)Convert.ChangeType(uint.Parse(attr.Value), typeof(T));

                case TypeCode.Decimal:
                    return (T)Convert.ChangeType(decimal.Parse(attr.Value), typeof(T));

                case TypeCode.Double:
                    return (T)Convert.ChangeType(double.Parse(attr.Value), typeof(T));

                case TypeCode.Boolean:
                    return (T)Convert.ChangeType(bool.Parse(attr.Value), typeof(T));
            }

            return (T)Convert.ChangeType(attr.Value, typeof(T));
        }

        /// <summary>
        /// コマンドファクトリの例外
        /// </summary>
        public class TagCommandFactoryException : Exception {}

        /// <summary>
        /// 属性が見つからない例外
        /// </summary>
        public class CannotFindAttributeException : TagCommandFactoryException
        {
            /// <summary>
            /// 
            /// </summary>
            public string AttributeName { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            public CannotFindAttributeException(string name) => AttributeName = name;
        }
    }
}
