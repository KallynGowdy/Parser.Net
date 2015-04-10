using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KallynGowdy.ParserGenerator.Parsers;

namespace KallynGowdy.ParserGenerator.Tests
{
	/// <summary>
	/// Tests for syntax trees.
	/// </summary>
	public class SyntaxTreeTests
	{

		public void Test_AddNode()
		{
			var syntaxTree = new SyntaxTree<string>(
				new SyntaxNode<string>()
			);
		}

	}
}
