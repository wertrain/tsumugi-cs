using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// NULL オブジェクト
    /// </summary>
    public class NullObject : IObject
    {
        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Null;

        /// <summary>
        /// 評価式
        /// </summary>
        /// <returns>評価式</returns>
        public string Inspect() => "null";
    }
}
