namespace Portshool
{
    partial class Child
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
            this.name_L = new System.Windows.Forms.Label();
            this.Surname_L = new System.Windows.Forms.Label();
            this.GradeDataW = new System.Windows.Forms.DataGridView();
            this.AchivmentData = new System.Windows.Forms.DataGridView();
            this.Exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Ochag_L = new System.Windows.Forms.Label();
            this.Adress_l = new System.Windows.Forms.Label();
            this.birthday_l = new System.Windows.Forms.Label();
            this.Class_L = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.GradeDataW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AchivmentData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // name_L
            // 
            this.name_L.AutoSize = true;
            this.name_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.name_L.Location = new System.Drawing.Point(123, 53);
            this.name_L.Name = "name_L";
            this.name_L.Size = new System.Drawing.Size(51, 24);
            this.name_L.TabIndex = 0;
            this.name_L.Text = "Имя";
            // 
            // Surname_L
            // 
            this.Surname_L.AutoSize = true;
            this.Surname_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Surname_L.Location = new System.Drawing.Point(123, 11);
            this.Surname_L.Name = "Surname_L";
            this.Surname_L.Size = new System.Drawing.Size(101, 24);
            this.Surname_L.TabIndex = 0;
            this.Surname_L.Text = "Фамилия";
            // 
            // GradeDataW
            // 
            this.GradeDataW.BackgroundColor = System.Drawing.Color.Beige;
            this.GradeDataW.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GradeDataW.Location = new System.Drawing.Point(12, 206);
            this.GradeDataW.Name = "GradeDataW";
            this.GradeDataW.Size = new System.Drawing.Size(367, 519);
            this.GradeDataW.TabIndex = 1;
            // 
            // AchivmentData
            // 
            this.AchivmentData.BackgroundColor = System.Drawing.Color.Beige;
            this.AchivmentData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AchivmentData.Location = new System.Drawing.Point(409, 206);
            this.AchivmentData.Name = "AchivmentData";
            this.AchivmentData.Size = new System.Drawing.Size(379, 519);
            this.AchivmentData.TabIndex = 2;
            // 
            // Exit
            // 
            this.Exit.Location = new System.Drawing.Point(610, 10);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(178, 23);
            this.Exit.TabIndex = 3;
            this.Exit.Text = "Деаутефикация";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(123, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "Успеваемость";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(537, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Достижения";
            // 
            // Ochag_L
            // 
            this.Ochag_L.AutoSize = true;
            this.Ochag_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Ochag_L.Location = new System.Drawing.Point(123, 98);
            this.Ochag_L.Name = "Ochag_L";
            this.Ochag_L.Size = new System.Drawing.Size(98, 24);
            this.Ochag_L.TabIndex = 0;
            this.Ochag_L.Text = "Отчество";
            // 
            // Adress_l
            // 
            this.Adress_l.AutoSize = true;
            this.Adress_l.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Adress_l.Location = new System.Drawing.Point(281, 53);
            this.Adress_l.Name = "Adress_l";
            this.Adress_l.Size = new System.Drawing.Size(67, 24);
            this.Adress_l.TabIndex = 6;
            this.Adress_l.Text = "Адрес";
            // 
            // birthday_l
            // 
            this.birthday_l.AutoSize = true;
            this.birthday_l.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.birthday_l.Location = new System.Drawing.Point(281, 11);
            this.birthday_l.Name = "birthday_l";
            this.birthday_l.Size = new System.Drawing.Size(151, 24);
            this.birthday_l.TabIndex = 7;
            this.birthday_l.Text = "Дата рождения";
            // 
            // Class_L
            // 
            this.Class_L.AutoSize = true;
            this.Class_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Class_L.Location = new System.Drawing.Point(281, 98);
            this.Class_L.Name = "Class_L";
            this.Class_L.Size = new System.Drawing.Size(68, 24);
            this.Class_L.TabIndex = 8;
            this.Class_L.Text = "Класс";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 110);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // Child
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 770);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Class_L);
            this.Controls.Add(this.birthday_l);
            this.Controls.Add(this.Adress_l);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.AchivmentData);
            this.Controls.Add(this.GradeDataW);
            this.Controls.Add(this.Surname_L);
            this.Controls.Add(this.Ochag_L);
            this.Controls.Add(this.name_L);
            this.Name = "Child";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ученик";
            this.Load += new System.EventHandler(this.Child_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GradeDataW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AchivmentData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label name_L;
        private System.Windows.Forms.Label Surname_L;
        private System.Windows.Forms.DataGridView GradeDataW;
        private System.Windows.Forms.DataGridView AchivmentData;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Ochag_L;
        private System.Windows.Forms.Label Adress_l;
        private System.Windows.Forms.Label birthday_l;
        private System.Windows.Forms.Label Class_L;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
