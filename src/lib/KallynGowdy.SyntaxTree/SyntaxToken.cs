using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines a syntax token. That is, a terminal element in the represented language.
	/// As such, <see cref="SyntaxToken"/> values can only contain primitive values.
	/// </summary>
	public struct SyntaxToken
	{
		/// <summary>
		/// Gets the value that this token represents.
		/// </summary>
		public object Value
		{
			get;
		}


		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
