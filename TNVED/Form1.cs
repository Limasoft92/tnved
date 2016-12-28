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
        List<Codes> code2 = new List<Codes>();
        BindingSource source = new BindingSource();

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

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int ind = 0;
            bool yes = false;
            //string io = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            int ec = e.ColumnIndex;
            int er = e.RowIndex;
            foreach (Codes c1 in code1)
            {
                if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() != null)
                {
                    if (c1.number.Equals(dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString()))
                    {
                        yes = true;
                        ind = c1.id;
                        break;
                    }
                }
            }
            if (yes) {
                code2[e.RowIndex] = code1[ind];
                code2[e.RowIndex].id = e.RowIndex + 1;
                code2.Add(new Codes());
                source.ResetBindings(false);
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Green;
            }
            else
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Red;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            source.DataSource = code1;
            dataGridView1.DataSource = source;
            code2.Add(new Codes());
            source.ResetBindings(false);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].Width = 316;
            dataGridView1.Columns[3].Width = 60;
            dataGridView1.Columns[4].Width = 100;
            dataGridView1.Columns[5].Width = 100;

            
        }
    }


    public class ReadFromFile
    {
        public  List<Codes> WriteInRAM()
        {
            string[] lines = System.IO.File.ReadAllLines(@"D:\Development\CODES\codes_all.txt");
            string temp = "", temp2 = "", temp3 = "", temp4 = "", temp5 = "";
            char[] symbol = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] array;
            int first=0, last=0, i = 0;
            int isLet = 0;

            List<Codes> code = new List<Codes>();
            Codes obj2 = new Codes();

            foreach (string line in lines)
            {
                if (line.Length > 70)
                {
                    Codes obj = new Codes();
                    if (isLet == 3)
                    {
                        first = 0;
                        last = 0;
                        temp = line.Remove(5, 1);
                        temp = temp.Remove(7, 1);
                        temp = temp.Remove(10, 1);
                        temp2 = temp;
                        temp3 = temp;
                        temp4 = temp;
                        temp = temp.Substring(1, 10);
                        first = temp.IndexOfAny(symbol);
                        last = temp.LastIndexOfAny(symbol);
                        if ((last - first) != 9)                        
                        {
                            obj2.tax = temp5 + line.Substring(56, 17);
                            code.Add(obj2);
                            i++;
                            isLet = 0;
                        }
                        else
                        {
                            isLet = 0;
                        }
                    }
                    if (isLet == 0)
                    {
                        first = 0;
                        last = 0;
                        temp = line.Remove(5, 1);
                        temp = temp.Remove(7, 1);
                        temp = temp.Remove(10, 1);
                        temp2 = temp;
                        temp3 = temp;
                        temp4 = temp;
                        temp = temp.Substring(1, 10);
                        first = temp.IndexOfAny(symbol);
                        last = temp.LastIndexOfAny(symbol);
                        if ((last - first) == 9)                    //если обнаружен 10-значный код
                        {
                            obj.id = i;
                            obj.number = temp;
                            temp2 = temp2.Substring(15, 31);
                            obj.description = temp2;
                            temp3 = temp3.Substring(49, 6);
                            obj.unit = temp3;
                            temp4 = temp4.Substring(56, 16);
                            array = temp4.ToCharArray();
                            for (int j = 0; j < array.Length; j++)      //проверка на наличие букв в строке с пошлиной
                            {
                                if (char.IsLetter(array[j]))
                                {
                                    isLet = 1;
                                    break;
                                }
                            }
                            obj.tax = temp4;
                            obj2 = obj;
                            if (isLet == 0)
                            {
                                code.Add(obj);
                                i++;
                            }
                        } 
                    }
                    if (isLet == 2)
                    {
                        temp5 = temp4 + line.Substring(56, 18);
                        isLet = 3;
                    }
                    if (isLet == 1)     //пропуск строки
                    {
                        isLet = 2;
                    }
                }
            }
            return code;
        }
    }


    public class Codes
    {
        [System.ComponentModel.DisplayName("№")]
        public int id {get; set;}
        [System.ComponentModel.DisplayName("Код ТН ВЭД")]
        public string number {get; set;}
        [System.ComponentModel.DisplayName("Описание")]
        public string description {get; set;}
        [System.ComponentModel.DisplayName("Ед.изм.")]
        public string unit {get; set;}
        [System.ComponentModel.DisplayName("Пошлина, %")]
        public string tax {get; set;}
        [System.ComponentModel.DisplayName("Стоимость")]
        public float cost {get; set;}

        public Codes() 
        {
            id = 0;
            number = "";
            description = "";
            unit = "";
            tax = "";
            cost = 0;
        }
    }
}

