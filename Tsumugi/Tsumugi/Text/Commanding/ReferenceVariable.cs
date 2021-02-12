using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Text.Commanding
{
    /// <summary>
    /// 変数の参照を表す型
    /// </summary>
    public class ReferenceVariable<T>
    {
        /// <summary>
        /// 変数名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 値の取得
        /// </summary>
        /// <returns></returns>
        public T GetValueOrDefault()
        {
            if (Referenced)
            {
                return _value;
            }
            return default(T);
        }

        /// <summary>
        /// 値の取得
        /// </summary>
        /// <returns></returns>
        public bool TryGetValue(out T value)
        {
            if (Referenced)
            {
                value = _value;
                return true;
            }
            value = default(T);

            return false;
        }

        /// <summary>
        /// 値を持っているか（変数の参照が解決しているか）
        /// </summary>
        public bool HasValue => Referenced;

        /// <summary>
        /// 値の設定
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value)
        {
            _value = value;
            Referenced = true;
        }

        /// <summary>
        /// 参照済みかどうか
        /// </summary>
        private bool Referenced { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">変数名</param>
        public ReferenceVariable(string name)
        {
            Name = name;
            Referenced = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        public ReferenceVariable(T value) => SetValue(value);

        /// <summary>
        /// 変数
        /// </summary>
        private T _value;
    }
}
