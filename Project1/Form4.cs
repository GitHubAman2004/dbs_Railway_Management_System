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
    public partial class Form4 : Form
    {
        string username, password, from, to, date, train, type;
        string name, age, gender, contact;

        public Form4(string a, string b, string c, string d, string e, string f, string g)
        {
            username = a;
            password = b;
            from = c;
            to = d;
            date = e;
            train = f;
            type = g;

            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form4_Paint);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            // Style controls
            StyleTextBox(textBox1);
            StyleTextBox(textBox2);
            StyleTextBox(textBox3);
            StyleTextBox(textBox4);

            button1.BackColor = Color.FromArgb(40, 80, 120);
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            // Add a bold title label
            Label labelTitle = new Label();
            labelTitle.Text = "Passenger Details";
            labelTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.BackColor = Color.Transparent;
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(50, 30);
            this.Controls.Add(labelTitle);
        }

        private void Form4_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(52, 73, 94),    // Dark blue-gray railway tone
                Color.FromArgb(149, 165, 166), // Light gray steel tone
                90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void StyleTextBox(TextBox tb)
        {
            tb.BackColor = Color.White;
            tb.ForeColor = Color.Black;
            tb.Font = new Font("Segoe UI", 10F);
            tb.BorderStyle = BorderStyle.FixedSingle;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            name = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            age = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            gender = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            contact = textBox4.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(age) ||
                string.IsNullOrWhiteSpace(gender) ||
                string.IsNullOrWhiteSpace(contact))
            {
                MessageBox.Show("Please fill in all passenger details before proceeding.",
                                "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string passengerId = "";

            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT passenger_id FROM passenger WHERE name = @username";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                passengerId = reader["passenger_id"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Passenger ID not found for the provided name.",
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Show the ticket details
            MessageBox.Show("Ticket Details:\n\n" +
                            "Name: " + name + "\n" +
                            "Age: " + age + "\n" +
                            "Gender: " + gender + "\n" +
                            "Contact: " + contact + "\n\n" +
                            "From: " + from + "\n" +
                            "To: " + to + "\n" +
                            "Date: " + date + "\n" +
                            "Train: " + train + "\n" +
                            "Coach Type: " + type + "\n" +
                            "Passenger ID: " + passengerId + "\n" +
                            "Platform No.: " + "1");

            this.Close();
        }


        private void label5_Click(object sender, EventArgs e)
        {
            // Placeholder - optional
        }
    }
}
