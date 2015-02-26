using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageBrowser
{
    public static class Extensions
    {
        public static string Or(this string s, string fallback)
        {
            return string.IsNullOrEmpty(s) ? fallback : s;
        }
    }
}