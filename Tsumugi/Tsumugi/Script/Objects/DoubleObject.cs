using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// 倍精度浮動小数点数オブジェクト
    /// </summary>
    public class DoubleObject : IObject
    {
        /// <summary>
        /// 倍精度浮動小数点数値
        /// </double>
        public double Value { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">整数値</param>
        public DoubleObject(double value) => Value = value;

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Double;

        /// <summary>
        /// 整数値の評価結果
        /// </summary>
        /// <returns>評価結果</returns>
        public string Inspect() => Value.ToString();
    }
}
