using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteConnection _connection;
        public MainWindow()
        {
            InitializeComponent();

            _connection = new SQLiteConnection("Data Source=contact-manager.db;");
            _connection.Open();

            LoadContacts();
        }



        private void LoadContacts()
        {
            try
            {
                string query = "SELECT Id, Name, Phone FROM Contacts";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, _connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Set the DataGrid's item source to display contacts
                contactDataGrid.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex.Message);
            }
        }

    




        private void contactDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contactDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)contactDataGrid.SelectedItem;

                // Get the selected contact's details
                string name = row["Name"].ToString();
                string phone = row["Phone"].ToString();

                // Open the ContactDetailsWindow and pass the selected contact's details
                ContactDetailsWindow detailsWindow = new ContactDetailsWindow(name, phone);
                detailsWindow.Show();
            }
        }





        // Method to reload contacts in the DataGrid
        private void ReloadContacts()
        {
            try
            {
                string query = "SELECT Id, Name, Phone FROM Contacts";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, _connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Set the DataGrid's item source to display contacts
                contactDataGrid.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        // Method to add a new contact and synchronize with the database
        private void AddContact(string name, string phone)
        {
            try
            {
                var cmd = new SQLiteCommand(@"
        INSERT INTO Contacts (Name, Phone)
        VALUES (@Name, @Phone)", _connection);

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Phone", phone);

                cmd.ExecuteNonQuery();

                // No need to reload contacts here; it will be done in the event handler
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to delete a contact and synchronize with the database
        private void DeleteContact(int contactId)
        {
            try
            {
                var cmd = new SQLiteCommand(@"
        DELETE FROM Contacts WHERE Id = @Id", _connection);

                cmd.Parameters.AddWithValue("@Id", contactId);

                cmd.ExecuteNonQuery();

                // No need to reload contacts here; it will be done in the event handler
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to update a contact and synchronize with the database
        private void UpdateContact(int contactId, string newName, string newPhone)
        {
            try
            {
                var cmd = new SQLiteCommand(@"
        UPDATE Contacts SET Name = @Name, Phone = @Phone WHERE Id = @Id", _connection);

                cmd.Parameters.AddWithValue("@Name", newName);
                cmd.Parameters.AddWithValue("@Phone", newPhone);
                cmd.Parameters.AddWithValue("@Id", contactId);

                cmd.ExecuteNonQuery();

                // No need to reload contacts here; it will be done in the event handler
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Event handler for Add button click (example)
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Get values from UI elements (e.g., txtContactName.Text, txtContactPhone.Text)
            string name = txtContactName.Text;
            string phone = txtContactPhone.Text;

            // Add the contact (will sync with the database)
            AddContact(name, phone);

            // Reload contacts in the DataGrid after adding
            ReloadContacts();
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new SQLiteCommand(txtCommand.Text, _connection);


            var response = cmd.ExecuteScalar();


            MessageBox.Show(response?.ToString());
        }


        /**private void CreateTableButton_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new SQLiteCommand(@"
CREATE TABLE Contacts (
    Id          int             IDENTITY(1,1),
    Name        varchar(255),
    Phone       varchar(255)
);
", _connection);


            cmd.ExecuteNonQuery();
        }**/

        private void CreateTableButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cmd = new SQLiteCommand(@"
        CREATE TABLE IF NOT EXISTS Contacts (
            Id      INTEGER PRIMARY KEY AUTOINCREMENT,
            Name    VARCHAR(255),
            Phone   VARCHAR(255)
        )", _connection);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating table: " + ex.Message);
            }
        }



        private void btAddContact_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new SQLiteCommand(@"
INSERT INTO Contacts 
(Name, Phone)
VALUES " +
$"('{txtContactName.Text}','{txtContactPhone.Text}')", _connection);


            cmd.ExecuteNonQuery();


            txtContactName.Clear();
            txtContactPhone.Clear();
        }


        /**  private void btMoveLeft_Click(object sender, RoutedEventArgs e)
          {
              string cmdText = string.Format(@"
SELECT Name, Phone
FROM Contacts
WHERE Name < '{0}' 
ORDER BY Name DESC", txtContactName.Text);


              var cmd = new SQLiteCommand(cmdText, _connection);


              var reader = cmd.ExecuteReader();


              if (reader.Read())
              {
                  txtContactName.Text = reader.GetValue(0).ToString();
                  txtContactPhone.Text = reader.GetValue(1).ToString();
              }
              else
              {
                  txtContactName.Text = string.Empty;
                  txtContactPhone.Text = string.Empty;
              }
          }


          private void btMoveRight_Click(object sender, RoutedEventArgs e)
          {
              string cmdText = string.Format(@"
SELECT Name, Phone
FROM Contacts
WHERE Name > '{0}' 
ORDER BY Name ASC", txtContactName.Text);


              var cmd = new SQLiteCommand(cmdText, _connection);


              var reader = cmd.ExecuteReader();


              if (reader.Read())
              {
                  txtContactName.Text = reader.GetValue(0).ToString();
                  txtContactPhone.Text = reader.GetValue(1).ToString();
              }
              else
              {
                  txtContactName.Text = string.Empty;
                  txtContactPhone.Text = string.Empty;
              }
          }**/


        private void btMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            string cmdText = string.Format(@"
        SELECT Name, Phone
        FROM Contacts
        WHERE Name < '{0}' 
        ORDER BY Name DESC
        LIMIT 1", txtContactName.Text);

            var cmd = new SQLiteCommand(cmdText, _connection);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                txtContactName.Text = reader["Name"].ToString();
                txtContactPhone.Text = reader["Phone"].ToString();
            }
            else
            {
                txtContactName.Text = string.Empty;
                txtContactPhone.Text = string.Empty;
            }
        }

        private void btMoveRight_Click(object sender, RoutedEventArgs e)
        {
            string cmdText = string.Format(@"
        SELECT Name, Phone
        FROM Contacts
        WHERE Name > '{0}' 
        ORDER BY Name ASC
        LIMIT 1", txtContactName.Text);

            var cmd = new SQLiteCommand(cmdText, _connection);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                txtContactName.Text = reader["Name"].ToString();
                txtContactPhone.Text = reader["Phone"].ToString();
            }
            else
            {
                txtContactName.Text = string.Empty;
                txtContactPhone.Text = string.Empty;
            }
        }


        // Method to import contacts from a .csv file and synchronize with the database
        private void ImportContactsFromCSV(string filePath)
        {
            try
            {
                // Read all lines from the .csv file
                string[] lines = File.ReadAllLines(filePath);

                // Skip the header if it exists (assuming the first line is the header)
                bool isFirstLine = true;

                foreach (string line in lines)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue; // Skip the header
                    }

                    // Split the line into fields assuming comma (',') as delimiter
                    string[] fields = line.Split(',');

                    // Assuming the .csv format is: Name,Phone
                    if (fields.Length >= 2)
                    {
                        string name = fields[0].Trim();
                        string phone = fields[1].Trim();

                        // Insert each contact into the database
                        var cmd = new SQLiteCommand(@"
                INSERT INTO Contacts (Name, Phone)
                VALUES (@Name, @Phone)", _connection);

                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Phone", phone);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Reload contacts in the DataGrid after importing
                ReloadContacts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Event handler for importing contacts from a .csv file
        private void ImportCSVButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    // Call the method to import contacts from the .csv file
                    ImportContactsFromCSV(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ExportContactsToCSV(string filePath)
        {
            try
            {
                // Retrieve contacts data from the database
                var cmd = new SQLiteCommand("SELECT Name, Phone FROM Contacts", _connection);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Create a new .csv file and write contacts data
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write header
                    writer.WriteLine("Name,Phone");

                    // Write each contact data to the .csv file
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string name = row["Name"].ToString();
                        string phone = row["Phone"].ToString();
                        writer.WriteLine($"{name},{phone}");
                    }
                }

                MessageBox.Show("Contacts exported successfully to " + filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Event handler for exporting contacts to a .csv file
        private void ExportCSVButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // Call the method to export contacts to the .csv file
                    ExportContactsToCSV(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


    }
}
  