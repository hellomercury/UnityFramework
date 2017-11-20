using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        //public SQLite3DbHandle DatabaseHandle { get { return handle; } }
        private SQLite3DbHandle handle;

        private StringBuilder stringBuilder;

        public SQLite3Handle(string InDataBasePath) :
            this(InDataBasePath, SQLite3OpenFlags.ReadWrite)
        {
        }

        public SQLite3Handle(string InDataBasePath, SQLite3OpenFlags InFlags)
        {
            Assert.raiseExceptions = true;
            Assert.IsFalse(string.IsNullOrEmpty(InDataBasePath), "数据库路径不能为空！");

            if (SQLite3Result.OK != SQLite3.Open(ConvertStringToUTF8Bytes(InDataBasePath),
                out handle, InFlags.GetHashCode(), IntPtr.Zero))
            {
                SQLite3.Close(handle);
                handle = IntPtr.Zero;
                Debug.LogError("数据库打开失败！");
            }
            else
            {
                stringBuilder = new StringBuilder(1024);
            }
        }

        public Object[] SelectSingleData(string InTableName, int InValue)
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            Object[] obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InValue);

            SQLite3Statement stmt;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    obj = GetObjects(stmt, SQLite3.ColumnCount(stmt));
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            SQLite3.Finalize(stmt);

            return obj;
        }

        public List<Object[]> SelectMultiData(string InTableName, string InSelectColumnName, string InCommon)
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            List<Object[]> obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT ")
                .Append(InSelectColumnName)
                .Append(" FROM ")
                .Append(InTableName)
                .Append(" WHERE ")
                .Append(InCommon);

            SQLite3Statement stmt;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
            {
                obj = new List<object[]>();
                int count = SQLite3.ColumnCount(stmt);

                while (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    obj.Add(GetObjects(stmt, count));
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            SQLite3.Finalize(stmt);

            return obj;
        }

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

        public T SelectTByID<T>(int InID) where T : Base, new()
        {
            return SelectT<T>("WHERE ID = " + InID);
        }

        public T SelectTByIndex<T>(int InIndex) where T : Base, new()
        {
            return SelectT<T>("WHERE rowid = " + (InIndex + 1));
        }


        public T SelectTByKeyValue<T, U>(U InKey, SQLite3Operator InOperator, object InValue) where T : Base, new()
        {
            Assert.IsNotNull(InValue);

            string key;
            if (InKey is Enum) key = InKey.ToString();
            else if (InKey is string) key = InKey as string;
            else key = SyncFactory.GetSyncProperty(typeof(T)).Infos[InKey.GetHashCode()].Name;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append(" WHERE ")
                .Append(key)
                .Append(GetOperatorString(InOperator))
                .Append("'")
                .Append(InValue is string ? InValue.ToString().Replace("'", "''") : InValue)
                .Append("'");

            return SelectT<T>(stringBuilder.ToString());
        }

        public T SelectT<T>(string InCondition) where T : Base, new()
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            T t = null;

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            SQLite3Statement stmt;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(" ")
                .Append(InCondition);

            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    int count = SQLite3.ColumnCount(stmt);

                    t = GetT(new T(), property.Infos, stmt, property.InfosLength);
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            SQLite3.Finalize(stmt);

            return t;
        }

        public Dictionary<int, T> SelectDictT<T>(string InCommand = "") where T : Base, new()
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);
            Dictionary<int, T> value = null;

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(InCommand);

            SQLite3Statement stmt;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
            {
                int count = SQLite3.ColumnCount(stmt);
                int length = property.Infos.Length;

                Assert.IsTrue(count == length, property.ClassName + " : 数据库列与类属性个数不一致！");
                value = new Dictionary<int, T>();
                SQLite3Result result;
                while (true)
                {
                    result = SQLite3.Step(stmt);
                    if (SQLite3Result.Row == result)
                    {
                        T t = GetT(new T(), property.Infos, stmt, count);

                        value.Add((int)property.Infos[0].GetValue(t, null), t);
                    }
                    else if (SQLite3Result.Done == result)
                    {
                        break;
                    }
                    else
                    {
                        Debug.LogError(SQLite3.GetErrmsg(stmt));
                        break;
                    }
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            SQLite3.Finalize(stmt);

            return value;
        }

        public List<T> SelectListT<T, U>(U InKey, SQLite3Operator InOperator, params int[] InValue) where T : Base, new()
        {
            string key;
            if (InKey is string) key = InKey as string;
            else if (InKey is Enum) key = InKey.ToString();
            else key = SyncFactory.GetSyncProperty(typeof(T)).Infos[InKey.GetHashCode()].Name;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append(" WHERE ")
                .Append(key)
                .Append(GetOperatorString(InOperator));

            if (InValue.Length == 1) stringBuilder.Append(InValue[0]);
            else if (InValue.Length == 2) stringBuilder.Append(InValue[0]).Append(" AND ").Append(InValue[1]);
            else Debug.LogError("参数过多！");

            return SelectListT<T>(stringBuilder.ToString());
        }

        public List<T> SelectListT<T>(string InCondition = "") where T : Base, new()
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            List<T> value = null;
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(" ")
                .Append(InCondition);

            SQLite3Statement stmt;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
            {
                int count = SQLite3.ColumnCount(stmt);
                int length = property.Infos.Length;

                Assert.IsTrue(count == length, property.ClassName + " : 数据库列与类属性个数不一致！");
                value = new List<T>();
                SQLite3Result result;
                while (true)
                {
                    result = SQLite3.Step(stmt);
                    if (SQLite3Result.Row == result)
                    {
                        value.Add(GetT(new T(), property.Infos, stmt, count));
                    }
                    else if (SQLite3Result.Done == result)
                    {
                        break;
                    }
                    else
                    {
                        Debug.LogError(SQLite3.GetErrmsg(stmt));
                        break;
                    }
                }
            }
            else
            {
                stringBuilder.Append("\nError : ").Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            SQLite3.Finalize(stmt);

            return value;
        }

        private T GetT<T>(T InValue, PropertyInfo[] InInfos, SQLite3Statement InStmt, int InCount) where T : Base, new()
        {
            Type type;
            for (int i = 0; i < InCount; ++i)
            {
                type = InInfos[i].PropertyType;

                if (typeof(int) == type)
                {
                    InInfos[i].SetValue(InValue, SQLite3.ColumnInt(InStmt, i), null);
                }
                else if (typeof(long) == type)
                {
                    InInfos[i].SetValue(InValue, SQLite3.ColumnInt64(InStmt, i), null);
                }
                else if (typeof(float) == type)
                {
                    InInfos[i].SetValue(InValue, (float)SQLite3.ColumnDouble(InStmt, i), null);
                }
                else if (typeof(double) == type)
                {
                    InInfos[i].SetValue(InValue, SQLite3.ColumnDouble(InStmt, i), null);
                }
                else if (typeof(string) == type)
                {
                    InInfos[i].SetValue(InValue, SQLite3.ColumnText(InStmt, i), null);
                }
            }

            return InValue;
        }

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

                object[] objs = property.Infos[i].GetCustomAttributes(typeof(ConstraintAttribute), false);
                if (objs.Length == 1 && objs[0] is ConstraintAttribute)
                    stringBuilder.Append((objs[0] as ConstraintAttribute).Constraint);
                stringBuilder.Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Debug.LogError(stringBuilder);
            Exec(stringBuilder.ToString());
        }

        public void Insert(string InTableName, params object[] InValues)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("INSERT INTO ").Append(InTableName).Append(" VALUES(");

            int length = InValues.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append("'")
                    .Append(InValues[i].ToString().Replace("'", "''"))
                    .Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

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

        public void InsertAllT<T>(List<T> InValue) where T : Base
        {
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

        public void Update(string InTableName, string InCondition, params string[] InValues)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ").Append(InTableName).Append(" SET ");

            int length = InValues.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InValues[i]).Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(" WHERE ").Append(InCondition);

            Exec(stringBuilder.ToString());
        }

        public void UpdateT<T>(T InValue) where T : Base
        {
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

        public void UpdateTByKeyValue<T>(int InIndex, T InValue) where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

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

        public void UpdateOrInsert<T>(T InT) where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            SQLite3Statement stmt;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ID = ")
                .Append(property.Infos[0].GetValue(InT, null));

            bool isUpdate = false;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
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

        public void DeleteByID(string InTableName, int InID)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InID);

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //重建内置索引值
        }

        public void DeleteT<T>(T InID) where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ID = ")
                .Append(property.Infos[0].GetValue(InID, null));

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //重建内置索引值 
        }

        public void DeleteAllT<T>() where T : Base
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(property.ClassName);

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //重建内置索引值 
        }

        public void Exec(string InCommand)
        {
            SQLite3Statement stmt;

            if (SQLite3Result.OK == SQLite3.Prepare2(handle, InCommand, Encoding.UTF8.GetByteCount(InCommand), out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Done != SQLite3.Step(stmt))
                {
                    Debug.LogError(InCommand + "\nError : " + SQLite3.GetErrmsg(stmt));
                }
            }
            else
            {
                Debug.LogError(InCommand + "\nError : " + SQLite3.GetErrmsg(stmt));
            }

            SQLite3.Finalize(stmt);
        }

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

        private byte[] ConvertStringToUTF8Bytes(string InContent)
        {
            int length = Encoding.UTF8.GetByteCount(InContent);
            byte[] bytes = new byte[length + 1];
            Encoding.UTF8.GetBytes(InContent, 0, InContent.Length, bytes, 0);

            return bytes;
        }

        private string ConvertStringToUTF8String(string InContent)
        {
            return Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(InContent));
        }

        private string GetOperatorString(SQLite3Operator InOperator)
        {
            switch (InOperator)
            {
                case SQLite3Operator.Equal:
                    return " = ";

                case SQLite3Operator.NotEqual:
                    return " != ";

                case SQLite3Operator.Greater:
                    return " > ";

                case SQLite3Operator.GreaterOrEqual:
                    return " >= ";

                case SQLite3Operator.Less:
                    return " < ";

                case SQLite3Operator.LessOrEqual:
                    return " <= ";

                case SQLite3Operator.Between:
                    return " BETWEEN ";

                default:
                    return string.Empty;
            }
        }

        public static bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }
    }
}