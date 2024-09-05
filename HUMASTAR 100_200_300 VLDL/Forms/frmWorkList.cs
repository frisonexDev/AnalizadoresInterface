﻿using Galileo.Connect.Model;
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




            var ds = result.Where(x => x.Estado == "Activo").OrderByDescending(x=>x.Fecha).ToList();

            //var ds = result.OrderByDescending(x => x.Fecha).ToList();

            dataGridView1.DataSource = ds;
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

        private void btnSendWorkList_Click(object sender, EventArgs e)
        {
            List<OrdenResponse> list = (List<OrdenResponse>)dataGridView1.DataSource;

            ASTMMessage msg = new ASTMMessage();
            msg.header = new MessageHeader("|", "\\", "^", "&");
            msg.header.RecordTypeId = "H";
            msg.header.SenderNameId = "GALILEO";
            //msg.header.DelimiterDefinition = "|\\^&"; //Revisar para ponerlo ene l config
            msg.header.ProcessingId = "P";
            msg.header.SegmentDelimeter = "^";
            msg.header.TimeOfMessage = DateTime.Now;

            msg.PatienInformationList = new List<PatientInformation>();

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {


                StringBuilder message = new StringBuilder();


                bool selected = (bool)item.Cells[0].Value;
                

                if (selected)
                {
                    string orden = item.Cells[1].FormattedValue.ToString();

                    OrdenResponse orderRq = (OrdenResponse) list.Where(x => x.CodigoOrden == orden).FirstOrDefault();


                    if (((MainForm)this.Owner).Protocol.Contains("HL7"))
                    {
                        MessageBox.Show(((MainForm)this.Owner).Protocol);
                    }
                    else if (((MainForm)this.Owner).Protocol.Contains("ASTM"))//Esto le estpy poniendo por la prueba lo que viene es ASTM
                    {
                        PatientInformation patient;
                        foreach (var or1 in orderRq.Detalles.OrderBy(x => x.CodigoMuestra))
                        {
                            patient = new PatientInformation();
                            patient.RecordTypeId = "P";
                            patient.SequenceNumber = (msg.PatienInformationList.Count + 1).ToString();
                            patient.PatientId = orderRq.Identificacion; //Noesta enviando la identificacion.
                            patient.PatientName = orderRq.Apellido + " " + orderRq.Nombre;
                            patient.BirthDate = orderRq.FechaNacimiento;

                            patient.PatientSex = orderRq.Sexo;

                            patient.Diagnosis = "";

                            patient.OrderRecordList = new List<OrderRecord>();
                            patient.LaboratoryAssignedPatientId = orden + ((MainForm)this.Owner).insMngr.Instrumento.SpecimenSeparator + or1.CodigoMuestra; ;

                            int cont = msg.PatienInformationList.Where(x => x.LaboratoryAssignedPatientId == patient.LaboratoryAssignedPatientId).Count();
                            if ( cont == 0)
                            {

                                foreach (var or in orderRq.Detalles.Where(x => orden + ((MainForm)this.Owner).insMngr.Instrumento.SpecimenSeparator + x.CodigoMuestra == patient.LaboratoryAssignedPatientId))
                                {


                                    if (!or.CodigoExamenHomologado.Contains("*"))
                                    {

                                        OrderRecord order = new OrderRecord();
                                        order.RecordTypeId = "O";
                                        order.SecuenceNumber = (patient.OrderRecordList.Count + 1).ToString();
                                        order.InstrumentSpecimenId = orden + ((MainForm)this.Owner).insMngr.Instrumento.SpecimenSeparator + or.CodigoMuestra;
                                        order.UniversalTestIdPartOne = or.CodigoExamenHomologado;

                                        if (orderRq.Prioridad == "R")

                                            order.Priority = ASTM.Priority.R;
                                        else
                                            order.Priority = ASTM.Priority.S;

                                        order.RequestedOrderderedDateTime = orderRq.Fecha;
                                        order.SpecimenCollectionDateTime = orderRq.Fecha;
                                        order.LaboratoryField1 = "";
                                        patient.OrderRecordList.Add(order);

                                    }
                                }
                                msg.PatienInformationList.Add(patient);
                            }
                        }

                        //Especifico para H100
                        string patientId = "";


                            var actualMuestra = "";

                       


                       

                    }








                }

               
            }

            

            msg.Terminator = new MessageTerminator();
            msg.Terminator.RecordTypeId = "L";
            msg.Terminator.SecuenceNumber = "1";



            string streamText = msg.Serialize();

            string filename = "GALILEO_"  + DateTime.Now.ToString("yyyyMMddhhmmss") +  ".astm";

            ((MainForm)this.Owner).SendMessage(streamText, filename);
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                item.Cells[0].Value = chkSelectAll.Checked;
            }
        }
    }
}
