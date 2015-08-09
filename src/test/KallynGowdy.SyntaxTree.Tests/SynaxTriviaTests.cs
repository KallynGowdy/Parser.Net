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

        [Fact]
        public void Test_FindChangeAfterSyntaxNode()
        {
            SyntaxTrivia trivia = new SyntaxTrivia(" ");

            IEnumerable<ISyntaxElement> changes = trivia.DetectChanges("  "); 

            // Note how similar characters are detected as occurring after the original trivia
            Assert.Collection(changes,
                change => Assert.Equal(trivia, change),
                change =>
                {
                    Assert.IsType<SyntaxChangeNode>(change);
                    Assert.Equal(new SyntaxChangeNode(" "), change);
                });
        }

        [Fact]
        public void Test_FindChangeBeforeSyntaxNode()
        {
            SyntaxTrivia trivia = new SyntaxTrivia(" ");

            IEnumerable<ISyntaxElement> changes = trivia.DetectChanges("\t ");

            Assert.Collection(changes,
                change =>
                {
                    Assert.IsType<SyntaxChangeNode>(change);
                    Assert.Equal(new SyntaxChangeNode("\t"), change);
                },
                change => Assert.Equal(trivia, change));
        }

        [Fact]
        public void Test_FindChangesBeforeAndAfterTrivia()
        {
            SyntaxTrivia trivia = new SyntaxTrivia("  ");

            IEnumerable<ISyntaxElement> changes = trivia.DetectChanges("\t  \r\n");

            Assert.Collection(changes,
                change =>
                {
                    Assert.IsType<SyntaxChangeNode>(change);
                    Assert.Equal(new SyntaxChangeNode("\t"), change);
                },
                change => Assert.Equal(trivia, change),
                change =>
                {
                    Assert.IsType<SyntaxChangeNode>(change);
                    Assert.Equal(new SyntaxChangeNode("\r\n"), change);
                });
        }

        [Fact]
        public void Test_FindCompleteChangeReplacingTrivia()
        {
            SyntaxTrivia trivia = new SyntaxTrivia(" ");

            IEnumerable<ISyntaxElement> changes = trivia.DetectChanges("Different");

            Assert.Collection(changes,
                change =>
                {
                    Assert.IsType<SyntaxChangeNode>(change);
                    Assert.Equal(new SyntaxChangeNode("Different"), change);
                });
        }
    }
}
