using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using SQLite3DbHandle = System.IntPtr;
using SQLite3Statement = System.IntPtr;
using Object = System.Object;
using Framework.DataStruct;

namespace Framework.SQLite3
{
    public class SQLite3Handle
    {
        private SQLite3DbHandle handle;

        private StringBuilder stringBuilder;

        public SQLite3Handle(string InDataBasePath) :
            this(InDataBasePath, SQLite3OpenFlags.ReadWrite)
        {
        }

        public SQLite3Handle(string InDataBasePath, SQLite3OpenFlags InFlags)
        {
            Assert.IsFalse(string.IsNullOrEmpty(InDataBasePath), "Database path can not be null.");

            if (SQLite3Result.OK != SQLite3.Open(ConvertStringToUTF8Bytes(InDataBasePath),
                out handle, (int)InFlags, IntPtr.Zero))
            {
                SQLite3.Close(handle);
                handle = IntPtr.Zero;
                Debug.LogError( "Database failed to open.");
            }
            else
            {
                stringBuilder = new StringBuilder(1024);
            }
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="InSQLStetement">SQL Statement.</param>
        public void CreateTable(string InSQLStetement)
        {
            Exec(InSQLStetement);
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InColumnNameAndType">In column name and type.</param>
        public void CreateTable(string InTableName, params string[] InColumnNameAndType)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("CREATE TABLE ").Append(InTableName).Append(" (");
            int length = InColumnNameAndType.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InColumnNameAndType[i]).Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public void CreateTable<T>() where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            Exec("DROP TABLE IF EXISTS " + property.ClassName);

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("CREATE TABLE ").Append(property.ClassName).Append("(");
            int length = property.Infos.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append(property.Infos[i].Name);

                PropertyInfo info = property.Infos[i];
                Type type = property.Infos[i].PropertyType;

                if (info.PropertyType == typeof(int) || info.PropertyType == typeof(long))
                {
                    stringBuilder.Append(" INTEGER ");
                }
                else if (info.PropertyType == typeof(float) || info.PropertyType == typeof(double))
                {
                    stringBuilder.Append(" REAL ");
                }
                else if (info.PropertyType == typeof(string))
                {
                    stringBuilder.Append(" TEXT ");
                }
                else
                {
                    stringBuilder.Append(" BLOB ");
                }

                object[] objs = property.Infos[i].GetCustomAttributes(typeof(SQLite3ConstraintAttribute), false);
                if (objs.Length == 1 && objs[0] is SQLite3ConstraintAttribute)
                    stringBuilder.Append((objs[0] as SQLite3ConstraintAttribute).Constraint);

                stringBuilder.Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Execute insert SQL statement.
        /// </summary>
        /// <param name="InSQLstatement">In SQL statement.</param>
        public void Insert(string InSQLstatement)
        {
            Exec(InSQLstatement);
        }

        /// <summary>
        /// Execute insert SQL statement Through the assembly parameters into SQL statements.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InData">Data inserted to the table.</param>
        public void Insert(string InTableName, params object[] InData)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("INSERT INTO ").Append(InTableName).Append(" VALUES(");

            int length = InData.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append("'")
                    .Append(InData[i].ToString().Replace("'", "''"))
                    .Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Insert subclass of Base into the table.
        /// </summary>
        /// <param name="InValue">Subclass of Base object.</param>
        /// <typeparam name="T">Subclass of Base</typeparam>
        public void InsertT<T>(T InValue) where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("INSERT INTO ").Append(property.ClassName).Append(" VALUES(");

            int length = property.Infos.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append("'")
                     .Append(property.Infos[i].GetValue(InValue, null).ToString().Replace("'", "''"))
                    .Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Insert some Base subclasses into the table.
        /// </summary>
        /// <param name="InValue">Some Base subclasses list.</param>
        /// <typeparam name="T">subclass of Base.</typeparam>
        public void InsertAllT<T>(List<T> InValue) where T : Base
        {
            if (null == InValue) throw new ArgumentNullException();
            int count = InValue.Count;
            if (count > 0)
            {
                SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

                for (int i = 0; i < count; ++i)
                {
                    stringBuilder.Remove(0, stringBuilder.Length);
                    stringBuilder.Append("INSERT INTO ").Append(property.ClassName).Append(" VALUES(");

                    int length = property.Infos.Length;
                    for (int j = 0; j < length; j++)
                    {
                        stringBuilder.Append("'")
                             .Append(property.Infos[j].GetValue(InValue[i], null).ToString().Replace("'", "''"))
                            .Append("', ");
                    }
                    stringBuilder.Remove(stringBuilder.Length - 2, 2);
                    stringBuilder.Append(")");

                    Exec(stringBuilder.ToString());
                }
            }
        }

        /// <summary>
        /// According to the SQL statement to update the table. 
        /// </summary>
        /// <param name="InSQLStatement">In SQL statement.</param>
        public void Update(string InSQLStatement)
        {
            Exec(InSQLStatement);
        }

        /// <summary>
        /// Execute update SQL statement Through the assembly parameters into SQL statements.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InCondition">Analyzing conditions.</param>
        /// <param name="InData">Data update to the table.</param>
        public void Update(string InTableName, string InCondition, params string[] InData)
        {
            if (InData.Length < 1) throw new ArgumentNullException();

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ").Append(InTableName).Append(" SET ");

            int length = InData.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InData[i]).Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(" WHERE ").Append(InCondition);

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// According to the subclass of Base to update the table.
        /// </summary>
        /// <param name="InValue">Base object.</param>
        /// <typeparam name="T">subclass of Base</typeparam>
        public void UpdateT<T>(T InValue) where T : Base
        {
            if (null == InValue) throw new ArgumentNullException();

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ").Append(property.ClassName).Append(" SET ");

            int length = property.Infos.Length;
            for (int i = 1; i < length; i++)
            {
                stringBuilder.Append(property.Infos[i].Name)
                    .Append(" = '")
                    .Append(property.Infos[i].GetValue(InValue, null).ToString().Replace("'", "''"))
                    .Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(" WHERE ID = ").Append(property.Infos[0].GetValue(InValue, null));

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// The value obtained by Key Reflection updates the table
        /// </summary>
        /// <param name="InIndex">The index of the object property.</param>
        /// <param name="InValue">Base subclass object</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public void UpdateTByKeyValue<T>(int InIndex, T InValue) where T : Base
        {
            if (null == InValue) throw new ArgumentNullException();
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            if (InIndex < 0 || InIndex >= property.InfosLength) throw new ArgumentOutOfRangeException();

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ")
                .Append(property.ClassName)
                .Append(" SET ")
                .Append(property.Infos[InIndex].Name)
                .Append(" = '")
                 .Append(property.Infos[InIndex].GetValue(InValue, null).ToString().Replace("'", "''"))
                .Append("' WHERE ID = ")
                .Append(property.Infos[0].GetValue(InValue, null));

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// According to the Base subclass object updates the table or insert into the table.
        /// </summary>
        /// <param name="InT">Base subclass object.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public void UpdateOrInsert<T>(T InT) where T : Base
        {
            if (null == InT) throw new ArgumentNullException();
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            SQLite3Statement stmt;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ID = ")
                .Append(property.Infos[0].GetValue(InT, null));

            bool isUpdate = false;
            string sql = stringBuilder.ToString();
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, sql, GetUTF8ByteCount(sql), out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    isUpdate = SQLite3.ColumnCount(stmt) > 0;
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }
            SQLite3.Finalize(stmt);

            if (isUpdate) UpdateT(InT);
            else InsertT(InT);
        }

        /// <summary>
        /// According to the ID from the table to read a piece of data.
        /// </summary>
        /// <returns>a piece of data.</returns>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InID">In ID as table key.</param>
        public Object[] SelectSingleData(string InTableName, int InID)
        {
            if (handle == IntPtr.Zero) throw new NullReferenceException("Please open database first.");

            Object[] obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InID);

            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());
            obj = GetObjects(stmt, SQLite3.ColumnCount(stmt));

            SQLite3.Finalize(stmt);

            return obj;
        }

        /// <summary>
        /// According to the ID from the table to read multiple data.
        /// </summary>
        /// <returns>The multiple data.</returns>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InColumnName">In column name.</param>
        /// <param name="InOperator">In Operator</param>
        /// <param name="InCondition">In condition.</param>
        public List<Object[]> SelectMultiData(string InTableName, string InColumnName, string InOperator, string InCondition)
        {
            List<Object[]> obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT ")
                .Append(InColumnName)
                .Append(" FROM ")
                .Append(InTableName)
                .Append(" WHERE ")
                .Append(InOperator)
                .Append(" ")
                .Append(InCondition);

            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());

            obj = new List<object[]>();
            int count = SQLite3.ColumnCount(stmt);

            while (SQLite3Result.Row == SQLite3.Step(stmt))
            {
                obj.Add(GetObjects(stmt, count));
            }

            SQLite3.Finalize(stmt);

            return obj;
        }

