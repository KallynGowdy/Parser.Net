using System;

namespace KallynGowdy.SyntaxTree
{
    /// <summary>
    /// Defines a structure that represents a piece of syntax trivia. That is, a section of code/content that was labeled as not meaningful. (essentially noise)
    /// </summary>
    public struct SyntaxTrivia : IEquatable<SyntaxTrivia>
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
            return obj is SyntaxTrivia && Equals((SyntaxTrivia) obj);
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
    }
}