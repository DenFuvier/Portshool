namespace Portshool
{
    partial class Teacher
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
            this.ViewsChildren = new System.Windows.Forms.DataGridView();
            this.Surname_L = new System.Windows.Forms.Label();
            this.Ochag_L = new System.Windows.Forms.Label();
            this.name_L = new System.Windows.Forms.Label();
            this.Exit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ViewsChildren)).BeginInit();
            this.SuspendLayout();
            // 
            // ViewsChildren
            // 
            this.ViewsChildren.BackgroundColor = System.Drawing.Color.Beige;
            this.ViewsChildren.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ViewsChildren.Location = new System.Drawing.Point(12, 160);
            this.ViewsChildren.Name = "ViewsChildren";
            this.ViewsChildren.Size = new System.Drawing.Size(776, 564);
            this.ViewsChildren.TabIndex = 5;
            // 
            // Surname_L
            // 
            this.Surname_L.AutoSize = true;
            this.Surname_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Surname_L.Location = new System.Drawing.Point(12, 50);
            this.Surname_L.Name = "Surname_L";
            this.Surname_L.Size = new System.Drawing.Size(101, 24);
            this.Surname_L.TabIndex = 2;
            this.Surname_L.Text = "Фамилия";
            // 
            // Ochag_L
            // 
            this.Ochag_L.AutoSize = true;
            this.Ochag_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Ochag_L.Location = new System.Drawing.Point(12, 95);
            this.Ochag_L.Name = "Ochag_L";
            this.Ochag_L.Size = new System.Drawing.Size(98, 24);
            this.Ochag_L.TabIndex = 3;
            this.Ochag_L.Text = "Отчество";
            // 
            // name_L
            // 
            this.name_L.AutoSize = true;
            this.name_L.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.name_L.Location = new System.Drawing.Point(12, 8);
            this.name_L.Name = "name_L";
            this.name_L.Size = new System.Drawing.Size(51, 24);
            this.name_L.TabIndex = 4;
            this.name_L.Text = "Имя";
            // 
            // Exit
            // 
            this.Exit.Location = new System.Drawing.Point(610, 8);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(178, 23);
            this.Exit.TabIndex = 6;
            this.Exit.Text = "Деаутефикация";
            this.Exit.UseVisualStyleBackColor = true;
            // 
            // Teacher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 731);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.ViewsChildren);
            this.Controls.Add(this.Surname_L);
            this.Controls.Add(this.Ochag_L);
            this.Controls.Add(this.name_L);
            this.Name = "Teacher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Учитель";
            this.Load += new System.EventHandler(this.Teacher_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ViewsChildren)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView ViewsChildren;
        private System.Windows.Forms.Label Surname_L;
        private System.Windows.Forms.Label Ochag_L;
        private System.Windows.Forms.Label name_L;
        private System.Windows.Forms.Button Exit;
    }
}