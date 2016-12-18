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
            //float cost = textBox2.Text;

            if (num == "")
            {
                /*foreach (Codes c1 in code1)
                {
                    listBox1.Items.Add(c1.id + "  " + c1.number);
                }*/
                //listBox1.Items.Add(code1[0].id + "     " + code1[1].number + "     " + code1[0].description + "     " + code1[0].unit + "     " + code1[0].tax);
                
                MessageBox.Show("Вы не ввели код ТН ВЭД!");

            }
            else if (num.Length != 10)
            {
                MessageBox.Show("Введите 10-значный код ТН ВЭД!");
            }
            else
            {
                foreach (Codes c1 in code1)
                {
                    if (c1.number.Contains(num))
                    {
                        listBox1.Items.Add(c1.number + "     " + c1.description + "     " + c1.unit + "     " + c1.tax);
                        textBox1.Text = "";
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
                        //Console.WriteLine(i);
                        obj.number = temp;
                        //Console.WriteLine(temp);
                        temp2 = temp2.Substring(15, 31);
                        obj.description = temp2;
                        //Console.WriteLine(temp2);
                        temp3 = temp3.Substring(49, 6);
                        obj.unit = temp3;
                        //Console.WriteLine(temp3);
                        temp4 = temp4.Substring(56, 18);
                        obj.tax = temp4;
                        //Console.WriteLine(temp4);
                        code.Add(obj);
                        i++;
                    }
                }
            }
            
            return code;
            /*foreach (Codes c in code)
            {
                Console.WriteLine(c.id + "  " + c.number + "  " + c.description + "  " + c.unit + "  " + c.tax);
            }*/

            //Console.WriteLine("Press any key to exit.");
            //System.Console.ReadKey();
        }
    }

    public class Codes
    {
        public int id;
        public string number;
        public string description;
        public string unit;
        public string tax;
    }
}

