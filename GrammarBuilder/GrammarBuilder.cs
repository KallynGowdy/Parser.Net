using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.Grammar;

namespace GrammarBuilder
{
    /// <summary>
    /// Defines a helper class that builds a context free grammar using recursive functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GrammarBuilder<T> where T : IEquatable<T>
    {
        private List<Production<T>> productions;

        public FormFactory<T> Factory
        {
            get;
            private set;
        }

        /// <summary>
        /// Builds a context free grammar based on the given action.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public ContextFreeGrammar<T> Build(Func<FormBuilder<T>, NonTerminal<T>> form)
        {
            FormBuilder<T> formBuilder = Factory.Get();
            return new ContextFreeGrammar<T>(form(formBuilder), productions);
        }

        /// <summary>
        /// Gets a non terminal object that can be used as reference to the Form used in the given function.
        /// </summary>
        /// <returns></returns>
        public NonTerminal<T> GetDetachedForm(Func<FormBuilder<T>, NonTerminal<T>> form)
        {
            return form(Factory.Get());
        }

        public GrammarBuilder()
        {
            productions = new List<Production<T>>();
            Factory = new FormFactory<T>(productions);
        }
    }

    /// <summary>
    /// Defines a class that creates FormBuilder objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FormFactory<T> : IFactory<FormBuilder<T>>, IFactory<FormBuilder<T>, bool>
    {
        /// <summary>
        /// Gets the current number index of the next name for a FormBuilder object.
        /// </summary>
        public int CurrentFormIndex
        {
            get;
            private set;
        }

        private List<Production<T>> productions;

        public FormFactory(List<Production<T>> productions)
        {
            this.productions = productions;
        }

        private string getNewName()
        {
            return string.Format("Form{0}", CurrentFormIndex++);
        }

        /// <summary>
        /// Gets a new FormBuilder object.
        /// </summary>
        /// <returns></returns>
        public FormBuilder<T> Get()
        {
            return new FormBuilder<T>(getNewName(), this.productions, this);
        }

        /// <summary>
        /// Gets a new FormBuilder object with the given not parameter to determine if the form should be negated.
        /// </summary>
        /// <param name="not"></param>
        /// <returns></returns>
        public FormBuilder<T> Get(bool not)
        {
            return new FormBuilder<T>(getNewName(), this.productions, this, not);
        }
    }

    public class FormBuilder<T>
    {
        private List<Production<T>> productions;

        /// <summary>
        /// Gets the name of the form builder object.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        private NonTerminal<T> nonTerminal;

        private FormFactory<T> factory;

        /// <summary>
        /// Whether the operation should be negated.
        /// </summary>
        private bool not = false;

        /// <summary>
        /// The minimum number of times to repeat the form.
        /// </summary>
        private int minNum = 1;

        /// <summary>
        /// The maximum number of times to repeat the form. -1 means infinite.
        /// </summary>
        private int maxNum = 1;

        ///// <summary>
        ///// Whether to act lazy or not.
        ///// </summary>
        //private bool lazy = false;

        ///// <summary>
        ///// Causes the FormBuilder to act lazily (or not lazily if negation is on).
        ///// </summary>
        //public FormBuilder<T> Lazy
        //{
        //    get
        //    {
        //        if (not)
        //        {
        //            this.lazy = false;
        //        }
        //        else
        //        {
        //            this.lazy = true;
        //        }
        //        return this;
        //    }
        //}

        public FormBuilder(string name, List<Production<T>> productions, FormFactory<T> factory, bool not = false)
        {
            this.factory = factory;
            this.productions = productions;
            this.Name = name;
            nonTerminal = new NonTerminal<T>(name);
            this.not = not;
        }

        /// <summary>
        /// Gets a grammar element representing the derivation of the grammar elements.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public NonTerminal<T> Form(Func<FormBuilder<T>, IEnumerable<GrammarElement<T>>> form)
        {
            IEnumerable<GrammarElement<T>> elements = form(factory.Get(not));
            if (not)
            {
                foreach (GrammarElement<T> element in elements)
                {
                    element.Negated = !element.Negated;
                }
            }
            productions.Add(new Production<T>(nonTerminal, elements.ToArray()));
            return nonTerminal;
        }

        /// <summary>
        /// Gets a grammar element representing the derivation of the grammar elements.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public NonTerminal<T> Form(Func<FormBuilder<T>, GrammarElement<T>> form)
        {
            GrammarElement<T> element = form(factory.Get(not));
            if (not)
            {
                element.Negated = !element.Negated;
            }
            productions.Add(new Production<T>(nonTerminal, element));
            return nonTerminal;
        }

