using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tsumugi.Script.AbstractSyntaxTree
{
    /// <summary>
    /// 
    /// </summary>
    public class Root : INode
    {
        /// <summary>
        /// ステートメントのリスト
        /// </summary>
        public List<IStatement> Statements { get; set; }

        public string TokenLiteral()
        {
            return Statements.FirstOrDefault()?.TokenLiteral() ?? "";
        }
    }
}
