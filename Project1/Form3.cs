using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
        public Form3(string a,string b,string c, string d, string e)
        {
            username = a;
            password = b;
            from = c;
            to = d;
            date = e;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(username,password,from,to,date,train,type);
            form4.Show();
            this.Close();
        }

        


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway_fill;Uid=dbs;Pwd=salted_password_#;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string fromStation = from;  // TextBox for "From" station
                    string toStation = to;      // TextBox for "To" station

                    // SQL query to find train names between given stations
                    string query = @"
            SELECT t.train_name
            FROM train t
            JOIN schedule sc ON t.train_id = sc.train_id
            JOIN station s1 ON sc.src_station_id = s1.station_id
            JOIN station s2 ON sc.dest_station_id = s2.station_id
            WHERE s1.station_name = @fromStation AND s2.station_name = @toStation;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fromStation", fromStation);
                        cmd.Parameters.AddWithValue("@toStation", toStation);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox1.Items.Clear();  // Clear previous results

                            while (reader.Read())
                            {
                                // Add train name to the ComboBox
                                comboBox1.Items.Add(reader["train_name"].ToString());
                            }

                            if (comboBox1.Items.Count > 0)
                            {
                                comboBox1.SelectedIndex = 0;  // Select the first item by default
                            }
                            else
                            {
                                MessageBox.Show("No trains found between the specified stations.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }



            train = comboBox1.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //get price from database
            textBox1.Text = price.ToString();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();  // Clear existing items (if any)
            comboBox2.Items.Add("1AC");
            comboBox2.Items.Add("2AC");
            comboBox2.Items.Add("3AC");
            comboBox2.Items.Add("Sleeper");

            this.WindowState = FormWindowState.Maximized;  // Maximizes the form
            this.FormBorderStyle = FormBorderStyle.None;    // Removes the border
            this.TopMost = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = comboBox2.Text;
        }
    }
}
