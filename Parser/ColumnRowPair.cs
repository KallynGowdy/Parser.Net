using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Defines a column that relates to a row.
    /// </summary>
    /// <typeparam name="TRow">The type of the column.</typeparam>
    /// <typeparam name="TColumn">The type of the row.</typeparam>
    public struct ColumnRowPair<TRow, TColumn>
    {
        /// <summary>
        /// Gets or sets the value of the column.
        /// </summary>
        public TRow Row
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the row.
        /// </summary>
        public TColumn Column
        {
            get;
            set;
        }

        public ColumnRowPair(TRow row, TColumn column)
            : this()
        {
            this.Column = column;
            this.Row = row;
        }
    }
}
