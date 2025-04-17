using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project1
{
    public partial class Form1 : Form
    {
        string username;
        string password;
        public Form1()
        {
            InitializeComponent();
            PictureBox pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.CenterImage,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(pictureBox);

            string localImagePath = @"C:\Users\ANIKAIT CHITLANGIA\Downloads\b.jpg";
            string onlineImageUrl = "https://i.ibb.co/q3Vx5kTT/c.png"; // Replace with actual direct image link

            try
            {
                if (System.IO.File.Exists(localImagePath))
                {
                    pictureBox.Image = System.Drawing.Image.FromFile(localImagePath);
                }
                else
                {
                    // Local image doesn't exist — fallback to online
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(onlineImageUrl);
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            pictureBox.Image = System.Drawing.Image.FromStream(ms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string username = txtUsername.Text; // Assuming TextBox for username
            //string password = txtPassword.Text; // Assuming PasswordBox
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway_fill;Uid=dbs;Pwd=salted_password_#;";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CheckLogin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_username", username);
                        cmd.Parameters.AddWithValue("@p_hashed_password", ComputeSha256Hash(password));

                        int loginStatus = Convert.ToInt32(cmd.ExecuteScalar());
                        
                        switch(loginStatus)
                        {
                            case 2:
                                Form6 adminForm = new Form6(username);
                                adminForm.Show();
                                this.Hide(); // Close login window
                                break;
                            case 1:
                                Form2 passengerForm = new Form2(username);
                                passengerForm.Show();
                                this.Hide();
                                break;
                            default:
                                MessageBox.Show("Invalid username or password");
                                break;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }


        private void SignUp_Click(object sender, EventArgs e)
        {

            Form5 form5 = new Form5();
            form5.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //check if username and password matches in admin 
            Form6 form6 = new Form6();
            form6.Show();

            //if not display error message in messagebox
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            username=textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            password=textBox2.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;  // Maximizes the form
            this.FormBorderStyle = FormBorderStyle.None;    // Removes the border
            this.TopMost = true;
        }
    }
}
