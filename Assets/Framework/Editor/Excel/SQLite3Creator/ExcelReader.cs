using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class ExcelReader
    {
        public struct ExcelData
        {
            public string SheetName;
            public int HeadLength, ContentLength, ColumnLength;
            public ICell[][] Head;
            public ICell[][] Content;
        }

        public static ExcelData[] GetSingleExcelData(string InExcelPath)
        {
            List<ExcelData> datas;
            FileInfo info = new FileInfo(InExcelPath);
            if (info.Exists && info.Name[0] != '~'
                && (info.Extension.Equals(".xlsx") || info.Extension.Equals(".xls")))
            {
                using (FileStream stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IWorkbook book;

                    if (info.Extension.Equals(".xlsx")) book = new XSSFWorkbook(stream);
                    else book = new HSSFWorkbook(stream);

                    int sheetCount = book.NumberOfSheets;
                    datas = new List<ExcelData>(sheetCount);

                    for (int i = 0; i < sheetCount; i++)
                    {
                        ISheet sheet = book.GetSheetAt(i);

                        int rowCount = sheet.LastRowNum + 1;

                        if (rowCount > 2)
                        {
                            ExcelData data = new ExcelData();
                            data.SheetName = sheet.SheetName.Equals("Sheet1") || sheet.SheetName.Equals("工作表1")
                                ? info.Name.Replace(info.Extension, string.Empty) : sheet.SheetName;

                            data.HeadLength = 3;
                            IRow row1 = sheet.GetRow(0);
                            IRow row2 = sheet.GetRow(1);
                            IRow row3 = sheet.GetRow(2);
                            int colCount = row1.LastCellNum;
                            if (colCount == row2.LastCellNum)
                            {
                                data.ColumnLength = colCount;
                                data.Head = new ICell[3][];
                                data.Head[0] = new ICell[colCount];
                                data.Head[1] = new ICell[colCount];
                                data.Head[2] = row3 == null ? null : new ICell[colCount];

                                if (!row1.GetCell(0).StringCellValue.Equals("ID"))
                                {
                                    Debug.LogWarning(data.SheetName + " Warning : The first column name must be 'ID' and the data has been automatically modified by the script!\nPlease fixed excel file.");
                                    row1.GetCell(0).SetCellValue("ID");
                                }

                                for (int j = 0; j < colCount; ++j)
                                {
                                    data.Head[0][j] = row1.GetCell(j);

                                    data.Head[1][j] = row2.GetCell(j);

                                    if (null != row3) data.Head[2][j] = row3.GetCell(j);
                                }

                                if(rowCount > 3)
                                {
                                    int length = rowCount - 3;
                                    List<ICell[]> content = new List<ICell[]>(length);
                                    IRow row;
                                    for (int j = 0, m = 3; j < length; ++j, ++m)
                                    {
                                        row = sheet.GetRow(m);
                                        if (null != row)
                                        {
                                            ICell[] cells = new ICell[colCount];
                                            for (int k = 0; k < colCount; ++k)
                                            {
                                                cells[k] = row.GetCell(k);
                                            }
                                            content.Add(cells);
                                        }
                                    }

                                    data.Content = content.ToArray();
                                    data.ContentLength = content.Count;
                                }

                                datas.Add(data);
                            }
                            else EditorUtility.DisplayDialog("Error", info.Name + " property name and type number does not match.", "Ok");
                        }
                        else EditorUtility.DisplayDialog("Error", info.Name + " missing basic configuration information, property name and type.", "Ok");
                    }

                    book.Close();
                    stream.Close();

                    return datas.ToArray();
                }
            }
            return null;
        }
    }
}