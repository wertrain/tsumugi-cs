using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tsumugi.Text.Lexing;

namespace TsumugiUnitTest
{
    [TestClass]
    public class TsumugiUnitTest
    {
        [TestMethod]
        public void TestMethodLexerBasic()
        {
            var tokens = new List<Token>();
            tokens.Add(new Token(TokenType.Label, "label"));
            tokens.Add(new Token(TokenType.Tag, "tag"));
            tokens.Add(new Token(TokenType.TagAttributeName, "attr1"));
            tokens.Add(new Token(TokenType.TagAttributeValue, "0"));
            tokens.Add(new Token(TokenType.TagAttributeName, "attr2"));
            tokens.Add(new Token(TokenType.TagAttributeValue, "test"));
            tokens.Add(new Token(TokenType.TagEnd, string.Empty));
            tokens.Add(new Token(TokenType.Tag, "tag2"));
            tokens.Add(new Token(TokenType.TagAttributeName, "attr3"));
            tokens.Add(new Token(TokenType.TagAttributeValue, "aaa"));
            tokens.Add(new Token(TokenType.TagEnd, string.Empty));
            tokens.Add(new Token(TokenType.Text, "テキスト"));
            tokens.Add(new Token(TokenType.Label, "label2"));
            tokens.Add(new Token(TokenType.LabelHeadline, "見出し"));
            tokens.Add(new Token(TokenType.Tag, "tag3"));
            tokens.Add(new Token(TokenType.TagAttributeName, "attr4"));
            tokens.Add(new Token(TokenType.TagAttributeValue, "ok"));
            tokens.Add(new Token(TokenType.Text, "これはテキストになります。改行は無効でタグを使わない場合は [ エスケープします。タグも"));
            tokens.Add(new Token(TokenType.Tag, "nw"));
            tokens.Add(new Token(TokenType.TagEnd, string.Empty));
            tokens.Add(new Token(TokenType.Text, "使えます。タグを置くとテキストが分割します。"));
            tokens.Add(new Token(TokenType.EOF, string.Empty));


            var lexer = new Lexer(
                ":label" + Environment.NewLine +
                "[tag attr1=0 attr2=test]" + Environment.NewLine +
                "[tag2    attr3   =  aaa]" + Environment.NewLine +
                "テキスト" + Environment.NewLine +
                ":label2|見出し" + Environment.NewLine + 
                "@tag3 attr4=ok" + Environment.NewLine +
                "これはテキストになります。" + Environment.NewLine +
               @"改行は無効でタグを使わない場合は \[ エスケープします。" + Environment.NewLine +
                "タグも[nw]使えます。" + Environment.NewLine +
                "タグを置くとテキストが分割します。" + Environment.NewLine +
            "");
            foreach (var testToken in tokens)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(testToken.Type, token.Type, string.Format("{0} token type is wrong.", testToken.Type.ToString()));
                Assert.AreEqual(testToken.Literal, token.Literal, string.Format("{0} literal is wrong.", testToken.Literal));
            }
        }

        [TestMethod]
        public void TestMethodParsing()
        {
            var lexer = new Lexer(
                ":start|開始位置" + Environment.NewLine +
                "[var wtime=1000][jump target=notfound]" + Environment.NewLine +
                "こんにちは[r]" + Environment.NewLine +
                "これは Tsumugi のテスト[wait time=notdefine]です。[l][cm]" + Environment.NewLine +
                "ページをクリアしました。[l][r][cm][jump target=start]" + Environment.NewLine +
                "[if exp=wtime==1000]" + Environment.NewLine +
                "[endif]" + Environment.NewLine +
            "");

            var parser = new Tsumugi.Text.Parsing.Parser(lexer);
            var commandQueue = parser.ParseProgram();

            var commands = new List<Tsumugi.Text.Commanding.CommandBase>();
            commands.Add(new Tsumugi.Text.Commanding.Commands.LabelCommand("start", "開始位置"));
            commands.Add(new Tsumugi.Text.Commanding.Commands.DefineVariablesCommand(
                new List<Tsumugi.Text.Commanding.Commands.DefineVariablesCommand.Variable>()
                {
                    new Tsumugi.Text.Commanding.Commands.DefineVariablesCommand.Variable()
                    {
                        Name = "wtime",
                        Value = "1000"
                    }
                })
            );
            commands.Add(new Tsumugi.Text.Commanding.Commands.JumpCommand("notfound"));
            commands.Add(new Tsumugi.Text.Commanding.Commands.PrintTextCommand("こんにちは"));
            commands.Add(new Tsumugi.Text.Commanding.Commands.NewLineCommand());
            commands.Add(new Tsumugi.Text.Commanding.Commands.PrintTextCommand("これは Tsumugi のテスト"));
            commands.Add(new Tsumugi.Text.Commanding.Commands.WaitTimeCommand("notdefine"));
            commands.Add(new Tsumugi.Text.Commanding.Commands.PrintTextCommand("です。"));
            commands.Add(new Tsumugi.Text.Commanding.Commands.WaitKeyCommand());
            commands.Add(new Tsumugi.Text.Commanding.Commands.NewPageCommand());
            commands.Add(new Tsumugi.Text.Commanding.Commands.PrintTextCommand("ページをクリアしました。"));
            commands.Add(new Tsumugi.Text.Commanding.Commands.WaitKeyCommand());
            commands.Add(new Tsumugi.Text.Commanding.Commands.NewLineCommand());
            commands.Add(new Tsumugi.Text.Commanding.Commands.NewPageCommand());
            commands.Add(new Tsumugi.Text.Commanding.Commands.JumpCommand("start"));

            foreach (var command in commands)
            {
                var test = commandQueue.dequeue();
                Assert.AreEqual(test.GetType(), command.GetType());
                switch (test)
                {
                    case Tsumugi.Text.Commanding.Commands.LabelCommand label:
                        var labelCommand = (Tsumugi.Text.Commanding.Commands.LabelCommand)command;
                        Assert.AreEqual(label.Name, labelCommand.Name);
                        Assert.AreEqual(label.Headline, labelCommand.Headline);
                        break;
                }
            }
            
        }


