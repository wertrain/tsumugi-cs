using System.Collections.Generic;

namespace Tsumugi.Text.Parsing
{
    /// <summary>
    /// タグ
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// 属性
        /// </summary>
        public class Attribute
        {
            /// <summary>
            /// 属性名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 属性の値
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// タグの名前
        /// </summary>
        public string Name;

        /// <summary>
        /// タグの属性
        /// </summary>
        public List<Tag.Attribute> Attributes;
    }
}
