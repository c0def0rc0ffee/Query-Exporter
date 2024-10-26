using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QueryExporter.DAL_; // Namespace for SQLAdapter
using QueryExporter.BLL_; // Namespace for FileHandler
using WinForms = System.Windows.Forms; // Alias for System.Windows.Forms
using MessageBox = System.Windows.MessageBox;
using System.IO;
using OfficeOpenXml;

namespace QueryExporter
{
    public partial class MainWindow : Window
    {
        private SQLAdapter _sqlAdapter;
        private FileHandler _fileHandler;

        /// <summary>
        /// Initialises the main window and sets up initial configurations.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ConnectionStringTextBox.Text = "Server=TITANIA;Database=Enterprise_Demo;User Id=222;Password=222;TrustServerCertificate=true;";
            SetPlaceholderText(QueryTextBox, "Enter SQL query here");
            ExecuteButton.IsEnabled = false; // Disable Execute button initially
            _fileHandler = new FileHandler(); // Initialise the FileHandler instance
        }

        /// <summary>
        /// Handles the Connect button click event to establish a database connection.
        /// </summary>
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConnectionStringTextBox.Text;
            _sqlAdapter = new SQLAdapter(connectionString);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MessageBox.Show("Connection successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteButton.IsEnabled = true; // Enable Execute button after successful connection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ExecuteButton.IsEnabled = false; // Ensure Execute button is disabled if connection fails
            }
        }

        /// <summary>
        /// Handles the Execute button click event to execute the SQL query and display the results.
        /// </summary>
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string query = QueryTextBox.Text;

            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Please enter a SQL query.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SqlCommand command = new SqlCommand(query);
                DataSet dataSet = _sqlAdapter.ExecuteQuery(command);
                ResultsDataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query execution failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Export button click event to export the results to a selected file format.
        /// </summary>
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsDataGrid.ItemsSource == null)
            {
                MessageBox.Show("No data available to export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Set the LicenseContext to NonCommercial to comply with EPPlus licensing requirements
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Use SaveFileDialog to select file format and save location
            using (var saveFileDialog = new WinForms.SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xlsx)|*.xlsx|JSON Files (*.json)|*.json";
                saveFileDialog.DefaultExt = "csv";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == WinForms.DialogResult.OK)
                {
                    try
                    {
                        // Determine file extension from selected file type
                        var filePath = saveFileDialog.FileName;
                        var dataTable = ((DataView)ResultsDataGrid.ItemsSource).ToTable();

                        // Call the appropriate export method based on file extension
                        switch (Path.GetExtension(filePath).ToLower())
                        {
                            case ".csv":
                                _fileHandler.ExportDataTableToCsv(dataTable, filePath);
                                break;
                            case ".xlsx":
                                _fileHandler.ExportDataTableToExcel(dataTable, filePath);
                                break;
                            case ".json":
                                _fileHandler.ExportDataTableToJson(dataTable, filePath);
                                break;
                            default:
                                MessageBox.Show("Unsupported file format selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                        }

                        MessageBox.Show("Data exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Close button click event to shut down the application.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Handles the Minimise button click event to minimise the application.
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Maximise button click event to maximise or restore the application.
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Handles the Browse button click event to open a folder browser dialogue and set the export location.
        /// </summary>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new WinForms.FolderBrowserDialog())
            {
                WinForms.DialogResult result = folderDialog.ShowDialog();

                if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    ExportLocationTextBox.Text = System.IO.Path.Combine(folderDialog.SelectedPath, "export.csv");
                }
            }
        }

        /// <summary>
        /// Handles the GotFocus event for the text boxes to clear placeholder text.
        /// </summary>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (System.Windows.Controls.TextBox)sender;
            if (textBox.Foreground == Brushes.Gray)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.White; // Change to white
            }
        }

        /// <summary>
        /// Handles the LostFocus event for the text boxes to set placeholder text if empty.
        /// </summary>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (System.Windows.Controls.TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == nameof(QueryTextBox))
                {
                    SetPlaceholderText(textBox, "Enter SQL query here");
                }
            }
        }

        /// <summary>
        /// Sets the placeholder text for a text box.
        /// </summary>
        /// <param name="textBox">The text box to set the placeholder for.</param>
        /// <param name="placeholderText">The placeholder text to set.</param>
        private void SetPlaceholderText(System.Windows.Controls.TextBox textBox, string placeholderText)
        {
            textBox.Text = placeholderText;
            textBox.Foreground = Brushes.Gray; // Change to grey for placeholder
        }

        /// <summary>
        /// Handles the New menu item click event.
        /// </summary>
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("New menu item clicked.");
        }

        /// <summary>
        /// Handles the Open menu item click event.
        /// </summary>
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open menu item clicked.");
        }

        /// <summary>
        /// Handles the Save menu item click event.
        /// </summary>
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Save menu item clicked.");
        }

        /// <summary>
        /// Handles the Exit menu item click event to shut down the application.
        /// </summary>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Handles the Undo menu item click event.
        /// </summary>
        private void UndoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Undo menu item clicked.");
        }

        /// <summary>
        /// Handles the Redo menu item click event.
        /// </summary>
        private void RedoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redo menu item clicked.");
        }

        /// <summary>
        /// Handles the About menu item click event.
        /// </summary>
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("About menu item clicked.");
        }

        /// <summary>
        /// Handles the mouse left button down event for the title bar to allow dragging the window.
        /// </summary>
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeButton_Click(sender, e);
            }
            else
            {
                DragMove();
            }
        }
    }
}
