using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace TNVED
{
    public partial class Form1 : Form
    {
        ReadFromFile ob = new ReadFromFile();
        Codes obj1 = new Codes();
        List<Codes> code1 = new List<Codes>();
        List<Codes> code2 = new List<Codes>();
        BindingSource source = new BindingSource();
        double taxSum = 0, sum = 0, taxCost = 0, taxCostNds = 0, taxCostSum = 0, taxCostNdsSum = 0;
        double nds = (double)0.18;
        double tax1 = 0;                                    //первое число в строке

        public Form1()
        {
            InitializeComponent();
            code1 = ob.WriteInRAM();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int ind = 0;
            bool yes = false;
            int eCol = e.ColumnIndex;
            int eRow = e.RowIndex;
            string curCell = "";
            char[] symbol = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            
            try                                            //проверка на ввод букв
            {
                long s = Convert.ToInt64(dataGridView1[eCol, eRow].Value);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Ввод букв не допускается! Пожалуйста, исправьте!");  
            }

            if (dataGridView1[eCol, eRow].Value != null)
            {
                curCell = dataGridView1[eCol, eRow].Value.ToString();
            }

            foreach (Codes c1 in code1)
            {
                if (curCell != null)                        //если текущая ячейка не равна нуля (т.е. изменена)
                {
                    if (c1.number.Equals(curCell))          //если код содержится в файле
                    {
                        yes = true;
                        ind = c1.id;
                        break;
                    }
                }
            }

            if (yes) {                                      //если код содержится в файле
                if (code2[eRow].id == 0)
                {
                    code2[eRow].Copy(code1[ind]);
                    code2[eRow].id = code2.Count;                  
                    code2.Add(new Codes());
                }
                else
                {
                    code2[eRow].Copy(code1[ind]);
                }
                source.ResetBindings(false);         
                if (eCol == 1)                    //подсветка только столбца "Код ТН ВЭД"
                {
                    dataGridView1[eCol, eRow].Style.BackColor = Color.LightGreen;
                }

                dataGridView1.CurrentCell = dataGridView1[eCol, eRow + 1];       //перевод курсора на следующуу строку
            }
            else
            {
                if (eCol == 1)                    //подсветка только столбца "Код ТН ВЭД"
                {
                    dataGridView1[eCol, eRow].Style.BackColor = Color.Tomato;
                }
            }

            sum = 0;
            taxSum = 0;
            taxCostSum = 0;
            taxCostNdsSum = 0;

            string pattern = @"\b((\d+\W+\d)|(\d+))";
            Regex regex = new Regex(pattern);

            foreach (Codes c2 in code2)
            {
                tax1 = 0;
                taxCost = 0;
                taxCostNds = 0;

                if (c2.tax.Contains("не менее"))
                {
                    Match match = regex.Match(c2.tax);
                    tax1 = Double.Parse(match.Groups[1].Value.ToString());
                    sum += c2.cost;
                    taxCost = c2.cost * tax1 / 100;
                    taxCostSum += taxCost;
                    taxCostNds = (c2.cost + taxCost) * nds;
                    taxCostNdsSum += taxCostNds;
                    taxSum += c2.cost + taxCost + taxCostNds;               
                }
                else
                {
                    sum += c2.cost;                                         //общая стоимость груза без пошлин
                    taxCost = c2.cost * Double.Parse(c2.tax) / 100;         //пошлина по коду
                    taxCostSum += taxCost;                                  //сумма пошлины
                    taxCostNds = (c2.cost + taxCost) * nds;                 //НДС по коду
                    taxCostNdsSum += taxCostNds;                            //сумма НДС
                    taxSum += c2.cost + taxCost + taxCostNds;               //общая стоимость с учётом всех пошлин
                }
            }
            textBox1.Text = "Стоимость груза без учёта пошлин составляет " + Convert.ToString(Math.Round(sum, 4)) + " €\r\n" +
                "Сумма пошлины составляет " + Convert.ToString(Math.Round(taxCostSum, 4)) + " €\r\n" +
                   "Сумма НДС составляет " + Convert.ToString(Math.Round(taxCostNdsSum, 4)) + " €\r\n" +
                       "Стоимость груза с учётом всех пошлин составляет " + Convert.ToString(Math.Round(taxSum, 4)) + " €";
            textBox2.Text = Convert.ToString(Math.Round(sum,2)) + " €";
            textBox3.Text = Convert.ToString(Math.Round(taxSum,2)) + " €";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            source.DataSource = code1;
            dataGridView1.DataSource = source;
            code2.Add(new Codes());
            source.ResetBindings(false);
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.Gainsboro;

            dataGridView1.Columns[1].Width = 100;
            ((DataGridViewTextBoxColumn)dataGridView1.Columns[1]).MaxInputLength = 10;

            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[2].Width = 320;
            dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.Gainsboro;

            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[3].Width = 60;
            dataGridView1.Columns[3].DefaultCellStyle.BackColor = Color.Gainsboro;

            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[4].Width = 320;
            dataGridView1.Columns[4].DefaultCellStyle.BackColor = Color.Gainsboro;

            dataGridView1.Columns[5].Width = 100;
            ((DataGridViewTextBoxColumn)dataGridView1.Columns[5]).MaxInputLength = 15;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)           //для исключения ошибки при вводе пустого поля
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value == null)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = (double)0;
            }
        }
    }


    public class ReadFromFile
    {
        public List<Codes> WriteInRAM()
        {
            string[] lines = System.IO.File.ReadAllLines(@"D:\Development\CODES\codes_all.txt");
            string temp = "", temp2 = "", temp3 = "", temp4 = "";
            char[] symbol = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] array;
            char[] probel = {' '};
            int i = 0;
            int isLet = 0, isDig = 0;
            int noDig = 0;
            

            StreamWriter sw = new StreamWriter(@"D:\Development\CODES\log.txt",false);

            List<Codes> code = new List<Codes>();
            Codes obj2 = new Codes();

            foreach (string line in lines)
            {
                string tempRegex = "";
                if (line.Length > 74)
                {
                    Codes obj = new Codes();
                    isDig = 0;
                    temp = line.Substring(0, 15);
                    temp = temp.Replace(" ", string.Empty);
                    temp = temp.Trim('+');
                    temp2 = temp;
                    temp3 = temp;
                    temp4 = temp;
                    array = temp.ToCharArray();
                    tempRegex = Regex.Match(temp, @"\d{10,10}").Value;      //от 10 до 10 цифр последовательно
                    isDig = (tempRegex != "" ? tempRegex.Length : 0);

                    if (isDig != 10 && isLet >= 2 && isLet <= 4)               
                    {
                        obj2.tax += line.Substring(57, 18);
                        obj2.tax = Regex.Replace(obj2.tax, @"\s+", " ");
                        isLet++;
                        continue;
                    }
                    if (isDig == 10 && isLet >= 2)               
                    {
                        string tempTax1 = "";
                        obj2.tax = Regex.Match(obj2.tax, @"[0-9]{1,}[^&]{1,}\b(кг|шт|л|м2|м3|см2|см3|пару|т)").Value;
                        obj2.tax1 = Double.Parse(Regex.Replace(Regex.Match(obj2.tax, @"[^а-я|А-Я|\s]{1,}").Value, @"[,]$", ""));
                        obj2.unit2 = Regex.Match(obj2.tax, @"\b(кг|шт|л|м2|см3|пару)").Value;
                        code.Add(obj2);
                        sw.WriteLine(obj2.tax);
                        isLet = 0;
                        i++;
                    }
                    if (isDig == 10)                                //если обнаружен 10-значный код
                    {
                        isLet = 0;
                        obj.id = i;
                        obj.number = temp;
                        temp2 = line.Substring(16, 32);
                        obj.description = Regex.Replace(temp2, @"\s+", " ");
                        if (Regex.IsMatch(obj.description, @"\b([И|и]сключен)[. ]"))    //пропуск исключённых кодов
                        {
                            //sw.WriteLine(obj.description);
                            continue;
                        }
                        temp3 = line.Substring(50, 7);
                        obj.unit = Regex.Replace(temp3, @"\s+", " ");
                        temp4 = line.Substring(57, 18);
                        array = temp4.ToCharArray();
                        if (Regex.IsMatch(temp4, @"[0-9]{1,}"))
                        {
                            if (Regex.IsMatch(temp4, @"[а-я|А-Я]{2,}"))
                            {
                                isLet = 2;
                            }
                        }
                        else
                        {
                            continue;
                        }
                        obj.tax = Regex.Replace(temp4, @"\s+", " ");                                         
                        obj2 = obj;
                        if (isLet <= 1)                             //если поле "Пошлина" без букв 
                        {
                            obj.tax1 = Double.Parse(Regex.Match(obj.tax, @"\b(\d+)").Value);
                            code.Add(obj);
                            i++;
                        }
                    }
                }
            }
            sw.Close();
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
        public double cost {get; set;}
        [System.ComponentModel.DisplayName("Количество")]
        public double amount { get; set; }                          //вес, шт, л
        public double tax1 { get; set; }                            //первое число
        public double tax2 { get; set; }                            //второе число
        public string unit2 { get; set; }                           //Дополнительные ед.изм.
        public byte taxType { get; set }                            //"...но не менее..."|"...евро(долларов) за..."|"...плюс..."



        public Codes() 
        {
            id = 0;
            number = "";
            description = "";
            unit = "";
            tax = "0";
            cost = 0;
        }

        public void Copy (Codes o)
        {
            number = o.number;
            description = o.description;
            unit = o.unit;
            tax = o.tax;
        }
    }
}

