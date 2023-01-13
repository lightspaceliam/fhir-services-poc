using Microsoft.Extensions.Primitives;
using System.Text;

namespace Api.Extensions
{
    public static class OptimisticQueryStringBuilderExtensions
    {
        /// <summary>
        /// Optimistically builds the query string and assumes query string composition and logic was handled by the subscribing client.
        /// Generic for all Hl7 Fhir Resources.
        /// </summary>
        /// <param name="queryPairs"></param>
        /// <returns></returns>
        public static string ToOptimisticQueryString(this List<KeyValuePair<string, StringValues>> queryPairs)
        {
            if (queryPairs.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var isFirst = true;
            foreach (var kvp in queryPairs)
            {
                var delimiter = isFirst == true ? "?" : "&";

                sb.Append(delimiter).Append(kvp.Key).Append('=').Append(kvp.Value.FirstOrDefault()?.ToString());

                isFirst = false;
            }

            var queryString = sb.ToString();
            if (sb.Length > 0)
            {
                sb.Clear();
            }

            return queryString;
        }
    }
}
