using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TsumugiUnitTest
{
    [TestClass]
    public class TsumugiParserUnitTest
    {
        [TestMethod]
        public void TestMethodParserBasic()
        {
            var parser = new Tsumugi.Parser.TsumugiParser();
            parser.Parse("Hello, Tsumugi!");

            Assert.AreNotEqual(parser.CommandQueue.Dequeue(), null);
        }

        [TestMethod]
        public void TestMethodParserError()
        {
            var parser = new Tsumugi.Parser.TsumugiParser();

            var script = "" +
                ":start|開始位置" +
                "[var wtime=1000][jump target=notfound]" +
                "こんにちは[r]" +
                "これは Tsumugi のテスト[wait time=notdefine]です。[l][cm]" +
                "ページをクリアしました。[l][r][cm][jump target=start]" +
                "[l]";
            parser.Parse(script);

            Assert.IsTrue(parser.Logger.Count(Tsumugi.Script.Logger.Categories.Error) == 2);
        }
    }
}
