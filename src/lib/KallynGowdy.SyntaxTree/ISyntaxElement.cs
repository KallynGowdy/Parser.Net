using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines an interface that represents an element of syntax.
	/// </summary>
	public interface ISyntaxElement
	{
        /// <summary>
        /// Gets the number of characters that this element spans.
        /// </summary>
		int Length { get; }
	}
}