        /// <summary>
        /// Resolve the database results.
        /// </summary>
        /// <returns>The objects.</returns>
        /// <param name="InStmt">In sqlite statement.</param>
        /// <param name="InCount">In result count.</param>
        private Object[] GetObjects(SQLite3Statement InStmt, int InCount)
        {
            Object[] objs = new object[InCount];

            for (int i = 0; i < InCount; ++i)
            {
                SQLite3DataType type = SQLite3.ColumnType(InStmt, i);

                switch (type)
                {
                    case SQLite3DataType.Integer:
                        objs[i] = SQLite3.ColumnInt(InStmt, i);
                        break;
                    case SQLite3DataType.Real:
                        objs[i] = SQLite3.ColumnDouble(InStmt, i);
                        break;
                    case SQLite3DataType.Text:
                        objs[i] = SQLite3.ColumnText(InStmt, i);
                        break;
                    case SQLite3DataType.Blob:
                        objs[i] = SQLite3.ColumnBlob(InStmt, i);
                        break;
                    case SQLite3DataType.Null:
                        objs[i] = null;
                        break;
                }
            }

            return objs;
        }

        /// <summary>
        /// Query the object from the database by ID.
        /// </summary>
        /// <returns>Base subclass object.</returns>
        /// <param name="InID">In table id as key.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T SelectTByID<T>(int InID) where T : Base, new()
        {
            return SelectT<T>("SELECT * FROM "
                              + SyncFactory.GetSyncProperty(typeof(T)).ClassName
                              + " WHERE ID = " + InID);
        }

