using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KallynGowdy.SyntaxTree.Tests
{
    public class SynaxTriviaTests
    {
        [Fact]
        public void Test_DefaultSyntaxTriviaReturnsEmptyContent()
        {
            SyntaxTrivia trivia = default(SyntaxTrivia);

            Assert.Equal("", trivia.Content);
            Assert.Equal(0, trivia.Length);
            Assert.Equal(trivia, default(SyntaxTrivia));
            Assert.Equal(trivia, new SyntaxTrivia());
            Assert.Equal(trivia, new SyntaxTrivia(""));
        }

        [Fact]
        public void Test_LengthMatchesContentLength()
        {
            SyntaxTrivia trivia = new SyntaxTrivia("Content");

            Assert.Equal("Content", trivia.Content);
            Assert.Equal(7, trivia.Length);
        }

        [Fact]
        public void Test_TriviaReturnsContentOnToString()
        {
            SyntaxTrivia trivia = new SyntaxTrivia("SomeContent");

            Assert.Equal("SomeContent", trivia.ToString());
            Assert.Same("SomeContent", trivia.Content);
        }
    }
}
