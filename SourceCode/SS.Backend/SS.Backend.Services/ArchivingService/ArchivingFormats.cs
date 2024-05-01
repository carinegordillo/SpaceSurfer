
using SS.Backend.SharedNamespace;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace SS.Backend.Services.ArchivingService
{
    public class ArchivingFormats
    {    
        public static string SaveToTextFile(DataTable dataTable, string directoryPath = "")
        {
            StringBuilder data = new StringBuilder();
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                data.Append(dataTable.Columns[i].ColumnName);
                data.Append(i == dataTable.Columns.Count - 1 ? "\n" : ",");
            }
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    data.Append(row[i].ToString());
                    data.Append(i == dataTable.Columns.Count - 1 ? "\n" : ",");
                }
            }

            string filePath = Path.Combine(directoryPath, $"Data_{DateTime.Now:yyyyMMddHHmmss}.txt");

            File.WriteAllText(filePath, data.ToString());

            return filePath;
        }
        

    }
}
