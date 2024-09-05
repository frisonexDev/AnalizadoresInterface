
namespace Galileo.Online
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picInstrumento = new System.Windows.Forms.PictureBox();
            this.lblNombreInstrumento = new System.Windows.Forms.Label();
            this.logScreen = new System.Windows.Forms.RichTextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripeConfig = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolConexiones = new System.Windows.Forms.ToolStripMenuItem();
            this.toolPruebas = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuLog = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripLog = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuCleanLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuWorkList = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripWorkList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripReadSample = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripWorkList = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEnviarResultado = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.stateProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picInstrumento)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.contextMenuStripeConfig.SuspendLayout();
            this.contextMenuStripLog.SuspendLayout();
            this.contextMenuStripWorkList.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picInstrumento
            // 
            this.picInstrumento.BackColor = System.Drawing.Color.White;
            this.picInstrumento.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picInstrumento.Location = new System.Drawing.Point(941, 40);
            this.picInstrumento.Name = "picInstrumento";
            this.picInstrumento.Size = new System.Drawing.Size(206, 197);
            this.picInstrumento.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInstrumento.TabIndex = 0;
            this.picInstrumento.TabStop = false;
            // 
            // lblNombreInstrumento
            // 
            this.lblNombreInstrumento.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblNombreInstrumento.Location = new System.Drawing.Point(941, 247);
            this.lblNombreInstrumento.Name = "lblNombreInstrumento";
            this.lblNombreInstrumento.Size = new System.Drawing.Size(206, 23);
            this.lblNombreInstrumento.TabIndex = 1;
            this.lblNombreInstrumento.Text = "lblNombreInstrumento";
            this.lblNombreInstrumento.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logScreen
            // 
            this.logScreen.BackColor = System.Drawing.Color.Black;
            this.logScreen.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.logScreen.ForeColor = System.Drawing.Color.White;
            this.logScreen.Location = new System.Drawing.Point(8, 40);
            this.logScreen.Name = "logScreen";
            this.logScreen.ReadOnly = true;
            this.logScreen.Size = new System.Drawing.Size(927, 461);
            this.logScreen.TabIndex = 6;
            this.logScreen.Text = "";
            this.logScreen.WordWrap = false;
            this.logScreen.TextChanged += new System.EventHandler(this.logScreen_TextChanged_1);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuConfig,
            this.toolStripMenuLog,
            this.toolStripMenuWorkList});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1159, 24);
            this.menuStrip.TabIndex = 7;
            this.menuStrip.Text = "                                                                           ";
            this.menuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip_ItemClicked);
            // 
            // toolStripMenuConfig
            // 
            this.toolStripMenuConfig.DropDown = this.contextMenuStripeConfig;
            this.toolStripMenuConfig.Name = "toolStripMenuConfig";
            this.toolStripMenuConfig.Size = new System.Drawing.Size(106, 20);
            this.toolStripMenuConfig.Text = "Configuraciones";
            // 
            // contextMenuStripeConfig
            // 
            this.contextMenuStripeConfig.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolConexiones,
            this.toolPruebas});
            this.contextMenuStripeConfig.Name = "contextMenuStrip1";
            this.contextMenuStripeConfig.OwnerItem = this.toolStripMenuConfig;
            this.contextMenuStripeConfig.Size = new System.Drawing.Size(166, 48);
            // 
            // toolConexiones
            // 
            this.toolConexiones.Name = "toolConexiones";
            this.toolConexiones.Size = new System.Drawing.Size(165, 22);
            this.toolConexiones.Text = "Co&municaciones";
            // 
            // toolPruebas
            // 
            this.toolPruebas.Image = global::Galileo.Online.Properties.Resources._3671883_translate_icon;
            this.toolPruebas.Name = "toolPruebas";
            this.toolPruebas.Size = new System.Drawing.Size(165, 22);
            this.toolPruebas.Text = "Homologaciones";
            // 
            // toolStripMenuLog
            // 
            this.toolStripMenuLog.DropDown = this.contextMenuStripLog;
            this.toolStripMenuLog.Name = "toolStripMenuLog";
            this.toolStripMenuLog.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuLog.Text = "Logs";
            // 
            // contextMenuStripLog
            // 
            this.contextMenuStripLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuCleanLog});
            this.contextMenuStripLog.Name = "contextMenuStripLog";
            this.contextMenuStripLog.OwnerItem = this.toolStripMenuLog;
            this.contextMenuStripLog.Size = new System.Drawing.Size(138, 26);
            this.contextMenuStripLog.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripLog_ItemClicked);
            this.contextMenuStripLog.MouseClick += new System.Windows.Forms.MouseEventHandler(this.contextMenuStripLog_MouseClick);
            // 
            // toolStripMenuCleanLog
            // 
            this.toolStripMenuCleanLog.Name = "toolStripMenuCleanLog";
            this.toolStripMenuCleanLog.Size = new System.Drawing.Size(137, 22);
            this.toolStripMenuCleanLog.Text = "Limpiar Log";
            // 
            // toolStripMenuWorkList
            // 
            this.toolStripMenuWorkList.DropDown = this.contextMenuStripWorkList;
            this.toolStripMenuWorkList.Name = "toolStripMenuWorkList";
            this.toolStripMenuWorkList.Size = new System.Drawing.Size(100, 20);
            this.toolStripMenuWorkList.Text = "Lista de Trabajo";
            // 
            // contextMenuStripWorkList
            // 
            this.contextMenuStripWorkList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripReadSample,
            this.toolStripWorkList,
            this.toolStripEnviarResultado});
            this.contextMenuStripWorkList.Name = "contextMenuStripWorkList";
            this.contextMenuStripWorkList.OwnerItem = this.toolStripMenuWorkList;
            this.contextMenuStripWorkList.Size = new System.Drawing.Size(163, 70);
            this.contextMenuStripWorkList.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripWorkList_ItemClicked);
            this.contextMenuStripWorkList.Click += new System.EventHandler(this.contextMenuStripWorkList_Click);
            // 
            // toolStripReadSample
            // 
            this.toolStripReadSample.Name = "toolStripReadSample";
            this.toolStripReadSample.Size = new System.Drawing.Size(162, 22);
            this.toolStripReadSample.Text = "Ingresar Muestra";
            // 
            // toolStripWorkList
            // 
            this.toolStripWorkList.Name = "toolStripWorkList";
            this.toolStripWorkList.Size = new System.Drawing.Size(162, 22);
            this.toolStripWorkList.Text = "Lista de Trabajo";
            // 
            // toolStripEnviarResultado
            // 
            this.toolStripEnviarResultado.Name = "toolStripEnviarResultado";
            this.toolStripEnviarResultado.Size = new System.Drawing.Size(162, 22);
            this.toolStripEnviarResultado.Text = "Enviar Resultado";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.stateProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 516);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1159, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.ForeColor = System.Drawing.Color.Black;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(82, 17);
            this.statusLabel.Text = "Desconectado";
            // 
            // stateProgressBar
            // 
            this.stateProgressBar.Name = "stateProgressBar";
            this.stateProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.Location = new System.Drawing.Point(987, 311);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(129, 66);
            this.lblStatus.TabIndex = 14;
            this.lblStatus.Text = "En Espera";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.TextChanged += new System.EventHandler(this.lblStatus_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1159, 538);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.logScreen);
            this.Controls.Add(this.lblNombreInstrumento);
            this.Controls.Add(this.picInstrumento);
            this.Controls.Add(this.menuStrip);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "GALILEO - ";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.frmPrincipal_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picInstrumento)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.contextMenuStripeConfig.ResumeLayout(false);
            this.contextMenuStripLog.ResumeLayout(false);
            this.contextMenuStripWorkList.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.PictureBox picInstrumento;
        private System.Windows.Forms.Label lblNombreInstrumento;
        private System.Windows.Forms.RichTextBox logScreen;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuConfig;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripeConfig;
        private System.Windows.Forms.ToolStripMenuItem toolConexiones;
        private System.Windows.Forms.ToolStripMenuItem toolPruebas;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar stateProgressBar;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLog;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuCleanLog;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuWorkList;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripWorkList;
        private System.Windows.Forms.ToolStripMenuItem toolStripReadSample;
        private System.Windows.Forms.ToolStripMenuItem toolStripWorkList;
        private System.Windows.Forms.ToolStripMenuItem toolStripEnviarResultado;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer timer1;
    }
}