        /// <summary>
        /// Query the object from the database by index.
        /// </summary>
        /// <returns>Base subclass object.</returns>
        /// <param name="InIndex">In index as key, the index value is automatically generated by the database.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T SelectTByIndex<T>(int InIndex) where T : Base, new()
        {
            return SelectT<T>("SELECT * FROM "
                              + SyncFactory.GetSyncProperty(typeof(T)).ClassName
                              + " WHERE rowid = " + (InIndex + 1));    //SQLite3 rowid begin with 1.
        }

        /// <summary>
        /// Query the object from the database by property index and perperty's value.
        /// </summary>
        /// <returns>Base subclass object.</returns>
        /// <param name="InPropertyIndex">In property index, The index value is specified by the SyncAttribute.</param>
        /// <param name="InExpectedValue">Expected values.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T SelectTByKeyValue<T>(int InPropertyIndex, object InExpectedValue) where T : Base, new()
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            if (InPropertyIndex < 0 || InPropertyIndex >= property.InfosLength) throw new IndexOutOfRangeException();

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(property.ClassName)
                         .Append(" WHERE ")
                         .Append(property.Infos[InPropertyIndex])
                         .Append(" = ")
                         .Append(InExpectedValue);

            return SelectT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the object from the database by property name and perperty's value.
        /// </summary>
        /// <returns>Base subclass object.</returns>
        /// <param name="InPropertyName">In property name.</param>
        /// <param name="InExpectedValue">Expected values.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T SelectTByKeyValue<T>(string InPropertyName, object InExpectedValue) where T : Base, new()
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(SyncFactory.GetSyncProperty(typeof(T)).ClassName)
                         .Append(" WHERE ")
                         .Append(InPropertyName)
                         .Append(" = ")
                         .Append(InExpectedValue);

            return SelectT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the object from the database by sql statement.
        /// </summary>
        /// <returns>Base subclass object.</returns>
        /// <param name="InSQLStatement">In sql statement.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T SelectT<T>(string InSQLStatement) where T : Base, new()
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            T t = default(T);
            SQLite3Statement stmt = ExecuteQuery(InSQLStatement);

            t = GetT(new T(), property.Infos, stmt, property.InfosLength);

            SQLite3.Finalize(stmt);

            return t;
        }

        /// <summary>
        /// Query the database by property indexes and expected value and return the dictionary.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InIndexes">property indexes, The index value is specified by the SyncAttribute.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public Dictionary<int, T> SelectDictT<T>(int[] InIndexes, string[] InOperators, int[] InExpectedValues) where T : Base, new()
        {
            if (null == InIndexes || null == InOperators || null == InExpectedValues) throw new ArgumentNullException();
            int length = InIndexes.Length;
            if (length != InOperators.Length || length != InExpectedValues.Length) throw new ArgumentException("Parameter length does not match.");

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(property.ClassName)
                         .Append(" WHERE ");

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(property.Infos[InIndexes[i]].Name)
                             .Append(" ")
                             .Append(InOperators[i])
                             .Append(" ")
                             .Append(InExpectedValues[i])
                             .Append(" AND ");
            }
            stringBuilder.Remove(stringBuilder.Length - 5, 5);

