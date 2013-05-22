using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser
{
    /// <summary>
    /// Defines a set of values that determine the possible errors(shift-reduce/reduce-reduce) that can occur with a parse table.
    /// </summary>
    [Flags]
    public enum ParseTableExceptionType
    {
        SHIFT_REDUCE = 1,
        REDUCE_REDUCE = 2,
        FIRST_SET = 4
    };

    /// <summary>
    /// Defines an exception that is thrown when given a parse table cannot be used with a certian parser.
    /// </summary>
    public class InvalidParseTableException<T> : Exception
    {
        /// <summary>
        /// Gets the invalid parse table that was provided.
        /// </summary>
        public IParseTable<T> ParseTable
        {
            get;
            private set;
        }

        public InvalidParseTableException(string message)
            : base(message)
        {
        }

        public InvalidParseTableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidParseTableException(string message, IParseTable<T> invalidTable) : base(message)
        {
            this.ParseTable = invalidTable;
        }

        public InvalidParseTableException(ParseTableExceptionType exceptions, IParseTable<T> invalidTable) : base(createExceptionMsg(exceptions))
        {
            this.ParseTable = invalidTable;
        }

        private static string createExceptionMsg(ParseTableExceptionType exceptions)
        {
            StringBuilder b = new StringBuilder();

            b.Append("The parse table contains the following conflicts: {");

            //shift-reduce conflict
            if((exceptions & ParseTableExceptionType.SHIFT_REDUCE) == ParseTableExceptionType.SHIFT_REDUCE)
            {
                b.Append("SHIFT_REDUCE,");
            }
            //reduce-reduce conflict
            if((exceptions & ParseTableExceptionType.REDUCE_REDUCE) == ParseTableExceptionType.REDUCE_REDUCE)
            {
                b.Append(" REDUCE_REDUCE,");
            }
            //first-set clash
            if((exceptions & ParseTableExceptionType.FIRST_SET) == ParseTableExceptionType.FIRST_SET)
            {
                b.Append(" FIRST_SET_CLASH");
            }

            string s = b.ToString();

            //remove possible trailing ','
            if(s.Last() == ',')
            {
                s = s.Remove(s.Length - 1);
            }

            s += " }";

            return s;
        }
    }
}
