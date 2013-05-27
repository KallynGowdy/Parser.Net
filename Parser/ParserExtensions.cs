using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Parser
{
    public static class ParserExtensions
    {
        /// <summary>
        /// Creates a deep copy of the given object by serializing and then deserializing the given object. Where T is Serializable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <exception cref="System.NotSupportedException"/>
        /// <returns></returns>
        public static T DeepCopy<T>(this T obj)
        {
            Type valType = typeof(T);
            if(valType.IsSerializable)
            {
                using(MemoryStream s = new MemoryStream())
                {
                    //serialize the object.
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(s, obj);
                    
                    //reset the stream position
                    s.Position = 0;

                    //return the deserialized object as a new object.
                    return (T) formatter.Deserialize(s);
                }
            }
            else
            {
                throw new NotSupportedException(string.Format("The given generic type parameter {0} must be serializable.", valType.FullName));
            }
        }

        /// <summary>
        /// Concats each individual element in the given dynamic object to produce a single string representing all elements.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ConcatArray(this object obj, string seperator = " ")
        {
            dynamic a = (dynamic)obj;
            if (a is IEnumerable<dynamic>)
            {
                StringBuilder b = new StringBuilder();
                foreach (dynamic e in a)
                {
                    b.Append(e.ToString());
                    b.Append(seperator);
                }
                b = b.Remove(b.Length - seperator.Length, seperator.Length);
                return b.ToString();
            }
            else
            {
                return a.ToString();
            }
        }
    }
}
