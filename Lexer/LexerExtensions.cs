using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis
{
    public static class LexerExtensions
    {
        /// <summary>
        /// Gets the line number of the character at the given index.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetLineNumber(this string s, int index)
        {
            //find the line number.
            return s.Take(index).Count(a => a == '\n') + 1;
        }

        /// <summary>
        /// Gets the column number of the character at the given index.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetColumnNumber(this string s, int index)
        {
            //find the absolute index of the last newline before the error index.
            var lastLine = s.Select((value, i) => new
            {
                value,
                i
            }).LastOrDefault(a => a.value == '\n');

            //get the column number
            int columnNumber = index + 1;
            if (lastLine != null)
            {
                columnNumber = index - lastLine.i + 1;
            }

            return columnNumber;
        }

        /// <summary>
        /// Gets the line number of the element at the given index, using newLine as the new line seperator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="newLine"></param>
        /// <returns></returns>
        public static int GetLineNumber<T>(this IEnumerable<T> input, int index, T newLine)
        {
            //find the line number.
            return input.Take(index).Count(a => newLine.Equals(a)) + 1;
        }

        /// <summary>
        /// Gets the column number of the element at the given index, using newLine as the new line seperator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="newLine"></param>
        /// <returns></returns>
        public static int GetColumnNumber<T>(this IEnumerable<T> input, int index, T newLine)
        {
            //find the absolute index of the last newline before the error index.
            var lastLine = input.Select((value, i) => new
            {
                value,
                i
            }).LastOrDefault(a => newLine.Equals(a));

            //get the column number
            int columnNumber = index + 1;
            if (lastLine != null)
            {
                columnNumber = index - lastLine.i + 1;
            }

            return columnNumber;
        }

        /// <summary>
        /// Gets the line number of the element at the given index, using comparer as the new line comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetLineNumber<T>(this IEnumerable<T> input, int index, Predicate<T> comparer)
        {
            //find the line number.
            return input.Take(index).Count(a => comparer(a)) + 1;
        }

        /// <summary>
        /// Gets the column number of the element at the given index, using comparer as the new line comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="comparer">A comparison operator that, given a value and index, determines whether it is a new line.</param>
        /// <returns></returns>
        public static int GetColumnNumber<T>(this IEnumerable<T> input, int index, Func<T, int, bool> comparer)
        {
            //find the absolute index of the last newline before the error index.
            var lastLine = input.Select((value, i) => new
            {
                value,
                i
            }).LastOrDefault(a => comparer(a.value, a.i));

            //get the column number
            int columnNumber = index + 1;
            if (lastLine != null)
            {
                columnNumber = index - lastLine.i + 1;
            }

            return columnNumber;
        }
    }
}
