using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Project1
{
    public partial class Form6 : Form
    {
        // Keep track of the currently selected button
        private Button currentButton;
        // Define colors
        private Color activeColor = Color.Yellow;
        private Color inactiveColor = Color.White;

        // Your existing variables
        int a, b, c, d;
        String uname;

        public Form6()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            // Your existing form load code...
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            // Load your counts from database as you're already doing...
            LoadCounts();

            // Set the Dashboard button as default active button
           // if (buttonDashboard != null)  // Make sure you've named your Dashboard button as "buttonDashboard"
            //{
              //  SetActiveButton(buttonDashboard);
            //}
        }

        // Method to set a button as active
        private void SetActiveButton(Button button)
        {
            // Reset previous button if any
            if (currentButton != null)
            {
                currentButton.BackColor = inactiveColor;
                currentButton.ForeColor = Color.Black;
            }

            // Set the new active button
            button.BackColor = activeColor;
            button.ForeColor = Color.Black;  // Or any other text color that looks good on yellow

            // Update current button reference
            currentButton = button;
        }

        // Click handlers for each button
        private void buttonDashboard_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            // Show dashboard content
            ShowDashboard();
        }

        private void buttonTicketInfo_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            // Show ticket information
            ShowTicketInfo();
        }

        private void buttonStaffInfo_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            // Show staff information
            ShowStaffInfo();
        }

        private void buttonTrainInfo_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            // Show train information
            ShowTrainInfo();
        }

        private void buttonPassengerInfo_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            // Show passenger information
            ShowPassengerInfo();
        }

        private void buttonFeedbackInfo_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            // Show feedback information
            ShowFeedbackInfo();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            // Handle logout
            this.Close();
        }

        // Methods to show different content
        private void ShowDashboard()
        {
            // Hide data grid if visible
            dataGridView1.Visible = false;

            // Show dashboard elements (counts, etc.)
            // This content is already visible in your form
        }

        private void ShowTicketInfo()
        {
            // Your existing ticket info code from button1_Click
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM ticket";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Show data grid
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ShowStaffInfo()
        {
            // Your existing staff info code from button2_Click
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";
            string query = "SELECT * FROM staff";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Show data grid
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ShowTrainInfo()
        {
            // Your existing train info code from button3_Click
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM train";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Show data grid
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ShowPassengerInfo()
        {
            // Your existing passenger info code from button4_Click
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM passenger";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Show data grid
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ShowFeedbackInfo()
        {
            // You would add code here to display feedback information
            // Similar to your other info methods
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM feedback"; // Assuming you have a feedback table

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Show data grid
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to load counts from database (extracted from your Form6_Load)
        private void LoadCounts()
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Load staff count
                    string query = "SELECT COUNT(staff_id) AS staff_count FROM staff";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        c = Convert.ToInt32(result);
                        textBox3.Text = c.ToString();
                    }

                    // Load train count
                    query = "SELECT COUNT(train_id) AS train_count FROM train";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        b = Convert.ToInt32(result);
                        textBox2.Text = b.ToString();
                    }

                    // Load passenger count
                    query = "SELECT COUNT(passenger_id) AS passenger_count FROM passenger";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        a = Convert.ToInt32(result);
                        textBox1.Text = a.ToString();
                    }

                    // Load admin count
                    query = "SELECT COUNT(admin_id) AS admin_count FROM admin";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        d = Convert.ToInt32(result);
                        textBox4.Text = d.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading counts: " + ex.Message);
            }
        }
    }
}