using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Defines a table that uses T1 and T2 to relate to T.
    /// </summary>
    /// <typeparam name="TRow">The type of the row objects</typeparam>
    /// <typeparam name="TColumn">The type of the column objects</typeparam>
    /// <typeparam name="TValue">The type of the values to store in the table</typeparam>
    [Serializable]
    public class Table<TRow, TColumn, TValue> : IDictionary<ColumnRowPair<TRow, TColumn>, TValue> where TRow : IEquatable<TRow> where TColumn : IEquatable<TColumn>
    {
        Dictionary<ColumnRowPair<TRow, TColumn>, TValue> lookup;

        /// <summary>
        /// Gets or sets(if it is null) the internal dictionary used by the table.
        /// </summary>
        public Dictionary<ColumnRowPair<TRow, TColumn>, TValue> InternalDictionary
        {
            get
            {
                return lookup;
            }
            set
            {
                if (value != null && lookup == null)
                {
                    lookup = value;
                }
            }
        }

        /// <summary>
        /// Creates a new Empty table.
        /// </summary>
        public Table()
        {
            lookup = new Dictionary<ColumnRowPair<TRow, TColumn>, TValue>();
        }

        /// <summary>
        /// Creates a new table from the given dictionary.
        /// </summary>
        /// <param name="items"></param>
        public Table(IDictionary<ColumnRowPair<TRow, TColumn>, TValue> items)
        {
            lookup = new Dictionary<ColumnRowPair<TRow, TColumn>, TValue>(items);
        }

        /// <summary>
        /// Adds the given Column, Row, and value to the table.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(ColumnRowPair<TRow, TColumn> key, TValue value)
        {
            lookup.Add(key, value);
        }

        /// <summary>
        /// Adds the given column, row, and value to the table.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void Add(TRow row, TColumn column, TValue value)
        {
            lookup.Add(new ColumnRowPair<TRow, TColumn>(row, column), value);
        }

        /// <summary>
        /// Determines if the given Column Row Pair is contained in the Table.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(ColumnRowPair<TRow, TColumn> key)
        {
            return lookup.ContainsKey(key);
        }

        /// <summary>
        /// Gets the Columns and Rows from the table as a pair.
        /// </summary>
        public ICollection<ColumnRowPair<TRow, TColumn>> Keys
        {
            get
            {
                return lookup.Keys;
            }
        }

        /// <summary>
        /// Removes the column/row/value from the table based on the key.
        /// Returns false if the key does not exist in the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(ColumnRowPair<TRow, TColumn> key)
        {
            return lookup.Remove(key);
        }

        /// <summary>
        /// Tries to get a value based on the given key,
        /// Returns ture if the operation was successful, otherwise false.
        /// value is set to the found value or default(T).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(ColumnRowPair<TRow, TColumn> key, out TValue value)
        {
            return lookup.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the collection of values stored in the table.
        /// </summary>
        public TValue[] Values
        {
            get
            {
                return lookup.Values.ToArray();
            }
        }


        /// <summary>
        /// Gets or sets the value at the given row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public TValue this[TRow row, TColumn column]
        {
            get
            {
                //box once to prevent multiple boxes
                //object r = (object)row;

                //object c = (object)column;

                //KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue> v = lookup.FirstOrDefault(a =>
                //{
                //    return r.Equals(a.Key.Row) && c.Equals(a.Key.Column);
                //});

                //return v.Value;
                
                return lookup[new ColumnRowPair<TRow,TColumn>(row, column)];

            }
            set
            {
                var columnRow = lookup.Where(a => a.Key.Row.Equals(row) && a.Key.Column.Equals(column)).First().Key;
                lookup[columnRow] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the value at the given column and row.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[ColumnRowPair<TRow, TColumn> key]
        {
            get
            {
                return lookup[key];
            }
            set
            {
                lookup[key] = value;
            }
        }

        /// <summary>
        /// Gets the value at the given index.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue this[int index]
        {
            get
            {
                return lookup.ElementAt(index).Value;
            }
        }

        /// <summary>
        /// Gets the columns related to the given row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public TColumn[] this[TRow row]
        {
            get
            {
                return lookup.Where(p => p.Key.Row.Equals(row)).Select(c => c.Key.Column).ToArray();
            }
        }

        /// <summary>
        /// Gets the rows related to the given column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public TRow[] this[TColumn column]
        {
            get
            {
                return lookup.Where(p => p.Key.Column.Equals(column)).Select(r => r.Key.Row).ToArray();
            }
        }

        /// <summary>
        /// Adds the given KeyValuePair to the table.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue> item)
        {
            lookup.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clears all of the values from the table.
        /// </summary>
        public void Clear()
        {
            lookup.Clear();
        }

        /// <summary>
        /// Returns whether the table contains the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue> item)
        {
            return lookup.Contains(item);
        }

        /// <summary>
        /// Copies the information in the table to the given array, starting at arrayIndex.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue>[] array, int arrayIndex)
        {
            KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue>[] a = lookup.ToArray();
            a.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of columns in the table. Since the table is always square, the number of
        /// rows equals the number of columns.
        /// </summary>
        public int Count
        {
            get
            {
                return lookup.Count;
            }
        }

        /// <summary>
        /// Determines if the table is read only, always false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the given item from the table based on the key.
        /// Returns false if the item does not exist.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue> item)
        {
            return lookup.Remove(item.Key);
        }

        /// <summary>
        /// Gets the enumerator for the table,
        /// enumerates columns and rows simultaneously.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue>> GetEnumerator()
        {
            return lookup.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the table,
        /// enumerates columns and rows simultaneously.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return lookup.GetEnumerator();
        }


        ICollection<TValue> IDictionary<ColumnRowPair<TRow, TColumn>, TValue>.Values
        {
            get
            {
                return lookup.Values;
            }
        }
    }
}
