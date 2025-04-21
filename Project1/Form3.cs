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
    public partial class Form3 : Form
    {
        string username, password, from, to, date;
        string train, type;
        double price = 120.9;
        string dayOfWeek;

        public Form3(string a, string b, string c, string d, string e, string f)
        {
            username = a;
            password = b;
            from = c;
            to = d;
            date = e;
            dayOfWeek = f;

            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form3_Paint);
            this.Shown += new EventHandler(Form3_Shown);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Clear(); // Don't pre-populate classes here

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            // Style controls
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

            button2.BackColor = Color.FromArgb(50, 50, 100);
            button2.ForeColor = Color.White;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            // Add railway-themed title label
            Label labelTitle = new Label();
            labelTitle.Text = "Available Trains";
            labelTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.BackColor = Color.Transparent;
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(50, 30);
            this.Controls.Add(labelTitle);
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            LoadTrains();
        }

        private void Form3_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(44, 62, 80),  // Dark railway steel
                Color.FromArgb(189, 195, 199),  // Light platform gray
                90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(username);
            form2.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(train) || string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Please select both train and class type before proceeding.");
                return;
            }

            Form4 form4 = new Form4(username, password, from, to, date, train, type);
            form4.Show();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            train = comboBox1.Text;
            if (!string.IsNullOrEmpty(train))
            {
                LoadAvailableClassesForTrain(train);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = comboBox2.Text;
        }

        // Load train names into comboBox1
        private void LoadTrains()
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT DISTINCT t.train_name FROM train t 
                        JOIN schedule sc ON t.train_id = sc.train_id 
                        JOIN station s1 ON sc.src_station_id = s1.station_id 
                        JOIN station s2 ON sc.dest_station_id = s2.station_id 
                        WHERE s1.station_name = @fromStation AND 
                              s2.station_name = @toStation AND 
                              sc.day_of_week = @dayOfWeek;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fromStation", from);
                        cmd.Parameters.AddWithValue("@toStation", to);
                        cmd.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox1.Items.Clear();

                            while (reader.Read())
                            {
                                comboBox1.Items.Add(reader["train_name"].ToString());
                            }

                            if (comboBox1.Items.Count > 0)
                            {
                                comboBox1.SelectedIndex = 0;
                            }
                            else
                            {
                                MessageBox.Show("No trains found between the specified stations.\nRedirecting in 3 seconds...", "No Trains", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                Timer timer = new Timer();
                                timer.Interval = 3000;
                                timer.Tick += (s, args) =>
                                {
                                    timer.Stop();
                                    Form2 form2 = new Form2(username);
                                    form2.Show();
                                    this.Close();
                                };
                                timer.Start();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting to database: " + ex.Message);
                }
            }
        }

        // Load available class types for a selected train
        private void LoadAvailableClassesForTrain(string trainName)
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT s.class FROM seat s
                        JOIN train t ON s.train_id = t.train_id
                        WHERE t.train_name = @trainName;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@trainName", trainName);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox2.Items.Clear();

                            while (reader.Read())
                            {
                                comboBox2.Items.Add(reader["class"].ToString());
                            }

                            if (comboBox2.Items.Count > 0)
                            {
                                comboBox2.SelectedIndex = 0;
                            }
                            else
                            {
                                MessageBox.Show("No class types available for this train.", "No Classes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading class types: " + ex.Message);
                }
            }
        }
    }
}
