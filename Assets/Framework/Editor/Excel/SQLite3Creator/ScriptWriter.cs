using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;
using UnityEngine.Assertions;

namespace Framework.Editor
{
    public class ScriptWriter
    {
        public static void Writer(string InPath, ICell[][] InData)
        {
            Assert.IsFalse(string.IsNullOrEmpty(InPath));
            Assert.IsNotNull(InData);

            FileInfo info = new FileInfo(InPath);
            List<string> oldContent;
            
            if (info.Exists)
            {
                oldContent = new List<string>(100);
                using (FileStream stream = info.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        bool isNeedRead = false;
                        while ( null != (line = reader.ReadLine()))
                        {
                            if (!isNeedRead && line.Contains("*Self Code Begin*")) isNeedRead = true;
                            if (isNeedRead) oldContent.Add(line);
                            if (isNeedRead && line.Contains("*Self Code End*")) isNeedRead = false;
                        }

                        reader.Close();
                    }
                    stream.Close();
                }
            }
            else
            {
                oldContent = new List<string>(4);
                oldContent.Add("    //-------------------------------*Self Code Begin*-------------------------------");
                oldContent.Add("    //自定义代码.");
                oldContent.Add("    //-------------------------------*Self Code End*   -------------------------------");
            }
            
            string filename = info.Name.Replace(info.Extension, string.Empty);
            StringBuilder sb = new StringBuilder(1024);
            int length = InData[0].Length;
            sb.Append("/*\n")
                .Append(" * 数据库数据表结构类\n")
                .Append(" * --->次类为代码自动生成<---\n")
                .Append(" * --->如需进行修改，请将修改内容放置在\"//自定义代码.\"，支持多行。\n")
                .Append(" *                                                                                                                 --szn\n")
                .Append(" */\n\n")
                .Append("using Framework.DataStruct;\n\n");
            //            .Append("namespace SQLite3.Data\n")
            //            .Append("{\n");

            sb.Append("    public enum ").Append(filename).Append("Enum\n")
                .Append("    {\n");
            for (int i = 0; i < length; i++)
            {
                sb.Append("        ").Append(InData[0][i].StringCellValue).Append(",\n");
            }
            sb.Append("        Max\n");
            sb.Append("    }\n\n");

            sb.Append("    public class ").Append(filename).Append(" : Base").Append("\n")
                .Append("    {\n");

            sb.Append("        private readonly int hashCode;\n\n");
            for (int i = 0; i < length; i++)
            {
                sb.Append("        [Sync((int)").Append(filename).Append("Enum.").Append(InData[0][i].StringCellValue).Append(")]\n")
                    .Append("        public ")
                    .Append(InData[1][i].StringCellValue)
                    .Append(" ")
                    .Append(InData[0][i].StringCellValue)
                    .Append(0 == i ? " { get; private set; }" : " { get; set; }");

                if (null == InData[2] || i >= InData[2].Length || null == InData[2][i])
                    sb.Append("\n\n");
                else
                    sb.Append("  //").Append(InData[2][i].StringCellValue).Append("\n\n");
            }

            sb.Append("        public ").Append(filename).Append("()\n")
                .Append("        {\n")
                .Append("        }\n\n");



            sb.Append("        public ").Append(filename).Append("(");
            for (int i = 0; i < length; ++i)
            {
                sb.Append(InData[1][i].StringCellValue)
                    .Append(" In").Append(InData[0][i].StringCellValue)
                    .Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(")\n");
            sb.Append("        {\n")
                .Append("            hashCode = InID;\n");

            for (int i = 0; i < length; ++i)
            {
                sb.Append("            ").Append(InData[0][i].StringCellValue)
                    .Append(" = In").Append(InData[0][i].StringCellValue)
                    .Append(";\n");
            }
            sb.Append("        }\n\n");

            int count = oldContent.Count;
            for (int i = 0; i < count; ++i)
            {
                sb.Append(oldContent[i]).Append("\n");
            }
            sb.Append("        \n\n");

            sb.Append("        public override int GetHashCode()\n")
                .Append("        {\n")
                .Append("            return hashCode;\n")
                .Append("        }\n\n");

            sb.Append("        public override string ToString()\n")
                .Append("        {\n")
                .Append("            return \"").Append(filename).Append(" : ID = \" + ID");

            for (int i = 1; i < length; ++i)
            {
                sb.Append("+ \", ").Append(InData[0][i].StringCellValue).Append(" = \" + ").
                Append(InData[0][i].StringCellValue);
            }
            sb.Append(";\n");
            sb.Append("        }\n\n");

            sb.Append("        public override bool Equals(object InObj)\n")
                .Append("        {\n")
                .Append("            if (null == InObj) return false;\n")
                .Append("            else return InObj is ").Append(filename).Append(" && (InObj as ")
                .Append(filename).Append(").ID == ID;\n")
                .Append("        }\n");

            sb.Append("    }\n");


            if(!info.Directory.Exists) info.Directory.Create();
            if (info.Exists) info.Delete();

            File.WriteAllText(InPath, sb.ToString(), Encoding.UTF8);
        }
    }
}

