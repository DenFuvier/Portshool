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
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Autoriz AAA = new Autoriz();
            AAA.Show();
            Close();
        }

        private void Teacher_Load(object sender, EventArgs e)
        {
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
                        $"WHERE user_id = {Session.UserId}";

                    MySqlCommand cmd = new MySqlCommand(stm, con);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        name_L.Text = reader["name"].ToString();
                        Surname_L.Text = reader["surname"].ToString();
                        Ochag_L.Text = reader["patronymic"].ToString();
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
            s.student_id,
            s.surname,
            s.name,
            s.patronymic
        FROM students s
        INNER JOIN classes c
            ON s.class_id = c.class_id
        INNER JOIN teachers t
            ON c.class_teacher_id = t.teacher_id
        WHERE t.user_id = " + Session.UserId;

                MySqlDataAdapter da =
                    new MySqlDataAdapter(query, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                ViewsChildren.DataSource = dt;
            }
        }
    }
}
