using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Project1
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            // Database connection string - replace placeholders
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway_fill;Uid=dbs;Pwd=salted_password_#;";

            // SQL query to execute
            string query = "SELECT * FROM admin;";

            // Create MySQL connection
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open connection to the database
                    connection.Open();

                    // Create a MySQL command to execute the query
                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Execute the query and retrieve the data
                        using (var reader = command.ExecuteReader())
                        {
                            // Check if data exists
                            if (reader.HasRows)
                            {
                                // StringBuilder to hold the result for display
                                var result = new System.Text.StringBuilder();

                                // Read through the rows and display the data
                                while (reader.Read())
                                {
                                    // Assuming the Platform table has 2 columns, adjust the column indices as needed
                                    result.AppendLine($"Column1: {reader[0]} | Column2: {reader[1]}");
                                }

                                // Show the data in the message box
                                MessageBox.Show(result.ToString(), "Query Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No data found in the Platform table.", "Query Results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If there's an error, show it in a message box
                    MessageBox.Show($"Connection failed: {ex.Message}", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
