namespace ConnecteurSage.Forms
{
    partial class ExportBonLivraison
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportBonLivraison));
            this.LinesDataGridView = new System.Windows.Forms.DataGridView();
            this.countryTextBox = new System.Windows.Forms.TextBox();
            this.zipCodeTextBox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.cityTextBox = new System.Windows.Forms.TextBox();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.thirdNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BonLivraisonDataGridView = new System.Windows.Forms.DataGridView();
            this.thirdIdTextBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.Button();
            this.customersDataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.LinesDataGridView)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BonLivraisonDataGridView)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customersDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LinesDataGridView
            // 
            this.LinesDataGridView.AllowUserToAddRows = false;
            this.LinesDataGridView.AllowUserToDeleteRows = false;
            this.LinesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.LinesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LinesDataGridView.Location = new System.Drawing.Point(6, 118);
            this.LinesDataGridView.MultiSelect = false;
            this.LinesDataGridView.Name = "LinesDataGridView";
            this.LinesDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LinesDataGridView.Size = new System.Drawing.Size(757, 152);
            this.LinesDataGridView.TabIndex = 14;
            // 
            // countryTextBox
            // 
            this.countryTextBox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countryTextBox.Location = new System.Drawing.Point(541, 83);
            this.countryTextBox.Name = "countryTextBox";
            this.countryTextBox.ReadOnly = true;
            this.countryTextBox.Size = new System.Drawing.Size(136, 24);
            this.countryTextBox.TabIndex = 13;
            // 
            // zipCodeTextBox
            // 
            this.zipCodeTextBox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zipCodeTextBox.Location = new System.Drawing.Point(150, 83);
            this.zipCodeTextBox.Name = "zipCodeTextBox";
            this.zipCodeTextBox.ReadOnly = true;
            this.zipCodeTextBox.Size = new System.Drawing.Size(105, 24);
            this.zipCodeTextBox.TabIndex = 11;
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(664, 558);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(110, 30);
            this.closeButton.TabIndex = 18;
            this.closeButton.Text = "Fermer";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click_1);
            // 
            // cityTextBox
            // 
            this.cityTextBox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cityTextBox.Location = new System.Drawing.Point(315, 83);
            this.cityTextBox.Name = "cityTextBox";
            this.cityTextBox.ReadOnly = true;
            this.cityTextBox.Size = new System.Drawing.Size(177, 24);
            this.cityTextBox.TabIndex = 12;
            // 
            // addressTextBox
            // 
            this.addressTextBox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressTextBox.Location = new System.Drawing.Point(150, 54);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.ReadOnly = true;
            this.addressTextBox.Size = new System.Drawing.Size(527, 24);
            this.addressTextBox.TabIndex = 10;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(14, 489);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(760, 63);
            this.groupBox4.TabIndex = 22;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Chemin d\'export :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(647, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 27);
            this.button1.TabIndex = 3;
            this.button1.Text = "Parcourir";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(6, 26);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(635, 27);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // thirdNameTextBox
            // 
            this.thirdNameTextBox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.thirdNameTextBox.Location = new System.Drawing.Point(395, 24);
            this.thirdNameTextBox.Name = "thirdNameTextBox";
            this.thirdNameTextBox.ReadOnly = true;
            this.thirdNameTextBox.Size = new System.Drawing.Size(282, 24);
            this.thirdNameTextBox.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BonLivraisonDataGridView);
            this.groupBox2.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(377, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(403, 190);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Bons de livraisons";
            // 
            // BonLivraisonDataGridView
            // 
            this.BonLivraisonDataGridView.AllowUserToAddRows = false;
            this.BonLivraisonDataGridView.AllowUserToDeleteRows = false;
            this.BonLivraisonDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.BonLivraisonDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BonLivraisonDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BonLivraisonDataGridView.Location = new System.Drawing.Point(3, 23);
            this.BonLivraisonDataGridView.Name = "BonLivraisonDataGridView";
            this.BonLivraisonDataGridView.ReadOnly = true;
            this.BonLivraisonDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BonLivraisonDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.BonLivraisonDataGridView.Size = new System.Drawing.Size(397, 164);
            this.BonLivraisonDataGridView.TabIndex = 0;
            this.BonLivraisonDataGridView.SelectionChanged += new System.EventHandler(this.BonLivraisonDataGridView_SelectionChanged);
            // 
            // thirdIdTextBox
            // 
            this.thirdIdTextBox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.thirdIdTextBox.Location = new System.Drawing.Point(150, 24);
            this.thirdIdTextBox.Name = "thirdIdTextBox";
            this.thirdIdTextBox.ReadOnly = true;
            this.thirdIdTextBox.Size = new System.Drawing.Size(182, 24);
            this.thirdIdTextBox.TabIndex = 8;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.LinesDataGridView);
            this.groupBox3.Controls.Add(this.countryTextBox);
            this.groupBox3.Controls.Add(this.cityTextBox);
            this.groupBox3.Controls.Add(this.zipCodeTextBox);
            this.groupBox3.Controls.Add(this.addressTextBox);
            this.groupBox3.Controls.Add(this.thirdNameTextBox);
            this.groupBox3.Controls.Add(this.thirdIdTextBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(11, 206);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(769, 276);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Detail";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(67, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "Adresse :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(498, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "Pays :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(273, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Ville :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(67, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Code postal :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(351, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Nom :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(69, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Code client :";
            // 
            // importButton
            // 
            this.importButton.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importButton.Location = new System.Drawing.Point(552, 558);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(107, 30);
            this.importButton.TabIndex = 17;
            this.importButton.Text = "Exporter";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // customersDataGridView
            // 
            this.customersDataGridView.AllowUserToAddRows = false;
            this.customersDataGridView.AllowUserToDeleteRows = false;
            this.customersDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.customersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.customersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customersDataGridView.Location = new System.Drawing.Point(3, 23);
            this.customersDataGridView.MultiSelect = false;
            this.customersDataGridView.Name = "customersDataGridView";
            this.customersDataGridView.ReadOnly = true;
            this.customersDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.customersDataGridView.Size = new System.Drawing.Size(350, 164);
            this.customersDataGridView.TabIndex = 0;
            this.customersDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.customersDataGridView_CellContentClick);
            this.customersDataGridView.SelectionChanged += new System.EventHandler(this.customersDataGridView_SelectionChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.customersDataGridView);
            this.groupBox1.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(11, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 190);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clients";
            // 
            // ExportBonLivraison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 598);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportBonLivraison";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ExportBonLivraison";
            ((System.ComponentModel.ISupportInitialize)(this.LinesDataGridView)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BonLivraisonDataGridView)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customersDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView LinesDataGridView;
        private System.Windows.Forms.TextBox countryTextBox;
        private System.Windows.Forms.TextBox zipCodeTextBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TextBox cityTextBox;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox thirdNameTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView BonLivraisonDataGridView;
        private System.Windows.Forms.TextBox thirdIdTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.DataGridView customersDataGridView;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}