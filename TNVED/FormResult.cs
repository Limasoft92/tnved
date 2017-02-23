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
    public partial class FormResult : Form
    {
        MainForm frm;
        public FormResult()
        {
            InitializeComponent();
        }

        private void FormResult_Load(object sender, EventArgs e)
        {
            frm = (MainForm)this.Owner;
            label1.Text = "База данных успешно обновлена!\n" + 
                "Всего добавлено " + frm.tCode.Count() + " кодов ТН ВЭД\n";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            frm.Saver(frm.tCode);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            frm.tCode.Clear();
            this.Close();
        }
    }
}
