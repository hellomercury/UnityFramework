using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class SQLite3Window : EditorWindow
    {
        private enum ValueType
        {
            INTEGER,
            REAL,
            TEXT,
            BLOB
        }

        private struct TableData
        {
            public bool IsEnable;
            public string TableName;
            public string[] ColumnName;
            public ValueType[] ColumnType;
            public string[] ColumnDescribe;
            public bool[] IsColumnEnable;
            public bool IsNeedCreateScript;

            public ICell[][] ExcelContent;
        }

        private static SQLite3Window window;
        private Vector2 scrollPos;

        private bool isFile, preSelect;
        private TableData[][] tableData, preTableData;
        private int sheetLength, rowLength, columnLength;
        private string dataPath;
        private string excelPath, preExcelPath, excelPathKey;

        private string dbPath, dbPathKey;
        private string scriptPath, scriptPathKey;

        [MenuItem("Framework/SQLite3 Window %&z")]
        static void Init()
        {
            window = CreateInstance<SQLite3Window>();
            window.LoadExcel(Application.dataPath + "/Design/StaticExcels/Item.xlsx");
            window.titleContent = new GUIContent("SQLite3", "Create SQLite3 table from excel.");
            window.minSize = new Vector2(520, 600);
            window.maxSize = new Vector2(520, 2000);
            window.Show();
        }

        private void OnEnable()
        {
            dataPath = Application.dataPath.Replace("\\", "/");
            
            excelPathKey = "EditorExcelPathKey";
            excelPath = preExcelPath = EditorPrefs.GetString(excelPathKey, "");

            isFile = preSelect = string.IsNullOrEmpty(excelPath) || excelPath.Contains(".");

            dbPathKey = "EditorDbPathKey";
            dbPath = EditorPrefs.GetString(dbPathKey, dataPath);
            

            scriptPathKey = "EditorScriptPathKey";
            scriptPath = EditorPrefs.GetString(scriptPathKey, dataPath);
        }

        public void LoadExcel(string InExcelPath)
        {
            ExcelReader.ExcelData[] excelData = ExcelReader.GetSingleExcelData(InExcelPath);
            TableData[] data = ConvertExcelToTableData(ref excelData);
            if (null != data) tableData = new[] { data };
        }

        public void LoadExcelDirectory(string InExcelDirectory)
        {
            DirectoryInfo dirInfos = new DirectoryInfo(InExcelDirectory);
            if (dirInfos.Exists)
            {
                FileInfo[] fileInfos = dirInfos.GetFiles();
                int length = fileInfos.Length;
                List<TableData[]> datas = new List<TableData[]>(length / 2);
                for (int i = 0; i < length; ++i)
                {
                    try
                    {
                        ExcelReader.ExcelData[] excelData = ExcelReader.GetSingleExcelData(fileInfos[i].FullName);
                        TableData[] data = ConvertExcelToTableData(ref excelData);
                        if (null != data)
                        {
                            datas.Add(data);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(fileInfos[i].Name + "\nError : " + e.Message);
                    }
                }

                tableData = datas.ToArray();
            }
        }

        private TableData[] ConvertExcelToTableData(ref ExcelReader.ExcelData[] InExcelData)
        {
            TableData[] data = null;
            if (null != InExcelData)
            {
                sheetLength = InExcelData.Length;
                data = new TableData[sheetLength];
                for (int i = 0; i < sheetLength; ++i)
                {
                    data[i].IsEnable = true;
                    data[i].TableName = InExcelData[i].SheetName;

                    columnLength = InExcelData[i].ColumnLength;
                    data[i].ColumnName = new string[columnLength];
                    data[i].ColumnDescribe = new string[columnLength];
                    data[i].ColumnType = new ValueType[columnLength];
                    data[i].IsColumnEnable = new bool[columnLength];
                    data[i].IsNeedCreateScript = true;
                    data[i].ExcelContent = InExcelData[i].Content;

                    for (int j = 0; j < columnLength; ++j)
                    {
                        data[i].ColumnName[j] = InExcelData[i].Head[0][j].StringCellValue;
                        string type = InExcelData[i].Head[1][j].StringCellValue;
                        switch (type)
                        {
                            case "int":
                            case "bool":
                                data[i].ColumnType[j] = ValueType.INTEGER;
                                break;
                            case "float":
                            case "double":
                                data[i].ColumnType[j] = ValueType.REAL;
                                break;
                            case "string":
                                data[i].ColumnType[j] = ValueType.TEXT;
                                break;
                            default:
                                data[i].ColumnType[j] = ValueType.BLOB;
                                break;
                        }
                        if (null == InExcelData[i].Head[2] || null == InExcelData[i].Head[2][j])
                            data[i].ColumnDescribe[j] = string.Empty;
                        else
                            data[i].ColumnDescribe[j] = InExcelData[i].Head[2][j].StringCellValue;

                        data[i].IsColumnEnable[j] = true;
                    }
                }
            }

            return data;
        }

        void OnGUI()
        {
            GUILayout.Label("Excel To SQLite3 Table", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        isFile = EditorGUILayout.ToggleLeft("Single Excel", isFile, GUILayout.Width(245));
                        isFile = !EditorGUILayout.ToggleLeft("Excel Directory", !isFile, GUILayout.Width(245));

                        if (preSelect != isFile)
                        {
                            TableData[][] temp = tableData;
                            tableData = preTableData;
                            preTableData = temp;
                            
                            string tempPath = excelPath;
                            excelPath = preExcelPath;
                            preExcelPath = tempPath;

                            preSelect = isFile;

                            if (!isFile && excelPath.Contains("."))
                                excelPath = excelPath.Substring(0, excelPath.LastIndexOf("/", StringComparison.Ordinal) + 1);

                            EditorPrefs.SetString(excelPathKey, excelPath);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(15);

                    if (isFile)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            excelPath = EditorGUILayout.TextField("Excel Path", excelPath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string path = excelPath.Substring(0, excelPath.LastIndexOf("/", StringComparison.Ordinal));
                                path = EditorUtility.OpenFilePanel("Open Excel Path", path, "xlsx,xls");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    excelPath = path.Contains(dataPath) ? path.Replace(dataPath, string.Empty) : path;
                                    if (path.Contains(dataPath))
                                    {
                                        excelPath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        excelPath = path;
                                    }

                                    EditorPrefs.SetString(excelPathKey, excelPath);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Preview"))
                        {
                            LoadExcel(dataPath + excelPath.Replace("Assets", string.Empty));
                        }
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        {
                            excelPath = EditorGUILayout.TextField("Excel Directory", excelPath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string path = EditorUtility.OpenFolderPanel("Open Excel Directory", excelPath, "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        excelPath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        excelPath = path;
                                    }

                                    EditorPrefs.SetString(excelPathKey, excelPath);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Preview"))
                        {
                            LoadExcelDirectory(dataPath + excelPath.Replace("Assets", string.Empty));
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                if (null != tableData)
                {
                    sheetLength = tableData.Length;
                    EditorGUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            dbPath = EditorGUILayout.TextField("Database Save Path", dbPath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string path = dbPath.Substring(0, dbPath.LastIndexOf("/", StringComparison.Ordinal));
                                path = EditorUtility.SaveFilePanel("Database Save Path", path, "database", "db");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        dbPath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        dbPath = path;
                                    }

                                    EditorPrefs.SetString(dbPathKey, dbPath);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            scriptPath = EditorGUILayout.TextField("Script Save Directory", scriptPath,
                                GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string path = EditorUtility.OpenFolderPanel("Script Save Directory", scriptPath, "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        scriptPath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        scriptPath = path;
                                    }

                                    EditorPrefs.SetString(scriptPathKey, scriptPath);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();


                        if (!isFile && sheetLength > 1)
                        {

                            if (GUILayout.Button("Create All"))
                            {

                            }

                        }
                    }
                    EditorGUILayout.EndVertical();

                    scrollPos = GUILayout.BeginScrollView(scrollPos);
                    for (int i = 0; i < sheetLength; ++i)
                    {
                        if (null != tableData[i])
                        {
                            rowLength = tableData[i].Length;
                            for (int j = 0; j < rowLength; ++j)
                            {
                                EditorGUILayout.BeginVertical("box");
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        tableData[i][j].TableName = EditorGUILayout.TextField("Table Name",
                                            tableData[i][j].TableName, GUILayout.Width(410));

                                        tableData[i][j].IsEnable = EditorGUILayout.ToggleLeft("Enable",
                                            tableData[i][j].IsEnable, GUILayout.Width(80));
                                    }
                                    GUILayout.EndHorizontal();

                                    if (tableData[i][j].IsEnable)
                                    {
                                        GUILayout.Space(10);

                                        columnLength = tableData[i][j].ColumnName.Length;
                                        for (int k = 0; k < columnLength; ++k)
                                        {
                                            tableData[i][j].IsColumnEnable[k] = EditorGUILayout.BeginToggleGroup(
                                                "Enable",
                                                tableData[i][j].IsColumnEnable[k]);
                                            {
                                                GUILayout.BeginHorizontal();
                                                {
                                                    tableData[i][j].ColumnName[k] =
                                                        EditorGUILayout.TextField(tableData[i][j].ColumnName[k],
                                                            GUILayout.MaxWidth(160));
                                                    tableData[i][j].ColumnDescribe[k] =
                                                        EditorGUILayout.TextField(tableData[i][j].ColumnDescribe[k],
                                                            GUILayout.MaxWidth(240));
                                                    tableData[i][j].ColumnType[k] =
                                                        (ValueType)
                                                            EditorGUILayout.EnumPopup(tableData[i][j].ColumnType[k],
                                                                GUILayout.MaxWidth(100));
                                                }
                                                GUILayout.EndHorizontal();
                                            }
                                            EditorGUILayout.EndToggleGroup();
                                        }

                                        tableData[i][j].IsNeedCreateScript =
                                            EditorGUILayout.ToggleLeft("Need create or update script?",
                                                tableData[i][j].IsNeedCreateScript, GUILayout.Width(390));
                                        GUILayout.Space(10);
                                        if (GUILayout.Button("Create", GUILayout.Width(490)))
                                        {

                                        }
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}