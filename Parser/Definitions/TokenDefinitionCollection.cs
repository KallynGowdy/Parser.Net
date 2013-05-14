using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace Parser.Defininitions
{
    /// <summary>
    /// Defines a serializable collection of TokenDefiniton(T) objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TokenDefinitionCollection<T> : IList<TokenDefinition<T>>
    {
        /// <summary>
        /// A list of token definitions.
        /// </summary>
        public List<TokenDefinition<T>> Definitions;

        public TokenDefinitionCollection(IEnumerable<TokenDefinition<T>> definitions)
        {
            this.Definitions = new List<TokenDefinition<T>>(definitions);
        }

        public TokenDefinitionCollection()
        {
            this.Definitions = new List<TokenDefinition<T>>();
        }

        /// <summary>
        /// Writes the given collection to the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="collection"></param>
        public static void WriteToFile(string path, TokenDefinitionCollection<T> collection)
        {
            DataContractSerializer serializer = new DataContractSerializer
            (
                typeof(TokenDefinitionCollection<T>),
                new Type[] { typeof(RegexOptions), typeof(StringedTokenDefinition) }
            );

            using (Stream s = File.Open(path, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true
                };
                using (XmlWriter writer = XmlWriter.Create(s, settings))
                {
                    serializer.WriteObject(writer, collection);
                }
            }

        }

        /// <summary>
        /// Reads a collection from the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TokenDefinitionCollection<T> ReadCollection(string path)
        {
            DataContractSerializer serializer = new DataContractSerializer
            (
                typeof(TokenDefinitionCollection<T>),
                new Type[] { typeof(RegexOptions), typeof(StringedTokenDefinition) }
            );

            using (Stream s = File.Open(path, FileMode.Open))
            {
                return (TokenDefinitionCollection<T>)serializer.ReadObject(s);
            }
        }

        /// <summary>
        /// Gets the generic enumerator for this collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TokenDefinition<T>> GetEnumerator()
        {
            return Definitions.GetEnumerator();
        }

        /// <summary>
        /// Gets the non-generic enumerator for this collection.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Definitions.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the given item.
        /// Returns -1 if the index could not be found.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TokenDefinition<T> item)
        {
            return Definitions.IndexOf(item);
        }

        /// <summary>
        /// Inserts the given item at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public void Insert(int index, TokenDefinition<T> item)
        {
            Definitions.Insert(index, item);
        }

        /// <summary>
        /// Removes the object from the collection at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public void RemoveAt(int index)
        {
            Definitions.RemoveAt(index);
        }

        /// <summary>
        /// Gets the item at index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TokenDefinition<T> this[int index]
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

        /// <summary>
        /// Adds the given item to the end of the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(TokenDefinition<T> item)
        {
            Definitions.Add(item);
        }

        /// <summary>
        /// Clears this collection of all items.
        /// </summary>
        public void Clear()
        {
            Definitions.Clear();
        }

        /// <summary>
        /// Returns whether this collection contains(has) the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TokenDefinition<T> item)
        {
            return Definitions.Contains(item);
        }

        /// <summary>
        /// Copies this collection to the given array, starting at arrayIndex.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TokenDefinition<T>[] array, int arrayIndex)
        {
            Definitions.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return Definitions.Count;
            }
        }

        /// <summary>
        /// Returns whether this collection is Read only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the given item from the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TokenDefinition<T> item)
        {
            return Definitions.Remove(item);
        }
    }
}
