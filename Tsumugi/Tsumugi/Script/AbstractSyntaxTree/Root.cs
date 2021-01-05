using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tsumugi.Script.AbstractSyntaxTree
{
    /// <summary>
    /// ルートノード
    /// </summary>
    public class Root : INode
    {
        /// <summary>
        /// ステートメントのリスト
        /// </summary>
        public List<IStatement> Statements { get; set; }

        /// <summary>
        /// トークンのリテラル
        /// </summary>
        /// <returns>トークンのリテラル</returns>
        public string TokenLiteral()
        {
            return Statements.FirstOrDefault()?.TokenLiteral() ?? "";
        }

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode()
        {
            var builder = new StringBuilder();
            foreach (var ast in this.Statements)
            {
                builder.AppendLine(ast.ToCode());
            }
            return builder.ToString().TrimEnd();
        }
    }
}
