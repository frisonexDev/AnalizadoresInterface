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
    public partial class frmEnviarResultado : Form
    {
        public frmEnviarResultado()
        {
            InitializeComponent();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            //((MainForm)this.Owner).insMngr.PostResultado(txtOrden.Text, comboBoxPruebas.Text, txtResultado.Text, chkValidado.Checked);
        }

        private void frmEnviarResultado_Load(object sender, EventArgs e)
        {
            var instrumento = ((MainForm)this.Owner).insMngr.Instrumento;

            comboBoxPruebas.Items.AddRange(instrumento.DetallesInstrumento.ToArray());
            comboBoxPruebas.ValueMember = "Homologacion";
            comboBoxPruebas.ValueMember = "Examen";

        }
    }
}
