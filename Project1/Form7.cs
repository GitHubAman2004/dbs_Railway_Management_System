using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Project1
{
    public partial class Form7 : Form
    {
        // Update your connection string here
        string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

        public Form7()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            // Optional: Set focus or initialize things
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            string feedback = feedbackTextBox.Text;

            if (string.IsNullOrWhiteSpace(feedback))
            {
                MessageBox.Show("Please enter your feedback.");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO feedback (comments) VALUES (@comments)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@comments", feedback);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thank you for your feedback!");
                    feedbackTextBox.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}

