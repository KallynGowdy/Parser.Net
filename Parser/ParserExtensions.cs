using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace KallynGowdy.ParserGenerator
{
	public static class ParserExtensions
	{
		/// <summary>
		///     Creates a deep copy of the given object by serializing and then deserializing the given object. Where T is
		///     Serializable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <exception cref="System.NotSupportedException" />
		/// <returns></returns>
		public static T DeepCopy<T>(this T obj)
		{
			Type valType = typeof (T);
			if (valType.IsSerializable)
			{
				using (var s = new MemoryStream())
				{
					//serialize the object.
					var formatter = new BinaryFormatter();
					formatter.Serialize(s, obj);

					//reset the stream position
					s.Position = 0;

					//return the deserialized object as a new object.
					return (T) formatter.Deserialize(s);
				}
			}
			throw new NotSupportedException(string.Format("The given generic type parameter {0} must be serializable.", valType.FullName));
		}

		/// <summary>
		///     Concats each individual element in the given dynamic object to produce a single string representing all elements.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static string ConcatStringArray(this object obj, string seperator = " ")
		{
			if (obj is IEnumerable<object>)
			{
				if (((IEnumerable<object>) obj).Count() > 0)
				{
					var b = new StringBuilder();
					foreach (var e in (IEnumerable<object>) obj)
					{
						b.Append(e);
						b.Append(seperator);
					}
					b = b.Remove(b.Length - seperator.Length, seperator.Length);
					return b.ToString();
				}
				return "";
			}
			return obj.ToString();
		}
	}
}