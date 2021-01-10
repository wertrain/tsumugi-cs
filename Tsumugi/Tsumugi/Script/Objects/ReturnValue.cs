using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// Return 評価オブジェクト
    /// </summary>
    class ReturnValue : IObject
    {
        /// <summary>
        /// 評価オブジェクト
        /// </summary>
        public IObject Value { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">評価オブジェクト</param>
        public ReturnValue(IObject value) => Value = value;

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Return;

        /// <summary>
        /// 評価式
        /// </summary>
        /// <returns>評価式</returns>
        public string Inspect() => Value.Inspect();
    }
}
