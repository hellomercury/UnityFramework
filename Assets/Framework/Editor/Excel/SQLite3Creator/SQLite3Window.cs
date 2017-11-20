using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class SQLite3Window : EditorWindow
    {
        private static SQLite3Window window;
        private Vector2 scrollPos;
        float progressValue = 1.0f;

        private bool isFile, preSelect;
        private TableData[][] tableData, preTableData;
        private int sheetLength, rowLength, columnLength;
        private string dataPath;
        private string excelPath, preExcelPath, excelPathKey;

        private string databasePath, databasePathKey;
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

            databasePathKey = "EditorDbPathKey";
            databasePath = EditorPrefs.GetString(databasePathKey, dataPath);
            

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
                        Debug.LogError(fileInfos[i].Name + "\nError : " + e.Message);
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
                    data[i].SQLite3Type = new ValueType[columnLength];
                    data[i].CSharpType = new string[columnLength];
                    data[i].IsColumnEnable = new bool[columnLength];
                    data[i].IsNeedCreateScript = true;
                    data[i].ExcelContent = InExcelData[i].Content;

                    for (int j = 0; j < columnLength; ++j)
                    {
                        data[i].ColumnName[j] = InExcelData[i].Head[0][j].StringCellValue;

                        data[i].CSharpType[j] = InExcelData[i].Head[1][j].StringCellValue;
                        switch (data[i].CSharpType[j])
                        {
                            case "int":
                            case "bool":
                                data[i].SQLite3Type[j] = ValueType.INTEGER;
                                break;
                            case "float":
                            case "double":
                                data[i].SQLite3Type[j] = ValueType.REAL;
                                break;
                            case "string":
                                data[i].SQLite3Type[j] = ValueType.TEXT;
                                break;
                            default:
                                data[i].SQLite3Type[j] = ValueType.BLOB;
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
                            databasePath = EditorGUILayout.TextField("Database Save Path", databasePath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string path = databasePath.Substring(0, databasePath.LastIndexOf("/", StringComparison.Ordinal));
                                path = EditorUtility.SaveFilePanel("Database Save Path", path, "database", "db");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        databasePath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        databasePath = path;
                                    }

                                    EditorPrefs.SetString(databasePathKey, databasePath);
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
                                    scriptPath += "/";
                                    EditorPrefs.SetString(scriptPathKey, scriptPath);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();


                        if (!isFile && sheetLength > 1)
                        {

                            if (GUILayout.Button("Create All"))
                            {
                                try
                                {
                                    string dbpath = databasePath.Replace("Assets/", string.Empty);
                                    SQLite3Creator.ClearAllTable(dbpath);
                                    for (int i = 0; i < sheetLength; i++)
                                    {
                                        rowLength = tableData[i].Length;
                                        for (int j = 0; j < rowLength; j++)
                                        {
                                            if (tableData[i][j].IsEnable)
                                            {
                                                progressValue = 1.0f;
                                                if (tableData[i][j].IsNeedCreateScript) 
                                                {
                                                    progressValue = .5f;
                                                    EditorUtility.DisplayProgressBar("Convert excel to cshap script...", "Convert excel named: " + tableData[i][j].TableName, i * progressValue / sheetLength);
                                                    ScriptWriter.Writer(scriptPath + tableData[i][j].TableName + ".cs", ref tableData[i][j]); 
                                                }
                                                EditorUtility.DisplayProgressBar("Convert excel to sqlite3 table...", "Convert excel named: " + tableData[i][j].TableName, i * .5f / sheetLength);

                                                SQLite3Creator.Creator(ref tableData[i][j], dbpath);
                                            }
                                        }
                                    }

                                    EditorUtility.DisplayProgressBar("Refresh assetdatabase...", "Waiting convert finish...", 1);

                                    AssetDatabase.Refresh();
                                    EditorUtility.ClearProgressBar();

                                    EditorUtility.DisplayDialog("Tips", "Convert excel to sqlite3 table finished.", "OK");
                                }
                                catch (Exception ex)
                                {
                                    EditorUtility.DisplayDialog("Error", "Convert excel to sqlite3 table has an error:" + ex.Message, "OK");
                                }
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
                                                    tableData[i][j].SQLite3Type[k] =
                                                        (ValueType)
                                                            EditorGUILayout.EnumPopup(tableData[i][j].SQLite3Type[k],
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
                                            try
                                            {
                                                if (tableData[i][j].IsNeedCreateScript) ScriptWriter.Writer(scriptPath + tableData[i][j].TableName + ".cs", ref tableData[i][j]);
                                                SQLite3Creator.Creator(ref tableData[i][j], databasePath.Replace("Assets/", string.Empty));
                                            
                                                EditorUtility.DisplayDialog("Tips", "Convert excel to sqlite3 table finished.", "OK");
                                            }
                                            catch (Exception ex)
                                            {
                                                EditorUtility.DisplayDialog("Error", "Convert excel to sqlite3 table has an error:" + ex.Message, "OK");
                                            }
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