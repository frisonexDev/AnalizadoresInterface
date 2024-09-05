
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
using Galileo.Protocol3_1;
using Galileo.Utils.ConversionModel;

namespace Galileo.Online
{
    public partial class MainForm : Form
    {

        public string ServerLIS { get { return ConfigurationManager.AppSettings["ServerLIS"]; } }

        public string ServerSeguridad { get { return ConfigurationManager.AppSettings["ServerSeguridad"]; } }


        public string Instrumento { get { return ConfigurationManager.AppSettings["Instrumento"]; } }
        public string Sender { get { return ConfigurationManager.AppSettings["Sender"]; } }
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

        public int IdLaboratorio;
        public string IdInstrumento;
        public string ApiToken;
        public string SerialMessage;


        SerialPort mySerialPort;

        private static string mensaje;

        private static string respuesta;
        private static string estatus;


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

            InicializarApp();

        }

        private void InicializarApp()
        {
            string LogPath = Environment.CurrentDirectory + "\\logs";
            string CapPath = Environment.CurrentDirectory + "\\data";

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
                            foreColor = Color.LightGray;
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

                log.Add(textLog, includeDateLog);

                stateProgressBar.Value = 100;

                statusLabel.Text = statusText;

            });

        }

        private void EventWriterRxTx(string emisor , string textLog,  bool includeDateLog = true)
        {

            string imprimir = textLog.Replace("<CR><LF>", "<CR>\n");
            imprimir = imprimir.Replace("<LF><CR>", "<CR>\n");
            imprimir = imprimir.Replace("<CR>", "<CR>\n");
            imprimir = imprimir.Replace("<LF>", "<CR>\n");

            //string screenLog = DateTime.Now + "\t" + textLog;
            this.Invoke((MethodInvoker)delegate
            {

                var msgs = imprimir.Split("\n",StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in msgs)
                {
                    Color foreColor = Color.White;


                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.SelectionLength = 0;

                    logScreen.SelectionColor = foreColor;
                    logScreen.AppendText(DateTime.Now + "\t" + emisor + ":\t" +  item);
                    logScreen.AppendText(Environment.NewLine);
                    logScreen.SelectionColor = logScreen.ForeColor;


                    log.Add(DateTime.Now + "\t" + emisor + ":\t" + item, includeDateLog);

                }





            });

        }

        private void EventWriterRxTxDebug(string emisor, string textLog, bool includeDateLog = true)
        {

            //string imprimir = textLog.Replace("<CR><LF>", "<CR>\n");
            //imprimir = imprimir.Replace("<LF><CR>", "<CR>\n");
            //imprimir = imprimir.Replace("<CR>", "<CR>\n");
            //imprimir = imprimir.Replace("<LF>", "<CR>\n");

            //string screenLog = DateTime.Now + "\t" + textLog;
            this.Invoke((MethodInvoker)delegate
            {

               
                    Color foreColor = Color.White;


                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.SelectionLength = 0;

                    logScreen.SelectionColor = foreColor;
                    logScreen.AppendText(DateTime.Now + "\t" + emisor + ":\t" + textLog);
                    logScreen.AppendText(Environment.NewLine);
                    logScreen.SelectionColor = logScreen.ForeColor;


                    log.Add(DateTime.Now + "\t" + emisor + ":\t" + textLog, includeDateLog);

                





            });

        }


        private System.Net.Sockets.Socket listener;
        private IPEndPoint endPoint;

        private static readonly byte[] Localhost = { 192, 168, 5, 54 };
        private const int Port = 6501;

        private void ListenTCP_2()
        {

            System.Net.IPAddress address = new IPAddress(Localhost);
            System.Net.IPEndPoint endPoint = new IPEndPoint(address, Port);

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            Console.WriteLine("Listening to port {0}", endPoint);
            listener.Listen(3);
            // Declare your variables.
            // Do not declare variables inside loops like for, foreach, while etc.
            // Because with every iteration, a new variable will be created.
            // If your loop iterates 1000 times, you will end up creating 1000 variables instead of just one variable.
            byte[] buffer;
            int count;
            string data;
            string tempData;
            string response = String.Empty;
            int start;
            int end;
            try
            {
                // true here make sure that the thread keep listening to the port.
                while (true)
                {
                    buffer = new byte[4096];

                    // Take care of incoming connection ...
                    Socket receiver = listener.Accept();
                    Console.WriteLine("Taking care of incoming connection.");
                    // Handle the message if one is received.
                    while (true)
                    {
                        count = receiver.Receive(buffer);
                        data = Encoding.UTF8.GetString(buffer, 0, count);

                        // Search for a Vertical Tab (VT) character to find start of MLLP frame.
                        start = data.IndexOf((char)0x0b);
                        if (start >= 0)
                        {
                            // Search for a File Separator (FS) character to find the end of the frame.
                            end = data.IndexOf((char)0x1c);
                            if (end > start)
                            {
                                // Remove the MLLP charachters
                                tempData = Encoding.UTF8.GetString(buffer, 4, count - 12);
                                // Do what you want with the received message
                                //response = HandleMessage(tempData);

                                // Send response
                                receiver.Send(Encoding.UTF8.GetBytes(response));
                                Console.WriteLine("Acknowledgment sent.");
                                break;
                            }
                        }
                    }

                    // close connection
                    receiver.Shutdown(SocketShutdown.Both);
                    receiver.Close();

                    Console.WriteLine("Connection closed.");
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                // Exception handling
            }
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
            mensaje = "";

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

                            EventWriter(true, data, LogEvent.Conectado, "Recibiendo", false);
                            //EventWriter(false, "Recibiendo: " + LocalPort, LogEvent.Conectado, "Recibiendo");


                            if (data.Contains(Convert.ToChar("\x1C")))
                            {
                                mensaje += data;

                                EventWriterRxTx("CMD600", ASTM.WriteTxRx(mensaje), true);

                                HL7Helper helper = new HL7Helper();
                                helper.Content = mensaje;
                                HL7Record obj = helper.ToObject();

                                foreach (var item in obj.messages)
                                {
                                    if (item.Header.MessageType == "ORU^R01") //Envio de resultados
                                    {
                                        // Procesar resultado

                                        ProcessOrder(item);
                                        {
                                            //Formar Respuesta OK;
                                            HL7Message resp = new HL7Message();

                                            resp.Header = new Utils.HL7Model.MessageHeader();
                                            resp.Header.FieldSeparator = "^~\\&";
                                            resp.Header.SendingApplication = "FRISONEX";
                                            resp.Header.SendingFacility = "GALILEO";
                                            resp.Header.DateTimeOfMessage = DateTime.Now.ToString("yyyyMMddHHmmss");
                                            resp.Header.MessageType = "ACK^R01";
                                            resp.Header.MessageControlId = item.Header.MessageControlId;
                                            resp.Header.ProcessingId = "P";
                                            resp.Header.VersionId = "2.3.1";
                                            resp.Header.AppACK = "0";
                                            resp.Header.CharacterSet = "ASCII";

                                            MessageAcknowledgmentSegment msa = new MessageAcknowledgmentSegment
                                            {
                                                AcknowledgmentCode = "AA",
                                                ConfirmationCode = "1",
                                                MessageControlID = item.Header.MessageControlId,
                                                TextMessage = "Message accepted",
                                                ExpectedSequenceNumber = "",
                                                DelayedAcknowledgmentType = "",
                                                ErrorCondition = "0"
                                            };

                                            resp.MessageAcknowledgment = msa;

                                            //MSA|AA|1|Message accepted|||0|<

                                            //EventWriter(true, "Preparando la respuesta a la recepción", LogEvent.Conectado, "Conectado");

                                            Thread.Sleep(5000);

                                            string message = resp.Serializar();
                                            string softMessage = ASTM.WriteSpecialChars(message);

                                            EventWriterRxTx("HOST", softMessage, true);

                                            byte[] data = Encoding.UTF8.GetBytes(message);

                                            byte[] hl7Data = Encoding.UTF8.GetBytes(message);
                                            int dataLength = hl7Data.Length;
                                            byte[] dataToSend = new byte[dataLength + 3];
                                            dataToSend[0] = 0x0b; // Add a Vertical Tab (VT) character
                                            Array.Copy(hl7Data, 0, dataToSend, 1, dataLength);
                                            dataToSend[dataLength + 1] = 0x1c; // Add File Separator (FS) charachter
                                            dataToSend[dataLength + 2] = 0x0d; // Add carriage return (CR) charachter

                                            try
                                            {
                                                stream.Write(dataToSend);
                                            }

                                            catch (IOException ex)
                                            {
                                                // Si se produce una excepción, maneja el error
                                                Console.WriteLine("Error al enviar el mensaje: " + ex.Message);
                                            }
                                        }

                                        {
                                            //Formar Respuesta OK 2 // Esto es raro pero el equipo ya no envio alarma. Para el Luis del futuro o para quien le aesto :D;

                                            HL7Message resp = new HL7Message();

                                            resp.Header = new Utils.HL7Model.MessageHeader();
                                            resp.Header.FieldSeparator = "^~\\&";
                                            //resp.Header.FieldSeparator = "|";
                                            resp.Header.SendingApplication = "FRISONEX";
                                            resp.Header.SendingFacility = "GALILEO";
                                            resp.Header.DateTimeOfMessage = DateTime.Now.ToString("yyyyMMddHHmmss");
                                            resp.Header.MessageType = "ACK^Q03";
                                            resp.Header.MessageControlId = item.Header.MessageControlId;
                                            resp.Header.ProcessingId = "P";
                                            resp.Header.VersionId = "2.3.1";
                                            resp.Header.AppACK = "0";
                                            resp.Header.CharacterSet = "ASCII";

                                            MessageAcknowledgmentSegment msa = new MessageAcknowledgmentSegment
                                            {
                                                AcknowledgmentCode = "AR",
                                                ConfirmationCode = "1",
                                                MessageControlID = item.Header.MessageControlId,
                                                TextMessage = "Unsopported message type",
                                                ExpectedSequenceNumber = "",
                                                DelayedAcknowledgmentType = "",
                                                ErrorCondition = "200"
                                            };

                                            resp.MessageAcknowledgment = msa;

                                            //MSA|AA|1|Message accepted|||0|<

                                            //EventWriter(true, "Preparando la respuesta a la recepción", LogEvent.Conectado, "Conectado");

                                            Thread.Sleep(300);

                                            string message = resp.Serializar();
                                            string softMessage = ASTM.WriteSpecialChars(message);

                                            EventWriterRxTx("HOST", softMessage, true);

                                            byte[] data = Encoding.UTF8.GetBytes(message);

                                            byte[] hl7Data = Encoding.UTF8.GetBytes(message);
                                            int dataLength = hl7Data.Length;
                                            byte[] dataToSend = new byte[dataLength + 3];
                                            dataToSend[0] = 0x0b; // Add a Vertical Tab (VT) character
                                            Array.Copy(hl7Data, 0, dataToSend, 1, dataLength);
                                            dataToSend[dataLength + 1] = 0x1c; // Add File Separator (FS) charachter
                                            dataToSend[dataLength + 2] = 0x0d; // Add carriage return (CR) charachter

                                            try
                                            {
                                                stream.Write(dataToSend);
                                            }

                                            catch (IOException ex)
                                            {
                                                // Si se produce una excepción, maneja el error
                                                Console.WriteLine("Error al enviar el mensaje: " + ex.Message);
                                            }
                                        }

                                        // Enviar el mensaje al servidor
                                        //stream.Write(data, 0, data.Length);
                                    }

                                    else if (item.Header.MessageType == "QRY^Q02") //Query
                                    {

                                        //Thread.Sleep(4000);

                                        // Consultar la Orden

                                        EventWriter(true, "Consultando al LIS la orden: " + item.QueryDef.WhoSubjectFilter, LogEvent.Recibiendo, "Recibiendo");

                                        var orderQuery = ProcessQuery(item.QueryDef.WhoSubjectFilter);
                                        var queryId = item.QueryDef.QueryID;

                                        //Mensaje QCKQ02
                                        {

                                            Thread.Sleep(50);
                                            HL7Message resp = new HL7Message();

                                            resp.Header = new Utils.HL7Model.MessageHeader();
                                            resp.Header.FieldSeparator = "^~\\&";
                                            resp.Header.SendingApplication = "";
                                            resp.Header.SendingFacility = "";
                                            resp.Header.ReceivingApplication = "";
                                            resp.Header.DateTimeOfMessage = item.Header.DateTimeOfMessage;//DateTime.Now.AddMinutes(4).ToString("yyyyMMddHHmmss");
                                            resp.Header.MessageType = "QCK^Q02";
                                            resp.Header.MessageControlId = item.Header.MessageControlId;
                                            resp.Header.ProcessingId = "P";
                                            resp.Header.VersionId = "2.3.1";
                                            resp.Header.AppACK = "";
                                            resp.Header.CharacterSet = "ASCII";

                                            MessageAcknowledgmentSegment msa = new MessageAcknowledgmentSegment
                                            {
                                                AcknowledgmentCode = "AA",
                                                ConfirmationCode = "",
                                                MessageControlID = item.Header.MessageControlId,
                                                TextMessage = "Message accepted",
                                                ExpectedSequenceNumber = "",
                                                DelayedAcknowledgmentType = "",
                                                ErrorCondition = "0"
                                            };

                                            ErrorSegment err = new ErrorSegment
                                            {
                                                ErrorCode = "0"
                                            };

                                            QueryAcknowledgmentSegment qak = new QueryAcknowledgmentSegment
                                            {
                                                QueryTag = "SR",
                                                QueryResponse = "OK"
                                            };

                                            resp.MessageAcknowledgment = msa;
                                            resp.ErrorACK = err;
                                            resp.QueryACK = qak;

                                            var VT = char.ConvertFromUtf32(11);
                                            var FS = char.ConvertFromUtf32(28);
                                            var CR = char.ConvertFromUtf32(13);

                                            string message = VT + resp.Serializar() + FS + CR;
                                            string softMessage = ASTM.WriteSpecialChars(message);
                                            EventWriterRxTx("HOST", softMessage, true);
                                            //EventWriterRxTxDebug("DEBUG", message, true);

                                            byte[] hl7Data = Encoding.ASCII.GetBytes(message);

                                            try
                                            {
                                                stream.Write(hl7Data);
                                            }

                                            catch (IOException ex)
                                            {
                                                // Si se produce una excepción, maneja el error
                                                Console.WriteLine("Error al enviar el mensaje: " + ex.Message);
                                            }
                                        }


                                        // Mensaje DSRQ03

                                        {
                                            Thread.Sleep(50);
                                            HL7Message resp = new HL7Message();

                                            resp.Header = new Utils.HL7Model.MessageHeader();
                                            resp.Header.FieldSeparator = "^~\\&";
                                            resp.Header.SendingApplication = "";
                                            resp.Header.SendingFacility = "";
                                            resp.Header.ReceivingFacility = "";
                                            resp.Header.DateTimeOfMessage = item.Header.DateTimeOfMessage; //DateTime.Now.AddMinutes(4).ToString("yyyyMMddHHmmss");
                                            resp.Header.MessageType = "DSR^Q03";
                                            resp.Header.MessageControlId = item.Header.MessageControlId;
                                            resp.Header.ProcessingId = "P";
                                            resp.Header.VersionId = "2.3.1";
                                            resp.Header.AppACK = "";
                                            resp.Header.CharacterSet = "ASCII";

                                            MessageAcknowledgmentSegment msa = new MessageAcknowledgmentSegment
                                            {
                                                AcknowledgmentCode = "AA",
                                                ConfirmationCode = "",
                                                MessageControlID = item.Header.MessageControlId,
                                                TextMessage = "Message accepted",
                                                ExpectedSequenceNumber = "",
                                                DelayedAcknowledgmentType = "",
                                                ErrorCondition = "0"
                                            };

                                            ErrorSegment err = new ErrorSegment
                                            {
                                                ErrorCode = "0"
                                            };

                                            QueryAcknowledgmentSegment qak = new QueryAcknowledgmentSegment
                                            {
                                                QueryTag = "SR",
                                                QueryResponse = "OK"
                                            };

                                            QueryDefinitionSegment qrd = new QueryDefinitionSegment
                                            {
                                                QueryDateTime = DateTime.Now.AddMinutes(4).ToString("yyyyMMddHHmmss"),
                                                QueryFormatCode = "R",
                                                QueryPriority = "D",
                                                QueryID = queryId,
                                                DeferredResponseType = "",
                                                DeferredResponseDateTime = "",
                                                QuantityLimitedRequest = "RD",
                                                WhoSubjectFilter = item.QueryDef.WhoSubjectFilter,
                                                WhatSubjectFilter = "OTH",
                                                WhatDepartmentDataCode = "",
                                                WhatDataCodeValueQualifier = "",
                                                QueryResultsLevel = "T"
                                            };

                                            //QRF|.|.|.|.|.|RCT|COR|ALL||<CR
                                            QueryFilterSegment qrf = new QueryFilterSegment
                                            {
                                                WhereSubjectFilter = "BS-200",
                                                WhenDataStartDateTime = "",
                                                WhenDataEndDateTime = "",
                                                WhatUserQualifier = "",
                                                OtherQRYSubjectFilter = "",
                                                WhichDateTimeQualifier = "RCT",
                                                WhichDateTimeStatusQualifier = "COR",
                                                DateTimeSelectionQualifier = "ALL",
                                                WhenQuantityTimingQualifier = ""
                                            };


                                            //DSP | 1 || 1212 |.|.|< CR >


                                            // Crear objetos para los elementos de la tabla

                                            var or = orderQuery[0];

                                            string patientID = or.Identificador;// "1212";1
                                            string bedNumber = "";//2
                                            string patientName = or.NombrePaciente;//3
                                            string dateOfBirth = or.FechaNacimiento.ToString("yyyyMMddhhmmss");  //"19620824000000"; // Formato YYYYMMDDHHmmSS//4
                                            string sex = or.Genero;//"M";//5
                                            string bloodType = "";// 6
                                            string race = ""; // Dejar en blanco//7
                                            string patientAddress = ""; // Dejar en blanco //8
                                            string postalCode = ""; // Dejar en blanco//9
                                            string homePhoneNumber = ""; // Dejar en blanco //10
                                            string samplePosition = ""; // Dejar en blanco//11
                                            string sampleCollectionTime = ""; // Dejar en blanco//12
                                            string notUsed1 = ""; // Dejar en blanco/13
                                            string notUsed2 = ""; // Dejar en blanco/14
                                            string patientType = "";//"Outpatient";//15
                                            string socialSecurityNumber = ""; // Dejar en blanco//15
                                            string chargeType = "";//17
                                            string ethnicGroup = ""; //18 Dejar en blanco
                                            string birthPlace = ""; // 19Dejar en blanco
                                            string nationality = ""; // 20 Dejar en blanco
                                            string barCode = item.QueryDef.WhoSubjectFilter;//21"0019";
                                            string sampleID = queryId.ToString();//22
                                            string sendingTime = ""; // Dejar en blanco//23
                                            string statOrNot = "N";//24
                                            string notUsed3 = ""; //25 Dejar en blanco
                                            string sampleType = "Suero";
                                            string fetchDoctor = "";//27
                                            string fetchDepartment = "";//28

                                            string pruebas = "";

                                            foreach (var p in or.Resultados)
                                            {
                                                pruebas = pruebas + p.CodigoExamenHomologado + "^^^,";
                                            }

                                            string testInfo = pruebas.Substring(0, pruebas.Length - 1); ;//"1^^^,2^^^,5^^^";

                                            List<DisplayDataSegment> dspList = new List<DisplayDataSegment>();

                                            DisplayDataSegment dsp1 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "1",
                                                DisplayLevel = "",
                                                DataLine = patientID,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp1);

                                            DisplayDataSegment dsp2 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "2",
                                                DisplayLevel = "",
                                                DataLine = bedNumber,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp2);

                                            DisplayDataSegment dsp3 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "3",
                                                DisplayLevel = "",
                                                DataLine = patientName,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp3);

                                            DisplayDataSegment dsp4 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "4",
                                                DisplayLevel = "",
                                                DataLine = dateOfBirth,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp4);

                                            DisplayDataSegment dsp5 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "5",
                                                DisplayLevel = "",
                                                DataLine = sex,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp5);
                                            DisplayDataSegment dsp6 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "6",
                                                DisplayLevel = "",
                                                DataLine = bloodType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp6);

                                            DisplayDataSegment dsp7 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "7",
                                                DisplayLevel = "",
                                                DataLine = race,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp7);

                                            DisplayDataSegment dsp8 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "8",
                                                DisplayLevel = "",
                                                DataLine = patientAddress,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp8);

                                            DisplayDataSegment dsp9 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "9",
                                                DisplayLevel = "",
                                                DataLine = postalCode,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp9);

                                            DisplayDataSegment dsp10 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "10",
                                                DisplayLevel = "",
                                                DataLine = homePhoneNumber,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp10);

                                            DisplayDataSegment dsp11 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "11",
                                                DisplayLevel = "",
                                                DataLine = samplePosition,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp11);
                                            DisplayDataSegment dsp12 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "12",
                                                DisplayLevel = "",
                                                DataLine = sampleCollectionTime,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp12);

                                            DisplayDataSegment dsp13 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "13",
                                                DisplayLevel = "",
                                                DataLine = notUsed1,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp13);
                                            DisplayDataSegment dsp14 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "14",
                                                DisplayLevel = "",
                                                DataLine = notUsed2,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp14);

                                            DisplayDataSegment dsp15 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "15",
                                                DisplayLevel = "",
                                                DataLine = patientType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp15);
                                            DisplayDataSegment dsp16 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "16",
                                                DisplayLevel = "",
                                                DataLine = socialSecurityNumber,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp16);
                                            DisplayDataSegment dsp17 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "17",
                                                DisplayLevel = "",
                                                DataLine = chargeType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp17);

                                            DisplayDataSegment dsp18 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "18",
                                                DisplayLevel = "",
                                                DataLine = ethnicGroup,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp18);

                                            DisplayDataSegment dsp19 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "19",
                                                DisplayLevel = "",
                                                DataLine = birthPlace,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp19);
                                            DisplayDataSegment dsp20 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "20",
                                                DisplayLevel = "",
                                                DataLine = nationality,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp20);

                                            DisplayDataSegment dsp21 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "21",
                                                DisplayLevel = "",
                                                DataLine = barCode,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp21);

                                            DisplayDataSegment dsp22 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "22",
                                                DisplayLevel = "",
                                                DataLine = sampleID,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp22);

                                            DisplayDataSegment dsp23 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "23",
                                                DisplayLevel = "",
                                                DataLine = sendingTime,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };


                                            dspList.Add(dsp23);
                                            DisplayDataSegment dsp24 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "24",
                                                DisplayLevel = "",
                                                DataLine = statOrNot,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp24);

                                            DisplayDataSegment dsp25 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "25",
                                                DisplayLevel = "",
                                                DataLine = notUsed3,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp25);
                                            DisplayDataSegment dsp26 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "26",
                                                DisplayLevel = "",
                                                DataLine = sampleType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp26);

                                            DisplayDataSegment dsp27 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "27",
                                                DisplayLevel = "",
                                                DataLine = fetchDoctor,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp27);

                                            DisplayDataSegment dsp28 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "28",
                                                DisplayLevel = "",
                                                DataLine = fetchDepartment,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp28);
                                            var tests = testInfo.Split(",");

                                            int idx = 29;
                                            foreach (var t in tests)
                                            {
                                                DisplayDataSegment dsp = new DisplayDataSegment
                                                {
                                                    SetIDDSP = idx.ToString(),
                                                    DisplayLevel = "",
                                                    DataLine = t,
                                                    LogicalBreakPoint = "",
                                                    ResultID = ""
                                                };

                                                dspList.Add(dsp);

                                                idx++;
                                            }

                                            ContinuationPointerSegment dsc = new ContinuationPointerSegment
                                            {
                                                ContinuationPointer = ""
                                            };

                                            resp.MessageAcknowledgment = msa;
                                            resp.ErrorACK = err;
                                            resp.QueryACK = qak;
                                            resp.QueryDef = qrd;
                                            resp.QueryFilter = qrf;
                                            resp.DisplayDataList = dspList;
                                            resp.ContinuesPointer = dsc;

                                            //MSA|AA|1|Message accepted|||0|<

                                            var VT = char.ConvertFromUtf32(11);
                                            var FS = char.ConvertFromUtf32(28);
                                            var CR = char.ConvertFromUtf32(13);

                                            string message = VT + resp.Serializar() + FS + CR;
                                            string softMessage = ASTM.WriteSpecialChars(message);
                                            EventWriterRxTx("HOST", softMessage, true);
                                            //EventWriterRxTxDebug("DEBUG", message, true);
                                            //TRAMA ENVIAR ANALIZADOR
                                            byte[] hl7Data = Encoding.ASCII.GetBytes(message);

                                            try
                                            {
                                                stream.Write(hl7Data);
                                            }

                                            catch (IOException ex)
                                            {
                                                // Si se produce una excepción, maneja el error
                                                Console.WriteLine("Error al enviar el mensaje: " + ex.Message);
                                            }

                                            /*

                                            byte[] hl7Data = Encoding.UTF8.GetBytes(message);
                                            int dataLength = hl7Data.Length;
                                            byte[] dataToSend = new byte[dataLength + 3];
                                            dataToSend[0] = 0x0b; // Add a Vertical Tab (VT) character
                                            Array.Copy(hl7Data, 0, dataToSend, 1, dataLength);
                                            dataToSend[dataLength + 1] = 0x1c; // Add File Separator (FS) charachter
                                            dataToSend[dataLength + 2] = 0x0d; // Add carriage return (CR) charachter

                                            */

                                        }

                                    }
                                    else 
                                    {
                                        //MessageBox.Show(mensaje);
                                    }
                                }

                                mensaje = "";
                                EventWriter(true, "*****FIN DEL MENSAJE******", LogEvent.Recibiendo, "Esperando");
                            }
                            else
                            {
                                mensaje += data;
                            }
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

        private void SendTCPData(NetworkStream stream, string data)
        {

        }

        private void ReceiveTCPData(string data)
        {

        }

        private void ReadMessage(string data, ref string result)
        {
            //try
            //{

            if (data != "")
            {
                //EventWriter(true, data, LogEvent.Conectado, "Recibiendo", false);
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
                else if (Protocol == "ASTM 1394")
                {
                    ASTMMessage msg = new ASTMMessage(ASTM.CleanMessage(data));
                    if (msg.header != null && msg.header.RecordTypeId == "H")
                    {
                        if (msg.PatienInformationList.Count > 0)
                        {
                            foreach (var patient in msg.PatienInformationList)
                            {
                                foreach (var order in patient.OrderRecordList)
                                {
                                    if (order.ReportTypes == "F")
                                    {
                                        ProcessOrder(msg);
                                    }
                                }
                            }

                            result = "OK";
                        }
                        else if (msg.Query != null)
                        {
                            result = ProcessQuery(msg);
                        }
                    }

                    else
                    {
                        EventWriter(true, "Mensaje ASTM Omitido", LogEvent.Conectado, "Conectado");
                    }

                }
                else if (Protocol == "3.1PROTOCOL")
                {
                    var msgs = data.Split(ASTM.Character.ETX);
                    foreach (var item in msgs)
                    {
                        Protocol3_1Message msg = new Protocol3_1Message(item);
                        if (msg.resultRecord.details.Count > 0)
                        {
                            ProcessOrder(msg);
                            var str = item;
                        }
                    }
                }
            }
        }

        private List<OrdenMuestraResponse> ProcessQuery(string orderNumber)
        {
            string orden = "";
            string muestra = "";

            if (orderNumber.Contains("-"))
            {
                orden = orderNumber.Split("-", StringSplitOptions.TrimEntries)[0];
                muestra = orderNumber.Split("-", StringSplitOptions.TrimEntries)[1];
            }

            var respLIS = insMngr.GetOrdenMuestra(orden, muestra);

            return respLIS;
        }

            private string ProcessQuery(ASTMMessage msg)
        {

            string orden = "";
            string muestra = "";

            if (msg.Query.SpecimenId.Contains("-"))
            {
                orden = msg.Query.SpecimenId.Split("-", StringSplitOptions.TrimEntries)[0];
                muestra = msg.Query.SpecimenId.Split("-", StringSplitOptions.TrimEntries)[1];
            }


            List<OrdenMuestraResponse> respLIS = insMngr.GetOrdenMuestra(orden, muestra);

            if (respLIS.Count > 0)
            {
                OrdenMuestraResponse resp = respLIS[0];

                ASTMMessage r = new ASTMMessage();
                r.header = new Utils.ASTMModel.MessageHeader();
                r.header.RecordTypeId = "H";
                r.header.DelimiterDefinition = "|\\^&";
                //Message Control Id
                //Password
                r.header.SenderNameId = "GalileoLIS^^";
                //Software version
                //Serial Number
                //SenderStreet
                //Reserved
                //Telephone
                //Characteristics
                //ReceiverId
                //Comments
                r.header.ProcessingId = "SA";
                r.header.VersionNo = "1394-97";
                r.header.TimeOfMessage = DateTime.Now;

                PatientInformation p = new PatientInformation();
                p.RecordTypeId = "P";
                p.SequenceNumber = "1";
                p.PracticeAssignedPatientId = "";
                p.PatientId = resp.Identificador;
                p.PatientName = resp.ApellidoPaciente + "," + resp.NombrePaciente;
                p.ReservedField = "";
                p.BirthDate = resp.FechaNacimiento;
                p.Age = "2";
                p.AgeUnits = "Y";

                p.PatientSex = resp.Genero; //fEMALE
                                           //PatientRace
                                           //ReservedField
                                           //Phone
                                           //Physicioan
                                           //SpecialField1
                                           //Surface
                                           //Height
                                           //Wight
                p.Diagnosis = resp.Diagnostico;
                //Medications
                //Diet
                //PracticeField1
                //PracticeField2
                //admission and dis
                //Nature
                //Alt Diag
                //Religion
                //Marital
                //Isolation
                //Service
                //Instit
                //Dosage

                p.OrderRecordList = new List<OrderRecord>();

                OrderRecord o = new OrderRecord();

                o.RecordTypeId = "O"; //1
                o.SecuenceNumber = msg.Query.SecuenceNumber;//2 -- Secuencia en la que se hace el Query
                o.SpecimenId = "1^^";
                o.InstrumentSpecimenId = msg.Query.SpecimenId.ToString();
                //^^^AFP
                o.UniversalTestIdPartOne = "";

                // descomponer tramas

                string pruebas = "";
                int i = 1;

                foreach (var test in resp.Resultados)
                {
                    pruebas = pruebas + i.ToString() + "^" + test.CodigoExamenHomologado + "^^\\";
                    i++;
                }
                string testInfo = pruebas.Substring(0, pruebas.Length - 1);
                //o.UniversalTestIdPartTwo = "1^AFP^^\\2^CEA^^";

                o.UniversalTestIdPartTwo = testInfo;               

                o.UniversalTestIdPartThree = "";
                o.Priority = ASTM.Priority.R; //
                o.RequestedOrderderedDateTime = DateTime.Now;
                o.SpecimenCollectionDateTime = DateTime.Now;
                o.DateTimeSpecimenReceived = DateTime.Now;
                o.SpecimenDescriptor = "serum";

                o.OrderingPhysician = "";
                o.PhysicianTelephoneNo = "";//Sending Departnment
                o.ReportTypes = "F";

                p.OrderRecordList.Add(o);

                r.PatienInformationList = new List<PatientInformation>();
                r.PatienInformationList.Add(p);

                r.Terminator = new MessageTerminator();
                r.Terminator.RecordTypeId = "L";
                r.Terminator.SecuenceNumber = "1";
                r.Terminator.TerminationCode = "N";

                var result_astm = r.SerializeASTM1394_97();

                return result_astm;
            }
            else
            { return "NO HA Y RESPUESTA"; }    
        }

        private void ProcessOrder(HL7Message msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);

            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);

            JsonHelper message = new JsonHelper(jsonResult);

            string sender = message.GetValue("ObservationRequest/FillerOrderNumber");

            var BarCode = "";
            var OrderNumber = "";
            var CodigoMuestra = "";

            BarCode = msg.ObservationRequest.PlaceOrderNumber;
            //BarCode = "202402220001-3";

            if (BarCode.Contains("-"))
            {
                OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
            }
            else {

                OrderNumber = BarCode;
                CodigoMuestra = "1";
            }

            string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
            if (Directory.Exists(ImagePath))
            {
                Directory.Delete(ImagePath, true);
                Directory.CreateDirectory(ImagePath);
            }

            EventWriter(false, jsonResult, LogEvent.Esperando, "Procesando Mensaje");

            List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();

            foreach (var result in msg.ObservationResultList)
            {
                JsonHelper line = new JsonHelper(JsonConvert.SerializeObject(result));
                var testName = result.ObservationIdentifier.Content;
                var testValue = result.ObservationValue.Value.Replace(",",".");

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
                    JsonHelper orderJson = new JsonHelper(JsonConvert.SerializeObject(order));

                    var OrderNumber = "";
                    var CodigoMuestra = "";
                    var BarCode = order.InstrumentSpecimenId;

                    if (BarCode != null)
                    {
                        OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                        if (BarCode.Split(SeparadorMuestra).Length > 1)
                        {
                            CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                        }
                        else
                        {
                            CodigoMuestra = "0";
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encuentra la orden en el campo: " + OrderField, "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                        BarCode = order.InstrumentSpecimenId;
                        OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                        CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
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
                        var testName = result.TestIdentifier.TestName;
                        var testValue = result.DataMeasurement.Value;

                        var resultado = insMngr.Instrumento.DetallesInstrumento.Where(X => X.Homologacion == testName).FirstOrDefault();

                        if (resultado != null)
                        {
                            foreach (var convertion in resultado.Conversiones)
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
                            rq.CodigoExamenHomologado = resultado.Homologacion;
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
                            rq.Orden = resultado.Orden;

                            EventWriter(true, resultado.Homologacion + " : " + testValue + " : " + rq.CodigoOrden + " : " + rq.CodigoMuestra + " : " + rq.IdLaboratorio, LogEvent.Enviando, "Enviando a Galileo");
                            results.Add(rq);



                        }
                        else
                        {

                            EventWriter(true, "No hay homologación para la prueba: " + testName, LogEvent.Error, "Homologación");

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

        private void ProcessOrder(Protocol3_1Message msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);

            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);


            JsonHelper message = new JsonHelper(jsonResult);

            string sender = this.Sender;





            List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();





            var BarCode = message.GetValue(OrderField);

            var barCodeParts = BarCode.Split(SeparadorMuestra);

            var OrderNumber = barCodeParts[0];
            var CodigoMuestra = "";

            if (barCodeParts.Length >= 2)

                CodigoMuestra = barCodeParts[1];



            string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
            if (Directory.Exists(ImagePath))
            {
                Directory.Delete(ImagePath, true);
                Directory.CreateDirectory(ImagePath);
            }

            foreach (var result in msg.resultRecord.details)
            {
                JsonHelper lineJson = new JsonHelper(JsonConvert.SerializeObject(result));
                var testName = lineJson.GetValue(TestName);
                var testValue = lineJson.GetValue(TestValue);

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

            EventWriter(false, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
            var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
            EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");





        }



        private void ListenSerial()
        {

            estatus = "Esperando";

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
                mySerialPort.DtrEnable = true;
                mySerialPort.Encoding = Encoding.ASCII;
                mySerialPort.ReadTimeout = 9000;
                mySerialPort.NewLine = ASTM.Character.EOTB;
                mySerialPort.DiscardNull = true;
                mySerialPort.ReadBufferSize = 2048;




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









        public delegate object GetTextBoxValueDelegate();






        private string CleanSerialData(string data)
        {

            //string newData = data;
            //int index = 0;
            //newData = data.



            return "";

        }

        private void ActualizarLabelStatus(string mensaje)
        {
            if (lblStatus.InvokeRequired)
            {
                // Si se está accediendo al control desde un subproceso distinto,
                // utilizar Invoke o BeginInvoke para realizar la actualización en el subproceso de la interfaz de usuario
                lblStatus.Invoke(new Action<string>(ActualizarLabelStatus), mensaje);
            }
            else
            {
                // Actualizar el control directamente en el subproceso de la interfaz de usuario
                lblStatus.Text = mensaje;
            }
        }




        private void DataReceivedHandler(
                          object sender,
                          SerialDataReceivedEventArgs e)
        {



            ASCIIHelper hlpAs = new ASCIIHelper();

            SerialPort sp = (SerialPort)sender;


            string data = sp.ReadExisting();



            //EventWriter(true, "CM260 1000: " + ASTM.WriteSpecialChars(data), LogEvent.Enviando, "Recibiendo", true);


            if (data.Contains(ASTM.Character.EOT))

            {
                if (respuesta != "")
                {
                    string envio = ASTM.CharacterENQ();
                    EventWriter(true, "GALILEO: " + ASTM.WriteSpecialChars(envio), LogEvent.Enviando, "Enviando", true);
                    sp.Write(envio);
                    Thread.Sleep(1000);
                    estatus = "Enviando";

                    return;
                }
                else
                {
                    estatus = "Esperando";
                }
            }

            if (data.Contains(ASTM.Character.ENQ))
            {

                if (estatus == "Esperando")
                {
                    //EventWriter(true, "CM260: " + ASTM.WriteSpecialChars(data), LogEvent.Recibiendo, "Enviando", true);

                    EventWriterRxTx("CM260", ASTM.WriteSpecialChars(data), true);


                    Thread.Sleep(4000);
                    //EventWriter(true, "GALILEO:" + ASTM.WriteSpecialChars(ASTM.Character.ACK), LogEvent.Enviando, "Enviando", true);
                    //EventWriter(true, "GALILEO:" + ASTM.WriteSpecialChars(ASTM.Character.ACK), LogEvent.Enviando, "Enviando", true);

                    EventWriterRxTx("GALILEO", ASTM.WriteSpecialChars(ASTM.Character.ACK), true);


                    //EventWriter(true, "GALILEO:"  + ASTM.Character.ACK, LogEvent.Enviando, "Enviando", true);
                    sp.Write(ASTM.Character.ACK);

                    //EventWriter(true, "GALILEO:" + "TEST", LogEvent.Enviando, "Enviando", true);
                    mensaje = "";
                    return;
                }
                else if (estatus == "Recibiendo")
                {
                    //EventWriter(true, "CM260: " + ASTM.WriteSpecialChars(data), LogEvent.Recibiendo, "Enviando", true);
                    EventWriterRxTx("CM260", ASTM.WriteSpecialChars(data), true);

                    Thread.Sleep(2000);
                    //EventWriter(true, "GALILEO:" + ASTM.WriteSpecialChars(ASTM.Character.ACK), LogEvent.Enviando, "Enviando", true);
                    EventWriterRxTx("GALILEO", ASTM.WriteSpecialChars(ASTM.Character.ACK), true);
                    var response = ASTM.CharacterACK();
                    sp.Write(ASTM.CharacterACK());
                    estatus = "Recibiendo";
                    mensaje = "";
                }
                else if (estatus == "Enviando")
                {
                    //EventWriter(true, "CM260: " + ASTM.WriteSpecialChars(data), LogEvent.Recibiendo, "Enviando", true);
                    EventWriterRxTx("CM260", ASTM.WriteSpecialChars(data), true);

                    Thread.Sleep(4000);
                    //EventWriter(true, "GALILEO:" + ASTM.WriteSpecialChars(ASTM.Character.ACK), LogEvent.Enviando, "Enviando", true);
                    EventWriterRxTx("GALILEO", ASTM.WriteSpecialChars(ASTM.Character.NAK), true);
                    sp.Write(ASTM.CharacterNAK());
                    estatus = "Esperando";
                    mensaje = "";
                }
            }

            if (data.Contains(ASTM.Character.NAK))
            {
                //EventWriter(true, "CM260: " + ASTM.WriteSpecialChars(data), LogEvent.Recibiendo, "Enviando", true);
                EventWriterRxTx("CM260", ASTM.WriteSpecialChars(data), true);
                estatus = "Esperando";
                mensaje = "";
            }


            if (data.Contains(ASTM.Character.ACK))
            {
                //EventWriter(true, "CM260: " + ASTM.WriteSpecialChars(data), LogEvent.Recibiendo, "Enviando", true);
                EventWriterRxTx("CM260", ASTM.WriteSpecialChars(data), true);
                if (estatus == "Enviando")
                {
                    Thread.Sleep(2000);
                    string envio = ASTM.CharacterSTX() + respuesta + ASTM.CharacterETX();
                    EventWriterRxTx("CM260", ASTM.WriteSpecialChars(envio), true);
                    //EventWriter(true, "GALILEO: " + ASTM.WriteSpecialChars(envio), LogEvent.Enviando, "Enviando", true);
                    sp.Write(envio);
                    //Thread.Sleep(2000);
                    //envio = ASTM.CharacterEOT();
                    //EventWriter(true, "GALILEO: " + ASTM.WriteSpecialChars(envio), LogEvent.Enviando, "Enviando", true);
                    //sp.Write(envio);
                    estatus = "Esperando";
                    mensaje = "";
                    return;
                }
            }



            mensaje = mensaje + data;

            if (mensaje.Contains(ASTM.Character.STX))
            {
                if (mensaje.Contains(ASTM.Character.ETX))
                {

                    //EventWriter(true, "CM260:\n" + ASTM.WriteSpecialChars(mensaje), LogEvent.Recibiendo, "Enviando", true);
                    EventWriterRxTx("CM260", ASTM.WriteSpecialChars(mensaje), true);

                    Thread.Sleep(2000);
                    // EventWriter(true, "GALILEO:" + ASTM.WriteSpecialChars(ASTM.Character.ACK), LogEvent.Enviando, "Enviando", true);

                    EventWriterRxTx("GALILEO", ASTM.WriteSpecialChars(ASTM.Character.ACK), true);
                    sp.WriteLine(ASTM.Character.ACK);


                    string result1 = "";
                    try
                    {
                        //mensaje = "\u00021H|\\^&|||Mindray^^|||||||RQ|1394-97|20230927130401\n\rQ|9|^202310100001-3||||||||||O\n\rL|1|N\r\n\u0003";
                        ReadMessage(mensaje, ref result1);
                    }

                    catch (Exception ex)
                    {
                        EventWriter(true, ex.Message, LogEvent.Error, "Error", true);
                    }
                    mensaje = "";

                    if (result1 != "OK" && result1 != "Error")
                    {


                        //result1 = "1H|\\^&|||Mindray^^|||||||SA|1394-97|20090910102501\nP|1||PATIENT111||Smith^^||19600315^45^Y|M||keshi||||||||zhenduan||01|||||A1|002||||||||\nO|1|1^^|202310120003-3|1^CA125^^\\2^CEA^^|R|20090910135300|20090910125300|||||||20130715102431|Urine|Dr.Who|Department1||Dr.Tom||||||O|||||\n; L|1|N\n";



                        respuesta = result1;
                        //EventWriter(true, "GALILEO: " + ASTM.WriteSpecialChars(ASTM.CharacterENQ()), LogEvent.Recibiendo, "Enviando", true);

                        EventWriterRxTx("GALILEO", ASTM.WriteSpecialChars(ASTM.Character.ENQ), true);

                        estatus = "Enviando";
                        mensaje = "";


                    }
                    else
                    {
                        estatus = "Esperando";
                    }
                }
            }

        }




        private void DataReceivedHandler_1(
                           object sender,
                           SerialDataReceivedEventArgs e)
        {



            ASCIIHelper hlpAs = new ASCIIHelper();

            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            if (lblStatus.Text != "En Espera")
            {
                if (lblStatus.Text != "Recibiendo")
                {
                    if (lblStatus.Text == "Enviando")
                    {
                        if (data == ASTM.Character.ACK)
                        {

                            StringBuilder sb1 = new StringBuilder();

                            sb1.Append(ASTM.Character.STX);
                            sb1.Append(ASTM.Character.ETX);
                            sb1.Append(ASTM.Character.EOT);
                            var chars1 = sb1.ToString().ToCharArray();
                            sp.Write(sb1.ToString());
                            return;

                        }
                        else if (data.Contains(ASTM.Character.NAK))
                        {
                            EventWriter(true, "Elisys no recibió la orden", LogEvent.Error, "Enviando");
                            ActualizarLabelStatus("En Espera");

                        }

                        return;


                    }
                    else
                    {
                        sp.Write(ASTM.Character.NAK);
                        return;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(SerialMessage);

            if (data.Contains(ASTM.Character.ENQ))
            {
                ActualizarLabelStatus("Recibiendo");
                sp.Write(ASTM.Character.ACK);
                return;
            }

            var chars = data.ToCharArray();




            //foreach (var item in chars)
            for (int i = 0; i < chars.Length; i++)

            {
                var item = chars[i];

                if (item.ToString() == ASTM.Character.ETX)
                {

                    string msg = sb.ToString();
                    msg = ASTM.CleanMessage(msg);
                    ActualizarLabelStatus("En Espera");
                    string result = "";
                    ReadMessage(msg, ref result);

                    if (result != "") //Cuando es HOST QUERY devuelve el ASTM respuesta
                    {
                        ActualizarLabelStatus("Enviando");


                    }

                    sb.Clear();


                }
                else
                {

                    sb.Append(ASTM.TransalateEspecialCharacters(item.ToString()));





                }
            }

            string clean_data = sb.ToString();

            SerialMessage = clean_data;

            sp.Write(ASTM.Character.ACK);
            sb.Append("<ACK>");

        }

        public void SendSerialSession(string text)
        {



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

                            //Thread.Sleep(1000);
                            string result = "";
                            //ReadMessage(data, ref result);

                            if (data.Contains(Convert.ToChar("\x1C")))
                            {
                                mensaje += data;
                                
                                EventWriterRxTx("CM260", ASTM.WriteTxRx(mensaje), true);


                                HL7Helper helper = new HL7Helper();
                                helper.Content = mensaje;
                                HL7Record obj = helper.ToObject();

                                foreach (var item in obj.messages)
                                {
                                    if (item.Header.MessageType == "ORU^R01") //Envio de resultados
                                    {
                                        // Procesar resultado

                                        ProcessOrder(item);


                                        //Formar Respuesta;

                                        HL7Message resp = new HL7Message();

                                        resp.Header = new Utils.HL7Model.MessageHeader();
                                        resp.Header.FieldSeparator = "^~\\&";
                                        resp.Header.DateTimeOfMessage = DateTime.Now.ToString("yyyyMMddHHmmss");
                                        resp.Header.MessageType = "ACK^R01";
                                        resp.Header.MessageControlId = "1";
                                        resp.Header.ProcessingId = "P";
                                        resp.Header.VersionId = "2.3.1";
                                        resp.Header.AppACK = "0";
                                        resp.Header.CharacterSet = "ASCII";

                                        MessageAcknowledgmentSegment msa = new MessageAcknowledgmentSegment
                                        {
                                            AcknowledgmentCode = "AA",
                                            ConfirmationCode = "1",
                                            MessageControlID = "",
                                            TextMessage = "Message accepted",
                                            ExpectedSequenceNumber = "",
                                            DelayedAcknowledgmentType = "",
                                            ErrorCondition = "0"
                                        };

                                        resp.MessageAcknowledgment = msa;

                                        //MSA|AA|1|Message accepted|||0|<

                                        string message = ASTM.Character.SB + resp.Serializar() + ASTM.Character.EB + "\n";
                                        string softMessage = ASTM.WriteSpecialChars(message);
                                        //byte[] data = Encoding.ASCII.GetBytes(message);


                                        // Enviar el mensaje al servidor
                                        //stream.Write(data, 0, data.Length);
                                    }

                                    else if (item.Header.MessageType == "QRY^Q02") //Query
                                    {
                                       


                                        // Consultar la Orden

                                        EventWriter(true, "Consultando al LIS la orden: " + item.QueryDef.WhatSubjectFilter, LogEvent.Recibiendo, "Recibiendo");

                                        var orderQuery = ProcessQuery(item.QueryDef.WhatSubjectFilter);

                                        //Formar Respuesta Inicial;

                                        {
                                            HL7Message resp = new HL7Message();

                                            resp.Header = new Utils.HL7Model.MessageHeader();
                                            resp.Header.FieldSeparator = "^~\\&";
                                            resp.Header.DateTimeOfMessage = DateTime.Now.ToString("yyyyMMddHHmmss");
                                            resp.Header.MessageType = "QCK^Q02";
                                            resp.Header.MessageControlId = "4";
                                            resp.Header.ProcessingId = "P";
                                            resp.Header.VersionId = "2.3.1";
                                            resp.Header.AppACK = "0";
                                            resp.Header.CharacterSet = "ASCII";

                                            MessageAcknowledgmentSegment msa = new MessageAcknowledgmentSegment
                                            {
                                                AcknowledgmentCode = "AA",
                                                ConfirmationCode = "4",
                                                MessageControlID = "",
                                                TextMessage = "Message accepted",
                                                ExpectedSequenceNumber = "",
                                                DelayedAcknowledgmentType = "",
                                                ErrorCondition = "0"
                                            };

                                            ErrorSegment err = new ErrorSegment
                                            {
                                                ErrorCode = "0"
                                            };

                                            QueryAcknowledgmentSegment qak = new QueryAcknowledgmentSegment
                                            {
                                                QueryTag = "SR",
                                                QueryResponse = "OK"
                                            };


                                            resp.MessageAcknowledgment = msa;
                                            resp.ErrorACK = err;
                                            resp.QueryACK = qak;


                                            //MSA|AA|1|Message accepted|||0|<

                                            string message = ASTM.Character.SB + resp.Serializar() + ASTM.Character.EB + "\n";
                                            string softMessage = ASTM.WriteSpecialChars(message);
                                            EventWriterRxTx("HOST",softMessage,true);
                                            
                                            
                                            //byte[] data = Encoding.ASCII.GetBytes(message);


                                            // Enviar el mensaje al servidor
                                            //stream.Write(data, 0, data.Length);
                                        }


                                        //Formar Respuesta al Query;
                                        EventWriter(true, "Creando la respuesta al analizador para: " + item.QueryDef.WhatSubjectFilter, LogEvent.Recibiendo, "Recibiendo");

                                        {
                                            HL7Message resp = new HL7Message();

                                            resp.Header = new Utils.HL7Model.MessageHeader();
                                            resp.Header.FieldSeparator = "^~\\&";
                                            resp.Header.DateTimeOfMessage = DateTime.Now.ToString("yyyyMMddHHmmss");
                                            resp.Header.MessageType = "DSR^Q03";
                                            resp.Header.MessageControlId = "4";
                                            resp.Header.ProcessingId = "P";
                                            resp.Header.VersionId = "2.3.1";
                                            resp.Header.AppACK = "0";
                                            resp.Header.CharacterSet = "ASCII";

                                            MessageAcknowledgmentSegment msa = new MessageAcknowledgmentSegment
                                            {
                                                AcknowledgmentCode = "AA",
                                                ConfirmationCode = "4",
                                                MessageControlID = "",
                                                TextMessage = "Message accepted",
                                                ExpectedSequenceNumber = "",
                                                DelayedAcknowledgmentType = "",
                                                ErrorCondition = "0"
                                            };

                                            ErrorSegment err = new ErrorSegment
                                            {
                                                ErrorCode = "0"
                                            };

                                            QueryAcknowledgmentSegment qak = new QueryAcknowledgmentSegment
                                            {
                                                QueryTag = "SR",
                                                QueryResponse = "OK"
                                            };

                                            QueryDefinitionSegment qrd = new QueryDefinitionSegment
                                            {
                                                QueryDateTime = "20231013123456",
                                                QueryFormatCode = "R",
                                                QueryPriority = "D",
                                                QueryID = "2",
                                                DeferredResponseType = "",
                                                DeferredResponseDateTime = "",
                                                QuantityLimitedRequest = "RD",
                                                WhoSubjectFilter = "",
                                                WhatSubjectFilter = "OTH",
                                                WhatDepartmentDataCode = "",
                                                WhatDataCodeValueQualifier = "",
                                                QueryResultsLevel = "T"
                                            };

                                            //QRF|.|.|.|.|.|RCT|COR|ALL||<CR
                                            QueryFilterSegment qrf = new QueryFilterSegment
                                            {
                                                WhereSubjectFilter = "",
                                                WhenDataStartDateTime = "",
                                                WhenDataEndDateTime = "",
                                                WhatUserQualifier = "",
                                                OtherQRYSubjectFilter = "",
                                                WhichDateTimeQualifier = "RCT",
                                                WhichDateTimeStatusQualifier = "COR",
                                                DateTimeSelectionQualifier = "ALL",
                                                WhenQuantityTimingQualifier = ""
                                            };


                                            //DSP | 1 || 1212 |.|.|< CR >


                                            // Crear objetos para los elementos de la tabla

                                            var or = orderQuery[0];

                                            string patientID = or.Identificador;// "1212";
                                            string bedNumber = "";
                                            string patientName = or.NombrePaciente;
                                            string dateOfBirth = or.FechaNacimiento.ToString("yyyyMMddhhmmss");  //"19620824000000"; // Formato YYYYMMDDHHmmSS
                                            string sex = or.Genero;//"M";
                                            string bloodType = "";
                                            string race = ""; // Dejar en blanco
                                            string patientAddress = ""; // Dejar en blanco
                                            string postalCode = ""; // Dejar en blanco
                                            string homePhoneNumber = ""; // Dejar en blanco
                                            string samplePosition = ""; // Dejar en blanco
                                            string sampleCollectionTime = ""; // Dejar en blanco
                                            string notUsed1 = ""; // Dejar en blanco
                                            string notUsed2 = ""; // Dejar en blanco
                                            string patientType = "";//"Outpatient";
                                            string socialSecurityNumber = ""; // Dejar en blanco
                                            string chargeType = "";
                                            string ethnicGroup = ""; // Dejar en blanco
                                            string birthPlace = ""; // Dejar en blanco
                                            string nationality = ""; // Dejar en blanco
                                            string barCode = item.QueryDef.WhatSubjectFilter;//"0019";
                                            string sampleID = "";
                                            string sendingTime = ""; // Dejar en blanco
                                            string statOrNot = "N";
                                            string notUsed3 = ""; // Dejar en blanco
                                            string sampleType = "";
                                            string fetchDoctor = "";
                                            string fetchDepartment = "";

                                            string pruebas = "";

                                            foreach (var p in or.Resultados)
                                            {
                                                pruebas = pruebas + p.CodigoExamenHomologado + "^^^,";
                                            }

                                            string testInfo = pruebas.Substring(0, pruebas.Length - 1); ;//"1^^^,2^^^,5^^^";


                                            List<DisplayDataSegment> dspList = new List<DisplayDataSegment>();

                                            DisplayDataSegment dsp1 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "1",
                                                DisplayLevel = "",
                                                DataLine = patientID,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp1);


                                            DisplayDataSegment dsp2 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "2",
                                                DisplayLevel = "",
                                                DataLine = bedNumber,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp2);

                                            DisplayDataSegment dsp3 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "3",
                                                DisplayLevel = "",
                                                DataLine = patientName,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp3);

                                            DisplayDataSegment dsp4 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "4",
                                                DisplayLevel = "",
                                                DataLine = dateOfBirth,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp4);

                                            DisplayDataSegment dsp5 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "5",
                                                DisplayLevel = "",
                                                DataLine = sex,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp5);
                                            DisplayDataSegment dsp6 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "6",
                                                DisplayLevel = "",
                                                DataLine = bloodType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp6);

                                            DisplayDataSegment dsp7 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "7",
                                                DisplayLevel = "",
                                                DataLine = race,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp7);

                                            DisplayDataSegment dsp8 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "8",
                                                DisplayLevel = "",
                                                DataLine = patientAddress,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp8);

                                            DisplayDataSegment dsp9 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "9",
                                                DisplayLevel = "",
                                                DataLine = postalCode,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp9);

                                            DisplayDataSegment dsp10 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "10",
                                                DisplayLevel = "",
                                                DataLine = homePhoneNumber,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp10);

                                            DisplayDataSegment dsp11 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "11",
                                                DisplayLevel = "",
                                                DataLine = samplePosition,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp11);
                                            DisplayDataSegment dsp12 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "12",
                                                DisplayLevel = "",
                                                DataLine = sampleCollectionTime,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp12);

                                            DisplayDataSegment dsp13 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "13",
                                                DisplayLevel = "",
                                                DataLine = notUsed1,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp13);
                                            DisplayDataSegment dsp14 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "14",
                                                DisplayLevel = "",
                                                DataLine = notUsed2,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp14);

                                            DisplayDataSegment dsp15 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "15",
                                                DisplayLevel = "",
                                                DataLine = patientType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp15);
                                            DisplayDataSegment dsp16 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "16",
                                                DisplayLevel = "",
                                                DataLine = socialSecurityNumber,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp16);
                                            DisplayDataSegment dsp17 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "17",
                                                DisplayLevel = "",
                                                DataLine = chargeType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };


                                            dspList.Add(dsp17);

                                            DisplayDataSegment dsp18 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "18",
                                                DisplayLevel = "",
                                                DataLine = ethnicGroup,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp18);

                                            DisplayDataSegment dsp19 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "19",
                                                DisplayLevel = "",
                                                DataLine = birthPlace,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };


                                            dspList.Add(dsp19);
                                            DisplayDataSegment dsp20 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "20",
                                                DisplayLevel = "",
                                                DataLine = nationality,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp20);

                                            DisplayDataSegment dsp21 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "21",
                                                DisplayLevel = "",
                                                DataLine = barCode,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp21);

                                            DisplayDataSegment dsp22 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "22",
                                                DisplayLevel = "",
                                                DataLine = sampleID,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp22);

                                            DisplayDataSegment dsp23 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "23",
                                                DisplayLevel = "",
                                                DataLine = sendingTime,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };


                                            dspList.Add(dsp23);
                                            DisplayDataSegment dsp24 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "24",
                                                DisplayLevel = "",
                                                DataLine = statOrNot,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp24);

                                            DisplayDataSegment dsp25 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "25",
                                                DisplayLevel = "",
                                                DataLine = notUsed3,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp25);
                                            DisplayDataSegment dsp26 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "26",
                                                DisplayLevel = "",
                                                DataLine = sampleType,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };

                                            dspList.Add(dsp26);

                                            DisplayDataSegment dsp27 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "27",
                                                DisplayLevel = "",
                                                DataLine = fetchDoctor,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };


                                            dspList.Add(dsp27);

                                            DisplayDataSegment dsp28 = new DisplayDataSegment
                                            {
                                                SetIDDSP = "28",
                                                DisplayLevel = "",
                                                DataLine = fetchDepartment,
                                                LogicalBreakPoint = "",
                                                ResultID = ""
                                            };


                                            dspList.Add(dsp28);
                                            var tests = testInfo.Split(",");

                                            int idx = 29;
                                            foreach (var t in tests)
                                            {
                                                DisplayDataSegment dsp = new DisplayDataSegment
                                                {
                                                    SetIDDSP = idx.ToString(),
                                                    DisplayLevel = "",
                                                    DataLine = t,
                                                    LogicalBreakPoint = "",
                                                    ResultID = ""
                                                };

                                                dspList.Add(dsp);

                                                idx++;
                                            }





                                            ContinuationPointerSegment dsc = new ContinuationPointerSegment
                                            {
                                                ContinuationPointer = ""
                                            };


                                            resp.MessageAcknowledgment = msa;
                                            resp.ErrorACK = err;
                                            resp.QueryACK = qak;
                                            resp.QueryDef = qrd;
                                            resp.QueryFilter = qrf;
                                            resp.DisplayDataList = dspList;
                                            resp.ContinuesPointer = dsc;

                                            //MSA|AA|1|Message accepted|||0|<

                                            string message = ASTM.Character.SB + resp.Serializar() + ASTM.Character.EB + "\n";
                                            string softMessage = ASTM.WriteSpecialChars(message);
                                            EventWriterRxTx("HOST", softMessage, true);
                                            //byte[] data = Encoding.ASCII.GetBytes(message);


                                            // Enviar el mensaje al servidor
                                            //stream.Write(data, 0, data.Length);
                                        }




                                    }
                                }

                                mensaje = "";
                                EventWriter(true, "*****FIN DEL MENSAJE******", LogEvent.Recibiendo, "Esperando");

                            }
                            else
                            {
                                mensaje += data;
                            }




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

        private void button1_Click_1(object sender, EventArgs e)
        {
            //string response = ProcessQuery(null);
        }

        private void lblStatus_TextChanged(object sender, EventArgs e)
        {
            if (lblStatus.Text == "Recibiendo")
            {
                lblStatus.BackColor = Color.GreenYellow;
            }
            else if (lblStatus.Text == "En Espera")
            {
                lblStatus.BackColor = Color.Transparent;
            }

            else if (lblStatus.Text == "Enviando")
            {
                lblStatus.BackColor = Color.IndianRed;
            }

        }

        private void btnActivarEnvio_Click(object sender, EventArgs e)
        {
            ActualizarLabelStatus("Enviando");
            mySerialPort.Write(ASTM.Character.ENQ);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblStatus.Text = estatus;
        }
    }
}
