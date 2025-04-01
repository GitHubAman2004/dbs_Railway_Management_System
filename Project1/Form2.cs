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
    public partial class Form2 : Form
    {
        string username, password;
        string from, to;
        string date;
        public Form2(string user, string pass)
        {
            InitializeComponent();
            username= user;
            password= pass;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(username,password,from,to,date);
            form3.Show();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway_fill;Uid=dbs;Pwd=salted_password_#;";
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
                        // Add station names to ComboBox
                        comboBox1.Items.Add(reader.GetString("station_name"));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            this.WindowState = FormWindowState.Maximized;  // Maximizes the form
            this.FormBorderStyle = FormBorderStyle.None;    // Removes the border
            this.TopMost = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            to=comboBox2.Text;
        }
        private void LoadStationNames()
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway_fill;Uid=dbs;Pwd=salted_password_#;";

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
                        // Add station names to both ComboBoxes
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            LoadStationNames();

            // Remove the selected item from ComboBox2
            if (comboBox1.SelectedItem != null)
            {
                comboBox2.Items.Remove(comboBox1.SelectedItem.ToString());
            }
            from = comboBox1.Text;
        }
    }
}
