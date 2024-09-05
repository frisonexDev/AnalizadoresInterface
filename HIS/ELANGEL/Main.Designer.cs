
namespace Galileo_HIS
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnLoadSample = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.pgnOrdenes = new System.Windows.Forms.ProgressBar();
            this.lblOrdenes = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pdfRepPac = new Apitron.PDF.Controls.PDFViewer();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.btnAbrirFile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dateEnd = new System.Windows.Forms.DateTimePicker();
            this.dateStart = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkValidados = new System.Windows.Forms.CheckBox();
            this.cmbArea = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pdfViewer1 = new DevExpress.XtraPdfViewer.PdfViewer();
            this.niInfo = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnDatosPaciente = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // btnLoadSample
            // 
            this.btnLoadSample.Location = new System.Drawing.Point(93, 96);
            this.btnLoadSample.Name = "btnLoadSample";
            this.btnLoadSample.Size = new System.Drawing.Size(129, 33);
            this.btnLoadSample.TabIndex = 1;
            this.btnLoadSample.Text = "Generar Reportes PDF";
            this.btnLoadSample.UseVisualStyleBackColor = true;
            this.btnLoadSample.Click += new System.EventHandler(this.btnLoadSample_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Location = new System.Drawing.Point(24, 179);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(839, 419);
            this.dataGridView1.TabIndex = 6;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // pgnOrdenes
            // 
            this.pgnOrdenes.Location = new System.Drawing.Point(24, 604);
            this.pgnOrdenes.Name = "pgnOrdenes";
            this.pgnOrdenes.Size = new System.Drawing.Size(839, 23);
            this.pgnOrdenes.TabIndex = 7;
            // 
            // lblOrdenes
            // 
            this.lblOrdenes.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblOrdenes.Location = new System.Drawing.Point(24, 630);
            this.lblOrdenes.Name = "lblOrdenes";
            this.lblOrdenes.Size = new System.Drawing.Size(839, 30);
            this.lblOrdenes.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 15);
            this.label1.TabIndex = 9;
            // 
            // pdfRepPac
            // 
            this.pdfRepPac.Document = null;
            this.pdfRepPac.EnableSearch = true;
            this.pdfRepPac.Location = new System.Drawing.Point(869, 133);
            this.pdfRepPac.Name = "pdfRepPac";
            this.pdfRepPac.SearchHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.pdfRepPac.Size = new System.Drawing.Size(695, 465);
            this.pdfRepPac.TabIndex = 15;
            this.pdfRepPac.Visible = false;
            // 
            // btnAbrirFile
            // 
            this.btnAbrirFile.Location = new System.Drawing.Point(1488, 603);
            this.btnAbrirFile.Name = "btnAbrirFile";
            this.btnAbrirFile.Size = new System.Drawing.Size(75, 23);
            this.btnAbrirFile.TabIndex = 16;
            this.btnAbrirFile.Text = "Abr&ir Archivo";
            this.btnAbrirFile.UseVisualStyleBackColor = true;
            this.btnAbrirFile.Visible = false;
            this.btnAbrirFile.Click += new System.EventHandler(this.btnAbrirFile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.dateEnd);
            this.groupBox1.Controls.Add(this.dateStart);
            this.groupBox1.Controls.Add(this.btnLoadSample);
            this.groupBox1.Location = new System.Drawing.Point(24, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(329, 135);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Opciones Globales:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(228, 96);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 33);
            this.button2.TabIndex = 17;
            this.button2.Text = "Ver &carpeta";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 23);
            this.label3.TabIndex = 16;
            this.label3.Text = "Fecha Fin:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 23);
            this.label2.TabIndex = 15;
            this.label2.Text = "Fecha Inicio:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 96);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 33);
            this.button1.TabIndex = 14;
            this.button1.Text = "Consultar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // dateEnd
            // 
            this.dateEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateEnd.Location = new System.Drawing.Point(153, 52);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.Size = new System.Drawing.Size(124, 23);
            this.dateEnd.TabIndex = 13;
            // 
            // dateStart
            // 
            this.dateStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateStart.Location = new System.Drawing.Point(153, 23);
            this.dateStart.Name = "dateStart";
            this.dateStart.Size = new System.Drawing.Size(124, 23);
            this.dateStart.TabIndex = 12;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkValidados);
            this.groupBox2.Controls.Add(this.cmbArea);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(359, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 135);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Opciones por Registro:";
            // 
            // chkValidados
            // 
            this.chkValidados.AutoSize = true;
            this.chkValidados.Location = new System.Drawing.Point(7, 82);
            this.chkValidados.Name = "chkValidados";
            this.chkValidados.Size = new System.Drawing.Size(102, 19);
            this.chkValidados.TabIndex = 16;
            this.chkValidados.Text = "Sólo Validados";
            this.chkValidados.UseVisualStyleBackColor = true;
            this.chkValidados.CheckedChanged += new System.EventHandler(this.chkValidados_CheckedChanged);
            // 
            // cmbArea
            // 
            this.cmbArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArea.FormattingEnabled = true;
            this.cmbArea.Items.AddRange(new object[] {
            "",
            "ALERGOLOGIA",
            "ANATOMIA PATOLOGICA",
            "ANTICUERPOS Y ANTIGENOS ENFERMEDADES INFECCIOSAS",
            "AUTOINMUNIDAD AUTOANTICUERPOS",
            "BIOLOGIA MOLECULAR",
            "COPROLOGIA",
            "QUIMICA SANGUINEA",
            "CITOMETRIA DE FLUJO",
            "COAGULACION Y HEMOSTASIA",
            "ENDOCRINOLOGIA Y MARCADORES TUMORALES",
            "GENETICA",
            "HEMATOLOGIA",
            "INMUNOLOGIA Y SEROLOGIA",
            "MICROBIOLOGIA",
            "PARASITOLOGIA",
            "TOXICOLOGIA Y DROGAS FARMACOLOGICAS",
            "UROANALISIS"});
            this.cmbArea.Location = new System.Drawing.Point(6, 52);
            this.cmbArea.Name = "cmbArea";
            this.cmbArea.Size = new System.Drawing.Size(317, 23);
            this.cmbArea.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 23);
            this.label4.TabIndex = 14;
            this.label4.Text = "Area Servicio:";
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Location = new System.Drawing.Point(123, 151);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(740, 23);
            this.txtBusqueda.TabIndex = 19;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 15);
            this.label5.TabIndex = 20;
            this.label5.Text = "Buscar Paciente:";
            // 
            // pdfViewer1
            // 
            this.pdfViewer1.Location = new System.Drawing.Point(0, 0);
            this.pdfViewer1.Name = "pdfViewer1";
            this.pdfViewer1.TabIndex = 0;
            // 
            // niInfo
            // 
            this.niInfo.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.niInfo.BalloonTipText = "Info";
            this.niInfo.BalloonTipTitle = "Infos";
            this.niInfo.Text = "Info";
            this.niInfo.Visible = true;
            // 
            // btnDatosPaciente
            // 
            this.btnDatosPaciente.Location = new System.Drawing.Point(702, 103);
            this.btnDatosPaciente.Name = "btnDatosPaciente";
            this.btnDatosPaciente.Size = new System.Drawing.Size(75, 23);
            this.btnDatosPaciente.TabIndex = 21;
            this.btnDatosPaciente.Text = "Paciente";
            this.btnDatosPaciente.UseVisualStyleBackColor = true;
            this.btnDatosPaciente.Visible = false;
            this.btnDatosPaciente.Click += new System.EventHandler(this.btnDatosPaciente_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(871, 672);
            this.Controls.Add(this.btnDatosPaciente);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBusqueda);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAbrirFile);
            this.Controls.Add(this.pdfRepPac);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblOrdenes);
            this.Controls.Add(this.pgnOrdenes);
            this.Controls.Add(this.dataGridView1);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GALILEO - HIS";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btnLoadSample;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ProgressBar pgnOrdenes;
        private System.Windows.Forms.Label lblOrdenes;
        private System.Windows.Forms.Label label1;
        private Apitron.PDF.Controls.PDFViewer pdfRepPac;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Button btnAbrirFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateEnd;
        private System.Windows.Forms.DateTimePicker dateStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbArea;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBusqueda;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkValidados;
        private DevExpress.XtraPdfViewer.PdfViewer pdfViewer1;
        private System.Windows.Forms.NotifyIcon niInfo;
        private System.Windows.Forms.Button btnDatosPaciente;
    }
}

