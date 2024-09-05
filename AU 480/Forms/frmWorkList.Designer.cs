
namespace Galileo.Online.Forms
{
    partial class frmWorkList
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.secFilters = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dateEnd = new System.Windows.Forms.DateTimePicker();
            this.dateStart = new System.Windows.Forms.DateTimePicker();
            this.btnPrintLabels = new System.Windows.Forms.Button();
            this.btnSendWorkList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.secFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 127);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(989, 423);
            this.dataGridView1.TabIndex = 0;
            // 
            // secFilters
            // 
            this.secFilters.Controls.Add(this.button1);
            this.secFilters.Controls.Add(this.dateEnd);
            this.secFilters.Controls.Add(this.dateStart);
            this.secFilters.Location = new System.Drawing.Point(12, 12);
            this.secFilters.Name = "secFilters";
            this.secFilters.Size = new System.Drawing.Size(989, 109);
            this.secFilters.TabIndex = 1;
            this.secFilters.TabStop = false;
            this.secFilters.Text = "Filtros";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(586, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Consultar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // dateEnd
            // 
            this.dateEnd.Location = new System.Drawing.Point(307, 46);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.Size = new System.Drawing.Size(238, 23);
            this.dateEnd.TabIndex = 1;
            // 
            // dateStart
            // 
            this.dateStart.Location = new System.Drawing.Point(19, 46);
            this.dateStart.Name = "dateStart";
            this.dateStart.Size = new System.Drawing.Size(252, 23);
            this.dateStart.TabIndex = 0;
            // 
            // btnPrintLabels
            // 
            this.btnPrintLabels.Location = new System.Drawing.Point(12, 573);
            this.btnPrintLabels.Name = "btnPrintLabels";
            this.btnPrintLabels.Size = new System.Drawing.Size(154, 34);
            this.btnPrintLabels.TabIndex = 2;
            this.btnPrintLabels.Text = "Imprimir Etiquetas";
            this.btnPrintLabels.UseVisualStyleBackColor = true;
            this.btnPrintLabels.Click += new System.EventHandler(this.btnPrintLabels_Click);
        
            // 
            // frmWorkList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 619);
            this.Controls.Add(this.btnSendWorkList);
            this.Controls.Add(this.btnPrintLabels);
            this.Controls.Add(this.secFilters);
            this.Controls.Add(this.dataGridView1);
            this.Name = "frmWorkList";
            this.Text = "frmWorkList";
            this.Load += new System.EventHandler(this.frmWorkList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.secFilters.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox secFilters;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateEnd;
        private System.Windows.Forms.DateTimePicker dateStart;
        private System.Windows.Forms.Button btnPrintLabels;
        private System.Windows.Forms.Button btnSendWorkList;
    }
}