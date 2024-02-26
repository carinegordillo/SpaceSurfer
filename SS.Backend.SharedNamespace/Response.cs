using System.Data;

namespace SS.Backend.SharedNamespace
{
    public class Response
    {
        public bool HasError { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public int RowsAffected { get; set; } = 0;
        public DataTable? ValuesRead { get; set; }
        public void PrintDataTable()
        {
            if (ValuesRead != null && ValuesRead.Rows.Count > 0)
            {
                Console.WriteLine("DataTable Contents:");

                // Print column names
                foreach (DataColumn column in ValuesRead.Columns)
                {
                    Console.Write(column.ColumnName + "\t");
                }
                Console.WriteLine();

                // Print data rows
                foreach (DataRow row in ValuesRead.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write(item + "\t");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("DataTable is either null or has no rows.");
            }
        }
    }

}
