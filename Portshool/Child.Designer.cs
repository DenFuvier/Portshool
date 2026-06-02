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
            this.Ochag_L = new System.Windows.Forms.Label();
            this.GradeDataW = new System.Windows.Forms.DataGridView();
            this.AchivmentData = new System.Windows.Forms.DataGridView();
            this.Exit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.GradeDataW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AchivmentData)).BeginInit();
            this.SuspendLayout();
            // 
            // name_L
            // 
            this.name_L.AutoSize = true;
            this.name_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.name_L.Location = new System.Drawing.Point(12, 9);
            this.name_L.Name = "name_L";
            this.name_L.Size = new System.Drawing.Size(51, 24);
            this.name_L.TabIndex = 0;
            this.name_L.Text = "Имя";
            // 
            // Surname_L
            // 
            this.Surname_L.AutoSize = true;
            this.Surname_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Surname_L.Location = new System.Drawing.Point(12, 51);
            this.Surname_L.Name = "Surname_L";
            this.Surname_L.Size = new System.Drawing.Size(101, 24);
            this.Surname_L.TabIndex = 0;
            this.Surname_L.Text = "Фамилия";
            // 
            // Ochag_L
            // 
            this.Ochag_L.AutoSize = true;
            this.Ochag_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Ochag_L.Location = new System.Drawing.Point(12, 96);
            this.Ochag_L.Name = "Ochag_L";
            this.Ochag_L.Size = new System.Drawing.Size(98, 24);
            this.Ochag_L.TabIndex = 0;
            this.Ochag_L.Text = "Отчество";
            // 
            // GradeDataW
            // 
            this.GradeDataW.BackgroundColor = System.Drawing.Color.Beige;
            this.GradeDataW.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GradeDataW.Location = new System.Drawing.Point(12, 161);
            this.GradeDataW.Name = "GradeDataW";
            this.GradeDataW.Size = new System.Drawing.Size(367, 564);
            this.GradeDataW.TabIndex = 1;
            // 
            // AchivmentData
            // 
            this.AchivmentData.BackgroundColor = System.Drawing.Color.Beige;
            this.AchivmentData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AchivmentData.Location = new System.Drawing.Point(409, 161);
            this.AchivmentData.Name = "AchivmentData";
            this.AchivmentData.Size = new System.Drawing.Size(379, 564);
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
            // Child
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 770);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label name_L;
        private System.Windows.Forms.Label Surname_L;
        private System.Windows.Forms.Label Ochag_L;
        private System.Windows.Forms.DataGridView GradeDataW;
        private System.Windows.Forms.DataGridView AchivmentData;
        private System.Windows.Forms.Button Exit;
    }
}