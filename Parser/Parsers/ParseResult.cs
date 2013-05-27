﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.Parsers
{
    /// <summary>
    /// Defines a class that contains information about the parsing action.
    /// This class is thread-safe (because it is immutable).
    /// </summary>
    public struct ParseResult<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets whether the parse was a success.
        /// </summary>
        public bool Success
        {
            get;
            private set;
        }

        private ParseTree<T> parseTree;

        /// <summary>
        /// Gets the parse tree, copied over so it is immutable.
        /// This tree may or may not be complete, depending on if the parse was a success.
        /// </summary>
        /// <returns></returns>
        public ParseTree<T> GetParseTree()
        {
            if (parseTree != null)
            {
                return new ParseTree<T>(parseTree);
            }
            return null;
        }

        /// <summary>
        /// Gets the errors that occured from the parse.
        /// </summary>
        public ReadOnlyCollection<ParseError> Errors
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the stack that the parse ended with.
        /// </summary>
        public ReadOnlyCollection<KeyValuePair<int, GrammarElement<T>>> Stack
        {
            get;
            private set;
        }

        public ParseResult(bool success, ParseTree<T> tree, IList<KeyValuePair<int, GrammarElement<T>>> parseStack, params ParseError[] errors)
            : this()
        {
            this.Success = success;
            this.parseTree = tree;
            if (parseStack != null)
            {
                this.Stack = new ReadOnlyCollection<KeyValuePair<int, GrammarElement<T>>>(parseStack);
            }
            if (errors != null)
            {
                this.Errors = new ReadOnlyCollection<ParseError>(errors);
            }
        }

        /// <summary>
        /// Defines a parse error.
        /// </summary>
        public struct ParseError
        {
            /// <summary>
            /// Gets the message of the error.
            /// </summary>
            public string Message
            {
                get;
                private set;
            }

            public ParseError(string message)
                : this()
            {
                this.Message = message;
            }
        }
    }
}