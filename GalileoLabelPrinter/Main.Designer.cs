
namespace GalileoLabelPrinter
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
            this.txtOrderFrom = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtOrderTo = new System.Windows.Forms.TextBox();
            this.chkSampleList = new System.Windows.Forms.CheckedListBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtOrderFrom
            // 
            this.txtOrderFrom.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtOrderFrom.Location = new System.Drawing.Point(52, 39);
            this.txtOrderFrom.Name = "txtOrderFrom";
            this.txtOrderFrom.Size = new System.Drawing.Size(255, 32);
            this.txtOrderFrom.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(648, 41);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(90, 33);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Buscar";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtOrderTo
            // 
            this.txtOrderTo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtOrderTo.Location = new System.Drawing.Point(347, 41);
            this.txtOrderTo.Name = "txtOrderTo";
            this.txtOrderTo.Size = new System.Drawing.Size(255, 32);
            this.txtOrderTo.TabIndex = 3;
            // 
            // chkSampleList
            // 
            this.chkSampleList.FormattingEnabled = true;
            this.chkSampleList.Location = new System.Drawing.Point(52, 114);
            this.chkSampleList.Name = "chkSampleList";
            this.chkSampleList.Size = new System.Drawing.Size(686, 256);
            this.chkSampleList.TabIndex = 4;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(283, 405);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(174, 23);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "Imprimir Etiquetas";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(515, 426);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 461);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.chkSampleList);
            this.Controls.Add(this.txtOrderTo);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtOrderFrom);
            this.Name = "Main";
            this.Text = "Galileo Label Printer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtOrderFrom;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtOrderTo;
        private System.Windows.Forms.CheckedListBox chkSampleList;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button button1;
    }
}

