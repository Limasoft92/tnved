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
        double taxSum = 0, sum = 0, taxCost = 0, taxCostNds = 0, taxCostSum = 0, taxCostNdsSum = 0, taxCost2 = 0;
        double nds = 0.18, convEurUsd = 1.043;

        DataGridViewComboBoxColumn column;

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
                if (curCell != null)                        //если текущая ячейка не равна нулю (т.е. изменена)
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

            foreach (Codes c2 in code2)
            {
                taxCost = 0;
                taxCostNds = 0;
                switch (c2.type)
                {
                    case 0:
                        sum += c2.cost;                                         //общая стоимость груза без пошлин
                        taxCost = c2.cost * c2.tax1 / 100;                      //пошлина по коду
                        taxCostSum += taxCost;                                  //сумма пошлины
                        taxCostNds = (c2.cost + taxCost) * nds;                 //НДС по коду
                        taxCostNdsSum += taxCostNds;                            //сумма НДС
                        taxSum += c2.cost + taxCost + taxCostNds;               //общая стоимость с учётом всех пошлин
                        break;
                    case 1:
                        sum += c2.cost;
                        taxCost = c2.cost * c2.tax1 / 100;
                        if ((c2.tax3 == 1 && c2.unit2.Equals("т")) || (c2.tax3 == 1000 && c2.unit2.Equals("кг")))
                        {
                            taxCost2 = c2.amount * c2.tax2 / 1000;               //пошлина по коду исходя из количества
                        }
                        else
                        {
                            taxCost2 = c2.amount * c2.tax2;
                        }
                        if (taxCost < taxCost2)
                        {
                            taxCost = taxCost2;
                        }
                        taxCostSum += taxCost;
                        taxCostNds = (c2.cost + taxCost) * nds;
                        taxCostNdsSum += taxCostNds;
                        taxSum += c2.cost + taxCost + taxCostNds;
                        break;
                    case 2:
                        sum += c2.cost;
                        if ((c2.tax3 == 1 && c2.unit2.Equals("т")) || (c2.tax3 == 1000 && c2.unit2.Equals("кг")))
                        {
                            taxCost = c2.amount * c2.tax2 / 1000;                     //пошлина по коду исходя из количества
                        }
                        else
                        {
                            taxCost = c2.amount * c2.tax2;
                        }
                        taxCostSum += taxCost;
                        taxCostNds = (c2.cost + taxCost) * nds;
                        taxCostNdsSum += taxCostNds;
                        taxSum += c2.cost + taxCost + taxCostNds;
                        break;
                    case 4:
                        sum += c2.cost;
                        if ((c2.tax3 == 1 && c2.unit2.Equals("т")) || (c2.tax3 == 1000 && c2.unit2.Equals("кг")))
                        {
                            taxCost = c2.amount * c2.tax2 / convEurUsd / 1000;        //пошлина по коду исходя из количества
                        }
                        else
                        {
                            taxCost = c2.amount * c2.tax2 / convEurUsd;
                        }
                        taxCostSum += taxCost;
                        taxCostNds = (c2.cost + taxCost) * nds;
                        taxCostNdsSum += taxCostNds;
                        taxSum += c2.cost + taxCost + taxCostNds;
                        break;
                    case 8:
                        sum += c2.cost;
                        taxCost = (c2.cost * c2.tax1 / 100) + (c2.amount * c2.tax2);
                        taxCostSum += taxCost;
                        taxCostNds = (c2.cost + taxCost) * nds;
                        taxCostNdsSum += taxCostNds;
                        taxSum += c2.cost + taxCost + taxCostNds;
                        break;
                }
                if (c2.id != 0)
                {
                    if (code2[c2.id - 1].type != 0)                                                       //разблокировка поля "Количество"
                    {
                        dataGridView1[6, c2.id - 1].ReadOnly = false;
                        dataGridView1[6, c2.id - 1].Style.BackColor = Color.White;
                    }
                    else
                    {
                        dataGridView1[6, c2.id - 1].ReadOnly = true;
                        dataGridView1[6, c2.id - 1].Style.BackColor = Color.Gainsboro;
                    }
                }
            }
            textBox1.Text = " Стоимость груза без учёта таможенных платежей " + Convert.ToString(Math.Round(sum, 4)) + " €\r\n" +
                " Сумма таможенной пошлины " + Convert.ToString(Math.Round(taxCostSum, 4)) + " €\r\n" +
                   " Сумма налога на добавленную стоимость (НДС) " + Convert.ToString(Math.Round(taxCostNdsSum, 4)) + " €\r\n" +
                     " Окончательная сумма таможенных платежей " + Convert.ToString(Math.Round((taxCostSum + taxCostNdsSum), 4)) + " €\r\n" +
                       " Стоимость груза с учётом таможенных платежей " + Convert.ToString(Math.Round(taxSum, 4)) + " €";
            textBox2.Text = Convert.ToString(Math.Round(sum,2)) + " €";
            textBox3.Text = Convert.ToString(Math.Round(taxSum,2)) + " €";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            source.DataSource = code2;
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

            dataGridView1.Columns[6].ReadOnly = true;
            dataGridView1.Columns[6].DefaultCellStyle.BackColor = Color.Gainsboro;
            ((DataGridViewTextBoxColumn)dataGridView1.Columns[6]).MaxInputLength = 15;

            dataGridView1.Columns[7].Visible = false;
            dataGridView1.Columns[8].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[10].Visible = false;
            dataGridView1.Columns[11].Visible = false;
            dataGridView1.Columns[12].Visible = false;


            //column = new DataGridViewComboBoxColumn();
            //column.HeaderText = "Колонка";
            //column.MaxDropDownItems = 5;
            //column.FlatStyle = FlatStyle.Flat;
            ////column.DisplayStyleForCurrentCellOnly = true;
            //column.DisplayIndex = 77;
            

            //foreach (Codes c1 in code1)
            //{
            //    column.Items.AddRange(c1.number);
            //}
            //dataGridView1.Columns.Add(column);


        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)           //для исключения ошибки при вводе пустого поля
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value == null)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = (double)0;
            }
        }

        //private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    if (e.ColumnIndex == column.DisplayIndex)
        //    {
        //        if (!this.column.Items.Contains(e.FormattedValue))
        //        {
        //            column.Items.Add(e.FormattedValue);
        //        }
        //    }
        //}

        //private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    if (dataGridView1.CurrentCellAddress.X == column.DisplayIndex)
        //    {
        //        ComboBox cb = e.Control as ComboBox;
        //        if (cb != null)
        //        {
        //            cb.DropDownStyle = ComboBoxStyle.DropDown;
        //        }
        //    }
        //}

        //private void dataGridView1_keyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (dataGridView1.CurrentCell.Value.ToString() != "6")
        //    {
        //        MessageBox.Show("Вправьте!");
        //    }
        //}
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
                        string tempType = "";
                        obj2.tax = Regex.Match(obj2.tax, @"[0-9]{1,}[^&]{1,}\b(кг|шт|л|м2|м3|см2|см3|пару|т)").Value;
                        obj2.tax1 = Double.Parse(Regex.Replace(Regex.Match(obj2.tax, @"[^а-я|А-Я|\s]{1,}").Value, @"[,]$", ""));
                        tempType = Regex.Match(obj2.tax, @"(но не мене|плюс|евро за|евр за|доллар)").Value;
                        switch (tempType)
                        {
                            case ("но не мене"):
                                obj2.type = 1;
                                break;
                            case ("евро за"):
                                obj2.type = 2;
                                break;
                            case ("евр за"):
                                obj2.type = 2;
                                break;
                            case ("доллар"):
                                obj2.type = 4;
                                break;
                            case ("плюс"):
                                obj2.type = 8;
                                break;
                        }
                        //sw.WriteLine(tempType);
                        obj2.tax2 = Double.Parse(Regex.Replace(Regex.Match(obj2.tax, @"[0-9,]{1,}\s(евро|евр|доллар|за)").Value, @"[,]?\s(евро|евр|доллар|за)", ""));
                        obj2.currency = Regex.Replace(Regex.Match(obj2.tax, @"\b(евро|евр|доллар)").Value, @"(евр)\b", "евро");
                        obj2.tax3 = int.Parse(Regex.Replace(Regex.Match(obj2.tax, @"\s\d+\s(кг|шт|л|м2|м3|см2|см3|пару|т)").Value, @"(кг|шт|л|м2|м3|см2|см3|пару|т)", ""));
                        obj2.unit2 = Regex.Match(obj2.tax, @"\s(кг|шт|л|м2|м3|см2|см3|пару|т)").Value;
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
                            obj.tax1 = Double.Parse(Regex.Match(obj.tax, @"(\d+[,]\d+)|\d+").Value);
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
        public byte type { get; set; }                              //"...но не менее..."|"...евро(долларов) за..."|"...плюс..."
        public double tax2 { get; set; }                            //второе число
        public string currency { get; set; }                        //валюта
        public int tax3 { get; set; }                               //третье число
        public string unit2 { get; set; }                           //Дополнительные ед.изм.



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
            tax1 = o.tax1;
            tax2 = o.tax2;
            tax3 = o.tax3;
            type = o.type;
            currency = o.currency;
            unit2 = o.unit2;

        }
    }
}

