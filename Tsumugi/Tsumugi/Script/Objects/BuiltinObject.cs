using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// 組み込み関数のデリゲート
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    delegate IObject BuiltinFunction(List<IObject> objects);

    /// <summary>
    /// 組み込み関数オブジェクト
    /// </summary>
    class BuiltinObject : IObject
    {
        /// <summary>
        /// 組み込み関数
        /// </summary>
        public BuiltinFunction Function { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="function">組み込み関数</param>
        public BuiltinObject(BuiltinFunction function) => Function = function;

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Builtin;

        /// <summary>
        /// 評価式
        /// </summary>
        /// <returns>評価式</returns>
        public string Inspect() => Function.ToString();
    }
}
