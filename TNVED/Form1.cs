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
        ReadFromFile ob = new ReadFromFile();
        Codes obj1 = new Codes();
        List<Codes> code1 = new List<Codes>();
        int z = 0;

        public Form1()
        {
            InitializeComponent();
            code1 = ob.WriteInRAM();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string num = textBox1.Text;
            string cos = textBox2.Text;

            if (num == "")
            {
                MessageBox.Show("Вы не ввели код ТН ВЭД!");
            }
            else if (num.Length != 10)
            {
                MessageBox.Show("Введите 10-значный код ТН ВЭД!");
            }
            else if (cos == "")
            {
                MessageBox.Show("Вы не ввели стоимость!");
            }
            else
            {
                foreach (Codes c1 in code1)
                {
                    if (c1.number.Contains(num))
                    {
                        c1.cost = (float)Convert.ToDouble(cos);
                        listBox1.Items.Add(c1.number + "     " + c1.description + "     " + c1.unit + "     " + c1.tax + "     " + c1.cost);
                        textBox1.Text = "";
                        textBox2.Text = "";
                    }
                    else
                    {
                        z++;
                    }
                }
                if (z == code1.Count)
                {
                    MessageBox.Show("Код " + num + " отсутствует в базе данных!");
                    z = 0;
                }
            }
        }
        
        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                button2.Visible = true;
            }
            else
            {
                button2.Visible = false;
            }
        }
    }


    public class ReadFromFile
    {
        public  List<Codes> WriteInRAM()
        {
            string[] lines = System.IO.File.ReadAllLines(@"D:\Development\CODES\codes_all.txt");
            string temp = "", temp2 = "", temp3 = "", temp4 = "";
            char[] symbol = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int first, last, i = 0;

            List<Codes> code = new List<Codes>();

            foreach (string line in lines)
            {
                Codes obj = new Codes();
                if (line.Length > 70)
                {
                    first = 0;
                    last = 0;
                    temp = line.Remove(5, 1);
                    temp2 = temp;
                    temp3 = temp;
                    temp4 = temp;
                    temp = temp.Remove(7, 1);
                    temp = temp.Remove(10, 1);
                    temp = temp.Substring(1, 10);
                    first = temp.IndexOfAny(symbol);
                    last = temp.LastIndexOfAny(symbol);
                    if ((last - first) == 9)
                    {
                        obj.id = i;
                        obj.number = temp;
                        temp2 = temp2.Substring(15, 31);
                        obj.description = temp2;
                        temp3 = temp3.Substring(49, 6);
                        obj.unit = temp3;
                        temp4 = temp4.Substring(56, 18);
                        obj.tax = temp4;
                        code.Add(obj);
                        i++;
                    }
                }
            }
            return code;
        }
    }


    public class Codes
    {
        public int id;
        public string number;
        public string description;
        public string unit;
        public string tax;
        public float cost;
    }
}

