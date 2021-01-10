using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// 整数値オブジェクト
    /// </summary>
    public class IntegerObject : IObject
    {
        /// <summary>
        /// 整数値
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">整数値</param>
        public IntegerObject(int value) => Value = value;

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Integer;

        /// <summary>
        /// 整数値の評価結果
        /// </summary>
        /// <returns>評価結果</returns>
        public string Inspect() => Value.ToString();
    }
}
