﻿using UnityEngine;
using System.Text;
using NPOI.SS.UserModel;
using Framework.SQLite3;
using System.IO;

namespace Framework.Editor
{
    public class SQLite3Creator
    {

        public static void DeleteDatabase(string InDatabasePath)
        {
            string path = Application.dataPath + "/" + InDatabasePath;

            if (File.Exists(path)) File.Delete(path);
        }

        public static void Creator(ref TableData InTableData, string InDatabasePath)
        {
            string path = Application.dataPath + "/" + InDatabasePath;

            SQLite3Handle handle = new SQLite3Handle(path, SQLite3OpenFlags.Create | SQLite3OpenFlags.ReadWrite);

            StringBuilder sb = new StringBuilder(512);

            handle.Exec("DROP TABLE IF EXISTS " + InTableData.TableName);

            sb.Append("CREATE TABLE ")
              .Append(InTableData.TableName)
              .Append("(");

            int length = InTableData.ColumnName.Length;
            for (int i = 0; i < length; i++)
            {
                if (InTableData.IsColumnEnables[i])
                {
                    sb.Append(InTableData.ColumnName[i])
                      .Append(" ")
                      .Append(InTableData.SQLite3Types[i])
                      .Append(GetConnstraint(InTableData.SQLite3Constraints[i]))
                      .Append(", ");
                }
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(")");
            handle.Exec(sb.ToString());

            if (null != InTableData.ExcelContents)
            {
                length = InTableData.ExcelContents.Length;
                int subLength;
                ICell cell;
                for (int i = 0; i < length; i++)
                {
                    subLength = InTableData.ExcelContents[i].Length;
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO ").Append(InTableData.TableName).Append(" VALUES(");
                    for (int j = 0; j < subLength; j++)
                    {
                        if (InTableData.IsColumnEnables[j])
                        {
                            cell = InTableData.ExcelContents[i][j];
                            switch (InTableData.SQLite3Types[j])
                            {
                                case SQLite3ValueType.INTEGER:
                                    if (null == cell)
                                        sb.Append(0);
                                    else
                                    {
                                        switch (cell.CellType)
                                        {
                                            case CellType.Numeric:
                                                sb.Append((int)cell.NumericCellValue);
                                                break;

                                            case CellType.String:
                                                int result;
                                                sb.Append(int.TryParse(cell.StringCellValue, out result)
                                                    ? result
                                                    : 0);
                                                break;

                                            case CellType.Boolean:
                                                sb.Append(cell.BooleanCellValue ? 1 : 0);
                                                break;

                                            default:
                                                sb.Append(0);
                                                break;
                                        }
                                    }
                                    break;

                                case SQLite3ValueType.REAL:
                                    if (null == cell)
                                        sb.Append(0);
                                    else
                                    {
                                        switch (cell.CellType)
                                        {
                                            case CellType.Numeric:
                                                sb.Append(cell.NumericCellValue);
                                                break;

                                            case CellType.String:
                                                double result;
                                                sb.Append(double.TryParse(cell.StringCellValue, out result)
                                                    ? result
                                                    : 0);
                                                break;

                                            case CellType.Boolean:
                                                sb.Append(cell.BooleanCellValue ? 1 : 0);
                                                break;

                                            default:
                                                sb.Append(0);
                                                break;
                                        }
                                    }
                                    break;

                                default:
                                    if (null == cell)
                                        sb.Append("''");
                                    else
                                    {
                                        switch (cell.CellType)
                                        {
                                            case CellType.Numeric:
                                                sb.Append("\'")
                                                    .Append(cell.NumericCellValue)
                                                    .Append("\'");
                                                break;

                                            case CellType.String:
                                                sb.Append("\'")
                                                    .Append(cell.StringCellValue.Replace("'", "''"))
                                                    .Append("\'");
                                                break;

                                            case CellType.Boolean:
                                                sb.Append("\'")
                                                    .Append(cell.BooleanCellValue.ToString())
                                                    .Append("\'");
                                                break;

                                            default:
                                                sb.Append("''");
                                                break;
                                        }
                                    }
                                    break;
                            }
                            sb.Append(", ");
                        }
                    }
                    sb.Remove(sb.Length - 2, 2);
                    sb.Append(")");
                    handle.Exec(sb.ToString());
                }
            }

            handle.CloseDB();
        }

        private static string GetConnstraint(SQLite3Constraint InConstraint)
        {
            if (InConstraint == SQLite3Constraint.Default) return string.Empty;
            else
            {
                string constraint = string.Empty;
                if ((InConstraint & SQLite3Constraint.PrimaryKey) != 0) constraint += " PRIMARY KEY ";

                if ((InConstraint & SQLite3Constraint.AutoIncrement) != 0) constraint += " AUTOINCREMENT ";
                if ((InConstraint & SQLite3Constraint.NotNull) != 0) constraint += " NOT NULL ";
                if ((InConstraint & SQLite3Constraint.Unique) != 0) constraint += " UNIQUE ";

                return constraint;
            }
        }
    }
}