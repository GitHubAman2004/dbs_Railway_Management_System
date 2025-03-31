using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project1
{
    public partial class Form3 : Form
    {
        string train, type;
        double price = 120.9;
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //select from database and display in form3_load
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
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = comboBox2.Text;
        }
    }
}
