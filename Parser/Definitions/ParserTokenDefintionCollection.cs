using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis.Defininitions;

namespace Parser.Definitions
{
    /// <summary>
    /// Provides a serializable collection of ParserTokenDefintion objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ParserTokenDefintionCollection<T> : IList<ParserTokenDefinition<T>>
    {
        /// <summary>
        /// The defintions that this collection contains.
        /// </summary>
        public List<ParserTokenDefinition<T>> Definitions = new List<ParserTokenDefinition<T>>();

        /// <summary>
        /// Gets the regular TokenDefinition objects from this collection.
        /// </summary>
        /// <returns></returns>
        public TokenDefinitionCollection<T> GetNormalDefinitions()
        {
            return new TokenDefinitionCollection<T>(Definitions);
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
            get { return Definitions.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
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