        /// <summary>
        /// Gets a grammar element representing the derivation of the grammar elements.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public NonTerminal<T> Form(Func<IEnumerable<GrammarElement<T>>> form)
        {
            IEnumerable<GrammarElement<T>> elements = form();
            if (not)
            {
                foreach (GrammarElement<T> element in elements)
                {
                    element.Negated = !element.Negated;
                }
            }
            productions.Add(new Production<T>(nonTerminal, elements.ToArray()));
            return nonTerminal;
        }

        /// <summary>
        /// Gets a grammar element representing the derivation of the given grammar elements.
        /// </summary>
        /// <param name="form">/param>
        /// <returns></returns>
        public NonTerminal<T> Form(params GrammarElement<T>[] form)
        {
            return this.Form(() => form);
        }

        /// <summary>
        /// Gets a grammar element representing the derivation of the grammar elements.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public NonTerminal<T> Form(Func<GrammarElement<T>> form)
        {
            GrammarElement<T> element = form();
            if (not)
            {
                element.Negated = !element.Negated;
            }
            productions.Add(new Production<T>(nonTerminal, element));
            return nonTerminal;
        }

        /// <summary>
        /// Gets a grammar element representing the possible derivation of one of the given forms.
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        public NonTerminal<T> OneOf(params Func<FormBuilder<T>, IEnumerable<GrammarElement<T>>>[] forms)
        {
            foreach (var form in forms)
            {
                Form((a) => form(factory.Get(this.not)));
            }
            return nonTerminal;
        }

        /// <summary>
        /// Gets a grammar element representing the possible derivation of one of the given forms.
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        public NonTerminal<T> OneOf(params Func<FormBuilder<T>, GrammarElement<T>>[] forms)
        {
            foreach (var form in forms)
            {
                Form((a) => form(factory.Get(this.not)));
            }
            return nonTerminal;
        }

        /// <summary>
        /// Gets a grammar element representing the possible derivation of one of the given forms.
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        public NonTerminal<T> OneOf(params Func<IEnumerable<GrammarElement<T>>>[] forms)
        {
            foreach (var form in forms)
            {
                Form(() => form());
            }
            return nonTerminal;
        }

        /// <summary>
        /// Gets a grammar element representing the possible derivation of one of the given forms.
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        public NonTerminal<T> OneOf(params Func<GrammarElement<T>>[] forms)
        {
            foreach (var form in forms)
            {
                Form(() => form());
            }
            return nonTerminal;
        }

        /// <summary>
        /// Gets the non-terminal element of this form builder.
        /// </summary>
        /// <returns></returns>
        public NonTerminal<T> Detach()
        {
            return nonTerminal;
        }

        /// <summary>
        /// Performs the opposite of the next operation.
        /// </summary>
        public FormBuilder<T> Not
        {
            get
            {
                this.not = !this.not;
                return this;
            }
        }

        /// <summary>
        /// Defines that the form should occur one or more times.
        /// </summary>
        public FormBuilder<T> OneOrMore
        {
            get
            {
                this.minNum = 1;
                this.maxNum = -1;
                return this;
            }
        }

        /// <summary>
        /// Defines that the form should occur zero or more times.
        /// </summary>
        public FormBuilder<T> ZeroOrMore
        {
            get
            {
                this.minNum = 0;
                this.maxNum = -1;
                return this;
            }
        }

        /// <summary>
        /// Defines that the form should occur zero or one times.
        /// </summary>
        public FormBuilder<T> ZeroOrOne
        {
            get
            {
                this.minNum = 0;
                this.maxNum = 1;
                return this;
            }
        }

        /// <summary>
        /// Defines that the form should occur minimum times or more.
        /// </summary>
        /// <param name="minimum"></param>
        /// <returns></returns>
        public FormBuilder<T> NumOrMore(int minimum)
        {
            if (minimum < 0)
            {
                throw new ArgumentOutOfRangeException("minimum", "Must be greater than or equal to zero");
            }
            this.minNum = minimum;
            this.maxNum = -1;
            return this;
        }

        /// <summary>
        /// Defines that the form should occur minimum to maximum times.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public FormBuilder<T> NumToNum(int minimum, int maximum)
        {
            if (minimum < 0)
            {
                throw new ArgumentOutOfRangeException("minimum", "Must be greater than or equal to zero");
            }
            else if (maximum < 0)
            {
                throw new ArgumentOutOfRangeException("maximum", "Must be greater than or equal to zero");
            }
            this.minNum = minimum;
            this.maxNum = maximum;
            return this;
        }

        /// <summary>
        /// Defines that the form should occur for the specified number of times.
        /// </summary>
        /// <param name="numTimes"></param>
        /// <returns></returns>
        public FormBuilder<T> Times(int numTimes)
        {
            if (numTimes < 0)
            {
                throw new ArgumentOutOfRangeException("numTimes", "Must be greater than or equal to zero");
            }
            this.minNum = numTimes;
            this.maxNum = numTimes;
            return this;
        }
    }
}
