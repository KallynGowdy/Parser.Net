using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Parser.Collections
{
    /// <summary>
    /// Defines a table that uses T1 and T2 to relate to T.
    /// </summary>
    /// <typeparam name="TRow">The type of the row objects</typeparam>
    /// <typeparam name="TColumn">The type of the column objects</typeparam>
    /// <typeparam name="TValue">The type of the values to store in the table</typeparam>
    [Serializable]
    public class Table<TRow, TColumn, TValue> : IDictionary<TRow, KeyedDictionary<TColumn, TValue>>
        where TRow : IEquatable<TRow>
        where TColumn : IEquatable<TColumn>
    {
        KeyedDictionary<TRow, KeyedDictionary<TColumn, TValue>> lookup;

        //Dictionary<ColumnRowPair<TRow, TColumn>, TValue> lookup;

        /// <summary>
        /// Gets or sets(if it is null) the internal dictionary used by the table.
        /// </summary>
        public KeyedDictionary<TRow, KeyedDictionary<TColumn, TValue>> InternalDictionary
        {
            get
            {
                return lookup.DeepCopy();
            }
            internal set
            {
                if (value != null && lookup == null)
                {
                    lookup = value;
                }
            }
        }

        /// <summary>
        /// Gets the IEqualityComparer for the Rows of the table.
        /// </summary>
        public IEqualityComparer<TRow> RowComparer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the IEqualityComparer for the Columns of the table.
        /// </summary>
        public IEqualityComparer<TColumn> ColumnComparer
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new Empty table.
        /// </summary>
        public Table()
        {
            lookup = new KeyedDictionary<TRow, KeyedDictionary<TColumn, TValue>>();
            this.RowComparer = EqualityComparer<TRow>.Default;
            this.ColumnComparer = EqualityComparer<TColumn>.Default;
        }

        /// <summary>
        /// Creates a new table from the given dictionary.
        /// </summary>
        /// <param name="items"></param>
        public Table(IDictionary<TRow, KeyedDictionary<TColumn, TValue>> items)
            : this()
        {
            lookup = new KeyedDictionary<TRow, KeyedDictionary<TColumn, TValue>>(items);
        }

        /// <summary>
        /// Creates a new Table with the given IEqualityComparer objects used to compare keys.
        /// </summary>
        /// <param name="rowComparer"></param>
        /// <param name="columnComparer"></param>
        public Table(IEqualityComparer<TRow> rowComparer, IEqualityComparer<TColumn> columnComparer)
        {
            this.RowComparer = rowComparer;
            this.ColumnComparer = columnComparer;
            lookup = new KeyedDictionary<TRow, KeyedDictionary<TColumn, TValue>>(RowComparer);
        }

        /// <summary>
        /// Adds the given Column, Row, and value to the table.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(ColumnRowPair<TRow, TColumn> key, TValue value)
        {
            lookup.Add(key.Row,
            new KeyedDictionary<TColumn, TValue>
            {
                {key.Column, value}
            });
        }

        /// <summary>
        /// Adds the given column, row, and value to the table.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void Add(TRow row, TColumn column, TValue value)
        {
            if (lookup.ContainsKey(row))
            {
                lookup[row].Add(column, value);
            }
            else
            {
                lookup.Add(row, new KeyedDictionary<TColumn, TValue>
                {
                    {column, value}
                });
            }
        }

        /// <summary>
        /// Determines if the given Column Row Pair is contained in the Table.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(ColumnRowPair<TRow, TColumn> key)
        {
            return lookup.ContainsKey(key.Row) && lookup[key.Row].ContainsKey(key.Column);
        }

        /// <summary>
        /// Determines if the given column and row are contained in this table.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool ContainsKey(TRow row, TColumn column)
        {
            return lookup.ContainsKey(row) && lookup[row].ContainsKey(column);
        }

        /// <summary>
        /// Gets the Columns and Rows from the table as a pair.
        /// </summary>
        public ICollection<TRow> Rows
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
            return lookup[key.Row].Remove(key.Column);
        }

        /// <summary>
        /// Tries to get a value based on the given key,
        /// Returns true if the operation was successful, otherwise false.
        /// value is set to the found value or default(T).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(ColumnRowPair<TRow, TColumn> key, out TValue value)
        {
            KeyedDictionary<TColumn, TValue> v;
            if (!lookup.TryGetValue(key.Row, out v))
            {
                return v.TryGetValue(key.Column, out value);
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Tries to get a value based on the given key,
        /// Returns true if the operation was successful, otherwise false.
        /// value is set to the found value of default(T).
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TRow row, TColumn column, out TValue value)
        {
            KeyedDictionary<TColumn, TValue> v;
            if (lookup.TryGetValue(row, out v))
            {
                return v.TryGetValue(column, out value);
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Gets the collection of values stored in the table.
        /// </summary>
        public TValue[] Values
        {
            get
            {
                return lookup.Values.SelectMany(a => a.Values).ToArray();
            }
        }


        /// <summary>
        /// Gets or sets the value at the given row and column.
        /// Returns default (TValue) if the given row, column pair cannot be found.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public TValue this[TRow row, TColumn column]
        {
            get
            {
                TValue result;
                this.TryGetValue(row, column, out result);
                return result;
            }
            set
            {
                lookup[row][column] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the value at the given column and row.
        /// Returns default(TValue) if the key cannot be found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[ColumnRowPair<TRow, TColumn> key]
        {
            get
            {
                TValue result;
                TryGetValue(key, out result);
                return result;
            }
            set
            {
                lookup[key.Row][key.Column] = value;
            }
        }

        /// <summary>
        /// Gets the rows of the table given the column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public TRow[] GetRows(TColumn column)
        {
            return lookup.Where(a => a.Value.ContainsKey(column)).Select(a => a.Key).ToArray();
        }

        /// <summary>
        /// Gets the columns of the table given the row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public TColumn[] GetColumns(TRow row)
        {
            return lookup[row].Select(a => a.Key).ToArray();
        }

        /// <summary>
        /// Adds the given KeyValuePair to the table.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<ColumnRowPair<TRow, TColumn>, TValue> item)
        {
            if (lookup.ContainsKey(item.Key.Row))
            {
                lookup[item.Key.Row].Add(item.Key.Column, item.Value);
            }
            else
            {
                lookup.Add(item.Key.Row,
                new KeyedDictionary<TColumn, TValue>
                {
                    {item.Key.Column, item.Value}
                });
            }
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
            return lookup.ContainsKey(item.Key.Row) && lookup[item.Key.Row].ContainsKey(item.Key.Column) && lookup[item.Key.Row][item.Key.Column].Equals(item.Value);
        }

        /// <summary>
        /// Copies the information in the table to the given array, starting at arrayIndex.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TRow, KeyedDictionary<TColumn, TValue>>[] array, int arrayIndex)
        {
            var temp = lookup.ToArray();
            temp.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of rows in the table. Since the table is always square, the number of
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
            
            return lookup[item.Key.Row].Remove(item.Key.Column);
        }

        /// <summary>
        /// Gets the enumerator for the table,
        /// enumerates columns and rows simultaneously.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TRow, KeyedDictionary<TColumn, TValue>>> GetEnumerator()
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

        public void Add(TRow key, KeyedDictionary<TColumn, TValue> value)
        {
            lookup.Add(key, value);
        }

        public bool ContainsKey(TRow key)
        {
            return lookup.ContainsKey(key);
        }

        public ICollection<TRow> Keys
        {
            get
            {
                return lookup.Keys;
            }
        }

        public bool Remove(TRow key)
        {
            return lookup.Remove(key);
        }

        public bool TryGetValue(TRow key, out KeyedDictionary<TColumn, TValue> value)
        {
            return lookup.TryGetValue(key, out value);
        }

        ICollection<KeyedDictionary<TColumn, TValue>> IDictionary<TRow, KeyedDictionary<TColumn, TValue>>.Values
        {
            get
            {
                return lookup.Values;
            }
        }

        public KeyedDictionary<TColumn, TValue> this[TRow key]
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

        public void Add(KeyValuePair<TRow, KeyedDictionary<TColumn, TValue>> item)
        {
            lookup.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TRow, KeyedDictionary<TColumn, TValue>> item)
        {
            return lookup.Contains(item);
        }

        public bool Remove(KeyValuePair<TRow, KeyedDictionary<TColumn, TValue>> item)
        {
            return lookup.Remove(item.Key);
        }

        IEnumerator<KeyValuePair<TRow, KeyedDictionary<TColumn, TValue>>> IEnumerable<KeyValuePair<TRow, KeyedDictionary<TColumn, TValue>>>.GetEnumerator()
        {
            return lookup.GetEnumerator();
        }
    }
}
