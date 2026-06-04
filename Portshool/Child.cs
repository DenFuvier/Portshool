using MySql.Data.MySqlClient;
using Portshool.HelpConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Portshool
{
    public partial class Child : Form
    {

        public Child()
        {
            InitializeComponent();

            if (!UiTheme.IsDesignMode)
            {
                PrepareInterface();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Autoriz AAA = new Autoriz();
            AAA.Show();
            Close();
        }

        private void Child_Load(object sender, EventArgs e)
        {
            if (UiTheme.IsDesignMode)
            {
                return;
            }

            LoadStudentInfo();
            LoadGrades();
            LoadAchievements();
        }
        private void LoadStudentInfo()
        {
            string cs = SqlConnect.GetConnect();

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                con.Open();

                string query =
                    $"SELECT s.surname, s.name, s.patronymic, s.birth_date, s.address, c.class_name " +
                    $"FROM students s " +
                    $"INNER JOIN classes c ON s.class_id = c.class_id " +
                    $"WHERE s.user_id = @userId";

                MySqlCommand cmd = new MySqlCommand(query, con);
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

                    birthday_l.Text =
                        "Дата рождения: " +
                        Convert.ToDateTime(reader["birth_date"])
                        .ToString("dd.MM.yyyy");

                    Adress_l.Text = "Адрес: " + reader["address"];
                    Class_L.Text = "Класс: " + reader["class_name"];

                    PortfolioFileHelper.LoadPersonPhoto(pictureBox1, surname, name, patronymic);
                }
            }
        }
        private void LoadGrades()
        {
            string cs = SqlConnect.GetConnect();

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                con.Open();

                string query = @"
        SELECT
            d.discipline_name,
            g.grade_name,
            g.grade_id,
            ap.assessment_date
        FROM academic_performance ap
        INNER JOIN students s
            ON ap.student_id = s.student_id
        INNER JOIN disciplines d
            ON ap.discipline_id = d.discipline_id
        INNER JOIN grades g
            ON ap.grade_id = g.grade_id
        WHERE s.user_id = @userId
        ORDER BY d.discipline_name, ap.assessment_date";

                MySqlDataAdapter da =
                    new MySqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@userId", Session.UserId);

                DataTable source = new DataTable();

                da.Fill(source);

                GradeDataW.DataSource = BuildGradeMatrix(source);
            }
        }

        private DataTable BuildGradeMatrix(DataTable source)
        {
            DataTable result = new DataTable();
            result.Columns.Add("Предмет");

            Dictionary<string, List<int>> gradePointsBySubject = new Dictionary<string, List<int>>();
            Dictionary<string, DataRow> rowsBySubject = new Dictionary<string, DataRow>();

            foreach (DataRow sourceRow in source.Rows)
            {
                string dateColumn = Convert.ToDateTime(sourceRow["assessment_date"]).ToString("dd.MM.yyyy");

                if (!result.Columns.Contains(dateColumn))
                {
                    result.Columns.Add(dateColumn);
                }
            }

            result.Columns.Add("Средняя оценка");

            foreach (DataRow sourceRow in source.Rows)
            {
                string subject = sourceRow["discipline_name"].ToString();
                string dateColumn = Convert.ToDateTime(sourceRow["assessment_date"]).ToString("dd.MM.yyyy");
                string gradeName = sourceRow["grade_name"].ToString();
                int gradePoint = GetGradePoint(Convert.ToInt32(sourceRow["grade_id"]));

                if (!rowsBySubject.ContainsKey(subject))
                {
                    DataRow newRow = result.NewRow();
                    newRow["Предмет"] = subject;
                    result.Rows.Add(newRow);
                    rowsBySubject[subject] = newRow;
                    gradePointsBySubject[subject] = new List<int>();
                }

                DataRow row = rowsBySubject[subject];

                if (row[dateColumn] == DBNull.Value || string.IsNullOrWhiteSpace(row[dateColumn].ToString()))
                {
                    row[dateColumn] = gradeName;
                }
                else
                {
                    row[dateColumn] = row[dateColumn] + ", " + gradeName;
                }

                gradePointsBySubject[subject].Add(gradePoint);
            }

            foreach (string subject in rowsBySubject.Keys)
            {
                List<int> points = gradePointsBySubject[subject];
                double average = 0;

                foreach (int point in points)
                {
                    average += point;
                }

                rowsBySubject[subject]["Средняя оценка"] = (average / points.Count).ToString("0.00");
            }

            return result;
        }

        private int GetGradePoint(int gradeId)
        {
            switch (gradeId)
            {
                case 1:
                    return 5;
                case 2:
                    return 4;
                case 3:
                    return 3;
                case 4:
                    return 2;
                default:
                    return 0;
            }
        }
        private void LoadAchievements()
        {
            string cs = SqlConnect.GetConnect();

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                con.Open();

                string query = @"
        SELECT
            a.title AS 'Название',
            a.achievement_type AS 'Тип',
            a.achievement_date AS 'Дата'
        FROM achievements a
        INNER JOIN students s
            ON a.student_id = s.student_id
        WHERE s.user_id = @userId";

                MySqlDataAdapter da =
                    new MySqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@userId", Session.UserId);

                DataTable dt = new DataTable();

                da.Fill(dt);

                AchivmentData.DataSource = dt;
            }
        }

        private void PrepareInterface()
        {
            UiTheme.ApplyForm(this);
            ClientSize = new System.Drawing.Size(980, 720);
            MinimumSize = new System.Drawing.Size(900, 640);
            Text = "Электронное портфолио - ученик";

            Controls.Add(UiTheme.CreateTitle("Портфолио ученика", 24, 18, 480));

            pictureBox1.Location = new System.Drawing.Point(24, 78);
            pictureBox1.Size = new System.Drawing.Size(130, 150);
            UiTheme.StylePhotoBox(pictureBox1);

            Exit.Text = "Выйти";
            Exit.Location = new System.Drawing.Point(ClientSize.Width - 132, 24);
            Exit.Size = new System.Drawing.Size(108, 34);
            Exit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            UiTheme.StyleSecondaryButton(Exit);

            Surname_L.Location = new System.Drawing.Point(174, 78);
            name_L.Location = new System.Drawing.Point(174, 124);
            Ochag_L.Location = new System.Drawing.Point(174, 170);
            birthday_l.Location = new System.Drawing.Point(496, 78);
            Adress_l.Location = new System.Drawing.Point(496, 124);
            Class_L.Location = new System.Drawing.Point(496, 170);

            foreach (Label label in new[] { Surname_L, name_L, Ochag_L, birthday_l, Adress_l, Class_L })
            {
                UiTheme.StyleInfoLabel(label);
                label.Size = new System.Drawing.Size(292, 38);
            }

            label1.Text = "Учебная динамика";
            label1.Location = new System.Drawing.Point(24, 230);
            label1.Size = new System.Drawing.Size(560, 28);
            UiTheme.StyleSectionLabel(label1);

            label2.Text = "Достижения и активность";
            label2.Location = new System.Drawing.Point(620, 230);
            label2.Size = new System.Drawing.Size(320, 28);
            UiTheme.StyleSectionLabel(label2);

            GradeDataW.Location = new System.Drawing.Point(24, 264);
            GradeDataW.Size = new System.Drawing.Size(560, 420);
            GradeDataW.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleGrid(GradeDataW);

            AchivmentData.Location = new System.Drawing.Point(620, 264);
            AchivmentData.Size = new System.Drawing.Size(336, 420);
            AchivmentData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UiTheme.StyleGrid(AchivmentData);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
