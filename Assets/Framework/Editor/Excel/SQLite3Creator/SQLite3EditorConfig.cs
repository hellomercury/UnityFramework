using Framework.SQLite3;
using NPOI.SS.UserModel;

namespace Framework.Editor
{
    public enum SQLite3ValueType
    {
        INTEGER,
        REAL,
        TEXT,
        BLOB
    }

    public struct TableData
    {
        public bool IsEnable;
        public string TableName;
        public string[] ColumnName;
        public SQLite3ValueType[] SQLite3Types;
        public SQLite3Constraint[] SQLite3Constraints;
        public string[] CSharpTypes;
        public string[] ColumnDescribes;
        public bool[] IsColumnEnables;
        public bool IsNeedCreateScript;

        public ICell[][] ExcelContents;
    }

    public class SQLite3EditorConfig
    {
    }
}
