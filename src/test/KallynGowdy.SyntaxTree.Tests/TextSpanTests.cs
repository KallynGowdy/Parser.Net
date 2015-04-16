using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KallynGowdy.SyntaxTree.Tests
{
	/// <summary>
	/// Tests for <see cref="TextSpan"/> objects.
	/// </summary>
	public class TextSpanTests
	{
		[Fact]
		public void Test_CreatingDefaultTextSpanResultsInDefaultValues()
		{
			TextSpan span = default(TextSpan);

			Assert.Equal(0, span.Start);
			Assert.Equal(0, span.Length);
			Assert.Equal(0, span.End);
			Assert.True(span.IsEmpty);
			Assert.Equal(default(TextSpan), span);
		}

		[Fact]
		public void Test_DefaultTextSpanEqualsTextSpanWithZerosInConstructor()
		{
			TextSpan first = default(TextSpan);
			TextSpan second = new TextSpan(0, 0);

			Assert.Equal(first, second);
		}

		[Fact]
		public void Test_CreateTextSpanWithNonDefaultValues()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new TextSpan(-1, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => new TextSpan(0, -1));

			TextSpan span = new TextSpan(10, 100);

			Assert.Equal(10, span.Start);
			Assert.Equal(100, span.Length);
			Assert.Equal(110, span.End);
			Assert.False(span.IsEmpty);
			Assert.Equal(new TextSpan(10, 100), span);
		}

		[Theory]
		[MemberData("Test_SpansOverlap_Data")]
		public void Test_SpansOverlap(TextSpan first, TextSpan second, TextSpan? expectedOverlap)
		{
			Assert.Equal(expectedOverlap != null, first.OverlapsWith(second));
			Assert.Equal(expectedOverlap, first.Overlap(second));
		}

		public static object[] Test_SpansOverlap_Data => new object[]
		{
			new object[]
			{
				new TextSpan(0, 10),
				new TextSpan(1, 10),
				new TextSpan(1, 9)
			},
			new object[]
			{
				// '''''-
				// '-----
				new TextSpan(5, 1),
				new TextSpan(1, 5),
				new TextSpan(5, 1)
			},
			new object[]
			{
				new TextSpan(0, 2),
				new TextSpan(1, 1),
				new TextSpan(1, 1)
			},
			new object[]
			{
				new TextSpan(1, 1),
				new TextSpan(0, 2),
				new TextSpan(1, 1)
			},
			new object[]
			{
				new TextSpan(0, 10),
				new TextSpan(1, 11),
				new TextSpan(1, 9)
			},
			new object[]
			{
				new TextSpan(0, 1),
				new TextSpan(0, 1),
				new TextSpan(0, 1)
			},
			new object[]
			{
				new TextSpan(0, 1),
				new TextSpan(1, 1),
				null
			},
			new object[]
			{
				new TextSpan(1, 1),
				new TextSpan(0, 1),
				null
			},
			new object[]
			{
				new TextSpan(0, 0),
				new TextSpan(0, 0),
				null
			},
		};
		
	}
}
