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
            this.button_tmp = new System.Windows.Forms.Button();
            this.button_reprocess = new System.Windows.Forms.Button();
            this.label7_reprocess_hour = new System.Windows.Forms.Label();
            this.numericUpDown_hour = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1_reprocess_cd = new System.Windows.Forms.NumericUpDown();
            this.label7_reprocess = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox_reprocess_activate = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton_tarif_produit = new System.Windows.Forms.RadioButton();
            this.radioButton_tarif_cmd_EDI = new System.Windows.Forms.RadioButton();
            this.radioButton_tarif_client = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButton_tarif_categorie = new System.Windows.Forms.RadioButton();
            this.checkBox_activer_tarif = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_reprocess_cd)).BeginInit();
            this.groupBox4.SuspendLayout();
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
            this.groupBox2.Size = new System.Drawing.Size(399, 124);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paramétrage Général :";
            // 
            // checkBox_activate_compt_g_taxe
            // 
            this.checkBox_activate_compt_g_taxe.AutoSize = true;
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
            this.button1.Location = new System.Drawing.Point(330, 601);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Annuler";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(249, 601);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 212);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(399, 63);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chemin de l\'import EDI :";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(324, 25);
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
            this.textBox2.Location = new System.Drawing.Point(19, 28);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(299, 20);
            this.textBox2.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_tmp);
            this.groupBox3.Controls.Add(this.button_reprocess);
            this.groupBox3.Controls.Add(this.label7_reprocess_hour);
            this.groupBox3.Controls.Add(this.numericUpDown_hour);
            this.groupBox3.Controls.Add(this.numericUpDown1_reprocess_cd);
            this.groupBox3.Controls.Add(this.label7_reprocess);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.checkBox_reprocess_activate);
            this.groupBox3.Location = new System.Drawing.Point(12, 425);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(399, 170);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Retraitement des fichiers en erreur";
            // 
            // button_tmp
            // 
            this.button_tmp.Enabled = false;
            this.button_tmp.Location = new System.Drawing.Point(142, 130);
            this.button_tmp.Name = "button_tmp";
            this.button_tmp.Size = new System.Drawing.Size(111, 23);
            this.button_tmp.TabIndex = 25;
            this.button_tmp.Text = "Retraitement tmp";
            this.button_tmp.UseVisualStyleBackColor = true;
            this.button_tmp.Click += new System.EventHandler(this.button_tmp_Click);
            // 
            // button_reprocess
            // 
            this.button_reprocess.Enabled = false;
            this.button_reprocess.Location = new System.Drawing.Point(19, 130);
            this.button_reprocess.Name = "button_reprocess";
            this.button_reprocess.Size = new System.Drawing.Size(117, 23);
            this.button_reprocess.TabIndex = 20;
            this.button_reprocess.Text = "Retraitement Erreurs";
            this.button_reprocess.UseVisualStyleBackColor = true;
            this.button_reprocess.Click += new System.EventHandler(this.button_reprocess_Click);
            // 
            // label7_reprocess_hour
            // 
            this.label7_reprocess_hour.AutoSize = true;
            this.label7_reprocess_hour.Enabled = false;
            this.label7_reprocess_hour.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7_reprocess_hour.Location = new System.Drawing.Point(75, 63);
            this.label7_reprocess_hour.Name = "label7_reprocess_hour";
            this.label7_reprocess_hour.Size = new System.Drawing.Size(174, 14);
            this.label7_reprocess_hour.TabIndex = 24;
            this.label7_reprocess_hour.Text = "Retraitement tous les X heure(s).";
            // 
            // numericUpDown_hour
            // 
            this.numericUpDown_hour.DecimalPlaces = 1;
            this.numericUpDown_hour.Enabled = false;
            this.numericUpDown_hour.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numericUpDown_hour.Location = new System.Drawing.Point(19, 64);
            this.numericUpDown_hour.Name = "numericUpDown_hour";
            this.numericUpDown_hour.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown_hour.TabIndex = 23;
            this.numericUpDown_hour.ValueChanged += new System.EventHandler(this.numericUpDown_hour_ValueChanged);
            // 
            // numericUpDown1_reprocess_cd
            // 
            this.numericUpDown1_reprocess_cd.Enabled = false;
            this.numericUpDown1_reprocess_cd.Location = new System.Drawing.Point(19, 95);
            this.numericUpDown1_reprocess_cd.Name = "numericUpDown1_reprocess_cd";
            this.numericUpDown1_reprocess_cd.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown1_reprocess_cd.TabIndex = 22;
            this.numericUpDown1_reprocess_cd.ValueChanged += new System.EventHandler(this.numericUpDown1_reprocess_cd_ValueChanged);
            // 
            // label7_reprocess
            // 
            this.label7_reprocess.AutoSize = true;
            this.label7_reprocess.Enabled = false;
            this.label7_reprocess.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7_reprocess.Location = new System.Drawing.Point(75, 95);
            this.label7_reprocess.Name = "label7_reprocess";
            this.label7_reprocess.Size = new System.Drawing.Size(189, 14);
            this.label7_reprocess.TabIndex = 21;
            this.label7_reprocess.Text = "Cette fonctionnalité est désactiver !";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(296, 14);
            this.label6.TabIndex = 20;
            this.label6.Text = "Voici les paramètres des fichiers en erreur sur X heures.";
            // 
            // checkBox_reprocess_activate
            // 
            this.checkBox_reprocess_activate.AutoSize = true;
            this.checkBox_reprocess_activate.Location = new System.Drawing.Point(19, 43);
            this.checkBox_reprocess_activate.Name = "checkBox_reprocess_activate";
            this.checkBox_reprocess_activate.Size = new System.Drawing.Size(59, 17);
            this.checkBox_reprocess_activate.TabIndex = 16;
            this.checkBox_reprocess_activate.Text = "Activer";
            this.checkBox_reprocess_activate.UseVisualStyleBackColor = true;
            this.checkBox_reprocess_activate.CheckedChanged += new System.EventHandler(this.checkBox_reprocess_activate_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton_tarif_produit);
            this.groupBox4.Controls.Add(this.radioButton_tarif_cmd_EDI);
            this.groupBox4.Controls.Add(this.radioButton_tarif_client);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.radioButton_tarif_categorie);
            this.groupBox4.Controls.Add(this.checkBox_activer_tarif);
            this.groupBox4.Location = new System.Drawing.Point(12, 281);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(399, 138);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Paramétrage Tarif :";
            // 
            // radioButton_tarif_produit
            // 
            this.radioButton_tarif_produit.AutoSize = true;
            this.radioButton_tarif_produit.Enabled = false;
            this.radioButton_tarif_produit.Location = new System.Drawing.Point(19, 104);
            this.radioButton_tarif_produit.Name = "radioButton_tarif_produit";
            this.radioButton_tarif_produit.Size = new System.Drawing.Size(111, 17);
            this.radioButton_tarif_produit.TabIndex = 28;
            this.radioButton_tarif_produit.TabStop = true;
            this.radioButton_tarif_produit.Text = "Tarif Fiche Produit";
            this.radioButton_tarif_produit.UseVisualStyleBackColor = true;
            // 
            // radioButton_tarif_cmd_EDI
            // 
            this.radioButton_tarif_cmd_EDI.AutoSize = true;
            this.radioButton_tarif_cmd_EDI.Enabled = false;
            this.radioButton_tarif_cmd_EDI.Location = new System.Drawing.Point(19, 81);
            this.radioButton_tarif_cmd_EDI.Name = "radioButton_tarif_cmd_EDI";
            this.radioButton_tarif_cmd_EDI.Size = new System.Drawing.Size(123, 17);
            this.radioButton_tarif_cmd_EDI.TabIndex = 27;
            this.radioButton_tarif_cmd_EDI.TabStop = true;
            this.radioButton_tarif_cmd_EDI.Text = "Tarif Commande EDI";
            this.radioButton_tarif_cmd_EDI.UseVisualStyleBackColor = true;
            // 
            // radioButton_tarif_client
            // 
            this.radioButton_tarif_client.AutoSize = true;
            this.radioButton_tarif_client.Enabled = false;
            this.radioButton_tarif_client.Location = new System.Drawing.Point(159, 104);
            this.radioButton_tarif_client.Name = "radioButton_tarif_client";
            this.radioButton_tarif_client.Size = new System.Drawing.Size(75, 17);
            this.radioButton_tarif_client.TabIndex = 26;
            this.radioButton_tarif_client.TabStop = true;
            this.radioButton_tarif_client.Text = "Tarif Client";
            this.radioButton_tarif_client.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(16, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(380, 28);
            this.label7.TabIndex = 25;
            this.label7.Text = "Voici les paramètres pour choisir les type de tarifs des articles,\r\nTarif de la c" +
    "ommande (EDI), Tarif Catétorie (Sage) ou Tarif Client (Sage).";
            // 
            // radioButton_tarif_categorie
            // 
            this.radioButton_tarif_categorie.AutoSize = true;
            this.radioButton_tarif_categorie.Enabled = false;
            this.radioButton_tarif_categorie.Location = new System.Drawing.Point(159, 81);
            this.radioButton_tarif_categorie.Name = "radioButton_tarif_categorie";
            this.radioButton_tarif_categorie.Size = new System.Drawing.Size(94, 17);
            this.radioButton_tarif_categorie.TabIndex = 17;
            this.radioButton_tarif_categorie.TabStop = true;
            this.radioButton_tarif_categorie.Text = "Tarif Catégorie";
            this.radioButton_tarif_categorie.UseVisualStyleBackColor = true;
            // 
            // checkBox_activer_tarif
            // 
            this.checkBox_activer_tarif.AutoSize = true;
            this.checkBox_activer_tarif.Location = new System.Drawing.Point(19, 58);
            this.checkBox_activer_tarif.Name = "checkBox_activer_tarif";
            this.checkBox_activer_tarif.Size = new System.Drawing.Size(59, 17);
            this.checkBox_activer_tarif.TabIndex = 16;
            this.checkBox_activer_tarif.Text = "Activer";
            this.checkBox_activer_tarif.UseVisualStyleBackColor = true;
            this.checkBox_activer_tarif.CheckedChanged += new System.EventHandler(this.checkBox_activer_tarif_CheckedChanged);
            // 
            // GeneralConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 637);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GeneralConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration General";
            this.Load += new System.EventHandler(this.GeneralConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_reprocess_cd)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButton_tarif_client;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioButton_tarif_categorie;
        private System.Windows.Forms.CheckBox checkBox_activer_tarif;
        private System.Windows.Forms.RadioButton radioButton_tarif_cmd_EDI;
        private System.Windows.Forms.Button button_reprocess;
        private System.Windows.Forms.RadioButton radioButton_tarif_produit;
        private System.Windows.Forms.Button button_tmp;
    }
}