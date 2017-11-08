using NPOI.SS.UserModel;

namespace Framework.Editor
{
    public enum ValueType
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
        public ValueType[] SQLite3Type;
        public string[] CSharpType;
        public string[] ColumnDescribe;
        public bool[] IsColumnEnable;
        public bool IsNeedCreateScript;

        public ICell[][] ExcelContent;
    }

    public class SQLite3EditorConfig
    {
    }
}
