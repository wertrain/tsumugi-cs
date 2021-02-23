using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsumugiRenderer
{
    /// <summary>
    /// 
    /// </summary>
    class Utility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(int color)
        {
            return ToRawColor4(Color.FromArgb(color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(Color color)
        {
            return new SharpDX.Mathematics.Interop.RawColor4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color FromRawColor4(SharpDX.Mathematics.Interop.RawColor4 color)
        {
            return Color.FromArgb((int)(color.A * 255), (int)(color.R * 255), (int)(color.G * 255), (int)(color.B * 255));
        }
    }
}
