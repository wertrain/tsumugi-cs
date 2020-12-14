using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TsumugiUnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethodParserBasic()
        {
            var parser = new Tsumugi.Parser();
            parser.Parse("Hello, Tsumugi!");

            Assert.AreNotEqual(parser.CommandQueue.Dequeue(), null);
        }
    }
}
