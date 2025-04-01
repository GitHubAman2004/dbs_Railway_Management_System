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
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(pictureBox);

            // Image URL
            string imageUrl = "https://th.bing.com/th/id/OIP.sbKUH7qRcHxu1KKdi_FQMgHaEK?rs=1&pid=ImgDetMain";

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    // Download the image as a byte array
                    byte[] imageBytes = webClient.DownloadData(imageUrl);

                    // Convert bytes to an image and display it
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        pictureBox.Image = System.Drawing.Image.FromStream(ms);
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
            //check if username and password matches
            Form2 form2 = new Form2(username,password);
            form2.Show();

            //if not messagebox.sjow invalid password
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
