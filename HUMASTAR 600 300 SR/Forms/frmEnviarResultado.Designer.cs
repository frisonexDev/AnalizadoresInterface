
namespace Galileo.Online.Forms
{
    partial class frmEnviarResultado
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
            this.txtOrden = new System.Windows.Forms.TextBox();
            this.txtResultado = new System.Windows.Forms.TextBox();
            this.lblOrden = new System.Windows.Forms.Label();
            this.lblResultado = new System.Windows.Forms.Label();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.comboBoxPruebas = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkValidado = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtOrden
            // 
            this.txtOrden.Location = new System.Drawing.Point(164, 34);
            this.txtOrden.Name = "txtOrden";
            this.txtOrden.Size = new System.Drawing.Size(232, 23);
            this.txtOrden.TabIndex = 0;
            // 
            // txtResultado
            // 
            this.txtResultado.Location = new System.Drawing.Point(164, 112);
            this.txtResultado.Name = "txtResultado";
            this.txtResultado.Size = new System.Drawing.Size(232, 23);
            this.txtResultado.TabIndex = 1;
            // 
            // lblOrden
            // 
            this.lblOrden.AutoSize = true;
            this.lblOrden.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblOrden.Location = new System.Drawing.Point(45, 42);
            this.lblOrden.Name = "lblOrden";
            this.lblOrden.Size = new System.Drawing.Size(45, 15);
            this.lblOrden.TabIndex = 2;
            this.lblOrden.Text = "Orden:";
            // 
            // lblResultado
            // 
            this.lblResultado.AutoSize = true;
            this.lblResultado.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblResultado.Location = new System.Drawing.Point(45, 120);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(65, 15);
            this.lblResultado.TabIndex = 3;
            this.lblResultado.Text = "Resultado:";
            // 
            // btnEnviar
            // 
            this.btnEnviar.Location = new System.Drawing.Point(227, 188);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(75, 23);
            this.btnEnviar.TabIndex = 4;
            this.btnEnviar.Text = "&Enviar";
            this.btnEnviar.UseVisualStyleBackColor = true;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);
            // 
            // comboBoxPruebas
            // 
            this.comboBoxPruebas.FormattingEnabled = true;
            this.comboBoxPruebas.Location = new System.Drawing.Point(164, 74);
            this.comboBoxPruebas.Name = "comboBoxPruebas";
            this.comboBoxPruebas.Size = new System.Drawing.Size(232, 23);
            this.comboBoxPruebas.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(45, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Prueba:";
            // 
            // chkValidado
            // 
            this.chkValidado.AutoSize = true;
            this.chkValidado.Location = new System.Drawing.Point(164, 150);
            this.chkValidado.Name = "chkValidado";
            this.chkValidado.Size = new System.Drawing.Size(71, 19);
            this.chkValidado.TabIndex = 7;
            this.chkValidado.Text = "Validado";
            this.chkValidado.UseVisualStyleBackColor = true;
            // 
            // frmEnviarResultado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 223);
            this.Controls.Add(this.chkValidado);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxPruebas);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.lblOrden);
            this.Controls.Add(this.txtResultado);
            this.Controls.Add(this.txtOrden);
            this.Name = "frmEnviarResultado";
            this.Text = "Enviar Resultado";
            this.Load += new System.EventHandler(this.frmEnviarResultado_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOrden;
        private System.Windows.Forms.TextBox txtResultado;
        private System.Windows.Forms.Label lblOrden;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.ComboBox comboBoxPruebas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkValidado;
    }
}