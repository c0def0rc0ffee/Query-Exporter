using System;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using OfficeOpenXml;

namespace QueryExporter.BLL_
{
    internal class FileHandler
    {
        /// <summary>
        /// Exports the data in the DataTable to a CSV file.
        /// </summary>
        /// <param name="dataTable">The DataTable containing the data to export.</param>
        /// <param name="filePath">The file path or directory to save the CSV file.</param>
        public void ExportDataTableToCsv(DataTable dataTable, string filePath)
        {
            filePath = EnsureFilePath(filePath, ".csv");

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

        /// <summary>
        /// Exports the data in the DataTable to an Excel (.xlsx) file.
        /// </summary>
        /// <param name="dataTable">The DataTable containing the data to export.</param>
        /// <param name="filePath">The file path or directory to save the Excel file.</param>
        public void ExportDataTableToExcel(DataTable dataTable, string filePath)
        {
            filePath = EnsureFilePath(filePath, ".xlsx");

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Write the header row
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
                }

                // Write the data rows
                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                    {
                        worksheet.Cells[rowIndex + 2, colIndex + 1].Value = dataTable.Rows[rowIndex][colIndex].ToString();
                    }
                }

                FileInfo fi = new FileInfo(filePath);
                package.SaveAs(fi);
            }
        }

        /// <summary>
        /// Exports the data in the DataTable to a JSON file.
        /// </summary>
        /// <param name="dataTable">The DataTable containing the data to export.</param>
        /// <param name="filePath">The file path or directory to save the JSON file.</param>
        public void ExportDataTableToJson(DataTable dataTable, string filePath)
        {
            filePath = EnsureFilePath(filePath, ".json");

            var rows = new List<Dictionary<string, object>>();

            // Convert each row in the DataTable to a dictionary for JSON serialization
            foreach (DataRow row in dataTable.Rows)
            {
                var rowData = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    rowData[column.ColumnName] = row[column];
                }
                rows.Add(rowData);
            }

            var json = JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Ensures the file path includes the specified file extension and appends a default filename if only a directory is provided.
        /// </summary>
        /// <param name="filePath">The input file path or directory.</param>
        /// <param name="extension">The required file extension.</param>
        /// <returns>A complete file path with the correct extension.</returns>
        private string EnsureFilePath(string filePath, string extension)
        {
            if (Directory.Exists(filePath))
            {
                if (!filePath.EndsWith("\\"))
                {
                    filePath += "\\";
                }
                filePath += "export" + extension; // Use default name if only directory is given
            }
            else if (!filePath.EndsWith(extension))
            {
                filePath += extension; // Add extension if missing
            }

            return filePath;
        }
    }
}
