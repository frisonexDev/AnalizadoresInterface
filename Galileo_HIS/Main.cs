using Galileo.Connect.Manager;
using Galileo.Connect.Model;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galileo_HIS
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnLoadSample_Click(object sender, EventArgs e)
        {

            string file = Environment.CurrentDirectory + "\\SampleData\\Ejemplo.json";

            var fileStream = new FileStream(file,FileMode.Open);
            

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var data = streamReader.ReadToEnd();
                OrdenGalileo orden = JsonConvert.DeserializeObject<OrdenGalileo>(data);
                HISManager h = new HISManager("NqvtSj3vhSx4FkWFNObJJy67JLJQD9QG");

                //MessageBox.Show(h.PostOrden(orden).ToString());


            }


           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HISManager h = new HISManager("NqvtSj3vhSx4FkWFNObJJy67JLJQD9QG");
            var results = h.GetResultados(dateStart.Value, dateEnd.Value, 3, "1");

            List<ResultadosACK> acks = new List<ResultadosACK>();

            foreach (var item in results.ordenes)
            {
                foreach (var det in item.details)
                {
                    foreach (var r in det.envioResultado)
                    {
                        ResultadosACK ack = new ResultadosACK();
                        ack.idLaboratorio = 3;
                        ack.opcion = 1;
                        ack.idResultados = r.idResultado;
                        acks.Add(ack);
                    }
                   
                    
                }
            }

            h.ConfirmarResultado(acks);

            
        }
    }
}
