﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.Definitions
{
    /// <summary>
    /// Defines an exception that signifies missing a TokenDefinition for the contained Terminal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MissingTokenDefinition<T> : Exception
    {
        /// <summary>
        /// Gets the terminal that we are missing a definition for.
        /// </summary>
        public Terminal<string> MissingTerminal
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new MissingTokenDefinition exception object, with the given terminal as the
        /// terminal with the missing definition.
        /// </summary>
        /// <param name="missingTerminal"></param>
        public MissingTokenDefinition(Terminal<string> missingTerminal)
            : base(buildExceptionMsg(missingTerminal))
        {
            // TODO: Complete member initialization
            this.MissingTerminal = missingTerminal;
        }

        /// <summary>
        /// Creates an exception message for the given terminal.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static string buildExceptionMsg(Terminal<string> e)
        {
            return string.Format("The definitions are missing a TokenDefinition<{0}> for '{1}'", typeof(T).Name, e.ToString());
        }
    }
}
