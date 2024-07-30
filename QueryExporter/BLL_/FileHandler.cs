using System;
using System.Data;
using System.IO;

namespace QueryExporter.BLL_
{
    internal class FileHandler
    {
        /// <summary>
        /// Exports the data in the DataTable to a CSV file.
        /// </summary>
        /// <param name="dataTable">The DataTable containing the data to export.</param>
        /// <param name="filePath">The file path to save the CSV file.</param>
        public void ExportDataTableToCsv(DataTable dataTable, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                // Write the header row
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (i > 0) writer.Write(",");
                    writer.Write(dataTable.Columns[i].ColumnName);
                }
                writer.WriteLine();

                // Write the data rows
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (i > 0) writer.Write(",");
                        writer.Write(row[i].ToString());
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
