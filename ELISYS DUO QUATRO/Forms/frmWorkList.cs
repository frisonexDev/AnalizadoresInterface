using Galileo.Connect.Model;
using Galileo.Online.Helper;
using Galileo.Utils.ASTMModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galileo.Online.Forms
{
    public partial class frmWorkList : Form
    {
        public frmWorkList()
        {
            InitializeComponent();
        }

        private void frmWorkList_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //((MainForm)this.Owner).insMngr.GetOrden(dateTimeDesde.Value, dateTimeHasta.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // ((MainForm)this.Owner).insMngr.GetOrden(txtLabel.Text);
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var result = ((MainForm)this.Owner).insMngr.GetOrden(dateStart.Value, dateEnd.Value);
            dataGridView1.DataSource = result;
            dataGridView1.Columns[10].Visible = false;
        }

        private void btnPrintLabels_Click(object sender, EventArgs e)
        {

           

            foreach ( DataGridViewRow item in dataGridView1.Rows)
            {
                bool selected = item.Cells[0].Selected;
                string name = item.Cells[3].FormattedValue.ToString() + " " + item.Cells[2].FormattedValue.ToString();
                string ident = "1111";
                string tests = item.Cells[8].FormattedValue.ToString();
                string orden= item.Cells[1].FormattedValue.ToString();


                if (selected)
                {
                    string str = "^XA\"\n";
                 
                    str += "^FO250, 20^ADN, 11, 7^FD" + name + "^FS\"\n";
                    str += "^FO320, 40^ADN, 5, 3^FD ID:" + ident +" " + tests +" ^FS\"\n";
                    //str += "^FO30, 150^ADN, 11, 7^FD Texto de muestra 1 ^FS\"\n";
                    str += "^FO450, 80^ADN, 11, 7\"\n";
                    str += "^BCN, 80, Y, N, N^FD"  + orden + "^FS\"\n";
                    str += "^XZ\"\n";
                    RawPrinterHelper.SendStringToPrinter("Zebra", str);
                }
            }

            







           
        }

    
    }
}
