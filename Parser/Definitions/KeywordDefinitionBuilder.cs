using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Defininitions
{
    /// <summary>
    /// A class that provides a way to build a single regular expression that matches all of the given keywords.
    /// </summary>
    public class KeywordDefinitionBuilder
    {
        /// <summary>
        /// Gets the list of Keywords used by the builder to build a regular expression.
        /// </summary>
        public List<string> Keywords
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a regular expression pattern for the keywords stored in this object.
        /// </summary>
        /// <returns></returns>
        public string GetPattern()
        {
            StringBuilder builder = new StringBuilder();

            var distinctKeywords = Keywords.Distinct();
            foreach(string keyword in distinctKeywords)
            {
                //append the keyword with the OR identifier for the next possible keyword
                //the result is (\bkeyword\b|\bkeyword2\b ...)
                builder.AppendFormat(@"\b{0}\b|", keyword);
            }

            //remove the last '|' character
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        public KeywordDefinitionBuilder()
        {
            this.Keywords = new List<string>();
        }

        public KeywordDefinitionBuilder(IEnumerable<string> keywords)
        {
            this.Keywords = new List<string>(keywords);
        }

        /// <summary>
        /// Gets a regular expression pattern for the given keywords.
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static string GetPattern(IEnumerable<string> keywords)
        {

            //we don't need to create a new KeywordDefinitionBuilder object for this..
            //inlining will/should make it faster

            StringBuilder builder = new StringBuilder();

            var distinctKeywords = keywords.OrderBy(a => a).Distinct();
            foreach (string keyword in distinctKeywords)
            {
                //append the keyword with the OR identifier for the next possible keyword
                //the result is (\bkeyword\b|\bkeyword2\b ...)
                builder.AppendFormat(@"\b{0}\b|", keyword);
            }

            //remove the last '|' character
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

    }
}
