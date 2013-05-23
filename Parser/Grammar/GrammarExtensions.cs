using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using Parser.Grammar;

namespace Parser.Grammar
{
    /// <summary>
    /// A static class defining extension methods for the Grammar namespace.
    /// </summary>
    public static class GrammarExtensions
    {
        /// <summary>
        /// Returns a distinct group of elements by grouping the collection and then returning the first element of each group.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="collection"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
        {
            return collection.GroupBy(keySelector).Select(a => a.First());
        }

        /// <summary>
        /// Converts the current dictionary to a table.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static Table<T1, T2, T> ToTable<T1, T2, T>(this IDictionary<ColumnRowPair<T1, T2>, T> collection) where T1 : IEquatable<T1> where T2 : IEquatable<T2>
        {
            return new Table<T1,T2,T>(collection);
        }

        /// <summary>
        /// Converts the current collection to a table based on the given key selector funtion.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="KeySelector"></param>
        /// <returns></returns>
        public static Table<T1, T2, T> ToTable<T1, T2, T>(this IEnumerable<T> collection, Func<T, ColumnRowPair<T1, T2>> KeySelector)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            Table<T1, T2, T> table = new Table<T1,T2,T>();
            foreach(var item in collection)
            {
                table.Add(KeySelector(item), item);
            }
            return table;
        }

        public static Terminal<Token<T>> ToTerminal<T>(this Token<T> value, bool keep)
        {
            return new Terminal<Token<T>>(value, keep, a => value.TokenType.Equals(a.TokenType));
        }

        /// <summary>
        /// Returns a new Terminal(T) object whose value is currentValue.ToString().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Terminal<T> ToTerminal<T>(this T value, bool keep = true, Predicate<T> equalityOperator = null)
        {
            return new Terminal<T>(value, keep, equalityOperator);
        }

        /// <summary>
        /// Returns a new NonTerminal(T) object whose name is currentValue.ToString().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NonTerminal<T> ToNonTerminal<T>(this string value, bool keep = true)
        {
            return new NonTerminal<T>(value.ToString(), keep);
        }


        ///// <summary>
        ///// Returns a new NonTerminal(T) object whose name is currentValue.ToString().
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static NonTerminal<T> ToNonTerminal<T>(this T value, bool keep = true)
        //{
        //    return new NonTerminal<T>(value.ToString(), keep);
        //}
    }
}
