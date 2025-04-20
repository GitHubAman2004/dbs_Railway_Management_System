using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Project1
{
    public partial class Form5 : Form
    {
        string username, password, contact, email;

        public Form5()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form5_Paint);
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            // Apply styling to controls
            StyleTextBox(textBox1);
            StyleTextBox(textBox2);
            StyleTextBox(textBox3);
            StyleTextBox(textBox4);

            button1.BackColor = Color.FromArgb(40, 80, 120);
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            Label labelTitle = new Label();
            labelTitle.Text = "User Registration";
            labelTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.BackColor = Color.Transparent;
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(50, 30);
            this.Controls.Add(labelTitle);
        }

        private void Form5_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(44, 62, 80),    // Deep railway blue
                Color.FromArgb(149, 165, 166), // Metallic steel gray
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
            username = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            password = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            email = textBox3.Text;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            contact = textBox4.Text;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Optional - placeholder
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(contact) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("All fields must be filled!", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length <= 4)
            {
                MessageBox.Show("Password must be more than 4 characters long!",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format!", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SignupUser", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@p_username", username);
                        cmd.Parameters.AddWithValue("@p_contact", contact);
                        cmd.Parameters.AddWithValue("@p_email", email);
                        cmd.Parameters.AddWithValue("@p_password", password);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Registration successful!", "Success",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Registration failed.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        this.Close();
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Username or email already exists!", "Conflict Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
