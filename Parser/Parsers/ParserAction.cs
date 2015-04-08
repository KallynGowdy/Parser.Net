using System;
using System.Runtime.Serialization;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
    /// <summary>
    /// Provides an abstract class that defines an action that the parser should take when input is read based on the current state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public abstract class ParserAction<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the parse table that the Action uses to determine what to perform.
        /// </summary>
        [DataMember]
        public ParseTable<T> ParseTable
        {
            get;
            private set;
        }

        public ParserAction(ParseTable<T> table)
        {
            this.ParseTable = table;
        }
    }
}
