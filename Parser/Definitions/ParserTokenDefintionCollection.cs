using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using LexicalAnalysis.Definitions;

namespace Parser.Definitions
{
    /// <summary>
    /// Provides a serializable collection of ParserTokenDefintion objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ParserTokenDefinitionCollection<T> : IEnumerable<ParserTokenDefinition<T>>, IList<ParserTokenDefinition<T>> where T : IEquatable<T>
    {
        /// <summary>
        /// Creates a new collection of parser token definitions that allow matching of token types to terminal elements.
        /// </summary>
        /// <param name="list">The list of parser token definition objects to use in the collection, precedence(highest to lowest) is from the first element to the last element</param>
        /// <summary>
        /// The defintions that this collection contains.
        /// </summary>
        public List<ParserTokenDefinition<T>> Definitions = new List<ParserTokenDefinition<T>>();

        /// <summary>
        /// Gets the definition for the given token.
        /// Returns null if no definition is found.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ParserTokenDefinition<T> GetDefinition(Token<T> token)
        {
            return Definitions.FirstOrDefault(a => a.TokenTypeToMatch.Equals(token.TokenType));
        }

        /// <summary>
        /// Gets the regular TokenDefinition objects from this collection.
        /// </summary>
        /// <returns></returns>
        public TokenDefinitionCollection<T> GetNormalDefinitions()
        {
            return new TokenDefinitionCollection<T>(Definitions.Cast<TokenDefinition<T>>());
        }

        public int IndexOf(ParserTokenDefinition<T> item)
        {
            return Definitions.IndexOf(item);
        }

        public void Insert(int index, ParserTokenDefinition<T> item)
        {
            Definitions.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Definitions.RemoveAt(index);
        }

        public ParserTokenDefinition<T> this[int index]
        {
            get
            {
                return Definitions[index];
            }
            set
            {
                Definitions[index] = value;
            }
        }

        public void Add(ParserTokenDefinition<T> item)
        {
            Definitions.Add(item);
        }

        public void Clear()
        {
            Definitions.Clear();
        }

        public bool Contains(ParserTokenDefinition<T> item)
        {
            return Definitions.Contains(item);
        }

        public void CopyTo(ParserTokenDefinition<T>[] array, int arrayIndex)
        {
            Definitions.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return Definitions.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(ParserTokenDefinition<T> item)
        {
            return Definitions.Remove(item);
        }

        public IEnumerator<ParserTokenDefinition<T>> GetEnumerator()
        {
            return Definitions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Definitions.GetEnumerator();
        }
    }
}