            return SelectDictT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the database by property names and expected value and return the dictionary.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InPropertyNames">property names.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public Dictionary<int, T> SelectDictT<T>(string[] InPropertyNames, string[] InOperators, int[] InExpectedValues) where T : Base, new()
        {
            if (null == InPropertyNames || null == InOperators || null == InExpectedValues) throw new ArgumentNullException();
            int length = InPropertyNames.Length;
            if (length != InOperators.Length || length != InExpectedValues.Length) throw new ArgumentException("Parameter length does not match.");

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(property.ClassName)
                         .Append(" WHERE ");

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InPropertyNames[i])
                             .Append(" ")
                             .Append(InOperators[i])
                             .Append(" ")
                             .Append(InExpectedValues[i])
                             .Append(" AND ");
            }
            stringBuilder.Remove(stringBuilder.Length - 5, 5);

            return SelectDictT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the dictionary from the database by sql statement.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InSQLStatement">In SQL Statement</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public Dictionary<int, T> SelectDictT<T>(string InSQLStatement = "") where T : Base, new()
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);
            Dictionary<int, T> resultDict = null;

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(InSQLStatement);

            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());
            int count = SQLite3.ColumnCount(stmt);
            int length = property.Infos.Length;
            int id;
            resultDict = new Dictionary<int, T>();
            SQLite3Result result;
            while (true)
            {
                result = SQLite3.Step(stmt);
                if (SQLite3Result.Row == result)
                {
                    T t = GetT(new T(), property.Infos, stmt, count);
                    id = (int)property.Infos[0].GetValue(t, null);
                    if (!resultDict.ContainsKey(id)) resultDict.Add(id, t);
                }
                else if (SQLite3Result.Done == result)
                {
                    break;
                }
                else
                {
                    throw new Exception(SQLite3.GetErrmsg(stmt));
                }
            }
            SQLite3.Finalize(stmt);

            return resultDict;
        }

        /// <summary>
        /// Query the array by property indexes and expected value and return the dictionary.
        /// </summary>
        /// <returns>Returns the result of the query as a array.</returns>
        /// <param name="InIndexes">property indexes, The index value is specified by the SyncAttribute.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T[] SelectArrayT<T>(int[] InIndexes, string[] InOperators, int[] InExpectedValues) where T : Base, new()
        {
            if (null == InIndexes || null == InOperators || null == InExpectedValues) throw new ArgumentNullException();
            int length = InIndexes.Length;
            if (length != InOperators.Length || length != InExpectedValues.Length) throw new ArgumentException("Parameter length does not match.");

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(property.ClassName)
                         .Append(" WHERE ");

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(property.Infos[InIndexes[i]].Name)
                             .Append(" ")
                             .Append(InOperators[i])
                             .Append(" ")
                             .Append(InExpectedValues[i])
                             .Append(" AND ");
            }
            stringBuilder.Remove(stringBuilder.Length - 5, 5);

            return SelectArrayT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the database by property names and expected value and return the array.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InPropertyNames">property names.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T[] SelectArrayT<T>(string[] InPropertyNames, string[] InOperators, int[] InExpectedValues) where T : Base, new()
        {
            if (null == InPropertyNames || null == InOperators || null == InExpectedValues) throw new ArgumentNullException();
            int length = InPropertyNames.Length;
            if (length != InOperators.Length || length != InExpectedValues.Length) throw new ArgumentException("Parameter length does not match.");

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(property.ClassName)
                         .Append(" WHERE ");

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InPropertyNames[i])
                             .Append(" ")
                             .Append(InOperators[i])
                             .Append(" ")
                             .Append(InExpectedValues[i])
                             .Append(" AND ");
            }
            stringBuilder.Remove(stringBuilder.Length - 5, 5);

            return SelectArrayT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the database by sql statement and return the array.
        /// </summary>
        /// <returns>Returns the result of the query as a array.</returns>
        /// <param name="InSQLStatement">In SQL Statement.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public T[] SelectArrayT<T>(string InSQLStatement = "") where T : Base, new()
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            SQLite3Statement stmt = ExecuteQuery(InSQLStatement);

            List<T> resultList = new List<T>();
            SQLite3Result sqlite3Result;
            int count = SQLite3.ColumnCount(stmt);
            while (true)
            {
                sqlite3Result = SQLite3.Step(stmt);
                if (SQLite3Result.Row == sqlite3Result)
                {
                    resultList.Add(GetT(new T(), property.Infos, stmt, count));
                }
                else if (SQLite3Result.Done == sqlite3Result) break;
                else throw new Exception(SQLite3.GetErrmsg(stmt));
            }

            SQLite3.Finalize(stmt);

            return resultList.ToArray();
        }

        /// <summary>
        /// Convert query result from database to Base subclass object.
        /// </summary>
        /// <returns>Base subclass object.</returns>
        /// <param name="InBaseSubclassObj">In Base subclass object.</param>
        /// <param name="InPropertyInfos">In Base subclass property infos.</param>
        /// <param name="InStmt">SQLite3 result address.</param>
        /// <param name="InCount">In sqlite3 result count.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        private T GetT<T>(T InBaseSubclassObj, PropertyInfo[] InPropertyInfos, SQLite3Statement InStmt, int InCount) where T : Base, new()
        {
            Type type;
            for (int i = 0; i < InCount; ++i)
            {
                type = InPropertyInfos[i].PropertyType;

                if (typeof(int) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnInt(InStmt, i), null);
                }
                else if (typeof(long) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnInt64(InStmt, i), null);
                }
                else if (typeof(float) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, (float)SQLite3.ColumnDouble(InStmt, i), null);
                }
                else if (typeof(double) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnDouble(InStmt, i), null);
                }
                else if (typeof(string) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnText(InStmt, i), null);
                }
            }

            return InBaseSubclassObj;
        }

        /// <summary>
        /// Deletes the data by identifier.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InID">In identifier of data.</param>
        public void DeleteByID(string InTableName, int InID)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InID);

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //rebuild the built-in index.
        }

        /// <summary>
        /// Deletes the data by Base subclass object.
        /// </summary>
        /// <param name="InID">In Subclass object id.</param>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public void DeleteT<T>(T InID) where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ID = ")
                .Append(property.Infos[0].GetValue(InID, null));

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //rebuild the built-in index.
        }

        /// <summary>
        /// Clear table data by Base of subclass.
        /// </summary>
        /// <typeparam name="T">Subclass of Base.</typeparam>
        public void DeleteAllT<T>() where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(property.ClassName);

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //rebuild the built-in index.
        }

        /// <summary>
        /// Executed the SQL statement and return the address of sqlite3.
        /// </summary>
        /// <returns>the address of sqlite3.</returns>
        /// <param name="InSQLStatement">In sql statement.</param>
        private SQLite3Statement ExecuteQuery(string InSQLStatement)
        {
            SQLite3Statement stmt;

            if (SQLite3Result.OK == SQLite3.Prepare2(handle, InSQLStatement, GetUTF8ByteCount(InSQLStatement), out stmt, IntPtr.Zero))
                return stmt;
            throw new Exception(SQLite3.GetErrmsg(stmt));
        }

        /// <summary>
        /// Executed the SQL statement.
        /// </summary>
        /// <returns>The exec.</returns>
        /// <param name="InSQLStatement">In SQL Statement.</param>
        public void Exec(string InSQLStatement)
        {
            SQLite3Statement stmt;

            if (SQLite3Result.OK == SQLite3.Prepare2(handle, InSQLStatement, GetUTF8ByteCount(InSQLStatement), out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Done != SQLite3.Step(stmt)) throw new Exception(SQLite3.GetErrmsg(stmt));
            }
            else throw new Exception(SQLite3.GetErrmsg(stmt));

            SQLite3.Finalize(stmt);
        }

        /// <summary>
        /// Closes the database.
        /// </summary>
        public void CloseDB()
        {
            if (SQLite3DbHandle.Zero != handle)
            {
                if (SQLite3Result.OK != SQLite3.Close(handle))
                {
                    Debug.LogError(SQLite3.GetErrmsg(handle));
                }
                else
                {
                    handle = SQLite3DbHandle.Zero;
                }
            }
        }

        /// <summary>
        /// get utf8 bytes length of string.
        /// </summary>
        /// <returns>The UTF 8 bytes count.</returns>
        /// <param name="InStr">In original string.</param>
        private int GetUTF8ByteCount(string InStr)
        {
            return Encoding.UTF8.GetByteCount(InStr);
        }

        /// <summary>
        /// Converts the string to UTF 8 bytes.
        /// </summary>
        /// <returns>The string to UTF 8 bytes.</returns>
        /// <param name="InStr">In string.</param>
        private byte[] ConvertStringToUTF8Bytes(string InStr)
        {
            int length = Encoding.UTF8.GetByteCount(InStr);
            byte[] bytes = new byte[length + 1];
            Encoding.UTF8.GetBytes(InStr, 0, InStr.Length, bytes, 0);

            return bytes;
        }
    }
}