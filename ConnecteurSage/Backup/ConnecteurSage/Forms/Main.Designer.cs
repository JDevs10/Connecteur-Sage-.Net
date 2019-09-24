namespace ConnecteurSage
{
    partial class Main
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
        /// the contents of this method with
        /// the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.Fermer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 137);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(466, 198);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(458, 171);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Import";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(253, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 56);
            this.label4.TabIndex = 1;
            this.label4.Text = "Vous pouvez réaliser l\'import et \r\nl\'export de documents commerciaux.\r\nAussi bien" +
                " en manuel qu\'en \r\nautomatique.\r\n";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(17, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 126);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import de commandes";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(29, 74);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(158, 38);
            this.button2.TabIndex = 1;
            this.button2.Text = "Import/Export Planifié";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.MouseHover += new System.EventHandler(this.button2_MouseHover);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(158, 38);
            this.button1.TabIndex = 0;
            this.button1.Text = "Import Manuel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            this.button1.MouseHover += new System.EventHandler(this.button1_MouseHover);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.pictureBox3);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(458, 171);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Export";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button6);
            this.groupBox5.Location = new System.Drawing.Point(149, 116);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(293, 51);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Export de bons de livraisons";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(81, 22);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(126, 23);
            this.button6.TabIndex = 0;
            this.button6.Text = "Exporter";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Location = new System.Drawing.Point(149, 61);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(293, 51);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Export de factures";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(81, 22);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(126, 23);
            this.button5.TabIndex = 0;
            this.button5.Text = "Exporter";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::ConnecteurSage.Properties.Resources.document_export;
            this.pictureBox3.Location = new System.Drawing.Point(3, 23);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(140, 137);
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Location = new System.Drawing.Point(149, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(293, 51);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Export de commandes";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(81, 22);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(126, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "Exporter";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button7);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.pictureBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabPage4.Size = new System.Drawing.Size(458, 171);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Config";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(235, 132);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(110, 23);
            this.button7.TabIndex = 9;
            this.button7.Text = "Config Mail";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(label1);
            this.groupBox2.Controls.Add(label2);
            this.groupBox2.Location = new System.Drawing.Point(121, 26);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 100);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paramétrage";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(16, 29);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(96, 14);
            label1.TabIndex = 0;
            label1.Text = "DSN  : Non défini";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(16, 64);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(96, 14);
            label2.TabIndex = 1;
            label2.Text = "Nom : Non défini";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(121, 132);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(110, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Config DSN";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ConnecteurSage.Properties.Resources.configuration;
            this.pictureBox1.Location = new System.Drawing.Point(35, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel);
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(458, 171);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "About";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelVersion, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.labelCopyright, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.85714F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(452, 165);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Location = new System.Drawing.Point(6, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(443, 17);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Connecteur Sage";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Location = new System.Drawing.Point(6, 23);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(443, 17);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version : 0.4.6";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCopyright.Location = new System.Drawing.Point(6, 46);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(443, 17);
            this.labelCopyright.TabIndex = 21;
            this.labelCopyright.Text = "Copyright © 2013";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCompanyName.Location = new System.Drawing.Point(6, 69);
            this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(443, 17);
            this.labelCompanyName.TabIndex = 22;
            this.labelCompanyName.Text = "ANEXYS";
            this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(6, 95);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(443, 67);
            this.textBoxDescription.TabIndex = 23;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "Le connecteur Sage permet d\'importer ou d\'exporter des documents commerciaux éléc" +
                "tronique à partir de Sage V16.05, afin d\'utiliser la plateforme EDINEX (Anexys) " +
                "pour communiquer avec un tiers en EDI.";
            // 
            // Fermer
            // 
            this.Fermer.Location = new System.Drawing.Point(402, 344);
            this.Fermer.Name = "Fermer";
            this.Fermer.Size = new System.Drawing.Size(75, 23);
            this.Fermer.TabIndex = 1;
            this.Fermer.Text = "Fermer";
            this.Fermer.UseVisualStyleBackColor = true;
            this.Fermer.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 26);
            this.label3.TabIndex = 12;
            this.label3.Text = "Connecteur Sage v0.4.6\r\nImport et export de documents commerciaux.\r\n";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ConnecteurSage.Properties.Resources.sage_header;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(491, 78);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 375);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.Fermer);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connecteur SAGE";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button Fermer;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;

        public static void ModifierButtonDNS(string text)
        {
            label1.Text = "DSN  : " + text;
        }
        public static void ModifierButtonNom(string text)
        {
            label2.Text = "Nom : " + text;
        }

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button7;
        private static System.Windows.Forms.Label label2;
        private static System.Windows.Forms.Label label1;
    }

}