        [TestMethod]
        public void TestMethodParsingIf()
        {
            var scripts = new string[]
            {
                "[if exp=wtime==1000]" + Environment.NewLine +
                "[endif]",

                "[if exp=wtime==1000]" + Environment.NewLine +
                "[elif exp=wtime==1000]" + Environment.NewLine +
                "[endif]",

                "[if exp=wtime==1000]" + Environment.NewLine +
                "[else]" + Environment.NewLine +
                "[endif]",

                "[if exp=wtime==1000]" + Environment.NewLine +
                "[elif exp=wtime==1000]" + Environment.NewLine +
                "[else]" + Environment.NewLine +
                "[endif]",

                "[if exp=wtime==1000]" + Environment.NewLine +
                    "[if exp=wtime==1000]" + Environment.NewLine +
                    "[endif]" + Environment.NewLine +
                "[endif]",

                "[if exp=wtime==1000]" + Environment.NewLine +
                    "[if exp=wtime==1000]" + Environment.NewLine +
                        "[if exp=wtime==1000]" + Environment.NewLine +
                        "[endif]" + Environment.NewLine +
                    "[endif]" + Environment.NewLine +
                "[endif]",

                "[if exp=wtime==1000]" + Environment.NewLine +
                    "[if exp=wtime==1000]" + Environment.NewLine +
                        "[if exp=wtime==1000]" + Environment.NewLine +
                        "[else]" + Environment.NewLine +
                        "[endif]" + Environment.NewLine +
                    "[endif]" + Environment.NewLine +
                "[endif]"
            };

            foreach (var script in scripts)
            {
                var lexer = new Lexer(script);

                var parser = new Tsumugi.Text.Parsing.Parser(lexer);
                var commandQueue = parser.ParseProgram();
                Assert.IsTrue(parser.Logger.Count() == 0);
            }

        }

        [TestMethod]
        public void TestMethodParsingIfError()
        {
            var scripts = new string[]
            {
                // endif なし
                "[if exp=wtime==1000]" + Environment.NewLine +
                "[else]",
                // endif なし
                "[if exp=wtime==1000]" + Environment.NewLine +
                "[elif]" + Environment.NewLine +
                "[else]",
                // else のあとに elif
                "[if exp=wtime==1000]" + Environment.NewLine +
                "[else]" + Environment.NewLine +
                "[elif]" + Environment.NewLine +
                "[endif]",
                // else 複数
                "[if exp=wtime==1000]" + Environment.NewLine +
                "[else]" + Environment.NewLine +
                "[else]" + Environment.NewLine +
                "[endif]",
                // 入れ子内の不正
                "[if exp=wtime==1000]" + Environment.NewLine +
                    "[if exp=wtime==1000]" + Environment.NewLine +
                    "[else]" + Environment.NewLine +
                "[endif]"
            };

            foreach (var script in scripts)
            {
                var lexer = new Lexer(script);

                var parser = new Tsumugi.Text.Parsing.Parser(lexer);
                var commandQueue = parser.ParseProgram();
                Assert.IsTrue(parser.Logger.Count() > 0);
            }

        }
    }
}
