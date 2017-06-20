using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Instance.Utility
{
    public static class StringUtility
    {
        public static string GetRequstParameterValue(string url, string parmeterName)
        {
            Uri uri = new Uri(url);
            string queryString = uri.Query;
            var parameters = HttpUtility.ParseQueryString(queryString.ToLower());
            return parameters[parmeterName.ToLower()] ?? string.Empty;
        }

        public static string ToDate(this DateTime source)
        {
            if (null != source)
            {
                return source.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return string.Empty;
        }
    }
}