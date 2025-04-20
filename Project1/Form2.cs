using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Project1
{
    public partial class Form2 : Form
    {
        string username, password;
        string from, to;
        string date; string dayOfWeek;

        public Form2(string user)
        {
            InitializeComponent();
            username = user;

            // Enable custom drawing
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form2_Paint);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT station_name FROM station";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetString("station_name"));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            // Apply visual styles
            comboBox1.BackColor = Color.White;
            comboBox1.ForeColor = Color.Black;
            comboBox1.FlatStyle = FlatStyle.Flat;

            comboBox2.BackColor = Color.White;
            comboBox2.ForeColor = Color.Black;
            comboBox2.FlatStyle = FlatStyle.Flat;

            button1.BackColor = Color.FromArgb(50, 50, 100);
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            // Add title label
            Label labelTitle = new Label();
            labelTitle.Text = "Indian Railway Reservation";
            labelTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.BackColor = Color.Transparent;
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(50, 30);
            this.Controls.Add(labelTitle);
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(30, 30, 60),  // Dark steel blue
                Color.FromArgb(169, 169, 169),  // Light gray
                90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(username, password, from, to, date, dayOfWeek);
            //MessageBox.Show(from + " " + to);
            form3.Show();
            this.Close();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway_fill;Uid=dbs;Pwd=salted_password_#;";

        //    try
        //    {
        //        using (MySqlConnection conn = new MySqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            string query = @"
        //        SELECT 
        //            t.train_id,
        //            t.train_name,
        //            s.station_name AS running_from_station
        //        FROM 
        //            schedule sc
        //        JOIN 
        //            train t ON sc.train_id = t.train_id
        //        JOIN 
        //            station s ON sc.src_station_id = s.station_id
        //        ORDER BY 
        //            t.train_id;";

        //            using (MySqlCommand cmd = new MySqlCommand(query, conn))
        //            using (MySqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                StringBuilder result = new StringBuilder();

        //                while (reader.Read())
        //                {
        //                    result.AppendLine($"Train ID: {reader["train_id"]}, Name: {reader["train_name"]}, From: {reader["running_from_station"]}");
        //                }

        //                if (result.Length == 0)
        //                {
        //                    MessageBox.Show("No train data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                }
        //                else
        //                {
        //                    MessageBox.Show(result.ToString(), "Train Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //    // Navigate to Form3 after showing data
        //    Form3 form3 = new Form3(username, password, from, to, date,dayOfWeek);
        //    form3.Show();
        //    this.Close();
        //}





        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            LoadStationNames();

            if (comboBox1.SelectedItem != null)
            {
                comboBox2.Items.Remove(comboBox1.SelectedItem.ToString());
            }
            from = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            to = comboBox2.Text;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value;
            DateTime today = DateTime.Today;

            if (selectedDate <= today)
            {
                MessageBox.Show("Please select valid date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker1.Value = today.AddDays(1); // Reset to tomorrow
                return;
            }

            date = selectedDate.ToString("yyyy-MM-dd");
            dayOfWeek = selectedDate.ToString("ddd"); // "Mon", "Tue", etc.
        }


        private void LoadStationNames()
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT station_name FROM station";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string stationName = reader.GetString("station_name");
                        comboBox1.Items.Add(stationName);
                        comboBox2.Items.Add(stationName);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
