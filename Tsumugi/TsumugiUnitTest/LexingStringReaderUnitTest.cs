using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tsumugi.Script.Lexing;

namespace TsumugiUnitTest
{
    /// <summary>
    /// LexingStringReaderUnitTest の概要の説明
    /// </summary>
    [TestClass]
    public class LexingStringReaderUnitTest
    {
        [TestMethod]
        public void TestMethodBasic()
        {
            var input = @"if (true) {
                            if (true) {
                                return 10;
                            }
                            0;
                        }";
            var tests = new (int, int, int)[]{
                 (11, 0, 11),
                 (12, 1, 0),
                 (52, 1, 52 - 12),
                 (53, 2, 0),
            };

            foreach (var (seek, lines, columns) in tests)
            {
                var reader = new LexingStringReader(input);
                for (int i = 0; i < seek; ++i) reader.ReadChar();
                Assert.AreEqual(reader.Lines, lines);
                Assert.AreEqual(reader.Columns, columns);
            }
        }
    }
}
