using System;
using System.Collections.Generic;
using System.IO;
using Framework.SQLite3;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class SQLite3Window : EditorWindow
    {
        private static SQLite3Window window;
        private Vector2 scrollPos;
        float progressValue = 1.0f;

        private string selectPrefsKey;
        private bool isSingleFile, preSelect;
        private TableData[][] tableData, preTableData;
        private int sheetLength, rowLength, columnLength;
        private class PathConfig
        {
            private string dataPath;
            private string excelPath, excelPrefsKey;
            public string ExcelPath
            {
                get
                {
                    return string.IsNullOrEmpty(excelPath) ? EditorPrefs.GetString(excelPrefsKey, "") : excelPath;
                }

                set
                {
                    excelPath = value.Replace(dataPath, string.Empty);
                    EditorPrefs.SetString(excelPrefsKey, excelPath);
<<<<<<< HEAD
                }
=======
                    EditorPrefs.SetString(EditorConfig.EditorPrefsKeys, excelPrefsKey + ",");
                }
>>>>>>> fa0bab98ab1569fdd29a2c569277111a99ed536c
            }

            private string scriptPath, scriPrefsKey;
            public string ScriptPath
            {
                get
                {
                    return string.IsNullOrEmpty(scriptPath) ? EditorPrefs.GetString(scriPrefsKey, "") : scriptPath;
                }

                set
                {

                    scriptPath = value.Replace(dataPath, string.Empty);
                    EditorPrefs.SetString(scriPrefsKey, scriptPath);
                }
            }

            private string dbPath, dbPrefsKey;
            public string DbPath
            {
                get
                {
                    return string.IsNullOrEmpty(dbPath) ? EditorPrefs.GetString(dbPrefsKey, "") : dbPath;
                }

                set
                {
                    dbPath = value.Replace(dataPath, string.Empty);
                    EditorPrefs.SetString(dbPrefsKey, dbPath);
                }
            }

            public PathConfig(string InDataPath, string InExcelKey, string InScriptKey, string InDatabaseKey)
            {
                dataPath = InDataPath;

                excelPrefsKey = InExcelKey;
                excelPath = EditorPrefs.GetString(excelPrefsKey, string.Empty);

                scriPrefsKey = InScriptKey;
                scriptPath = EditorPrefs.GetString(scriPrefsKey, string.Empty);

                dbPrefsKey = InDatabaseKey;
                dbPath = EditorPrefs.GetString(dbPrefsKey, string.Empty);
            }
        }

        private string dataPath;
        private PathConfig config, singlePathConfig, multiPathConfig;

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

            singlePathConfig = new PathConfig(dataPath, "SingleExcelPathPrefsKey", "SingleScriptPathPrefsKey", "SingleDbPathPrefsKey");
            multiPathConfig = new PathConfig(dataPath, "MultiExcelPathPrefsKey", "MultiScriptPathPrefsKey", "MultiDbPathPrefsKey");

            selectPrefsKey = "SingleOrMultiSelectPrefsKey";
            isSingleFile = preSelect = EditorPrefs.GetBool(selectPrefsKey, true);
        }

        private void OnDisable()
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
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
                        if (null != data) datas.Add(data);
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
                    data[i].ColumnDescribes = new string[columnLength];
                    data[i].SQLite3Types = new SQLite3ValueType[columnLength];
                    data[i].SQLite3Constraints = new SQLite3Constraint[columnLength];
                    data[i].CSharpTypes = new string[columnLength];
                    data[i].IsColumnEnables = new bool[columnLength];
                    data[i].IsNeedCreateScript = true;
                    data[i].ExcelContents = InExcelData[i].Content;

                    for (int j = 0; j < columnLength; ++j)
                    {
                        data[i].ColumnName[j] = InExcelData[i].Head[0][j].StringCellValue;

                        data[i].CSharpTypes[j] = InExcelData[i].Head[1][j].StringCellValue;
                        switch (data[i].CSharpTypes[j])
                        {
                            case "int":
                            case "bool":
                                data[i].SQLite3Types[j] = SQLite3ValueType.INTEGER;
                                break;
                            case "float":
                            case "double":
                                data[i].SQLite3Types[j] = SQLite3ValueType.REAL;
                                break;
                            case "string":
                                data[i].SQLite3Types[j] = SQLite3ValueType.TEXT;
                                break;
                            default:
                                data[i].SQLite3Types[j] = SQLite3ValueType.BLOB;
                                break;
                        }
                        if (null == InExcelData[i].Head[2] || null == InExcelData[i].Head[2][j])
                            data[i].ColumnDescribes[j] = string.Empty;
                        else
                            data[i].ColumnDescribes[j] = InExcelData[i].Head[2][j].StringCellValue;

                        data[i].SQLite3Constraints[j] = SQLite3Constraint.PrimaryKey | SQLite3Constraint.Unique | SQLite3Constraint.AutoIncrement;

                        data[i].IsColumnEnables[j] = true;
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
                        isSingleFile = EditorGUILayout.ToggleLeft("Single Excel", isSingleFile, GUILayout.Width(245));
                        isSingleFile = !EditorGUILayout.ToggleLeft("Excel Directory", !isSingleFile, GUILayout.Width(245));

                        if (preSelect != isSingleFile)
                        {
                            TableData[][] temp = tableData;
                            tableData = preTableData;
                            preTableData = temp;

                            preSelect = isSingleFile;

                            EditorPrefs.SetBool(selectPrefsKey, isSingleFile);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(15);

                    if (isSingleFile)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            singlePathConfig.ExcelPath = EditorGUILayout.TextField("Excel Path", singlePathConfig.ExcelPath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string singlePath = singlePathConfig.ExcelPath;
                                int index = singlePath.LastIndexOf("/", StringComparison.Ordinal);
                                string path = -1 == index ? singlePath : singlePath.Substring(0, index);
                                path = EditorUtility.OpenFilePanel("Open Excel Path", path, "xlsx,xls");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        singlePath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        singlePath = path;
                                    }

                                    singlePathConfig.ExcelPath = singlePath;
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Preview"))
                        {
                            if (string.IsNullOrEmpty(singlePathConfig.ExcelPath))
                                EditorUtility.DisplayDialog("Tips", "Please select an excel file first.", "OK");
                            else
                                LoadExcel(dataPath + singlePathConfig.ExcelPath.Replace("Assets", string.Empty));
                        }
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        {
                            multiPathConfig.ExcelPath = EditorGUILayout.TextField("Excel Directory", multiPathConfig.ExcelPath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string multiPath = multiPathConfig.ExcelPath;
                                string path = EditorUtility.OpenFolderPanel("Open Excel Directory", multiPath, "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        multiPath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        multiPath = path;
                                    }

                                    multiPathConfig.ExcelPath = multiPath;
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Preview"))
                        {
                            if (string.IsNullOrEmpty(multiPathConfig.ExcelPath))
                                EditorUtility.DisplayDialog("Tips", "Please select a directory where excel is stored.", "OK");
                            else
                                LoadExcelDirectory(dataPath + multiPathConfig.ExcelPath.Replace("Assets", string.Empty));
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                if (null != tableData)
                {
                    sheetLength = tableData.Length;
                    config = isSingleFile ? singlePathConfig : multiPathConfig;
                    EditorGUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            config.DbPath = EditorGUILayout.TextField("Database Save Path", config.DbPath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string path = config.DbPath;
                                int index = path.LastIndexOf("/", StringComparison.Ordinal);
                                path = -1 == index ? path : path.Substring(0, index);
                                path = EditorUtility.SaveFilePanel("Database Save Path", path, "Database", "db");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        config.DbPath = path.Replace(dataPath, "Assets");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        config.DbPath = path;
                                    }
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            config.ScriptPath = EditorGUILayout.TextField("Script Save Directory", config.ScriptPath, GUILayout.Width(410));
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(82)))
                            {
                                string path = EditorUtility.OpenFolderPanel("Script Save Directory", config.ScriptPath, "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.Contains(dataPath))
                                    {
                                        config.ScriptPath = path.Replace(dataPath, "Assets") + "/";
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Can not open the floder out of the project path!");
                                        config.ScriptPath = path + "/";
                                    }
                                }
                            }
                        }
                        GUILayout.EndHorizontal();


                        if (!isSingleFile && sheetLength > 1)
                        {

                            if (GUILayout.Button("Create All"))
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(config.DbPath))
                                    {
                                        EditorUtility.DisplayDialog("Tips", "Please select the storage location of the database.", "OK");
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(config.ScriptPath))
                                        {
                                            EditorUtility.DisplayDialog("Tips", "Please select the storage location of the script.", "OK");
                                        }
                                        else
                                        {
                                            string dbpath = config.DbPath.Remove(0, 7);
                                            //SQLite3Creator.DeleteDatabase(dbpath);
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
                                                            ScriptWriter.Writer(multiPathConfig.ScriptPath + tableData[i][j].TableName + ".cs", ref tableData[i][j]);
                                                        }
                                                        EditorUtility.DisplayProgressBar("Convert excel to sqlite3 table...", "Convert excel named: " + tableData[i][j].TableName, i * .5f / sheetLength);

                                                        SQLite3Creator.Creator(ref tableData[i][j], dbpath);
                                                    }
                                                }
                                            }
                                            EditorUtility.ClearProgressBar();

                                            EditorUtility.DisplayDialog("Tips", "Convert excel to sqlite3 table finished.", "OK");

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    EditorUtility.DisplayDialog("Error", "Convert excel to sqlite3 table has an error:" + ex.Message, "OK");
                                }
                                Close();
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
                                            tableData[i][j].IsColumnEnables[k] = EditorGUILayout.BeginToggleGroup(
                                                "Enable",
                                                tableData[i][j].IsColumnEnables[k]);
                                            {
                                                GUILayout.BeginVertical("box");
                                                {
                                                    GUILayout.BeginHorizontal();
                                                    {
                                                        tableData[i][j].ColumnName[k] =
                                                            EditorGUILayout.TextField(tableData[i][j].ColumnName[k],
                                                                GUILayout.MaxWidth(100));
                                                        tableData[i][j].ColumnDescribes[k] =
                                                            EditorGUILayout.TextField(tableData[i][j].ColumnDescribes[k],
                                                                GUILayout.MaxWidth(300));
                                                        tableData[i][j].SQLite3Types[k] =
                                                            (SQLite3ValueType)
                                                                EditorGUILayout.EnumPopup(tableData[i][j].SQLite3Types[k],
                                                                    GUILayout.MaxWidth(100));

                                                        //tableData[i][j].SQLite3Constraints[k] =
                                                        //    (SQLite3Constraint)
                                                        //        EditorGUILayout.EnumPopup(tableData[i][j].SQLite3Constraints[k],
                                                        //            GUILayout.MaxWidth(100));
                                                    }
                                                    GUILayout.EndHorizontal();
                                                    GUILayout.Space(10);
                                                    GUILayout.BeginHorizontal();
                                                    {
                                                        SQLite3Constraint constraint = tableData[i][j].SQLite3Constraints[k];
                                                        bool isPrimaryKey = (constraint & SQLite3Constraint.PrimaryKey) != 0;
                                                        bool isAutoIncrement = (constraint & SQLite3Constraint.AutoIncrement) != 0;
                                                        bool isNotNull = (constraint & SQLite3Constraint.NotNull) != 0;
                                                        bool isUnique = (constraint & SQLite3Constraint.Unique) != 0;
                                                        bool isDefault = (constraint & SQLite3Constraint.Default) != 0;

                                                        isPrimaryKey = EditorGUILayout.ToggleLeft("PrimaryKey", isPrimaryKey, GUILayout.Width(90));
                                                        isAutoIncrement = EditorGUILayout.ToggleLeft("AutoIncrement", isAutoIncrement, GUILayout.Width(120));
                                                        isNotNull = EditorGUILayout.ToggleLeft("NotNull", isNotNull, GUILayout.Width(90));
                                                        isUnique = EditorGUILayout.ToggleLeft("Unique", isUnique, GUILayout.Width(90));
                                                        if (isPrimaryKey || isAutoIncrement || isNotNull || isUnique) isDefault = false;
                                                        isDefault = EditorGUILayout.ToggleLeft("Default", isDefault, GUILayout.Width(90));
                                                        if (isDefault) isPrimaryKey = isAutoIncrement = isNotNull = isUnique = false;
                                                        
                                                        if (isDefault) constraint |= SQLite3Constraint.Default;
                                                        else constraint = constraint & ~SQLite3Constraint.Default;

                                                        if (isPrimaryKey) constraint |= SQLite3Constraint.PrimaryKey;
                                                        else constraint = constraint & ~SQLite3Constraint.PrimaryKey;

                                                        if (isAutoIncrement) constraint |= SQLite3Constraint.AutoIncrement;
                                                        else constraint = constraint & ~SQLite3Constraint.AutoIncrement;

                                                        if (isNotNull) constraint |= SQLite3Constraint.NotNull;
                                                        else constraint = constraint & ~SQLite3Constraint.NotNull;

                                                        if (isUnique) constraint |= SQLite3Constraint.Unique;
                                                        else constraint = constraint & ~SQLite3Constraint.Unique;

                                                        tableData[i][j].SQLite3Constraints[k] = constraint;
                                                    }
                                                    GUILayout.EndHorizontal();
                                                }
                                                GUILayout.EndVertical();
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
                                                if (string.IsNullOrEmpty(config.DbPath))
                                                {
                                                    EditorUtility.DisplayDialog("Tips", "Please select the storage location of the database.", "OK");
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(config.ScriptPath))
                                                    {
                                                        EditorUtility.DisplayDialog("Tips", "Please select the storage location of the script.", "OK");
                                                    }
                                                    else
                                                    {
                                                        if (tableData[i][j].IsNeedCreateScript) ScriptWriter.Writer(config.ScriptPath + tableData[i][j].TableName + ".cs", ref tableData[i][j]);
                                                        SQLite3Creator.Creator(ref tableData[i][j], config.DbPath.Remove(0, 7));

                                                        EditorUtility.DisplayDialog("Tips", "Convert excel to sqlite3 table finished.", "OK");

                                                        if (isSingleFile) Close();
                                                    }
                                                }
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