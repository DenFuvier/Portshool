using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Portshool
{
    public static class UiTheme
    {
        public static readonly Color Page = Color.FromArgb(245, 247, 251);
        public static readonly Color Surface = Color.White;
        public static readonly Color Primary = Color.FromArgb(35, 88, 166);
        public static readonly Color PrimaryDark = Color.FromArgb(24, 59, 112);
        public static readonly Color Accent = Color.FromArgb(237, 147, 53);
        public static readonly Color Border = Color.FromArgb(220, 226, 236);
        public static readonly Color Text = Color.FromArgb(33, 42, 58);
        public static readonly Color MutedText = Color.FromArgb(95, 108, 128);

        public static readonly Font TitleFont = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        public static readonly Font TextFont = new Font("Segoe UI", 10F);
        public static readonly Font SmallFont = new Font("Segoe UI", 9F);

        public static bool IsDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                    || Process.GetCurrentProcess().ProcessName.ToLower().Contains("devenv");
            }
        }

        public static void ApplyForm(Form form)
        {
            form.BackColor = Page;
            form.Font = TextFont;
            form.ForeColor = Text;
            form.MinimumSize = new Size(760, 520);
        }

        public static void StylePrimaryButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Primary;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.Height = 36;
        }

        public static void StyleSecondaryButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Border;
            button.BackColor = Surface;
            button.ForeColor = PrimaryDark;
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.Height = 34;
        }

        public static void StyleTextBox(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = TextFont;
            textBox.ForeColor = Text;
            textBox.BackColor = Surface;
            textBox.Height = 28;
        }

        public static void StyleInfoLabel(Label label)
        {
            label.AutoSize = false;
            label.Font = SectionFont;
            label.ForeColor = Text;
            label.BackColor = Surface;
            label.Padding = new Padding(12, 8, 12, 8);
        }

        public static void StyleSectionLabel(Label label)
        {
            label.AutoSize = false;
            label.Font = SectionFont;
            label.ForeColor = PrimaryDark;
            label.BackColor = Page;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 6, 6, 6);
            grid.ColumnHeadersHeight = 42;

            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(223, 236, 255);
            grid.DefaultCellStyle.SelectionForeColor = PrimaryDark;
            grid.DefaultCellStyle.Font = TextFont;
            grid.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 251, 254);
        }

        public static void StylePhotoBox(PictureBox pictureBox)
        {
            pictureBox.BackColor = Surface;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public static Label CreateTitle(string text, int x, int y, int width)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 38),
                Font = TitleFont,
                ForeColor = PrimaryDark,
                BackColor = Page
            };
        }
    }
}
