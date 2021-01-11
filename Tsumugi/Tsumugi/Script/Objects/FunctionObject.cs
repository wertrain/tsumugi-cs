using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsumugi.Script.AbstractSyntaxTree.Expressions;
using Tsumugi.Script.AbstractSyntaxTree.Statements;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// 関数オブジェクト
    /// </summary>
    public class FunctionObject : IObject
    {
        /// <summary>
        /// 引数
        /// </summary>
        public List<Identifier> Parameters { get; set; } = new List<Identifier>();

        /// <summary>
        /// 関数ブロック
        /// </summary>
        public BlockStatement Body { get; set; }

        /// <summary>
        /// 環境
        /// </summary>
        public Evaluating.Enviroment Enviroment { get; set; }

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Function;

        /// <summary>
        /// 評価式
        /// </summary>
        /// <returns>評価式</returns>
        public string Inspect()
        {
            var parameters = Parameters.Select(p => p.ToCode());
            var builder = new StringBuilder();
            builder.Append("function");
            builder.Append("(");
            builder.Append(string.Join(", ", parameters));
            builder.Append(")");
            builder.Append(Body.ToCode());
            return builder.ToString();
        }
    }
}
