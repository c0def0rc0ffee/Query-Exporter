# 📊 Query Exporter

**Query Exporter** is a WPF application designed for querying SQL databases and exporting results to `.csv`, `.xlsx`, or `.json` formats. Built to simplify database interactions, making it easy to connect, execute, and export data in just a few clicks.

---

## ✨ Features

- **Database Connection**: Connect to a SQL database with a configurable connection string.
- **SQL Query Execution**: Run SQL queries and display results in a data grid view.
- **Flexible Export Options**: Export query results to `.csv`, `.xlsx`, or `.json` formats.
- **Duplicate Handling**: Option to overwrite or skip files when exporting.
- **File Path Management**: Automatically appends filenames and extensions based on selected file format.
- **Intuitive UI Controls**: User-friendly interface with customisable settings for buffer size, file location, and error handling.

---

## 🛠 Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/query-exporter.git
   cd query-exporter

2. **Open the Project**:
   - Open the solution file in Visual Studio and build the project.

3. **Install the Required Packages**:
   - Add `Newtonsoft.Json` via NuGet: `dotnet add package Newtonsoft.Json`
   - Add `ClosedXML` via NuGet: `dotnet add package ClosedXML`

---

## 🚀 How to Use Query Exporter

1. **Set Up Database Connection**:
   - Enter your SQL connection string in the designated text box and click **Connect** to establish a connection.

2. **Write and Execute SQL Query**:
   - Enter a SQL query in the query box, then click **Execute** to retrieve data.
   - Results appear in the data grid for quick review.

3. **Export Data**:
   - Click **Browse** to select a folder and choose your preferred export format (.csv, .xlsx, .json).
   - Click **Export** to save the results to the specified location.

4. **Review Exported Files**:
   - Find the exported data in your selected folder with the specified format and filename.
