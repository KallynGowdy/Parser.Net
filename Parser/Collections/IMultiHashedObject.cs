namespace KallynGowdy.ParserGenerator.Collections
{
	/// <summary>
	///     Provides an interface for objects that can have multiple valid hashes, therefore allowing for relational equality
	///     in dictionaries.
	/// </summary>
	public interface IMultiHashedObject
	{
		/// <summary>
		///     Gets the hash codes that this object matches to. Derived implementations should include the result of GetHashCode
		///     in the results.
		/// </summary>
		/// <returns></returns>
		int[] GetHashCodes();
	}
}