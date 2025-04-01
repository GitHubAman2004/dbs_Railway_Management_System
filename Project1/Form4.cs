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
        string username, password, from, to, date,train,type;
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
            MessageBox.Show("Ticket Details : \nFrom : " +from+"\nTo : " +to+"\nDate : " +date+"\nTrain No. : "+train+"\nPassenger ID : "+a+"\nPlatform No. : " +a+"\nCoach Type. : " +type);
            this.Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;  // Maximizes the form
            this.FormBorderStyle = FormBorderStyle.None;    // Removes the border
            this.TopMost = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            name = textBox1.Text;
        }
    }
}
