using System;
using System.Collections.Generic;
using System.Text;

namespace KallynGowdy.ParserGenerator.Grammar
{
    /// <summary>
    /// Defines a relation between a non terminal element and several terminal/non-terminal elements.
    /// </summary>
    [Serializable]
    public class Production<T>
    {
        /// <summary>
        /// Gets or sets the non terminal element that can be reduced to or derived from. (NonTerminal -> DerivedElements)
        /// </summary>
        public NonTerminal<T> NonTerminal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the elements that relate to the non-terminal.
        /// </summary>
        public List<GrammarElement<T>> DerivedElements
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the element at index, returns null if index is out of range.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GrammarElement<T> GetElement(int index)
        {
            if(index >= 0 && index < DerivedElements.Count)
            {
                return DerivedElements[index];
            }
            else
            {
                return null;
            }
        }

        public Production(NonTerminal<T> nonTerminal, params GrammarElement<T>[] derivedElements)
        {
            this.NonTerminal = nonTerminal;
            this.DerivedElements = new List<GrammarElement<T>>(derivedElements);
        }

        /// <summary>
        /// Returns a string representation of this Production in the form of: NonTerminal -> DerivedElements
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            //add the NonTerminal
            b.Append(NonTerminal);

            //then ' -> '
            b.Append(" -> ");

            //add each of the derived elements
            foreach(GrammarElement<T> element in DerivedElements)
            {
                b.Append(element);
            }

            return b.ToString();
        }
    }
}
