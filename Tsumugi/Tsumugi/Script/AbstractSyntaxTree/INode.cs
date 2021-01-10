using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.AbstractSyntaxTree
{
    /// <summary>
    /// ノードインターフェース
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// トークンのリテラル
        /// </summary>
        /// <returns>トークンのリテラル</returns>
        string TokenLiteral();

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        string ToCode();
    }
}
