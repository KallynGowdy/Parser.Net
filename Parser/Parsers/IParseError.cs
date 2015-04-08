namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	/// Defines a parse error.
	/// </summary>
	public interface IParseError
	{
		/// <summary>
		/// Gets the message of the error.
		/// </summary>
		string Message
		{
			get;
		}

		/// <summary>
		/// Gets the state that the error occured at.
		/// </summary>
		int State
		{
			get;
		}
	}
}