using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNVED
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           string temp = textBox1.Text;

            if (temp == "")
            {
                MessageBox.Show("Вы не ввели код ТН ВЭД!");
            }
            else
            {
                listBox1.Items.Add(temp);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

    }
}

