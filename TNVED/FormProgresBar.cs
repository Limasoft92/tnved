using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNVED
{
    public partial class FormProgresBar : Form
    {
        MainForm frm;
        public FormProgresBar()
        {
            InitializeComponent();
        }

        private void FormProgresBar_Load(object sender, EventArgs e)
        {
            frm = (MainForm)this.Owner;
            label1.Text = "Добавлено " + frm.tCode.Count() + " кодов ТН ВЭД";
            progressBar1.Maximum = frm.m;
            progressBar1.Value = frm.k;
            Task ts = Task.Factory.StartNew(UpdateBar).ContinueWith((t) => { buttonOk.Visible = true; }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateBar()                                                                    //обновление содержимого окна
        {
            while (frm.k != frm.m)
            {
                System.Threading.Thread.Sleep(50);
                this.Invoke((MethodInvoker)delegate
                {
                    label1.Text = "Добавлено " + frm.tCode.Count() + " кодов ТН ВЭД";
                    progressBar1.Maximum = frm.m;
                    progressBar1.Value = frm.k;
                });   
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
