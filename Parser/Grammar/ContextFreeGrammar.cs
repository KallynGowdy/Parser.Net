using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Parser.StateMachine;

namespace Parser.Grammar
{
    /// <summary>
    /// Defines a Context Free Grammar(CFG) such that given a string of terminal elements, they will be
    /// reduced to the starting element by the rules defined in the productions.
    /// </summary>
    [Serializable]
    public class ContextFreeGrammar<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Writes this context free grammar to the given stream
        /// </summary>
        /// <exception cref="System.Runtime.Serialization.SerializationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        public void WriteToStream(Stream stream)
        {

            DataContractSerializer ser = new DataContractSerializer(typeof(ContextFreeGrammar<T>));
            XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true
            });
            ser.WriteObject(writer, this);
        }

        /// <summary>
        /// Reads a context free grammar from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ContextFreeGrammar<T> ReadFromStream(Stream stream)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(ContextFreeGrammar<T>));
            return (ContextFreeGrammar<T>)ser.ReadObject(stream);
        }

        private List<Production<T>> productions;

        /// <summary>
        /// Gets or sets the list of productions used in this context free grammar.
        /// </summary>
        public IList<Production<T>> Productions
        {
            get
            {
                return productions;
            }
        }

        /// <summary>
        /// Gets or sets the list of Non-Terminal elements used in this context free grammar.
        /// </summary>
        public IEnumerable<NonTerminal<T>> NonTerminals
        {
            get
            {
                return Productions.Select(p => p.NonTerminal).Distinct();
            }
        }

        /// <summary>
        /// Gets or sets the list of Terminal elements used in this context free grammar.
        /// </summary>
        public IEnumerable<Terminal<T>> Terminals
        {
            get
            {
                return Productions.Select(p => p.DerivedElements.Where(e => e is Terminal<T>).Cast<Terminal<T>>()).SelectMany(a => a).Distinct();
            }
        }

        private NonTerminal<T> startElement;

        /// <summary>
        /// Gets the starting element of this context free grammar. 
        /// </summary>
        public NonTerminal<T> StartElement
        {
            get
            {
                return startElement;
            }
        }

        /// <summary>
        /// Gets the End of Input element
        /// used to denote the end of the input.
        /// </summary>
        public Terminal<T> EndOfInputElement
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new CFG given the starting non-terminal, and productions.
        /// </summary>
        /// <param name="startingElement"></param>
        /// <param name="productions"></param>
        public ContextFreeGrammar(NonTerminal<T> startingElement, IEnumerable<Production<T>> productions)
        {
            //add the production S -> startingElement
            this.startElement = new NonTerminal<T>("S'");
            this.productions = new List<Production<T>>();
            this.productions.Add(new Production<T>(startElement, startingElement));

            this.productions.AddRange(productions);
        }

        /// <summary>
        /// Creates a new CFG given the starting non-terminal, end of input terminal, and productions.
        /// </summary>
        /// <param name="startingElement"></param>
        /// <param name="endOfInput"></param>
        /// <param name="productions"></param>
        public ContextFreeGrammar(NonTerminal<T> startingElement, Terminal<T> endOfInput, IEnumerable<Production<T>> productions)
        {
            this.startElement = new NonTerminal<T>("S'");
            this.productions = new List<Production<T>>();
            this.productions.Add(new Production<T>(startElement, startingElement));
            this.productions.AddRange(productions);
            this.EndOfInputElement = endOfInput;
        }

        /// <summary>
        /// Returns a nicely formatted string representation of this CFG.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (Production<T> p in Productions)
            {
                builder.Append(p);
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
