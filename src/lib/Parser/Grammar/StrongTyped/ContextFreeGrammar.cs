using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KallynGowdy.ParserGenerator.Grammar.StrongTyped
{
	/// <summary>
	/// Defines an abstract class that represents a strongly typed context free grammar.
	/// </summary>
	public abstract class ContextFreeGrammar
	{

		public ContextFreeGrammar<string> CreateContextFreeGrammar()
		{
			Type t = this.GetType();

			NonTerminal<string> startingNonTerminal = null;
			List<Production<string>> productions = new List<Production<string>>();

			foreach (var property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (startingNonTerminal == null &&
					typeof(NonTerminal<string>).IsAssignableFrom(property.PropertyType))
				{
					startingNonTerminal = CreateNonTerminal(property);
				}
				else if (typeof(Production).IsAssignableFrom(property.PropertyType))
				{
					productions.Add(CreateProduction(property));
				}
			}

			return new ContextFreeGrammar<string>(startingNonTerminal, productions);
		}

		private NonTerminal<string> CreateNonTerminal(PropertyInfo property)
		{
			return (property.CanRead ? (NonTerminal<string>)property.GetValue(this) : null) ?? new NonTerminal<string>(property.Name);
        }

		private Production<string> CreateProduction(PropertyInfo property)
		{
			return ((property.CanRead ? (Production)property.GetValue(this) : null) ?? (Production)Activator.CreateInstance(property.PropertyType)).CreateProduction();
		}
	}
}
