using MySql.Data.MySqlClient;
using Portshool.HelpConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portshool
{
    public partial class Teacher : Form
    {
        public Teacher()
        {
            InitializeComponent();

            if (!UiTheme.IsDesignMode)
            {
                PrepareInterface();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Autoriz AAA = new Autoriz();
            AAA.Show();
            Close();
        }

        private void Teacher_Load(object sender, EventArgs e)
        {
            if (UiTheme.IsDesignMode)
            {
                return;
            }

            LoadTeacherInfo();
            LoadStudents();

        }
        private void LoadTeacherInfo()
        {
            string cs = SqlConnect.GetConnect();

            try
            {
                using (MySqlConnection con = new MySqlConnection(cs))
                {
                    con.Open();

                    string stm =
                        $"SELECT teacher_id, surname, name, patronymic " +
                        $"FROM teachers " +
                        $"WHERE user_id = @userId";

                    MySqlCommand cmd = new MySqlCommand(stm, con);
                    cmd.Parameters.AddWithValue("@userId", Session.UserId);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string surname = reader["surname"].ToString();
                        string name = reader["name"].ToString();
                        string patronymic = reader["patronymic"].ToString();

                        Surname_L.Text = "Фамилия: " + surname;
                        name_L.Text = "Имя: " + name;
                        Ochag_L.Text = "Отчество: " + patronymic;

                        PortfolioFileHelper.LoadPersonPhoto(pictureBox1, surname, name, patronymic);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadStudents()
        {
            string cs = SqlConnect.GetConnect();

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                con.Open();

                string query = @"
        SELECT
            s.student_id AS 'ID',
            s.surname AS 'Фамилия',
            s.name AS 'Имя',
            s.patronymic AS 'Отчество',
            c.class_name AS 'Класс'
        FROM students s
        INNER JOIN classes c
            ON s.class_id = c.class_id
        INNER JOIN teachers t
            ON c.class_teacher_id = t.teacher_id
        WHERE t.user_id = @userId";

                MySqlDataAdapter da =
                    new MySqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@userId", Session.UserId);

                DataTable dt = new DataTable();

                da.Fill(dt);

                ViewsChildren.DataSource = dt;
            }
        }

        private void PrepareInterface()
        {
            UiTheme.ApplyForm(this);
            ClientSize = new System.Drawing.Size(920, 680);
            MinimumSize = new System.Drawing.Size(840, 600);
            Text = "Электронное портфолио - учитель";

            Controls.Add(UiTheme.CreateTitle("Куратор портфолио", 24, 18, 460));

            pictureBox1.Location = new System.Drawing.Point(24, 82);
            pictureBox1.Size = new System.Drawing.Size(130, 150);
            UiTheme.StylePhotoBox(pictureBox1);

            Exit.Text = "Выйти";
            Exit.Location = new System.Drawing.Point(ClientSize.Width - 132, 24);
            Exit.Size = new System.Drawing.Size(108, 34);
            Exit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Exit.Click += new EventHandler(label1_Click);
            UiTheme.StyleSecondaryButton(Exit);

            Surname_L.Location = new System.Drawing.Point(174, 82);
            name_L.Location = new System.Drawing.Point(174, 128);
            Ochag_L.Location = new System.Drawing.Point(174, 174);

            foreach (Label label in new[] { Surname_L, name_L, Ochag_L })
            {
                UiTheme.StyleInfoLabel(label);
                label.Size = new System.Drawing.Size(340, 38);
            }

            ViewsChildren.Location = new System.Drawing.Point(24, 244);
            ViewsChildren.Size = new System.Drawing.Size(872, 400);
            ViewsChildren.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UiTheme.StyleGrid(ViewsChildren);
        }
    }
}
