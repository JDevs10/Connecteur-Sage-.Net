namespace ConnecteurSage.Forms
{
    partial class Validation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Validation));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.Enregistre_bouton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "Veuillez saisir la clé d\'activation :";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(23, 167);
            this.textBox1.MaxLength = 4;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(61, 26);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Window;
            this.textBox2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(86, 167);
            this.textBox2.MaxLength = 4;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(61, 26);
            this.textBox2.TabIndex = 8;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.Window;
            this.textBox3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(149, 167);
            this.textBox3.MaxLength = 4;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(61, 26);
            this.textBox3.TabIndex = 9;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.Window;
            this.textBox4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(212, 167);
            this.textBox4.MaxLength = 4;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(61, 26);
            this.textBox4.TabIndex = 10;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(151, 214);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 30);
            this.button2.TabIndex = 12;
            this.button2.Text = "Annuler";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Enregistre_bouton
            // 
            this.Enregistre_bouton.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Enregistre_bouton.Location = new System.Drawing.Point(47, 214);
            this.Enregistre_bouton.Name = "Enregistre_bouton";
            this.Enregistre_bouton.Size = new System.Drawing.Size(103, 30);
            this.Enregistre_bouton.TabIndex = 11;
            this.Enregistre_bouton.Text = "Valider";
            this.Enregistre_bouton.UseVisualStyleBackColor = true;
            this.Enregistre_bouton.Click += new System.EventHandler(this.Enregistre_bouton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ConnecteurSage.Properties.Resources.sage_header;
            this.pictureBox1.Location = new System.Drawing.Point(-21, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(314, 92);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // Validation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 257);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Enregistre_bouton);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Validation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connecteur Sage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Enregistre_bouton;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}