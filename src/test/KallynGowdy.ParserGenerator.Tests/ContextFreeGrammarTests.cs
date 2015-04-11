using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.Grammar.StrongTyped;
using Xunit;

namespace KallynGowdy.ParserGenerator.Tests
{
	/// <summary>
	/// Tests that describe and validate how Context Free Grammars should be created.
	/// </summary>
	public class ContextFreeGrammarTests
	{
		[Fact]
		public void Test_AddTerminalsCreatesProduction()
		{
			var ab = "AB".ToNonTerminal();
			var a = "A".ToTerminal();
			var b = "B".ToTerminal();

			var production = ab ^ a + b;

			Assert.Equal(
				new Production<string>("AB".ToNonTerminal(), "A".ToTerminal(), "B".ToTerminal())
				, production);
		}

		[Fact]
		public void Test_MultiplyTerminalsRepeatsSequenceAndCreatesProduction()
		{
			var ab = "AB".ToNonTerminal();
			var a = "A".ToTerminal();

			var production = ab ^ a * 3;

			Assert.Equal(
				new Production<string>("AB".ToNonTerminal(), "A".ToTerminal(), "A".ToTerminal(), "A".ToTerminal())
				, production);
		}

		public void Test_CreateContextFreeGrammar()
		{
			var ab = "AB".ToNonTerminal();
			var a = "A".ToTerminal();
			var b = "B".ToTerminal();

			var production = ab ^ a + b;

			var cfg = new ContextFreeGrammar<string>(ab, new[] { production });

			//var parser = cfg.CreateParser();

			//Assert.NotNull(parser);

			//Type parserType = parser.GetType();

			//Assert.True(parserType
        }

		public class ABProduction : Production
		{

			public NonTerminal<string> AB { get; }

			public Terminal<string> A { get; }

			public Terminal<string> B { get; }

		}

		public class ACProduction : Production
		{
			public NonTerminal<string> AB { get; }

			public Terminal<string> A { get; }

			public NonTerminal<string> AB_NT { get; } = new NonTerminal<string>("AB");
		}

		public class Grammar : ContextFreeGrammar
		{
			public NonTerminal<string> AB { get; } 

			public ABProduction AbProduction { get; }

			public ACProduction AcProduction { get; }
		}

		[Fact]
		public void Test_CreateProductionFromClass()
		{
			var production = new ABProduction().CreateProduction();

			Assert.Equal(
				new Production<string>("AB".ToNonTerminal(), "A".ToTerminal(), "B".ToTerminal())
				, production);
		}

		[Fact]
		public void Test_CreateContextFreeGrammarFromClass()
		{
			var cfg = new Grammar().CreateContextFreeGrammar();

			Assert.Equal(
				new ContextFreeGrammar<string>(
					"AB".ToNonTerminal(), 
					new []
					{
						new Production<string>("AB".ToNonTerminal(), "A".ToTerminal(), "B".ToTerminal()),
						new Production<string>("AB".ToNonTerminal(), "A".ToTerminal(), "AB".ToNonTerminal())
					}
				),
				cfg
			);
        }
	}
}
