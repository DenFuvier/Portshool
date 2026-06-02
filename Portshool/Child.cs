using MySql.Data.MySqlClient;
using Portshool.HelpConnect;
using System;
using System.Data;
using System.Windows.Forms;

namespace Portshool
{
    public partial class Child : Form
    {

        public Child()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Autoriz AAA = new Autoriz();
            AAA.Show();
            Close();
        }

        private void Child_Load(object sender, EventArgs e)
        {
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
                    $"SELECT surname, name, patronymic " +
                    $"FROM students " +
                    $"WHERE user_id = {Session.UserId}";

                MySqlCommand cmd = new MySqlCommand(query, con);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    name_L.Text = reader["name"].ToString();
                    Surname_L.Text = reader["surname"].ToString();
                    Ochag_L.Text = reader["patronymic"].ToString();
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
            d.discipline_name AS 'Предмет',
            g.grade_name AS 'Оценка',
            ap.assessment_date AS 'Дата'
        FROM academic_performance ap
        INNER JOIN students s
            ON ap.student_id = s.student_id
        INNER JOIN disciplines d
            ON ap.discipline_id = d.discipline_id
        INNER JOIN grades g
            ON ap.grade_id = g.grade_id
        WHERE s.user_id = " + Session.UserId;

                MySqlDataAdapter da =
                    new MySqlDataAdapter(query, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                GradeDataW.DataSource = dt;
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
        WHERE s.user_id = " + Session.UserId;

                MySqlDataAdapter da =
                    new MySqlDataAdapter(query, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                AchivmentData.DataSource = dt;
            }
        }
    }
}
