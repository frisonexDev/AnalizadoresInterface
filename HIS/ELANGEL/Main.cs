using Apitron.PDF.Rasterizer;
using Galileo.Connect.Manager;
using Galileo.Connect.Model;
using Galileo.Connect.Model.Resultados;
using Galileo.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Galileo_HIS
{
    public partial class Main : Form
    {
        public string ServerLIS { get { return ConfigurationManager.AppSettings["ServerLIS"]; } }

        public string ServerSeguridad { get { return ConfigurationManager.AppSettings["ServerSeguridad"]; } }


        public string Instrumento { get { return ConfigurationManager.AppSettings["Instrumento"]; } }
        public string InstrumentoImg { get { return ConfigurationManager.AppSettings["Imagen"]; } }

        public string ConnectionMode { get { return ConfigurationManager.AppSettings["ConnectionMode"]; } }

        public string ConnectionSetup { get { return ConfigurationManager.AppSettings["ConnectionSetup"]; } }

        public string InstrumentIP { get { return ConfigurationManager.AppSettings["InstrumentIP"]; } }
        public string LocalPort { get { return ConfigurationManager.AppSettings["LocalPort"]; } }

        public string COM { get { return ConfigurationManager.AppSettings["COM"]; } }
        public string Bauds { get { return ConfigurationManager.AppSettings["Bauds"]; } }
        public string DataBits { get { return ConfigurationManager.AppSettings["DataBits"]; } }

        public string Protocol { get { return ConfigurationManager.AppSettings["Protocol"]; } }

        public string ProcessFolder { get { return ConfigurationManager.AppSettings["ProcessFolder"]; } }
        public string WorkListFolder { get { return ConfigurationManager.AppSettings["WorkListFolder"]; } }

        public string ProcessedFolder { get { return ConfigurationManager.AppSettings["ProcessedFolder"]; } }

        public string License { get { return ConfigurationManager.AppSettings["License"]; } }

        public string SeparadorMuestra { get { return ConfigurationManager.AppSettings["SeparadorMuestra"]; } }

        public string OrderField { get { return ConfigurationManager.AppSettings["OrderField"]; } }

        public string TestName { get { return ConfigurationManager.AppSettings["TestName"]; } }

        public string TestValue { get { return ConfigurationManager.AppSettings["TestValue"]; } }

        public string RutaDestino { get { return ConfigurationManager.AppSettings["RutaReportes"]; } }
        public string fileGlobal;

        public int IdLaboratorio;
        public string IdInstrumento;
        public string ApiToken;
        public string SerialMessage;

      


        public enum LogEvent
        {
            Recibiendo,
            Enviando,
            Error,
            Esperando,
            Conectado,
            Desconectado
        }



        public LogHelper log;
        public CaptureHelper cap;
        public EncryptHelper enc;
        public FileHelper fil;
        public InstrumentManager insMngr;
        public Main()
        {
            InitializeComponent();
        }
        
        private void btnLoadSample_Click(object sender, EventArgs e)
        {
            
            HISManager h = new HISManager(ApiToken);

            int valMaximoProgreso = dataGridView1.Rows.Count;
            pgnOrdenes.Maximum =  valMaximoProgreso;
            string Area = "";
            //PARSEAR EL DATAGRID
            foreach (DataGridViewRow ordenes in dataGridView1.Rows)
            {
                //CREAR PDF DE LA ORDEN
                String numeroOrden = (string)ordenes.Cells[2].Value;
                String pacienteNombres = (string)ordenes.Cells[0].Value + " " + (string)ordenes.Cells[1].Value;
                DateTime fechaRegistro = (DateTime)ordenes.Cells[4].Value;
                String pacienteEdad = (string)ordenes.Cells[6].Value;
                String pacienteEstado = (string)ordenes.Cells[5].Value;
                
                //INFORMACIÓN ADICIONAL ORDEN
                var sede = h.ListarDatosAdicionales(numeroOrden, IdLaboratorio);
                String laboratorioDesc = sede.NombreLaboratorio;
                String sedeDesc = sede.NombreSede;
                String generoPaciente = sede.GenerodelPaciente;
                string nombreMedico = "";
                string telefonoMedico = "";
                string correoMedico = "";
                if (sede.IdMedico != 0)
                {
                    var medico = h.datosMedico(sede.IdMedico, IdLaboratorio);
                    nombreMedico = medico.Medico[0].Details[0].Apellido ?? "";
                    nombreMedico = nombreMedico + " ";
                    nombreMedico = nombreMedico + medico.Medico[0].Details[0].Nombre ?? "";
                    telefonoMedico = medico.Medico[0].Details[0].Telefono ?? "-";
                    correoMedico = medico.Medico[0].Details[0].Email ?? "-";
                }

                var result = h.ListarResultados(numeroOrden, IdLaboratorio);
                string origen = sede.NombreSede;
                bool todos = false;
                bool validados = chkValidados.Checked;
                if (!validados)
                {
                    todos = true;
                }
                PDF_MSP pFile = new PDF_MSP();
                pFile.filename = fechaRegistro.ToString("ddMMyyyy") + "_" + pacienteNombres;
                pFile.crearPDFFile(
                    numeroOrden, 
                    pacienteNombres,
                    generoPaciente,
                    nombreMedico,
                    telefonoMedico,
                    correoMedico,
                    laboratorioDesc,
                    sedeDesc, 
                    fechaRegistro,
                    origen,
                    pacienteEdad,
                    pacienteEstado, 
                    result.Resultados,
                        validados,
                        todos);
                pgnOrdenes.Value = pgnOrdenes.Value + 1;
                lblOrdenes.Text = "Procesando Orden...." + numeroOrden + ">>>" + pacienteNombres + ">>>" + sedeDesc;
                
                Application.DoEvents();
            }
            MessageBox.Show("Se han generado " + valMaximoProgreso + " registros","LIMS | G A L I L E O",MessageBoxButtons.OK,MessageBoxIcon.Information);
            lblOrdenes.Text = "";
            pgnOrdenes.Value = 0;

            //test pdf
            //PDF pFile = new PDF();
            //pFile.filename = "PRUEBA PDF";
            //pFile.crearPDFFile("202302030007","paciente","laboratorio",DateTime.Now);

            /*
            string file = Environment.CurrentDirectory + "\\SampleData\\Ejemplo.json";

            var fileStream = new FileStream(file,FileMode.Open);
            

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var data = streamReader.ReadToEnd();
                OrdenGalileo orden = JsonConvert.DeserializeObject<OrdenGalileo>(data);
                HISManager h = new HISManager("NqvtSj3vhSx4FkWFNObJJy67JLJQD9QG");

                MessageBox.Show(h.PostOrden(orden).ToString());


            }

            */

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //string xmlConfig = @"C:\Reportes\configs\lisReports.xml";
            string LogPath = Environment.CurrentDirectory + "\\logs";
            string CapPath = Environment.CurrentDirectory + "\\data";
            //XmlTextReader reader = null;
            //try
            //{
            //    reader = new XmlTextReader(xmlConfig);
            //    reader.WhitespaceHandling = WhitespaceHandling.None;

            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            string actualProcess = Process.GetCurrentProcess().ProcessName;
            
            Process[] InstanciasCorriendo = Process.GetProcessesByName(actualProcess);

            if (InstanciasCorriendo.Length > 1)
            {
                MessageBox.Show("No puede ejecutarse varias instancias de este programa varias veces", "GALILEO LIS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);

            }






            #region ManagersAndHelpers


            log = new LogHelper(LogPath);
            cap = new CaptureHelper(CapPath);
            enc = new EncryptHelper();
            fil = new FileHelper();


            #endregion





            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            string[] licenceDecrypt = enc.DecryptString(key, License).Split(";");

            IdLaboratorio = Convert.ToInt32(licenceDecrypt[1]);
            IdInstrumento = licenceDecrypt[0];
            ApiToken = licenceDecrypt[2];

           
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    HISManager h = new HISManager(ApiToken);
            //    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            //    String numeroOrden = row.Cells[2].Value.ToString();
            //    String pacienteNombres = row.Cells[0].Value.ToString() + " " + row.Cells[1].Value.ToString();
            //    DateTime fechaRegistro = (DateTime)row.Cells[4].Value;
            //    String pacienteEdad = row.Cells[6].Value.ToString();
            //    String pacienteEstado = row.Cells[5].Value.ToString();
            //    string area = cmbArea.Text;
            //    bool todos = false;
            //    bool validados = chkValidados.Checked;
            //    if (!validados)
            //    {
            //        todos = true;
            //    }

            //    //MessageBox.Show("Seleccione un área de estudio","LIMS | G A L I L E O",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            //    //MessageBox.Show(numeroOrden + fechaRegistro.ToString("ddMMyyyy HH:mm"));
            //    PDF_MSP pFile = new PDF_MSP();
            //    pFile.filename = fechaRegistro.ToString("ddMMyyyy") + "_" + pacienteNombres;

            //    var sede = h.ListarDatosAdicionales(numeroOrden, IdLaboratorio);
            //    String laboratorioDesc = sede.NombreLaboratorio;
            //    String sedeDesc = sede.NombreSede;
            //    String origen = sede.NombreSede;
            //    String generoPaciente = sede.GenerodelPaciente;
            //    string nombreMedico = "";
            //    string telefonoMedico="";
            //    string correoMedico="";
            //    if (sede.IdMedico!=0)
            //    {
            //        var medico = h.datosMedico(sede.IdMedico, IdLaboratorio);
            //        nombreMedico = medico.Medico[0].Details[0].Apellido ?? "";
            //        nombreMedico = nombreMedico + " "; 
            //        nombreMedico = nombreMedico + medico.Medico[0].Details[0].Nombre ?? "";  
            //        telefonoMedico = medico.Medico[0].Details[0].Telefono ?? "-";
            //        correoMedico = medico.Medico[0].Details[0].Email ?? "-";
            //    }
            //    var result = h.ListarResultados(numeroOrden, IdLaboratorio);
            //    if (area == "")
            //    {
            //        pFile.crearPDFFile(
            //            numeroOrden,
            //            pacienteNombres,
            //            generoPaciente,
            //            nombreMedico,
            //            telefonoMedico,
            //            correoMedico,
            //            laboratorioDesc,
            //            sedeDesc,
            //            fechaRegistro,
            //            origen,
            //            pacienteEdad,
            //            pacienteEstado,
            //            result.Resultados,
            //            validados,
            //            todos);
            //    }
            //    else
            //    {
            //        pFile.crearPDFFileArea(
            //            numeroOrden,
            //            pacienteNombres,
            //            generoPaciente,
            //            nombreMedico,
            //            telefonoMedico,
            //            correoMedico,
            //            laboratorioDesc,
            //            sedeDesc,
            //            fechaRegistro,
            //            origen,
            //            pacienteEdad,
            //            pacienteEstado,
            //            result.Resultados,
            //            area, validados, todos);
            //    }

            //    string filePaciente = RutaDestino + sedeDesc + "\\" + fechaRegistro.ToString("ddMMyyyy") + "\\" + pFile.filename + "_" + numeroOrden + ".pdf";
            //    FileStream fsRP = new FileStream(filePaciente, FileMode.Open);
            //    pdfRepPac.Document = new Document(fsRP);
            //    fileGlobal = filePaciente;
            //    fsRP.Close();
            //    var filePDF = new System.Diagnostics.ProcessStartInfo() { FileName = fileGlobal, UseShellExecute = true };
            //    System.Diagnostics.Process.Start(filePDF);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // opens the folder in explorer
            //Process.Start(RutaDestino);
            //// opens the folder in explorer
            Process.Start("explorer.exe", RutaDestino);
            //// throws exception
            //Process.Start(@"c:\does_not_exist");
            //// opens explorer, showing some other folder)
            //Process.Start("explorer.exe", @"c:\does_not_exist");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex] is DataGridViewButtonCell cell)
            {
                if (cell.Value == null || cell.Value == cell.OwningColumn.DefaultCellStyle.NullValue)
                {
                    cell.Value = "CREANDO";
                    try
                    {
                        HISManager h = new HISManager(ApiToken);
                        DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                        String numeroOrden = row.Cells[3].Value.ToString();
                        String pacienteNombres = row.Cells[1].Value.ToString() + " " + row.Cells[2].Value.ToString();
                        DateTime fechaRegistro = (DateTime)row.Cells[5].Value;
                        String pacienteEdad = row.Cells[7].Value.ToString();
                        String pacienteEstado = row.Cells[6].Value.ToString();
                        string area = cmbArea.Text;
                        bool todos = false;
                        bool validados = chkValidados.Checked;
                        if (!validados)
                        {
                            todos = true;
                        }

                        //MessageBox.Show("Seleccione un área de estudio","LIMS | G A L I L E O",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        //MessageBox.Show(numeroOrden + fechaRegistro.ToString("ddMMyyyy HH:mm"));
                        PDF_MSP pFile = new PDF_MSP();
                        pFile.filename = fechaRegistro.ToString("ddMMyyyy") + "_" + pacienteNombres;

                        var sede = h.ListarDatosAdicionales(numeroOrden, IdLaboratorio);
                        String laboratorioDesc = sede.NombreLaboratorio;
                        String sedeDesc = sede.NombreSede;
                        String origen = sede.NombreSede;
                        String generoPaciente = sede.GenerodelPaciente;
                        string nombreMedico = "";
                        string telefonoMedico = "";
                        string correoMedico = "";
                        if (sede.IdMedico != 0)
                        {
                            var medico = h.datosMedico(sede.IdMedico, IdLaboratorio);
                            nombreMedico = medico.Medico[0].Details[0].Apellido ?? "";
                            nombreMedico = nombreMedico + " ";
                            nombreMedico = nombreMedico + medico.Medico[0].Details[0].Nombre ?? "";
                            telefonoMedico = medico.Medico[0].Details[0].Telefono ?? "-";
                            correoMedico = medico.Medico[0].Details[0].Email ?? "-";
                        }
                        var result = h.ListarResultados(numeroOrden, IdLaboratorio);
                        if (area == "")
                        {
                            pFile.crearPDFFile(
                                numeroOrden,
                                pacienteNombres,
                                generoPaciente,
                                nombreMedico,
                                telefonoMedico,
                                correoMedico,
                                laboratorioDesc,
                                sedeDesc,
                                fechaRegistro,
                                origen,
                                pacienteEdad,
                                pacienteEstado,
                                result.Resultados,
                                validados,
                                todos);
                        }
                        else
                        {
                            pFile.crearPDFFileArea(
                                numeroOrden,
                                pacienteNombres,
                                generoPaciente,
                                nombreMedico,
                                telefonoMedico,
                                correoMedico,
                                laboratorioDesc,
                                sedeDesc,
                                fechaRegistro,
                                origen,
                                pacienteEdad,
                                pacienteEstado,
                                result.Resultados,
                                area, validados, todos);
                        }

                        string filePaciente = RutaDestino + sedeDesc + "\\" + fechaRegistro.ToString("ddMMyyyy") + "\\" + pFile.filename + "_" + numeroOrden + ".pdf";
                        FileStream fsRP = new FileStream(filePaciente, FileMode.Open);
                        pdfRepPac.Document = new Document(fsRP);
                        fileGlobal = filePaciente;
                        fsRP.Close();
                        var filePDF = new System.Diagnostics.ProcessStartInfo() { FileName = fileGlobal, UseShellExecute = true };
                        System.Diagnostics.Process.Start(filePDF);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    cell.Value = "VER PDF";
                }
                else
                {
                    cell.Value = cell.OwningColumn.DefaultCellStyle.NullValue;
                }
            }
        }

        private void btnAbrirFile_Click(object sender, EventArgs e)
        {

            var filePDF = new System.Diagnostics.ProcessStartInfo() { FileName = fileGlobal, UseShellExecute = true };
            System.Diagnostics.Process.Start(filePDF);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            HISManager h = new HISManager(ApiToken);

            var results = h.ListarOrdenes(dateStart.Value, dateEnd.Value, IdLaboratorio);
            //Resultados results = h.GetResultados(dateStart.Value, dateEnd.Value, IdLaboratorio, "1");
            //dataGridView1.Columns.Add("Column","Eliminar");
            var buttonColumn = new DataGridViewButtonColumn()
            {
                Name = "statusButton",
                HeaderText = "Archivo",
                UseColumnTextForButtonValue = false,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "VER PDF"
                }
            };
            this.dataGridView1.Columns.Add(buttonColumn);

            dataGridView1.DataSource = results.DetalleOrdenes;
            MessageBox.Show("Se han encontrado " + dataGridView1.Rows.Count + " registros", "LIMS | G A L I L E O", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", RutaDestino);
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            string buscar = txtBusqueda.Text;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells[1].Value.ToString().StartsWith(buscar.ToUpper()) && buscar != "")
                    {
                        item.Selected = true;
                        break;
                    }
                    else
                    {
                        item.Selected = false;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private void chkValidados_CheckedChanged(object sender, EventArgs e)
        {
            if (chkValidados.Checked)
            {
                niInfo.Icon = SystemIcons.Information;
                niInfo.BalloonTipText = "Se mostrarán sólo los resultados que han sido validados";
                niInfo.BalloonTipTitle = "Resultados LIS";
                niInfo.ShowBalloonTip(5000);
            }
            else
            {
                niInfo.Icon = SystemIcons.Information;
                niInfo.BalloonTipText = "Se mostrarán todos los resultados validados y no validados";
                niInfo.BalloonTipTitle = "Resultados LIS";
                niInfo.ShowBalloonTip(5000);
            }
        }

        private void btnDatosPaciente_Click(object sender, EventArgs e)
        {
            HISManager p = new HISManager(ApiToken);
            //var dataPaciente = p

        }
    }
}
