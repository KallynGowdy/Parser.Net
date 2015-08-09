using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace KallynGowdy.SyntaxTree
{
    /// <summary>
    /// Defines a structure that represents a piece of syntax trivia. That is, a section of code/content that was labeled as not meaningful. (essentially noise)
    /// </summary>
    public struct SyntaxTrivia : IEquatable<SyntaxTrivia>, ISyntaxElement
    {
        private readonly string content;

        public SyntaxTrivia(string content) : this()
        {
            this.content = content;
        }

        /// <summary>
        /// Gets the content that is contained in this piece of trivia.
        /// </summary>
        public string Content => content ?? "";

        /// <summary>
        /// Gets the number of characters that this syntax trivia contains.
        /// </summary>
        public int Length => Content.Length;

        public bool Equals(SyntaxTrivia other) => string.Equals(Content, other.Content);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SyntaxTrivia && Equals((SyntaxTrivia)obj);
        }

        public override int GetHashCode() => Content.GetHashCode();

        public static bool operator ==(SyntaxTrivia left, SyntaxTrivia right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SyntaxTrivia left, SyntaxTrivia right)
        {
            return !left.Equals(right);
        }

        public override string ToString() => Content;

        public IEnumerable<ISyntaxElement> DetectChanges(string newContent)
        {
            if (newContent == null) throw new ArgumentNullException(nameof(newContent));
            int i = newContent.IndexOf(Content, StringComparison.Ordinal);
            if (i >= 0)
            {
                int endIndex = i + Content.Length;
                if (i > 0)
                {
                    yield return new SyntaxChangeNode(newContent.Substring(0, i));
                }
                yield return this;
                if (endIndex < newContent.Length)
                {
                    yield return new SyntaxChangeNode(newContent.Substring(endIndex, newContent.Length - endIndex));
                }
            }
            else
            {
                yield return new SyntaxChangeNode(newContent);
            }
        }
    }
}