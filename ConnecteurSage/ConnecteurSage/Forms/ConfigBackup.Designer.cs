namespace ConnecteurSage.Forms
{
    partial class ConfigBackup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigBackup));
            this.button2 = new System.Windows.Forms.Button();
            this.enregistrer_config = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDown_exportLog = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_backupFiles = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_importErreur = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_importSuccess = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_importLog = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_generalLog = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_activateBackup = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_exportLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_backupFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_importErreur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_importSuccess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_importLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_generalLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(402, 257);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Annuler";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // enregistrer_config
            // 
            this.enregistrer_config.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enregistrer_config.Location = new System.Drawing.Point(321, 257);
            this.enregistrer_config.Name = "enregistrer_config";
            this.enregistrer_config.Size = new System.Drawing.Size(75, 23);
            this.enregistrer_config.TabIndex = 6;
            this.enregistrer_config.Text = "Enregistrer";
            this.enregistrer_config.UseVisualStyleBackColor = true;
            this.enregistrer_config.Click += new System.EventHandler(this.enregistrer_config_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.numericUpDown_exportLog);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numericUpDown_backupFiles);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDown_importErreur);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDown_importSuccess);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDown_importLog);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDown_generalLog);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 137);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Paramètre de nettoyage";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(132, 86);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 14);
            this.label12.TabIndex = 41;
            this.label12.Text = " jours";
            // 
            // numericUpDown_exportLog
            // 
            this.numericUpDown_exportLog.Location = new System.Drawing.Point(88, 84);
            this.numericUpDown_exportLog.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_exportLog.Name = "numericUpDown_exportLog";
            this.numericUpDown_exportLog.Size = new System.Drawing.Size(38, 22);
            this.numericUpDown_exportLog.TabIndex = 40;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 86);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(82, 14);
            this.label17.TabIndex = 39;
            this.label17.Text = "Log d\'export : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(395, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 14);
            this.label6.TabIndex = 30;
            this.label6.Text = " jours";
            // 
            // numericUpDown_backupFiles
            // 
            this.numericUpDown_backupFiles.Location = new System.Drawing.Point(351, 29);
            this.numericUpDown_backupFiles.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_backupFiles.Name = "numericUpDown_backupFiles";
            this.numericUpDown_backupFiles.Size = new System.Drawing.Size(38, 22);
            this.numericUpDown_backupFiles.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(412, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 14);
            this.label5.TabIndex = 28;
            this.label5.Text = " jours";
            // 
            // numericUpDown_importErreur
            // 
            this.numericUpDown_importErreur.Location = new System.Drawing.Point(368, 84);
            this.numericUpDown_importErreur.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_importErreur.Name = "numericUpDown_importErreur";
            this.numericUpDown_importErreur.Size = new System.Drawing.Size(38, 22);
            this.numericUpDown_importErreur.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(420, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 14);
            this.label3.TabIndex = 26;
            this.label3.Text = " jours";
            // 
            // numericUpDown_importSuccess
            // 
            this.numericUpDown_importSuccess.Location = new System.Drawing.Point(376, 56);
            this.numericUpDown_importSuccess.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_importSuccess.Name = "numericUpDown_importSuccess";
            this.numericUpDown_importSuccess.Size = new System.Drawing.Size(38, 22);
            this.numericUpDown_importSuccess.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(132, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 24;
            this.label2.Text = " jours";
            // 
            // numericUpDown_importLog
            // 
            this.numericUpDown_importLog.Location = new System.Drawing.Point(88, 56);
            this.numericUpDown_importLog.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_importLog.Name = "numericUpDown_importLog";
            this.numericUpDown_importLog.Size = new System.Drawing.Size(38, 22);
            this.numericUpDown_importLog.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(132, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 14);
            this.label1.TabIndex = 22;
            this.label1.Text = " jours";
            // 
            // numericUpDown_generalLog
            // 
            this.numericUpDown_generalLog.Location = new System.Drawing.Point(88, 29);
            this.numericUpDown_generalLog.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_generalLog.Name = "numericUpDown_generalLog";
            this.numericUpDown_generalLog.Size = new System.Drawing.Size(38, 22);
            this.numericUpDown_generalLog.TabIndex = 21;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(213, 31);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(110, 14);
            this.label13.TabIndex = 16;
            this.label13.Text = "Fichier EDI backup : ";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(213, 86);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(149, 14);
            this.label14.TabIndex = 15;
            this.label14.Text = "Fichier EDI import (Erreur) : ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(213, 58);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(157, 14);
            this.label15.TabIndex = 14;
            this.label15.Text = "Fichier EDI import (Success) : ";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 58);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(83, 14);
            this.label16.TabIndex = 13;
            this.label16.Text = "Log d\'import : ";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 31);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(76, 14);
            this.label18.TabIndex = 12;
            this.label18.Text = "Log général : ";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ConnecteurSage.Properties.Resources.configuration;
            this.pictureBox1.Location = new System.Drawing.Point(29, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(99, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(334, 44);
            this.label4.TabIndex = 12;
            this.label4.Text = "Veuillez completer les informations \r\nafin de garder les fichier envoyé en EDI et" +
    "/ou les logs généré, pendant une période de jours.";
            // 
            // checkBox_activateBackup
            // 
            this.checkBox_activateBackup.AutoSize = true;
            this.checkBox_activateBackup.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Bold);
            this.checkBox_activateBackup.Location = new System.Drawing.Point(21, 95);
            this.checkBox_activateBackup.Name = "checkBox_activateBackup";
            this.checkBox_activateBackup.Size = new System.Drawing.Size(131, 18);
            this.checkBox_activateBackup.TabIndex = 13;
            this.checkBox_activateBackup.Text = "Activer le nettoyage";
            this.checkBox_activateBackup.UseVisualStyleBackColor = true;
            this.checkBox_activateBackup.CheckedChanged += new System.EventHandler(this.checkBox_activateBackup_CheckedChanged);
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(99, 62);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(419, 14);
            this.label22.TabIndex = 42;
            this.label22.Text = "Le paramètre \"0 jours\" signifie que la fonctionnalité est déactivé.";
            // 
            // ConfigBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 286);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.checkBox_activateBackup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.enregistrer_config);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigBackup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sauvegarde";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_exportLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_backupFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_importErreur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_importSuccess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_importLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_generalLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button enregistrer_config;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown_generalLog;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown_backupFiles;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown_importErreur;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown_importSuccess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_importLog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_activateBackup;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDown_exportLog;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label22;
    }
}