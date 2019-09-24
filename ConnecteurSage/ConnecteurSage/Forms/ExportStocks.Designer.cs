namespace ConnecteurSage.Forms
{
    partial class ExportStocks
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cols_rows = new System.Windows.Forms.DataGridView();
            this.export_stockliste = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cols_rows)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cols_rows);
            this.groupBox2.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(809, 180);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Detail Stock";
            // 
            // cols_rows
            // 
            this.cols_rows.AllowUserToAddRows = false;
            this.cols_rows.AllowUserToDeleteRows = false;
            this.cols_rows.BackgroundColor = System.Drawing.SystemColors.Window;
            this.cols_rows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cols_rows.Location = new System.Drawing.Point(6, 25);
            this.cols_rows.Name = "cols_rows";
            this.cols_rows.ReadOnly = true;
            this.cols_rows.Size = new System.Drawing.Size(797, 149);
            this.cols_rows.TabIndex = 22;
            this.cols_rows.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cols_rows_CellContentClick);
            this.cols_rows.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cols_rows_CellDoubleClick);
            // 
            // export_stockliste
            // 
            this.export_stockliste.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.export_stockliste.Location = new System.Drawing.Point(549, 274);
            this.export_stockliste.Name = "export_stockliste";
            this.export_stockliste.Size = new System.Drawing.Size(133, 45);
            this.export_stockliste.TabIndex = 22;
            this.export_stockliste.Text = "Export Liste";
            this.export_stockliste.UseVisualStyleBackColor = true;
            this.export_stockliste.Click += new System.EventHandler(this.export_stockliste_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(12, 198);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(803, 63);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Chemin d\'export :";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(698, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 27);
            this.button2.TabIndex = 3;
            this.button2.Text = "Parcourir";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(6, 26);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(686, 27);
            this.textBox1.TabIndex = 2;
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(688, 274);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(133, 45);
            this.closeButton.TabIndex = 24;
            this.closeButton.Text = "Fermer";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // ExportStocks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 331);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.export_stockliste);
            this.Controls.Add(this.groupBox2);
            this.Name = "ExportStocks";
            this.Text = "ExportStocks";
            this.Load += new System.EventHandler(this.ExportStocks_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cols_rows)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView cols_rows;
        private System.Windows.Forms.Button export_stockliste;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button closeButton;
    }
}