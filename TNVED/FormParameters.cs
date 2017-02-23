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
    public partial class FormParameters : Form
    {
        MainForm frm;
        double nds, conv;
        public FormParameters(Form fp)
        {
            InitializeComponent();
            frm = (MainForm)fp;
        }

        private void FormParameters_Load(object sender, EventArgs e)
        {
            nds = frm.Nds;
            conv = frm.Conv;
            Copy();           
            frm.Update();
        }

        private void Copy()
        {
            try
            {
                textBoxNDS.Text= nds.ToString();
                textBoxConv.Text = conv.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка!\n", e.ToString());
                throw;
            }
        }

        private void textBoxNDS_TextChanged(object sender, EventArgs e)
        {
            textBoxNDS.Text = textBoxNDS.Text.Replace('.', ',');
            textBoxNDS.Select(textBoxNDS.Text.Length, textBoxNDS.Text.Length);                            //перемещение курсора в конец строки
        }

        private void textBoxConv_TextChanged(object sender, EventArgs e)
        {
            textBoxConv.Text = textBoxConv.Text.Replace('.', ',');
            textBoxConv.Select(textBoxConv.Text.Length, textBoxConv.Text.Length);                         //перемещение курсора в конец строки
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try                                                                                           //проверка на ввод букв
            {
                nds = double.Parse(textBoxNDS.Text);
                conv = double.Parse(textBoxConv.Text);
                frm.SaverParam(nds, conv);
                this.Close();                
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Ввод букв не допускается! Пожалуйста, исправьте!", "Недопустимые символы", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxNDS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                buttonSave.Focus();
            }
        }

        private void textBoxConv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                buttonSave.Focus();
            }
        }
    }
}
