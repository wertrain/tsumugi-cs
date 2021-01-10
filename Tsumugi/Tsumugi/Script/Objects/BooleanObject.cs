using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// 真偽値オブジェクト
    /// </summary>
    public class BooleanObject : IObject
    {
        /// <summary>
        /// 真偽値
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        public BooleanObject(bool value) => Value = value;

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Boolean;

        /// <summary>
        /// 真偽値の評価結果
        /// </summary>
        /// <returns>評価結果</returns>
        public string Inspect() => Value ? "true" : "false";
    }
}
