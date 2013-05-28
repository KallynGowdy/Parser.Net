using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.Parsers
{
    /// <summary>
    /// Defines a set of values that determine the possible errors(shift-reduce/reduce-reduce) that can occur with a parse table.
    /// </summary>
    [Flags]
    public enum ParseTableExceptionType
    {
        SHIFT_REDUCE = 1,
        REDUCE_REDUCE = 2,
        FIRST_SET = 3
    };

    /// <summary>
    /// Defines an exception that is thrown when given a parse table cannot be used with a certian parser.
    /// </summary>
    [Serializable]
    public class InvalidParseTableException<T> : Exception where T : IEquatable<T>
    {
        private Tuple<ParseTableExceptionType, int, GrammarElement<T>>[] conflicts;

        /// <summary>
        /// Gets the conflicts contained in the parse table.
        /// </summary>
        public ReadOnlyCollection<Tuple<ParseTableExceptionType, int, GrammarElement<T>>> Conflicts
        {
            get
            {
                return new ReadOnlyCollection<Tuple<ParseTableExceptionType, int, GrammarElement<T>>>(conflicts);
            }
        }

        /// <summary>
        /// Gets the invalid parse table that was provided.
        /// </summary>
        public IParseTable<T> ParseTable
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state that the exception occurs at.
        /// </summary>
        public int State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the input/column that the exception occurs at.
        /// </summary>
        public GrammarElement<T> NextInput
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

        public InvalidParseTableException(string message, IParseTable<T> invalidTable)
            : base(message)
        {
            this.ParseTable = invalidTable;
        }

        public InvalidParseTableException(ParseTableExceptionType exceptions, IParseTable<T> invalidTable, int state = -1, GrammarElement<T> nextInput = null)
            : base(createExceptionMsg(exceptions))
        {
            this.ParseTable = invalidTable;
            this.State = state;
            this.NextInput = nextInput;
        }

        public InvalidParseTableException(IParseTable<T> invalidTable, params Tuple<ParseTableExceptionType, int, GrammarElement<T>>[] conflicts)
            : base(createExceptionMsg(conflicts))
        {
            this.conflicts = conflicts;
            this.ParseTable = invalidTable;
        }



        private static string createExceptionMsg(Tuple<ParseTableExceptionType, int, GrammarElement<T>>[] conflicts)
        {
            StringBuilder b = new StringBuilder();

            b.AppendFormat("There are {0} conflicts contained in the parse table:", conflicts.Length);
            b.AppendLine();

            for(int i = 0; i < conflicts.Length; i++)
            {
                var conflict = conflicts[i];

                b.AppendFormat("{0}). {1} conflict at (State: {2}, Column {3})", i + 1, conflict.Item1, conflict.Item2, conflict.Item3);
            }

            return b.ToString();
        }

        static string createExceptionMsg(ParseTableExceptionType exceptions)
        {
            StringBuilder b = new StringBuilder();

            b.Append("The parse table contains the following conflicts: {");

            //shift-reduce conflict
            if ((exceptions & ParseTableExceptionType.SHIFT_REDUCE) == ParseTableExceptionType.SHIFT_REDUCE)
            {
                b.Append("SHIFT_REDUCE,");
            }
            //reduce-reduce conflict
            if ((exceptions & ParseTableExceptionType.REDUCE_REDUCE) == ParseTableExceptionType.REDUCE_REDUCE)
            {
                b.Append(" REDUCE_REDUCE,");
            }
            //first-set clash
            if ((exceptions & ParseTableExceptionType.FIRST_SET) == ParseTableExceptionType.FIRST_SET)
            {
                b.Append(" FIRST_SET_CLASH");
            }

            string s = b.ToString();

            //remove possible trailing ','
            if (s.Last() == ',')
            {
                s = s.Remove(s.Length - 1);
            }

            s += " }";

            return s;
        }
    }
}
