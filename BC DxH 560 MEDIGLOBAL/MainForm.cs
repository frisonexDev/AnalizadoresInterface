
using Galileo.Utils;
using Galileo.Utils.ASTMModel;
using Galileo.Utils.HL7Model;
using Galileo.Connect.Manager;
using Galileo.Connect.Model;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using Galileo.Utils.ConversionModel;
using System.Diagnostics;


namespace Galileo.Online
{
    public partial class MainForm : Form
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

        public string ResultsFolder { get { return ConfigurationManager.AppSettings["ResultsFolder"]; } }

        public string License { get { return ConfigurationManager.AppSettings["License"]; } }

        public string SeparadorMuestra { get { return ConfigurationManager.AppSettings["SeparadorMuestra"]; } }

        public string OrderField { get { return ConfigurationManager.AppSettings["OrderField"]; } }

        public string TestName { get { return ConfigurationManager.AppSettings["TestName"]; } }

        public string TestValue { get { return ConfigurationManager.AppSettings["TestValue"]; } }

        public int IdLaboratorio;
        public string IdInstrumento;
        public string ApiToken;
        public string SerialMessage;

        SerialPort mySerialPort;


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




        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string LogPath = Environment.CurrentDirectory + "\\logs";
            string CapPath = Environment.CurrentDirectory + "\\data";

            string actualProcess = Process.GetCurrentProcess().ProcessName;

            Process[] InstanciasCorriendo = Process.GetProcessesByName(actualProcess);

            if (InstanciasCorriendo.Length > 1)
            {
                MessageBox.Show("No puede ejecutarse varias instancias de este programa varias veces","GALILEO LIS",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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

            EventWriter(true, "Conectando con GALILEO ONLINE", LogEvent.Esperando, "Conectando");

            insMngr = new InstrumentManager(ServerLIS, IdInstrumento, IdLaboratorio, SeparadorMuestra, ApiToken);

            bool conectado = true;

            if (ConnectionSetup == "SERVER")
            {
                if (insMngr.GetInstrumento())
                {
                    //Configuraciones Locales
                    EventWriter(true, "Se encuentra el analizador configurado.", LogEvent.Conectado, "Conectado");
                }
                else
                {
                    EventWriter(true, "No implementada la sincronización con el servidor. Seleccione LOCAL.", LogEvent.Error, "OFFLINE");
                    conectado = false;
                }
            }
            else
            {

                EventWriter(true, "Se encuentra el analizador configurado de manera local.", LogEvent.Conectado, "Conectado");

            }





            if (conectado)
            {
                string config = JsonConvert.SerializeObject(insMngr.Instrumento);
                fil.WriteToFile(config, "Config.Json", Environment.CurrentDirectory + "\\DB\\");
                log.Add("Inicia la aplicación");
                this.Text = this.Text + this.Instrumento;
                this.lblNombreInstrumento.Text = this.Text;
               
                picInstrumento.Image = Image.FromFile(Environment.CurrentDirectory + "\\Images\\" + this.InstrumentoImg);
                log.Add("Esperando Conexión del Instrumento " + this.Instrumento);


                if (ConnectionMode == "TCP")
                {
                    Thread threadInput = new Thread(ListenTCP);
                    threadInput.Start();
                }
                else if (ConnectionMode == "FILES")
                {
                    Thread threadInput = new Thread(ListenFiles);
                    threadInput.Start();
                }
                else if (ConnectionMode == "SERIAL")
                {
                    Thread threadInput = new Thread(ListenSerial);
                    threadInput.Start();
                }
            }


        }



        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            //Ocultamos el icono de la bandeja de sistema
           
        }

        private void frmPrincipal_Resize(object sender, EventArgs e)
        {
          
        }

        private void SetState(bool displayLoader, string action)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    stateProgressBar.Value = 100;

