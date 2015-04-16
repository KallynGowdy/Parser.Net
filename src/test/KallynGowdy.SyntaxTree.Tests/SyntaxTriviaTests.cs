using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KallynGowdy.SyntaxTree.Tests
{
	/// <summary>
	/// Tests for <see cref="SyntaxTrivia"/> objects.
	/// </summary>
	public class SyntaxTriviaTests
	{

		public void Test_CreateNewSyntaxTrivia()
		{
			SyntaxTrivia trivia = new SyntaxTrivia("Hi!");

			Assert.Null(trivia.Tree);
			Assert.Null(trivia.Parent);
			Assert.Equal(new SyntaxTrivia("Hi!"), trivia);
			Assert.NotEqual(new SyntaxTrivia("Hello!"), trivia);
			Assert.Equal(3, trivia.Span.Length);
			Assert.Equal(0, trivia.Span.Start);
			Assert.Equal(3, trivia.Span.End);
		}

	}
}
