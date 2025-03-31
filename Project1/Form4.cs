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
    public partial class Form4 : Form
    {
        string name, age, gender, contact;
        public Form4()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            age= textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            gender= textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            contact= textBox4.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string a = "";
            MessageBox.Show("Ticket Details : \nFrom : " +a+"\nTo : " +a+"\nDate : " +a+"\nTrain No. : "+a+"\nPassenger ID : "+a+"\nPlatform No. : " +a+"\nSeat No. : " +a);
            this.Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            name = textBox1.Text;
        }
    }
}
