using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis
{
    /// <summary>
    /// Defines basic Token Types.
    /// </summary>
    public static partial class TokenTypes
    {
        /// <summary>
        /// Defines the token type "keyword"
        /// </summary>
        public const string KEYWORD = "keyword";

        /// <summary>
        /// Defines the token type "number"
        /// </summary>
        public const string NUMBER = "number";

        /// <summary>
        /// Defines the token type "unknown".
        /// </summary>
        public const string UNKNOWN = "unknown";

    }
}
