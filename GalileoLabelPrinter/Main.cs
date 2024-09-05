using Galileo.Connect.Manager;
using Galileo.Online.Helper;
using Galileo.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;


namespace GalileoLabelPrinter
{
    public partial class Main : Form
    {
        private Connection printerConnection;
        private ZebraPrinter printer;

        public string ServerLIS { get { return ConfigurationManager.AppSettings["ServerLIS"]; } }
        public string License { get { return ConfigurationManager.AppSettings["License"]; } }

        public string PrintMode { get { return ConfigurationManager.AppSettings["PrintMode"]; } }

        public string PrinterName { get { return ConfigurationManager.AppSettings["PrinterName"]; } }
        public string PrinterIP { get { return ConfigurationManager.AppSettings["PrinterIP"]; } }
        public string SeparadorMuestra { get { return ConfigurationManager.AppSettings["SeparadorMuestra"]; } }

        List<Galileo.Connect.Model.Label> labels;

        public int IdLaboratorio;
        public string IdInstrumento;
        public string ApiToken;
        public EncryptHelper enc;
        public InstrumentManager insMngr;

        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            enc = new EncryptHelper();
           

            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            string[] licenceDecrypt = enc.DecryptString(key, License).Split(";");

            IdLaboratorio = Convert.ToInt32(licenceDecrypt[1]);
            IdInstrumento = licenceDecrypt[0];
            ApiToken = licenceDecrypt[2];

            insMngr = new InstrumentManager(ServerLIS, IdInstrumento, IdLaboratorio, SeparadorMuestra, ApiToken);


        }

        private string GetTemplate()
        {
            try
            {
                string rutaEjecucion = Environment.CurrentDirectory + "\\Configs\\";
                string rutaArchivo = rutaEjecucion +  "LabelDesign.txt";

                // Verificar si el archivo existe
                if (File.Exists(rutaArchivo))
                {
                    // Crear un objeto StreamReader para leer el archivo
                    using (StreamReader lector = new StreamReader(rutaArchivo))
                    {
                        return lector.ReadToEnd();
                       
                    }
                }
                else
                {
                    return "Error: " + "El archivo no existe";
                }
            }
            catch (Exception ex)
            {
               
                return "Error: " + ex.Message;
            }
        }

        private static void PrintRaw(string template, string name)
        {
            string zplData = template;


            RawPrinterHelper.SendStringToPrinter(name, zplData);
        }

        private void PrintTCP(string template)
        {

            printerConnection = new TcpConnection(PrinterIP, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            try
            {
                printerConnection.Open();
                printer = ZebraPrinterFactory.GetInstance(printerConnection);
                MessageBox.Show("Conexión exitosa con la impresora Zebra.");
            }
            catch (ConnectionException ex)
            {
                MessageBox.Show("Error al conectar con la impresora Zebra: " + ex.Message);
            }


            if (printer != null && printerConnection != null && printerConnection.Connected)
            {
                // Crear la plantilla ZPL (sustituye estos datos con los tuyos)
                string zplData = template;

                try
                {
                    // Enviar la plantilla ZPL a la impresora para imprimir la etiqueta
                    printerConnection.Write(Encoding.Default.GetBytes(zplData));
                }
                catch (ConnectionException ex)
                {
                    MessageBox.Show("Error al imprimir la etiqueta: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Conéctate a una impresora Zebra primero.");
            }
        }

        private void ConnectTCPPrinter()
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string template = GetTemplate();
            PrintRaw(template, "ZDesigner GC420d (EPL)");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (txtOrderFrom.Text.Length <= 5)
            {
                txtOrderFrom.Text = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Day.ToString().PadLeft(2, Convert.ToChar("0")) + txtOrderFrom.Text.PadLeft(4, Convert.ToChar("0"));
            }

            if (txtOrderTo.Text.Length <= 5)
            {
                txtOrderTo.Text = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Day.ToString().PadLeft(2, Convert.ToChar("0")) + txtOrderTo.Text.PadLeft(4, Convert.ToChar("0"));
            }


            labels =   insMngr.GetMuestrasPorOrden(txtOrderFrom.Text, txtOrderTo.Text);

            var muestras = labels.Select(p => p.SampleName).Distinct();

            chkSampleList.Items.Clear();
            foreach (var m in muestras)
            {
                chkSampleList.Items.Add(m, true);
            }





        }

        public  string PopulateTemplate(string inputString, object obj)
        {
            string outputString = inputString;
            Type objectType = obj.GetType();
            PropertyInfo[] properties = objectType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                string propertyValue = property.GetValue(obj)?.ToString();

                if (propertyValue != null)
                {
                    // Utilizar una expresión regular para buscar todas las coincidencias del nombre de propiedad entre corchetes [nombrePropiedad]
                    string pattern = @"\[" + propertyName + @"\]";
                    outputString = Regex.Replace(outputString, pattern, propertyValue, RegexOptions.IgnoreCase);
                }
            }

            return outputString;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            string template = GetTemplate();
            string printerName = PrinterName;


            foreach (var item in labels)
            {
                var selected = chkSampleList.GetItemChecked(chkSampleList.Items.IndexOf(item.SampleName));
                if (selected)
                    PrintRaw(PopulateTemplate(template, item), printerName);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string template = GetTemplate();
            PrintRaw(template, "ZDesigner GC420d (EPL)");
        }
    }
}
