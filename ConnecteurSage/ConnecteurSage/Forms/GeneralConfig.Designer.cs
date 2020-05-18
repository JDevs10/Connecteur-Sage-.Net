namespace ConnecteurSage.Forms
{
    partial class GeneralConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralConfig));
            this.debugMode_checkBox = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_activate_compt_g_taxe = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_reprocess_activate = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7_reprocess = new System.Windows.Forms.Label();
            this.numericUpDown1_reprocess_cd = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_hour = new System.Windows.Forms.NumericUpDown();
            this.label7_reprocess_hour = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_reprocess_cd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).BeginInit();
            this.SuspendLayout();
            // 
            // debugMode_checkBox
            // 
            this.debugMode_checkBox.AutoSize = true;
            this.debugMode_checkBox.Location = new System.Drawing.Point(159, 28);
            this.debugMode_checkBox.Name = "debugMode_checkBox";
            this.debugMode_checkBox.Size = new System.Drawing.Size(88, 17);
            this.debugMode_checkBox.TabIndex = 12;
            this.debugMode_checkBox.Text = "Debug Mode";
            this.debugMode_checkBox.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ConnecteurSage.Properties.Resources.configuration;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_activate_compt_g_taxe);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.debugMode_checkBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 82);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(329, 144);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paramétrage";
            // 
            // checkBox_activate_compt_g_taxe
            // 
            this.checkBox_activate_compt_g_taxe.AutoSize = true;
            this.checkBox_activate_compt_g_taxe.Checked = true;
            this.checkBox_activate_compt_g_taxe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_activate_compt_g_taxe.Location = new System.Drawing.Point(112, 86);
            this.checkBox_activate_compt_g_taxe.Name = "checkBox_activate_compt_g_taxe";
            this.checkBox_activate_compt_g_taxe.Size = new System.Drawing.Size(77, 17);
            this.checkBox_activate_compt_g_taxe.TabIndex = 15;
            this.checkBox_activate_compt_g_taxe.Text = "Désactiver";
            this.checkBox_activate_compt_g_taxe.UseVisualStyleBackColor = true;
            this.checkBox_activate_compt_g_taxe.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(195, 84);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(156, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Nom : Non défini";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Compt. G. Taxe : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Affichage du Planificateur : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Statut du Planificateur : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(82, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 28);
            this.label4.TabIndex = 15;
            this.label4.Text = "Veuillez compléter les informations\r\nafin de gérer le comportement du connecteur." +
    "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(254, 447);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Annuler";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(173, 447);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Ok";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 232);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(329, 63);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chemin de l\'import :";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(256, 26);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(69, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "Parcourir";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(6, 28);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(244, 20);
            this.textBox2.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label7_reprocess_hour);
            this.groupBox3.Controls.Add(this.numericUpDown_hour);
            this.groupBox3.Controls.Add(this.numericUpDown1_reprocess_cd);
            this.groupBox3.Controls.Add(this.label7_reprocess);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.checkBox_reprocess_activate);
            this.groupBox3.Location = new System.Drawing.Point(12, 301);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(395, 137);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Retraitement des fichiers en erreur";
            // 
            // checkBox_reprocess_activate
            // 
            this.checkBox_reprocess_activate.AutoSize = true;
            this.checkBox_reprocess_activate.Location = new System.Drawing.Point(9, 43);
            this.checkBox_reprocess_activate.Name = "checkBox_reprocess_activate";
            this.checkBox_reprocess_activate.Size = new System.Drawing.Size(59, 17);
            this.checkBox_reprocess_activate.TabIndex = 16;
            this.checkBox_reprocess_activate.Text = "Activer";
            this.checkBox_reprocess_activate.UseVisualStyleBackColor = true;
            this.checkBox_reprocess_activate.CheckedChanged += new System.EventHandler(this.checkBox_reprocess_activate_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(296, 14);
            this.label6.TabIndex = 20;
            this.label6.Text = "Voici les paramètres des fichiers en erreur sur X heures.";
            // 
            // label7_reprocess
            // 
            this.label7_reprocess.AutoSize = true;
            this.label7_reprocess.Enabled = false;
            this.label7_reprocess.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7_reprocess.Location = new System.Drawing.Point(65, 95);
            this.label7_reprocess.Name = "label7_reprocess";
            this.label7_reprocess.Size = new System.Drawing.Size(189, 14);
            this.label7_reprocess.TabIndex = 21;
            this.label7_reprocess.Text = "Cette fonctionnalité est désactiver !";
            // 
            // numericUpDown1_reprocess_cd
            // 
            this.numericUpDown1_reprocess_cd.Enabled = false;
            this.numericUpDown1_reprocess_cd.Location = new System.Drawing.Point(9, 95);
            this.numericUpDown1_reprocess_cd.Name = "numericUpDown1_reprocess_cd";
            this.numericUpDown1_reprocess_cd.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown1_reprocess_cd.TabIndex = 22;
            this.numericUpDown1_reprocess_cd.ValueChanged += new System.EventHandler(this.numericUpDown1_reprocess_cd_ValueChanged);
            // 
            // numericUpDown_hour
            // 
            this.numericUpDown_hour.Enabled = false;
            this.numericUpDown_hour.Location = new System.Drawing.Point(9, 64);
            this.numericUpDown_hour.Name = "numericUpDown_hour";
            this.numericUpDown_hour.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown_hour.TabIndex = 23;
            this.numericUpDown_hour.ValueChanged += new System.EventHandler(this.numericUpDown_hour_ValueChanged);
            // 
            // label7_reprocess_hour
            // 
            this.label7_reprocess_hour.AutoSize = true;
            this.label7_reprocess_hour.Enabled = false;
            this.label7_reprocess_hour.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7_reprocess_hour.Location = new System.Drawing.Point(65, 63);
            this.label7_reprocess_hour.Name = "label7_reprocess_hour";
            this.label7_reprocess_hour.Size = new System.Drawing.Size(174, 14);
            this.label7_reprocess_hour.TabIndex = 24;
            this.label7_reprocess_hour.Text = "Retraitement tous les X heure(s).";
            // 
            // GeneralConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 479);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GeneralConfig";
            this.Text = "Configuration General";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_reprocess_cd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox debugMode_checkBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_activate_compt_g_taxe;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDown1_reprocess_cd;
        private System.Windows.Forms.Label label7_reprocess;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox_reprocess_activate;
        private System.Windows.Forms.Label label7_reprocess_hour;
        private System.Windows.Forms.NumericUpDown numericUpDown_hour;
    }
}