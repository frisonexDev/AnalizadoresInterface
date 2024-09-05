
namespace Galileo_HIS_El_Angel
{
    partial class frmResultados
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
            this.pdfResultados = new Apitron.PDF.Controls.PDFViewer();
            this.SuspendLayout();
            // 
            // pdfResultados
            // 
            this.pdfResultados.Document = null;
            this.pdfResultados.EnableSearch = true;
            this.pdfResultados.Location = new System.Drawing.Point(12, 12);
            this.pdfResultados.Name = "pdfResultados";
            this.pdfResultados.SearchHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.pdfResultados.Size = new System.Drawing.Size(1014, 668);
            this.pdfResultados.TabIndex = 0;
            // 
            // frmResultados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 692);
            this.Controls.Add(this.pdfResultados);
            this.Name = "frmResultados";
            this.Text = "Resultados Paciente";
            this.ResumeLayout(false);

        }

        #endregion

        private Apitron.PDF.Controls.PDFViewer pdfResultados;
    }
}