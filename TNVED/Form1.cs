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
using System.Threading;

namespace TNVED
{
    public partial class Form1 : Form
    {
        ReadFromFile ob = new ReadFromFile();
        Codes obj1 = new Codes();
        List<Codes> code1 = new List<Codes>();
        List<Codes> code2 = new List<Codes>();
        public List<Codes> tCode = new List<Codes>();
        public int k = 0, m = 0, n = 0;
        BindingSource source = new BindingSource();
        double taxSum = 0, sum = 0, taxCost = 0, taxCostNds = 0, taxCostSum = 0, taxCostNdsSum = 0, taxCost2 = 0;
        double convEurUsd = 1.043;

        public Form1()
        {
            InitializeComponent();
            OpenFromFile();
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

            if (yes)
            {                                      //если код содержится в файле
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
                        taxCostNds = (c2.cost + taxCost) * c2.nds / 100;                 //НДС по коду
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
                        taxCostNds = (c2.cost + taxCost) * c2.nds / 100;
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
                        taxCostNds = (c2.cost + taxCost) * c2.nds / 100;
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
                        taxCostNds = (c2.cost + taxCost) * c2.nds / 100;
                        taxCostNdsSum += taxCostNds;
                        taxSum += c2.cost + taxCost + taxCostNds;
                        break;
                    case 8:
                        sum += c2.cost;
                        taxCost = (c2.cost * c2.tax1 / 100) + (c2.amount * c2.tax2);
                        taxCostSum += taxCost;
                        taxCostNds = (c2.cost + taxCost) * c2.nds / 100;
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
            label4.Text = " Стоимость груза без учёта таможенных платежей " + Convert.ToString(Math.Round(sum, 4)) + " €\r\n" +
                " Сумма таможенной пошлины составляет " + Convert.ToString(Math.Round(taxCostSum, 4)) + " €\r\n" +
                   " Сумма НДС составляет " + Convert.ToString(Math.Round(taxCostNdsSum, 4)) + " €\r\n" +
                       " Окончательная сумма таможенных платежей " + Convert.ToString(Math.Round((taxCostSum + taxCostNdsSum), 4)) + " €\r\n" +
                           " Стоимость груза с учётом всех таможенных платежей " + Convert.ToString(Math.Round(taxSum, 4)) + " €";
            label5.Text = Convert.ToString(Math.Round(sum, 2)) + " €";
            label6.Text = Convert.ToString(Math.Round(taxSum, 2)) + " €";
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
            dataGridView1.Columns[2].Width = 415;
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

            dataGridView1.Columns[13].Width = 60;
            dataGridView1.Columns[13].DefaultCellStyle.BackColor = Color.Gainsboro;

            labelHelp.Text = (code1.Count() < 10000) ? "Обновите базу данных": "";                      //подсказка (краткая форма if)
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)           //для исключения ошибки при вводе пустого поля
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value == null)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = (double)0;
            }
        }

        private void MenuFileUpdate_Click(object sender, EventArgs e)                                   //Контекстное меню
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult dr = new DialogResult();
            dlg.Filter = "Текстовый файл|*.txt";
            dlg.Title = "Выберите текстовый файл с кодами ТН ВЭД";
            dr = dlg.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                ob.WriteInRAM(tCode, dlg.FileName, ref k, ref m);
                Form2 f2 = new Form2();
                f2.Owner = this;
                f2.ShowDialog();
                dlg.Title = "Выберите текстовый файл с кодами ТН ВЭД, для которых НДС равно 10%";
                dr = dlg.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    Thread th = new Thread(() => ob.EditNDS(tCode, dlg.FileName, ref k, ref n));        //отдельный поток
                    th.Start();
                    Form3 f3 = new Form3();
                    f3.Owner = this;
                    f3.ShowDialog();
                    //if (!th.IsAlive)
                    //{
                    //    th.Join();
                    //}
                }
            }
            Form4 f4 = new Form4();
            f4.Owner = this;
            f4.ShowDialog();
        }

        public void Saver(List<Codes> code)                                               //сохранение изменений
        {
            BinaryWriter bw = new BinaryWriter(File.Open("C:\\db.dat", FileMode.Create));
            code1 = code.ToList();
            bw.Write(code1.Count);
            foreach (Codes c1 in code1)
            {
                bw.Write(c1.id);
                bw.Write(c1.number.Length);
                bw.Write(c1.number);
                bw.Write(c1.description.Length);
                bw.Write(c1.description);
                bw.Write(c1.unit.Length);
                bw.Write(c1.unit);
                bw.Write(c1.tax.Length);
                bw.Write(c1.tax);
                bw.Write(c1.cost);
                bw.Write(c1.amount);
                bw.Write(c1.tax1);
                bw.Write(c1.type);
                bw.Write(c1.tax2);
                bw.Write(c1.currency.Length);
                bw.Write(c1.currency);
                bw.Write(c1.tax3);
                bw.Write(c1.unit2.Length);
                bw.Write(c1.unit2);
                bw.Write(c1.nds);
            }
            bw.Write(code1[0].description);
            labelHelp.Text = (code1.Count() < 10000) ? "Обновите базу данных" : "";       //подсказка (краткая форма if)
            code.Clear();
        }

        private void OpenFromFile()
        {
            using (var br = new BinaryReader(File.Open("C:\\db.dat", FileMode.Open, FileAccess.Read)))
            {
                int amount = 0;
                int counts = br.ReadInt32();

                for (int i = 0; i < counts; i++)
                {
                    code1.Add(new Codes());
                    code1[i].id = br.ReadInt32();
                    amount = br.ReadInt32();
                    code1[i].number = br.ReadString();
                    amount = br.ReadInt32();
                    code1[i].description = br.ReadString();
                    amount = br.ReadInt32();
                    code1[i].unit = br.ReadString();
                    amount = br.ReadInt32();
                    code1[i].tax = br.ReadString();
                    code1[i].cost = br.ReadDouble();
                    code1[i].amount = br.ReadDouble();
                    code1[i].tax1 = br.ReadDouble();
                    code1[i].type = br.ReadByte();
                    code1[i].tax2 = br.ReadDouble();
                    amount = br.ReadInt32();
                    code1[i].currency = br.ReadString();
                    code1[i].tax3 = br.ReadInt32();
                    amount = br.ReadInt32();
                    code1[i].unit2 = br.ReadString();
                    code1[i].nds = br.ReadDouble();
                }
            }
        }

        //public List<Codes> Upd1()                                                       //обновление кодов
        //{

        //}
        //public List<Codes> Upd2()                                                       //обновление исключений с НДС 10%
        //{

        //}
    }

    public class ReadFromFile
    {
        public List<Codes> WriteInRAM(List<Codes> code, string dir1, ref int n, ref int m)
        {
            string[] lines = System.IO.File.ReadAllLines(dir1);
            m = lines.Count();
            string temp = "", temp2 = "", temp3 = "", temp4 = "";
            char[] symbol = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] array;
            char[] probel = {' '};
            int i = 0;
            int isLet = 0, isDig = 0;
            n = 0;
            code.Clear();
            //StreamWriter sw = new StreamWriter(@"D:\Development\CODES\log.txt",false);
            //List<Codes> code = new List<Codes>();
            Codes obj2 = new Codes();

            foreach (string line in lines)
            {
                n++;
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
                        obj2.tax2 = Double.Parse(Regex.Replace(Regex.Match(obj2.tax, @"[0-9,]{1,}\s(евро|евр|доллар|за)").Value, @"[,]?\s(евро|евр|доллар|за)", ""));
                        obj2.currency = Regex.Replace(Regex.Match(obj2.tax, @"\b(евро|евр|доллар)").Value, @"(евр)\b", "евро");
                        obj2.tax3 = int.Parse(Regex.Replace(Regex.Match(obj2.tax, @"\s\d+\s(кг|шт|л|м2|м3|см2|см3|пару|т)").Value, @"(кг|шт|л|м2|м3|см2|см3|пару|т)", ""));
                        obj2.unit2 = Regex.Match(obj2.tax, @"\s(кг|шт|л|м2|м3|см2|см3|пару|т)").Value;
                        code.Add(obj2);
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
                        obj.nds = 18;                
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
            //sw.Close();
            return code;
        }
        public List<Codes> EditNDS(List<Codes> code, string dir2, ref int k, ref int n)
        {
            k = 0;
            n = 0;
            string[] excep = System.IO.File.ReadAllLines(dir2);
            foreach (Codes l in code)
            {
                n++;
                foreach (string exc in excep)               //определение НДС из файла исключений
                {
                    if (Regex.IsMatch(l.number, @"\b" + exc + @"\d*"))
                    {
                        l.nds = 10;
                        k++;
                        break;
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
        public double cost {get; set;}
        [System.ComponentModel.DisplayName("Количество")]
        public double amount { get; set; }                          //вес, шт, л
        public double tax1 { get; set; }                            //первое число
        public byte type { get; set; }                              //"...но не менее..."|"...евро(долларов) за..."|"...плюс..."
        public double tax2 { get; set; }                            //второе число
        public string currency { get; set; }                        //валюта
        public int tax3 { get; set; }                               //третье число
        public string unit2 { get; set; }                           //дополнительные ед.изм.
        [System.ComponentModel.DisplayName("НДС,%")]
        public double nds { get; set; }                             //НДС



        public Codes() 
        {
            id = 0;
            number = "";
            description = "";
            unit = "";
            tax = "0";
            cost = 0;
            currency = "";
            unit2 = "";
            nds = 0;
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
            nds = o.nds;
        }
    }
}

