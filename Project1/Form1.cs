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
using MySql.Data.MySqlClient; // Added for MySqlException
using System.Security.Cryptography; // Added for SHA256

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

        // Passenger login - Sends SHA256 hashed password
        private void button1_Click(object sender, EventArgs e)
        {
            // Get fresh values from textboxes
            username = textBox1.Text.Trim();
            password = textBox2.Text;

            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

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

                    // For debug purposes, you can uncomment to see the hash
                    // string hashedPwd = ComputeSha256Hash(password);
                    // MessageBox.Show($"Hashed password: {hashedPwd}");

                    using (MySqlCommand cmd = new MySqlCommand("CheckLogin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_username", username);
                        string hashedPassword = ComputeSha256Hash(password);
                        //MessageBox.Show("Hashed password: " + hashedPassword);
                        cmd.Parameters.AddWithValue("@p_hashed_password", hashedPassword);
                        // it is showing correct hashed password

                        object result = cmd.ExecuteScalar();
                        
                        if (result != null)
                        {
                            int loginStatus = Convert.ToInt32(result);
                            //MessageBox.Show("login status:" + loginStatus);
                            if (loginStatus == 1)
                            {
                                
                                Form2 passengerForm = new Form2(username);
                                passengerForm.Show();
                                //this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid passenger credentials");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Login check returned no result");
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

        // Admin login - Sends plaintext password
        //private void button2_Click(object sender, EventArgs e)
        //{
            // Get fresh values from textboxes
            //username = textBox1.Text.Trim();
            //password = textBox2.Text;

            //string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

            //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            //{
        //        MessageBox.Show("Please enter both username and password");
        //        return;
        //    }

        //    try
        //    {
        //        using (MySqlConnection conn = new MySqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            using (MySqlCommand cmd = new MySqlCommand("CheckLogin", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@p_username", username);
        //                cmd.Parameters.AddWithValue("@p_hashed_password", password); // Plain password for admin

        //                object result = cmd.ExecuteScalar();
        //                if (result != null)
        //                {
        //                    int loginStatus = Convert.ToInt32(result);

        //                    if (loginStatus == 2)
        //                    {
        //                        Form6 adminForm = new Form6();
        //                        adminForm.Show();
        //                        this.Hide();
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show("Invalid admin credentials");
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Login check returned no result");
        //                }
        //            }
        //        }
        //    }
        //    catch (MySqlException ex)
        //    {
        //        MessageBox.Show($"Database error: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error: {ex.Message}");
        //    }
        //}

        private void button3_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            username = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            password = textBox2.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;  // Maximizes the form
            this.FormBorderStyle = FormBorderStyle.None;    // Removes the border
            this.TopMost = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            
            // Get fresh values from textboxes
            username = textBox1.Text.Trim();
            password = textBox2.Text;

            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

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
                        string hashedPassword = ComputeSha256Hash(password);
                        cmd.Parameters.AddWithValue("@p_hashed_password", hashedPassword); // Plain password for admin

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int loginStatus = Convert.ToInt32(result);

                            if (loginStatus == 2)
                            {
                                Form6 adminForm = new Form6();
                                adminForm.Show();
                                //this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid admin credentials");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Login check returned no result");
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
