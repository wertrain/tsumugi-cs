using System;

namespace Tsumugi.Localize
{
    public static class Localizer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringLocalizer"></param>
        public static void SetStringLocalizer(StringLocalizer stringLocalizer)
        {
            if (s_stringLocalizer != null)
            {
                throw new InvalidOperationException("Some strings have already been localized by another string localizer");
            }
            s_stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static StringLocalizer GetStringLocalizer()
        {
            return s_stringLocalizer ?? (s_stringLocalizer = new StringLocalizer());
        }

        /// <summary>
        /// 
        /// </summary>
        private static StringLocalizer s_stringLocalizer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Localize(this string s)
        {
            return GetStringLocalizer().Localize(s, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Localize(this string s, string context)
        {
            return GetStringLocalizer().Localize(s, context);
        }
    }
}
