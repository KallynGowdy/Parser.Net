namespace KallynGowdy.ParserGenerator.Grammar
{
	/// <summary>
	///     Provides an interface for a Context Free Grammar Element.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IGrammarElement<T>
	{
		T InnerValue { get; set; }
		bool Keep { get; set; }
	}
}