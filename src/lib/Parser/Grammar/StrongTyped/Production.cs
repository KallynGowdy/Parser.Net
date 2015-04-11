using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KallynGowdy.ParserGenerator.Grammar.StrongTyped
{
	/// <summary>
	/// Defines an abstract class for a production that is strongly typed.
	/// </summary>
	public abstract class Production
	{

		public Production<string> CreateProduction()
		{
			Type t = this.GetType();
			NonTerminal<string> nonTerminal = null;
			List<GrammarElement<string>> grammarElements = new List<GrammarElement<string>>();
			foreach (var property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (nonTerminal == null &&
				    typeof (NonTerminal<string>).IsAssignableFrom(property.PropertyType))
				{
					nonTerminal = CreateNonTerminal(property);
				}
				else if(typeof(Terminal<string>).IsAssignableFrom(property.PropertyType))
				{
					grammarElements.Add(CreateTerminal(property));
				}
				else if (typeof (NonTerminal<string>).IsAssignableFrom(property.PropertyType))
				{
					grammarElements.Add(CreateNonTerminal(property));
				}
				else if(typeof(GrammarElement<string>) == property.PropertyType)
                {
					grammarElements.Add(CreateGrammarElement(property));
				}
			}

			return new Production<string>(nonTerminal, grammarElements.ToArray());
		}

		private NonTerminal<string> CreateNonTerminal(PropertyInfo property)
		{
			return (property.CanRead ? (NonTerminal<string>)property.GetValue(this) : null) ?? new NonTerminal<string>(property.Name);
		}

		private Terminal<string> CreateTerminal(PropertyInfo property)
		{
			return (property.CanRead ? (Terminal<string>)property.GetValue(this) : null) ?? new Terminal<string>(property.Name);
		}

		private GrammarElement<string> CreateGrammarElement(PropertyInfo property)
		{
			return (property.CanRead ? (GrammarElement<string>)property.GetValue(this) : null) ?? new Terminal<string>(property.Name);
		}
	}
}