                    statusLabel.Text = action;
                    //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    stateProgressBar.Value = 0;
                    stateProgressBar.Text = action;
                    //this.Cursor = System.Windows.Forms.Cursors.Default;
                });
            }
        }

        private void EventWriter(bool displayUser, string textLog, LogEvent logEvent, string statusText, bool includeDateLog = true)
        {
            string screenLog = DateTime.Now + "\t" + textLog;
            this.Invoke((MethodInvoker)delegate
            {
                if (displayUser)
                {

                    Color foreColor = Color.White;

                    switch (logEvent)
                    {
                        case LogEvent.Recibiendo:
                            foreColor = Color.LightGreen;
                            break;
                        case LogEvent.Enviando:
                            foreColor = Color.Blue;
                            break;
                        case LogEvent.Error:
                            foreColor = Color.Red;
                            break;
                        default:
                            break;
                    }

                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.SelectionLength = 0;

                    logScreen.SelectionColor = foreColor;
                    logScreen.AppendText(screenLog);
                    logScreen.AppendText(Environment.NewLine);
                    logScreen.SelectionColor = logScreen.ForeColor;
                }

                log.Add(textLog,includeDateLog);

                stateProgressBar.Value = 100;

                statusLabel.Text = statusText;

            });

        }



        private void ListenTCP()
        {


            var appSettings = ConfigurationManager.AppSettings;




            FileHelper fHlp = new();
            ImageHelper iHlp = new();


            TcpListener server = new TcpListener(IPAddress.Any, Convert.ToInt32(LocalPort));
            //TcpListener server = new TcpListener(IPAddress.Parse(InstrumentIP), Convert.ToInt32(LocalPort));
            server.Start();
            SetState(false, "Conectado");

            byte[] bytes = new Byte[40000];
            String data = null;

            EventWriter(true, "Escuchando el puerto: " + LocalPort + " por TCP/IP", LogEvent.Conectado, "Conectado");

            while (true)
            {






                TcpClient clt;
                if (server.Pending())
                {

                    Thread tmp_thread = new Thread(new ThreadStart(() =>
                    {
                        //EventWriter(false, "Recibiendo: " + LocalPort, LogEvent.Conectado, "Recibiendo");
                        int i = 0;
                        clt = server.AcceptTcpClient();

                        NetworkStream stream = clt.GetStream();
                 


                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {

                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                            ReceiveTCPData(data);

                           
                            SendTCPData(stream,ASTM.Character.ACK);

                           

                        }



                    }));
                    tmp_thread.Start();

                }
                else
                {
                    Thread.Sleep(1000); //<--- timeout
                }


            }

        }

        private  void SendTCPData(NetworkStream stream, string data)
        {
            stream.Write(ASCIIEncoding.ASCII.GetBytes(data));
            AppendTXTextBox(data);
            //EventWriter(true, data, LogEvent.Enviando, "Enviando");
        }

        private void ReceiveTCPData(string data)
        {
            AppendRXTextBox(data);
            AppendMessageTextBox(data);
            //EventWriter(true, data, LogEvent.Recibiendo, "Recibiendo");
        }

        private void ReadMessage(string data)
        {
            //try
            //{

            EventWriter(true, data, LogEvent.Conectado, "Recibiendo",false);

            if (Protocol == "HL7")
                {
                    HL7Helper helper = new HL7Helper();
                    helper.Content = data;
                    HL7Record obj = helper.ToObject();

                    foreach (var msg in obj.messages)
                    {
                        try
                        {
                            if (msg.ObservationRequest != null)
                            {
                            //EventWriter(true, "Procesando el mensaje HL7 para la muestra " + msg.ObservationRequest.FillerOrderNumber + " desde " + msg.Header.SendingApplication, LogEvent.Conectado, "Recibiendo");
                            ProcessOrder(msg);
                           
                            }
                            else
                            {
                                EventWriter(true, "Mensaje HL7 Omitido", LogEvent.Conectado, "Conectado");
                            }


                        }
                        catch (Exception ex)
                        {
                            //throw ex;
                            // EventWriter(true, "Mensaje HL7 no válido, no se reconoce ", LogEvent.Error, "Error");
                        }
                    }
                }
                else
                if (Protocol == "ASTM 1394")
                {

                   
                       
                        ASTMMessage msg = new ASTMMessage(data);
                        if (msg.header != null && msg.header.RecordTypeId == "H")
                        {

                            foreach (var mf in msg.PatienInformationList[0].OrderRecordList[0].ManufacturRecordList)
                            {
                                    ImageHelper imgHelper = new ImageHelper();
                                    imgHelper.DrawImage(mf.DataMeasurement.Value, mf.TestIdentifier.Manufacturer);

                            }
                            
                            //ProcessOrder(msg);

                    

                    

                    CreateResultMediglobal(msg,ResultsFolder);

                    




                }
                else
                        {
                            EventWriter(true, "Mensaje ASTM Omitido", LogEvent.Conectado, "Conectado");
                        }
                    
                }

            //}
            //catch (Exception ex)
            //{
            //    log.Add("Error" + ex.Message);
            //    EventWriter(true, "Error al procesar el mensaje " + ex.Message, LogEvent.Error, "Error");
            //}
        }

        public void CreateResultMediglobal(ASTMMessage messages, string path)
        {

            foreach (var patient in messages.PatienInformationList)
            {

                foreach (var message in patient.OrderRecordList)
                {



                    StringBuilder sb = new StringBuilder();
                    sb.Append("Serial No.:\t520369\r\n");
                    sb.Append("RecNo:\t46542\r\n");
                    sb.Append("Sample ID:\t" + message.SpecimenId.Replace("!", "") + "\r\n");
                    sb.Append("Patient ID:\t1\r\n");
                    sb.Append("Patient Name:\t" + patient.Patient.LastName + " " + patient.Patient.FirstName + "\r\n");
                    sb.Append("Mode:\tHuman\r\n");
                    sb.Append("Doctor:\t\r\n");
                    sb.Append("Birth(ymd):\t" + "" + "\r\n");
                    sb.Append("Sex:\t" + "Female" + "\r\n");


                    sb.Append("Test date(ymd):\t" + messages.header.TimeOfMessage.ToString("yyyyMMdd") + "\r\n");
                    sb.Append("Test time(hm):\t" + messages.header.TimeOfMessage.ToString("hhmmss") + "\r\n");
                    sb.Append("Param\tFlags\tValue\tUnit\t[min-max]\r\n");

                    var result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "WBC").FirstOrDefault();
                    if (result != null)
                        sb.Append("WBC\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");


                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "LY#").FirstOrDefault();
                    if (result != null)
                        sb.Append("LYM\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "NE#").FirstOrDefault();
                    if (result != null)
                        sb.Append("NEU\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "MO#").FirstOrDefault();
                    if (result != null)
                        sb.Append("MON\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "EO#").FirstOrDefault();
                    if (result != null)
                        sb.Append("EOS\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "BA#").FirstOrDefault();
                    if (result != null)
                        sb.Append("BAS\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");



                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "LY").FirstOrDefault();
                    if (result != null)
                        sb.Append("LY%\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "NE").FirstOrDefault();
                    if (result != null)
                        sb.Append("NE%\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "MO").FirstOrDefault();
                    if (result != null)
                        sb.Append("MO%\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "EO").FirstOrDefault();
                    if (result != null)
                        sb.Append("EO%\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "BA").FirstOrDefault();
                    if (result != null)
                        sb.Append("BA%\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "RBC").FirstOrDefault();
                    if (result != null)
                        sb.Append("RBC\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e6/uL", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "HGB").FirstOrDefault();
                    if (result != null)
                        sb.Append("HGB\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "HCT").FirstOrDefault();
                    if (result != null)
                        sb.Append("HCT\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "MCV").FirstOrDefault();
                    if (result != null)
                        sb.Append("MCV\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "MCH").FirstOrDefault();
                    if (result != null)
                        sb.Append("MCH\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "MCHC").FirstOrDefault();
                    if (result != null)
                        sb.Append("MCHC\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "RDW").FirstOrDefault();
                    if (result != null)
                        sb.Append("RDWc\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");


                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "RDW-SD").FirstOrDefault();
                    if (result != null)
                        sb.Append("RDWs\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");


                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "PLT").FirstOrDefault();
                    if (result != null)
                        sb.Append("PLT\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    //result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "P-LCR").FirstOrDefault();

                    //if (result != null)
                    //    sb.Append("PLCR\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("10*9/L", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^") + "]\r\n");

                    //result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "P-LCC").FirstOrDefault();
                    //if (result != null)
                    //    sb.Append("PLCC\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("10*9/L", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^") + "]\r\n");

                    //result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "P-LCC").FirstOrDefault();
                    //if (result != null)
                    //    sb.Append("PLCC\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("10*9/L", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^") + "]\r\n");

                    //result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "PDW").FirstOrDefault();
                    //if (result != null)
                    //    sb.Append("PDWs\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("10*9/L", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^") + "]\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "MPV").FirstOrDefault();
                    if (result != null)
                        sb.Append("MPV\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");

                    try
                    {
                        result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "@PCT").FirstOrDefault();
                        if (result != null)
                            sb.Append("PCT\t\t" + result.DataMeasurement.Value + "\t" + result.Units.Replace("x10e3/uL", "10^3/uL").Replace("10*12/L", "10^6/uL") + "\t[" + result.ResultAbnormalFlags.Replace("*", "^").Replace(" to ", "-") + "]\r\n");
                    }
                    catch (Exception ex)
                    {
                        string mensaje = "Error";
                    }


                    //sb.Append("RBC\t\t4.76\t10 ^ 6 / uL\t[4.00 - 5.50]\r\n");
                    //sb.Append("HGB\t\t14.4\tg / dL\t[12.0 - 17.4]\r\n");
                    //sb.Append("HCT\t\t42.2\t%\t[36.0 - 52.0]\r\n");
                    //sb.Append("MCV\t\t88.6\tfL\t[76.0 - 96.0]\r\n");
                    //sb.Append("MCH\t\t30.2\tpg\t[27.0 - 32.0]\r\n");
                    //sb.Append("MCHC\t\t34.1\tg / dL\t[30.0 - 35.0]\r\n");


                    //sb.Append("RDWc\t\t13.6\t%\t[11.5 - 15.5]\r\n");
                    //sb.Append("RDWs\t\t- 37.2\tfL\t[46.0 - 59.0]\r\n");

                    //sb.Append("PLT\t\t266\t10 ^ 3 / uL\t[150 - 400]\r\n");

                    //sb.Append("PLCR\t\t30.20\t%\t[0.00 - 0.00]\r\n");
                    //sb.Append("PLCC\t\t80\t10 ^ 3 / uL\t[0 - 0]\r\n");

                    //sb.Append("PDWc\t\t35.5\t%\t[0.0 - 0.0]\r\n");
                    //sb.Append("PDWs\t\t15.9\tfL\t[0.0 - 0.0]\r\n");
                    //sb.Append("MPV\t\t6.9\tfL\t[6.0 - 15.0]\r\n");c
                    //sb.Append("PCT\t\t0.18\t%\t[0.00 - 0.00]\r\n");

                    /*
                    sb.Append("Warnings:\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "WBC DIFF Scattergram. HS-MS BMP").FirstOrDefault();
                    if (result != null)
                        sb.Append("DiffImage\t" + result.ObservationValue.Arguments[3] + "\r\n");


                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "BASO Scattergram. LS-MS BMP").FirstOrDefault();
                    if (result != null)
                        sb.Append("BasoImage\t" + result.ObservationValue.Arguments[3] + "\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "RBC Histogram. BMP").FirstOrDefault();
                    if (result != null)
                        sb.Append("RBCImage\t" + result.ObservationValue.Arguments[3] + "\r\n");

                    result = message.ResultRecordList.Where(x => x.TestIdentifier.Manufacturer == "PLT Histogram. BMP").FirstOrDefault();
                    if (result != null)
                        sb.Append("PLTImage\t" + result.ObservationValue.Arguments[3] + "\r\n");
                    */

                    //sb.Append("BasoImage\tiVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAMAAABrrFhUAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAMAUExURf///wAA//8A/wAA/wAA/wAAAACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf//zAAAAKLGdn4AAAAodFJOU/////////////////////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADstB13AAAACXBIWXMAAA7DAAAOwwHHb6hkAAAD10lEQVR4Xu3b3XYaOxCEURO//zMnqqqWBnN8EmP7wlJ/OzD6YW6qJU0Ia+UFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIBN/a4WAAAAAAAAAAAAAAAAAIDz3aoFjsUmB7ofA54CYBdwCqgA8G+n/4cqngOdK1DZW5bAoXtv/9uX4+//eJwlaLgTRuS82nL2nIKmZbg5eN89oPz1GGxchFWDrq70HetwG6rbksK7AOPSsxCuQOtdoKXXc3AOOuq7A7T6yb/2QEet02fxayu05OXXS3X4qwN/Fq3oKYJqUfN9eOdr8RW9X/xEdvxcNPAHnVT+KoanrEcl5v7XwlcNmnH+wb2ae8KvarelVa8KfKoA+/PWnxsh/Wbu90C//FrzmV9tTX+/H/usSOpKPwvwkToc9LXYi+8arAr0Uun18qCXJNdJSPy/F+C12sPoDIQG9f4WP/9rUkWt/MM3pt9EIid6KtDLDO3WT4J/PQcOU0ue/Bo8m373fw1lwSv+4AI8W4TNKfXkUTc+96mBrzX9CTkOv6q5u/5sSu8C5O3RF2z2UEhYLXyt/5cLsJv7Cli/AtTJ99sTzWjJa/eLx+uT8yWkgyf/l2LXA3Cz5+BdfKnJz1vxN6iD0ubt6PPiz4Zni7Hbyk/OqfDmqU5G4usEaJzmWRuvflXgQ3vgY7+JbVSMEdvpkz+VcL8+t5W6Ogf9MnjlVO5KX7Prs0N/Cb0oabI7/apAvK4CnFmIypr03vtpM12pK/pqjiuF4r4bf3g37Jo8pBQJP61t8d90Y6YmX5+JvsPfCs5dG2C6rZhXc+V27xruTiu/3hqPbMqbhHc5x/QcvZkd9vwu5LQjtS7Orr45VD3xVtbkn8M1vbWUwAVQfr1WDRT2beAxfJi4untz6OyCm8MonNLqoioclHWZK+1OTsAbzq6XSlCd+cEhZciJHxcfAu2AzNhMnujqekP4nRnfdgolzyZICZJu5q1SuAarWzflzr358HsjjMtK5YxrqFeCO3wVxrO64wSqgpM5bTUKmuT3ifPHN7k5greBN0Iy6ZrYiumOrDbS0y2bW+lXlNEqXdL6OrgzLvq8hrl7a8rt7KPxH6Xzsj+o0L7T9VHjywEcfD0FE8+pyxxUu24rV29Xip8m8WfQNO/IfafwCchDYLxUAId7XPeS5LM9jffAUGFn9qsE9XlV4aQi1IPQKqqb1aoK6s/sudHN9q7s7iVhQs9ONsFJK/6gvgdMSa6msmt00Xx1D7ZSPqRvYmVfg+tysPtj8NZ7yU+vBhr6/yMAAAAAANjSy8sfSCUNOCLU32AAAAAASUVORK5CYII=\r\n");
                    //sb.Append("RBCImage\tiVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAYAAABccqhmAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAn/ SURBVHhe7drLcdtIGMRxhOIgfNPFRybgu66 + s8pHrfJgKQFHoBODUALKBIsBXyDYAPGYwTy +/ 6 + qq5aEXBIIdouktnp7e6sB2MQAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIbNHIBjva929eH7fNP5PtS7qqqrNr1jAJI2YwBc + fslP923P15u7pvj++ZeADmYNADfh137G3633ze / 7TsD4Aq / O9S3X / riFQKAZE0bgOPxVPL25f5YwRkAICfzPgN4MgDHffMW4e4VAYCUeRuA09sEfvsDOfEyAJQfyNPqAaD8QL7WDcCTzwQApG3VALQf + l3 / J6Bbrv9fAICkzRsAAEVhAADDGADAsIUDUJ0DIGcMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEACan++ + 8aYAsMQCK65WcAsBUGIBH9AegGCIUBSIAqfT9ACAxAZKrsKkAIDEBkquxDAXxjACJTRR8L4BMDEJEq + LMAPjEAEamCPwvgEwMQkSr4lAC + MACRqGJPDeALAxCJKvacAD4wAJGoUs8J4AMDEIkq9ZwAPjAAEahCLwmwFgMQgSrzkgBrMQARqDIvCbAWAxCBKvPSAGswABtTJV4TYA0GYGOqxGsCrMEAbEyVeE2ANRiAjakSrw2wFAOwMVXgtQGWYgA2pgq8NsBSDMCGVHl9BFiKAdiQKq + vAEswABtSxfUVYAkGYEOquL4CLJHeABz3dVVV5 + zqw / f5 / sb3Ydc55rKvj + djOVDF9RVgibQG4PtQ77qlbsfgdvu4r + p9To3vUcX1FWCJxN8CHOv99VXAd33Y3b8iyIkqrc8ASyQ9AKeX / JdXAKcx2O3024PUqdL6DjBXmgPQvhXolby973b7fhzSpwrrO8BcGb0C6HOvCPL5TEAV1neAuTL6DKDPfSaQxwCosoYIMFfGAzB2LC2qrKECzJHWAPT + 7Ne + Bdgdmt / 1DfEnweuxxKmihgowR3KvAO7 / Z5 / 79 / 9jx1KlShoywByJvwXInyppyABzMACBqZKGDjAVAxCYKmjoAFPlMQDNe / 67ZEQVNHSAqRiAgFQ5twgwFQMQkCrnFgGmYgACUuXcIsBUDEBAqpxbBZiCAQhIFXOrAFMwAAGpYm4VYAoGICBVzC0DPMMABKIKuXWAZxiAQFQhtw7wDAMQiCpkjABjGIBAVBljBBjDAASiyhgjwBgGIBBVxlgBhjAAAagSxgwwhAEIQJUwZoAhDEAAqoQxAwxhAAJQJYwZYAgDEIAqYewACgMQgCpg7AAKAxCAKmDsAAoD4JkqXwoBFAbAM1W + VAL0MQCeqeKlEqCPAfBMFS + VAH0MgGeqeKkE6GMAPFPFSylAFwPgmSpdSgG6GACPVOFSC9DFAHikCpdagC4GwCNVuBQDXDAAHqmypRjgggHwSJUtxQAXDIBHqmwpBrhgADxSZUs1gMMAeKJKlnIAhwHwRJUs5QAOA + CJKlnKARwGwBNVstQDMACeqIKlHoAB8EQVLPUADIAnqmA5BLYxAJ6ocuUQ2MYAeKCKlUtgGwPggSpWToFdDIAHqlQ5BXYxAB6oUuUU2MUAeKBKlVtgEwPggSpUboFNDMBKqkw5BjYxACupMuUa2MMArKSKlGtgDwOwkipSroE9DMBKqkg5B7YwACupEuUc2MIArKAKlHtgCwOwgipQCYEdDMAKqjwlBHYwACuo8pQS2MAArKCKU0pgAwOwgipOSUH5GICFVGFKC8rHACykClNiUDYGYCFVlhKDsjEAC6mylBiUjQFYSJWl1KBcDMBCqiilBuViABZQJSk5KBcDsIAqSelBmRiABVRBSg / KxAAsoApiISgPA7CAKoeFoDwMwEyqGJaCsjAAM6lSWArKwgDMpEphLSgHAzCTKoS1oBwMwAyqDJaD / DEAM6gSWA7yxwDMoEpgPcgbAzCRevITBiB3DMBE6slPbkGeGIAJ1BOe3Ad5ejoA + gK7Em5YRAYgiyA / owMwfIFdCTcsIgOQTZAXBuCJ / mNApgV5GBwAdVFdzkfP2QgDkF2QBwbgCfUYkGlB + uQAqIvZTfMV52yEAcgySB8DMEKdO5kXpI0BGKHOncwP0sUADFDnTZYHaXoYAHXx + mm + 6pyNMABFBOlZNQCn / 94AA1BMkBYGYED / nIm / IB2rB + B0O7CNB6B7riRMkIa7AVAXSqX5yja324ExAEUG8TEAPd3zJOGDuLwMwOm + gBgAE8H2GICO7vmQOMG2rgOgLsZQmq9u83h / IAyAySA8BuCsfy4knSAcrwNwOhYAA0CawL92ANSDPZbmn7XRxwIIPADqPEjagR / eB8DFOwaA9AI / ggyAi1cBB0D97CSfYB0GQPzsJN9gnmAD4OJNgAFQPy8pI5gu6AC4eOF5ANTPScoMxlXVr1 / ygRtL88 / aqGNDWYUBIB6CR5sNQDezrRwA9TMQ28FJlAHo56kFA6C + DyFTY0VV / fhRtyMwI29vVRt1LETemtJ3o76GkK3iPjebmr9//7ZRx0Jnyveufv/+Xf/582fz/Pz5s406Fjp8b308ZPje+njITPne1dfXVx0j//79a6OOhc7l5NWx0OG89fGQ4bz1cZdoAxAzMZ8QMcN56+OlZsp5MwCGwnnr46VmynmbHICYL8tihvPWx0vNlPM2OQCEkFOKH4CP16qu3J8Oz3n9ON3/+f5yve/l/fPh3+Wbz/r95bX+6Nw3dK5lPQaP5132tXfnezu3KddV3V/4ALgH6aV+/+zd//lev1SXJ8vA12SZj/q1vcCdIgyda1GPgTjvwq99W+bXj9Pt9pyeXNeB+wsfgOaJ8fJef/budw9efxnz/U1wibuo7gK7MtyKMHSu5TwG+rztXftToYfOb+j+sgegXb3TS5425yfEw0X/eL2tafYZH4DLuZb3GPQGwNK1d+f65PyG7i97ANxJdp4U7j2hexCKfBJcwwC0t61c+7uX9gzAeAYejIcHJ+uMD8DldnmPQW8A+inx2vfK7zJ0fkP3Fz0A7iRv6+7eJ50//bx74NwTJ88PgnR6RRg61+Ieg/vzLv7ai/I/3v/8ehf/CuDuT0HXJ8T5CXK+v7uM+ee+CC5D51rWY/B43iVf+/6fOKvOkM253rbeAhBC7sIAEGI2X/X/6JRDAhJzOmEAAAAASUVORK5CYII=\r\n");
                    //sb.Append("PLTImage\tiVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAYAAABccqhmAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAn6SURBVHhe7drNceNGF4ZRhuIgvJuNl0rA+9l6r / 1YeagmAUcwKwUxCSgTGA2SEki1KFC6JNF9z6l6qz4Jrs / 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\r\n");

                    //sb.Append("DiffImage\t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\r\n");
                    //sb.Append("BasoImage\t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\r\n");
                    //sb.Append("RBCImage\tiVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAYAAABccqhmAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAn/ SURBVHhe7drLcdtIGMRxhOIgfNPFRybgu66 + s8pHrfJgKQFHoBODUALKBIsBXyDYAPGYwTy +/ 6 + qq5aEXBIIdouktnp7e6sB2MQAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIYxAIBhDABgGAMAGMYAAIbNHIBjva929eH7fNP5PtS7qqqrNr1jAJI2YwBc + fslP923P15u7pvj++ZeADmYNADfh137G3633ze / 7TsD4Aq / O9S3X / riFQKAZE0bgOPxVPL25f5YwRkAICfzPgN4MgDHffMW4e4VAYCUeRuA09sEfvsDOfEyAJQfyNPqAaD8QL7WDcCTzwQApG3VALQf + l3 / J6Bbrv9fAICkzRsAAEVhAADDGADAsIUDUJ0DIGcMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEAgGEMAGAYAwAYxgAAhjEACan++ + 8aYAsMQCK65WcAsBUGIBH9AegGCIUBSIAqfT9ACAxAZKrsKkAIDEBkquxDAXxjACJTRR8L4BMDEJEq + LMAPjEAEamCPwvgEwMQkSr4lAC + MACRqGJPDeALAxCJKvacAD4wAJGoUs8J4AMDEIkq9ZwAPjAAEahCLwmwFgMQgSrzkgBrMQARqDIvCbAWAxCBKvPSAGswABtTJV4TYA0GYGOqxGsCrMEAbEyVeE2ANRiAjakSrw2wFAOwMVXgtQGWYgA2pgq8NsBSDMCGVHl9BFiKAdiQKq + vAEswABtSxfUVYAkGYEOquL4CLJHeABz3dVVV5 + zqw / f5 / sb3Ydc55rKvj + djOVDF9RVgibQG4PtQ77qlbsfgdvu4r + p9To3vUcX1FWCJxN8CHOv99VXAd33Y3b8iyIkqrc8ASyQ9AKeX / JdXAKcx2O3024PUqdL6DjBXmgPQvhXolby973b7fhzSpwrrO8BcGb0C6HOvCPL5TEAV1neAuTL6DKDPfSaQxwCosoYIMFfGAzB2LC2qrKECzJHWAPT + 7Ne + Bdgdmt / 1DfEnweuxxKmihgowR3KvAO7 / Z5 / 79 / 9jx1KlShoywByJvwXInyppyABzMACBqZKGDjAVAxCYKmjoAFPlMQDNe / 67ZEQVNHSAqRiAgFQ5twgwFQMQkCrnFgGmYgACUuXcIsBUDEBAqpxbBZiCAQhIFXOrAFMwAAGpYm4VYAoGICBVzC0DPMMABKIKuXWAZxiAQFQhtw7wDAMQiCpkjABjGIBAVBljBBjDAASiyhgjwBgGIBBVxlgBhjAAAagSxgwwhAEIQJUwZoAhDEAAqoQxAwxhAAJQJYwZYAgDEIAqYewACgMQgCpg7AAKAxCAKmDsAAoD4JkqXwoBFAbAM1W + VAL0MQCeqeKlEqCPAfBMFS + VAH0MgGeqeKkE6GMAPFPFSylAFwPgmSpdSgG6GACPVOFSC9DFAHikCpdagC4GwCNVuBQDXDAAHqmypRjgggHwSJUtxQAXDIBHqmwpBrhgADxSZUs1gMMAeKJKlnIAhwHwRJUs5QAOA + CJKlnKARwGwBNVstQDMACeqIKlHoAB8EQVLPUADIAnqmA5BLYxAJ6ocuUQ2MYAeKCKlUtgGwPggSpWToFdDIAHqlQ5BXYxAB6oUuUU2MUAeKBKlVtgEwPggSpUboFNDMBKqkw5BjYxACupMuUa2MMArKSKlGtgDwOwkipSroE9DMBKqkg5B7YwACupEuUc2MIArKAKlHtgCwOwgipQCYEdDMAKqjwlBHYwACuo8pQS2MAArKCKU0pgAwOwgipOSUH5GICFVGFKC8rHACykClNiUDYGYCFVlhKDsjEAC6mylBiUjQFYSJWl1KBcDMBCqiilBuViABZQJSk5KBcDsIAqSelBmRiABVRBSg / KxAAsoApiISgPA7CAKoeFoDwMwEyqGJaCsjAAM6lSWArKwgDMpEphLSgHAzCTKoS1oBwMwAyqDJaD / DEAM6gSWA7yxwDMoEpgPcgbAzCRevITBiB3DMBE6slPbkGeGIAJ1BOe3Ad5ejoA + gK7Em5YRAYgiyA / owMwfIFdCTcsIgOQTZAXBuCJ / mNApgV5GBwAdVFdzkfP2QgDkF2QBwbgCfUYkGlB + uQAqIvZTfMV52yEAcgySB8DMEKdO5kXpI0BGKHOncwP0sUADFDnTZYHaXoYAHXx + mm + 6pyNMABFBOlZNQCn / 94AA1BMkBYGYED / nIm / IB2rB + B0O7CNB6B7riRMkIa7AVAXSqX5yja324ExAEUG8TEAPd3zJOGDuLwMwOm + gBgAE8H2GICO7vmQOMG2rgOgLsZQmq9u83h / IAyAySA8BuCsfy4knSAcrwNwOhYAA0CawL92ANSDPZbmn7XRxwIIPADqPEjagR / eB8DFOwaA9AI / ggyAi1cBB0D97CSfYB0GQPzsJN9gnmAD4OJNgAFQPy8pI5gu6AC4eOF5ANTPScoMxlXVr1 / ygRtL88 / aqGNDWYUBIB6CR5sNQDezrRwA9TMQ28FJlAHo56kFA6C + DyFTY0VV / fhRtyMwI29vVRt1LETemtJ3o76GkK3iPjebmr9//7ZRx0Jnyveufv/+Xf/582fz/Pz5s406Fjp8b308ZPje+njITPne1dfXVx0j//79a6OOhc7l5NWx0OG89fGQ4bz1cZdoAxAzMZ8QMcN56+OlZsp5MwCGwnnr46VmynmbHICYL8tihvPWx0vNlPM2OQCEkFOKH4CP16qu3J8Oz3n9ON3/+f5yve/l/fPh3+Wbz/r95bX+6Nw3dK5lPQaP5132tXfnezu3KddV3V/4ALgH6aV+/+zd//lev1SXJ8vA12SZj/q1vcCdIgyda1GPgTjvwq99W+bXj9Pt9pyeXNeB+wsfgOaJ8fJef/budw9efxnz/U1wibuo7gK7MtyKMHSu5TwG+rztXftToYfOb+j+sgegXb3TS5425yfEw0X/eL2tafYZH4DLuZb3GPQGwNK1d+f65PyG7i97ANxJdp4U7j2hexCKfBJcwwC0t61c+7uX9gzAeAYejIcHJ+uMD8DldnmPQW8A+inx2vfK7zJ0fkP3Fz0A7iRv6+7eJ50//bx74NwTJ88PgnR6RRg61+Ieg/vzLv7ai/I/3v/8ehf/CuDuT0HXJ8T5CXK+v7uM+ee+CC5D51rWY/B43iVf+/6fOKvOkM253rbeAhBC7sIAEGI2X/X/6JRDAhJzOmEAAAAASUVORK5CYII=\r\n");
                    //sb.Append("PLTImage\tiVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAYAAABccqhmAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAn6SURBVHhe7drNceNGF4ZRhuIgvJuNl0rA+9l6r / 1YeagmAUcwKwUxCSgTGA2SEki1KFC6JNF9z6l6qz4Jrs / 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\r\n");



                    //sb.Append("LYM\t\t2.25\t10 ^ 3 / uL\t[1.30 - 4.00]\r\n");
                    //sb.Append("NEU\t\t4.38\t10 ^ 3 / uL\t[2.00 - 7.50]\r\n");
                    //sb.Append("MON\t\t0.48\t10 ^ 3 / uL\t[0.15 - 0.70]\r\n");
                    //sb.Append("EOS\t\t0.31\t10 ^ 3 / uL\t[0.00 - 0.50]\r\n");
                    //sb.Append("BAS\t\t0.09\t10 ^ 3 / uL\t[0.00 - 0.15]\r\n");

                    //sb.Append("LY %\t\t30.0\t%\t[21.0 - 40.0]\r\n");
                    //sb.Append("NE %\t\t58.3\t%\t[40.0 - 75.0]\r\n");
                    //sb.Append("MO %\t\t6.4\t%\t[3.0 - 7.0]\r\n");
                    //sb.Append("EO %\t\t4.1\t%\t[0.0 - 5.0]\r\n");
                    //sb.Append("BA %\t\t1.2\t%\t[0.0 - 1.5]\r\n");



                    string data = sb.ToString();



                    string filename = "Res Muestra " + message.SpecimenId.Replace("!", "") + " Paciente" + patient.PatientId + " 1 " + DateTime.Now.Day.ToString() + " "
                                + DateTime.Now.Month.ToString() + " " + DateTime.Now.Year.ToString() + ".txt";




                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string filepath = path + "\\" + filename;



                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }

                    if (!File.Exists(filepath))
                    {
                        using (StreamWriter sw = File.CreateText(filepath))
                        {
                            sw.WriteLine(data);
                            EventWriter(true, "Escribiendo el archivo " + filepath, LogEvent.Enviando, "Enviando", false);
                            EventWriter(true, data, LogEvent.Enviando, "Enviando", false);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(filepath))
                        {
                            sw.WriteLine(data);
                            EventWriter(true, "Actualizando el archivo " + filepath, LogEvent.Enviando, "Enviando", false);
                            EventWriter(true, data, LogEvent.Enviando, "Enviando", false);
                        }
                    }

                   

                }
            }

        }

        private void ProcessOrder(HL7Message msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);

            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);

            JsonHelper message = new JsonHelper(jsonResult);

            string sender = message.GetValue("Header/SendingApplication");

            string BarCode;
            string OrderNumber;
            string CodigoMuestra;

            try
            {


                BarCode = message.GetValue(OrderField);
                OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
            }
            catch
            {
                EventWriter(true, "No se encuentra el path: " + OrderField + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                return;
            }



            string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
            if (Directory.Exists(ImagePath))
            {
                Directory.Delete(ImagePath, true);
                Directory.CreateDirectory(ImagePath);
            }


            EventWriter(true, jsonResult, LogEvent.Esperando, "Procesando Mensaje");







            List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();

            foreach (var result in msg.ObservationResultList)
            {
                JsonHelper line = new JsonHelper(JsonConvert.SerializeObject(result));
                string testName;
                string testValue;

                try
                {
                    testName = line.GetValue(TestName);
                    testValue = line.GetValue(TestValue);
                }
                catch
                {
                    EventWriter(true, "No se encuentra el path: " + TestName + "-" + TestValue + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                    return;
                }

                foreach (var item in insMngr.Instrumento.DetallesInstrumento)
                {
                    if (item.Homologacion == testName)
                    {
                        foreach (var convertion in item.Conversiones)
                        {
                            ConvertionHelper h = new ConvertionHelper();
                            try
                            {

                                h.Evaluate(JsonConvert.DeserializeObject<Formula>(convertion.Formula), ref testValue);
                            }
                            catch (Exception ex)
                            {
                                string g = ex.Message;
                            }


                        }

                        GalileoPostResultRequest rq = new GalileoPostResultRequest();

                        rq.IdLaboratorio = IdLaboratorio;
                        rq.CodigoOrden = OrderNumber;
                        rq.CodigoMuestra = CodigoMuestra;
                        rq.CodigoExamenHomologado = item.Homologacion;
                        rq.Validado = false;
                        try
                        {
                            rq.ValorNumero = Convert.ToDouble(testValue);
                            rq.ValorTexto = null;


                        }
                        catch
                        {
                            rq.ValorTexto = testValue;
                        }

                        rq.Identificador = insMngr.Instrumento.Identificador;
                        rq.Orden = item.Orden;


                        results.Add(rq);


                    }


                }


            }


            EventWriter(false, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
            var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
            EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");



        }

        private void ProcessOrder(ASTMMessage msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);






            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);


            JsonHelper message = new JsonHelper(jsonResult);

            string sender = message.GetValue("header/SenderNameId");





            List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();

            foreach (var patient in msg.PatienInformationList)
            {
                foreach (var order in patient.OrderRecordList)
                {
                    string BarCode;
                    string OrderNumber;
                    string CodigoMuestra;

                    try
                    {
                        JsonHelper orderJson = new JsonHelper(JsonConvert.SerializeObject(order));

                        BarCode = orderJson.GetValue(OrderField);
                        OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                        CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                    }
                    catch
                    {
                        EventWriter(true, "No se encuentra el path: " + OrderField + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                        return;
                    }



                    string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
                    if (Directory.Exists(ImagePath))
                    {
                        Directory.Delete(ImagePath, true);
                        Directory.CreateDirectory(ImagePath);
                    }

                    foreach (var result in order.ResultRecordList)
                    {
                        JsonHelper lineJson = new JsonHelper(JsonConvert.SerializeObject(result));
                        string testName;
                        string testValue;

                        try
                        {
                            testName = lineJson.GetValue(TestName);
                            testValue = lineJson.GetValue(TestValue);
                        }
                        catch
                        {
                            EventWriter(true, "No se encuentra el path: " + TestName + "-" + TestValue + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                            return;
                        }

                        if (ConnectionSetup == "SERVER")
                        {
                            foreach (var item in insMngr.Instrumento.DetallesInstrumento)
                            {
                                if (item.Homologacion == testName)
                                {
                                    foreach (var convertion in item.Conversiones)
                                    {
                                        ConvertionHelper h = new ConvertionHelper();
                                        try
                                        {

                                            h.Evaluate(JsonConvert.DeserializeObject<Formula>(convertion.Formula), ref testValue);
                                        }
                                        catch (Exception ex)
                                        {
                                            string g = ex.Message;
                                        }


                                    }

                                    GalileoPostResultRequest rq = new GalileoPostResultRequest();

                                    rq.IdLaboratorio = IdLaboratorio;
                                    rq.CodigoOrden = OrderNumber;
                                    rq.CodigoMuestra = CodigoMuestra;
                                    rq.CodigoExamenHomologado = item.Homologacion;
                                    rq.Validado = false;
                                    try
                                    {
                                        rq.ValorNumero = Convert.ToDouble(testValue);
                                        rq.ValorTexto = null;


                                    }
                                    catch
                                    {
                                        rq.ValorTexto = testValue;
                                    }

                                    rq.Identificador = insMngr.Instrumento.Identificador;
                                    rq.Orden = item.Orden;

                                    EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                                    results.Add(rq);


                                }


                            }
                        }

                    }
                }


                EventWriter(false, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
                var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
                EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");

            }




            //foreach (var result in msg.ObservationResultList)
            //{
            //    if (result.ValueType == "ED")//Imagen
            //    {
            //        ImageHelper iHelper = new ImageHelper();
            //        iHelper.SaveImage(result.ObservationValue.Value, ImagePath, result.ObservationIdentifier.Text + ".bmp");
            //    }

            //    JObject rsss = JObject.Parse(JsonConvert.SerializeObject(result));

            //    var TestNameValue = (string)rsss[TestName];

            //   /* GalileoPostResultRequest r = new GalileoPostResultRequest();
            //    r.CodigoExamenHomologado = result.ObservationIdentifier.Identifier;
            //    r.CodigoMuestra = msg.ObservationRequest.

            //    */

            //}
        }


        private void ListenSerial()
        {


            SetState(false, "Conectado");

            EventWriter(true, "Escuchando el puerto: " + COM + " ", LogEvent.Conectado, "Conectado");

            mySerialPort = new SerialPort(COM);
            try
            {

                mySerialPort.BaudRate = Convert.ToInt32(Bauds);
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = Convert.ToInt32(DataBits);
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;



                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                mySerialPort.Open();

            }
            catch (Exception ex)
            {
                mySerialPort.Close();
                SetState(false, "Error al Conectar");

                EventWriter(true, "Error al conectar en el puerto: " + COM, LogEvent.Conectado, "Error");
                EventWriter(false, "Error al conectar en el puerto: " + COM + " " + ex.Message, LogEvent.Conectado, "Error");
            }
        }


        public void AppendRXTextBox(string value)
        {
            string text = value;
            switch (text)
            {
                case ASTM.Character.ENQ:
                    text = "<ENQ>";
                        break;

                case ASTM.Character.ACK:
                    text = "<ACK>";
                    break;

                case ASTM.Character.EOT:
                    text = "<EOT>";
                    break;

                case ASTM.Character.STX:
                    text = "<STX>";
                    break;

                case ASTM.Character.ETX:
                    text = "<ETX>";
                    break;

                default:
                    break;
            }

            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendRXTextBox), new object[] { text });
                return;
            }
            receiveTxt.Text = text;
            Thread.Sleep(20);
        }

        public void AppendMessageTextBox(string value)
        {
            if (value != ASTM.Character.EOT)
            {
                string text = ASTM.TranslateMessage(value);

                if (InvokeRequired)
                {
                    this.Invoke(new Action<string>(AppendMessageTextBox), new object[] { text });
                    return;
                }
                msgText.Text += text;
            }
            else
            {
                ReadMessage(msgText.Text);
                ClearMessageTextBox("");
            }
        }

        public void  ClearMessageTextBox(string value)
        {
           
              

                if (InvokeRequired)
                {
                    this.Invoke(new Action<string>(ClearMessageTextBox), new object[] { value });
                    return;
                }
                msgText.Text = "";
           
        }


        public delegate object GetTextBoxValueDelegate();

        public object GetRXTextBoxValue()
        {
            if (InvokeRequired)
                return Invoke(new GetTextBoxValueDelegate(GetRXTextBoxValue));
            return receiveTxt.Text;
        }

        public void AppendTXTextBox(string value)
        {

            string text = value;
            switch (text)
            {
                case ASTM.Character.ENQ:
                    text = "<ENQ>";
                    break;

                case ASTM.Character.ACK:
                    text = "<ACK>";
                    break;

                case ASTM.Character.EOT:
                    text = "<EOT>";
                    break;

                default:
                    break;
            }

            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTXTextBox), new object[] { text });
                return;
            }
            sendTxt.Text = text;
        }


        private  void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {

            ASCIIHelper hlpAs = new ASCIIHelper();

            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            StringBuilder sb = new StringBuilder();
            sb.Append(SerialMessage);

            if (data.Contains(ASTM.Character.ENQ))
            {
                sp.Write(ASTM.Character.ACK);
                return;
            }

            var chars = data.ToCharArray();

            


            foreach (var item in chars)
            {
                if (item.ToString() == ASTM.Character.EOT)
                {

                    string msg = sb.ToString();
                    ReadMessage(sb.ToString());
                    sb.Clear();


                }
                else
                {
                    if (item.ToString() != ASTM.Character.ETX && item.ToString() != ASTM.Character.STX)
                    {
                        sb.Append(item);
                    }
                }
            }

            SerialMessage = sb.ToString();

            sp.Write(ASTM.Character.ACK);

        }

        public void SendSerialSession(string text)
        {

            Thread threadInput = new Thread(WriteSerial);
            threadInput.Start(text);

        }

        private void WriteSerial(object obj)
        {
            bool sessionOpened = false;
            bool waitingForAck = true;

            //Enviar ENQ
            AppendTXTextBox("\x05");
           
            while (waitingForAck)
            {
                Thread.Sleep(1000);

                string rx = GetRXTextBoxValue().ToString();

                if (rx == "\\x06")
                {
                    sessionOpened = true;
                    waitingForAck = false;
                    AppendRXTextBox("");

                }
                else
                {
                    sessionOpened = false;
                }
            }




            AppendTXTextBox(obj.ToString());
        }

        private void ListenFiles()
        {


            var appSettings = ConfigurationManager.AppSettings;




            FileHelper fHlp = new();
            ImageHelper iHlp = new();




            EventWriter(true, "Sensando la ruta: " + ProcessFolder + " ", LogEvent.Conectado, "Conectado");

            while (true)
            {

                string[] filePaths = Directory.GetFiles(ProcessFolder);

                foreach (var file in filePaths)
                {
                    try
                    {
                        var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                        var newFile = ProcessedFolder + "\\" + DateTime.Now.Year.ToString().PadLeft(4, Convert.ToChar("0")) + DateTime.Now.Month.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Day.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Hour.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Minute.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Second.ToString().PadLeft(2, Convert.ToChar("0")) + fileStream.Name.Split("\\")[fileStream.Name.Split("\\").Length - 1];

                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            var data = streamReader.ReadToEnd();
                            EventWriter(true, "Se ha encontrado el archivo " + file, LogEvent.Recibiendo, "Recibiendo");

                            //EventWriter(true, data, LogEvent.Recibiendo, "Recibiendo");

                            Thread.Sleep(1000);

                            ReadMessage(data);



                            //EventWriter(true, "Fin del Mensaje", LogEvent.Recibiendo, "Conectado");



                        }
                        File.Move(file, newFile);
                        EventWriter(true, "Se mueve el archivo: " + newFile, LogEvent.Recibiendo, "Conectado");
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }


            }






            




        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetState(true, "Conectado");
        }

        private void contextMenuStripeConfig_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show(e.ClickedItem.Text);
        }

        private void frmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void logScreen_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void contextMenuStripLog_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void contextMenuStripLog_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Limpiar Log":
                    logScreen.Text = "";
                    break;
                
                default:
                    break;
            }
        }

        private void logScreen_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void contextMenuStripWorkList_Click(object sender, EventArgs e)
        {
            
        }

        private void contextMenuStripWorkList_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Ingresar Muestra":
                    var muestra = Interaction.InputBox("Ingrese el código de muestra", "Buscar Muestra");
                    if (muestra != "")
                    {
                        insMngr.GetOrden(muestra);

                    }

                    break;

                case "Lista de Trabajo":
                    Forms.frmWorkList form = new Forms.frmWorkList();

                    form.ShowDialog(this);

                    break;


                case "Enviar Resultado":
                    Forms.frmEnviarResultado formResultado = new Forms.frmEnviarResultado();

                    formResultado.ShowDialog(this);

                    break;
                default:
                    break;
            }
            
        }

        private void receiveTxt_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void sendTxt_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void msgText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
