using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Portshool.HelpConnect;

namespace Portshool
{
    public partial class Admin : Form
    {
        private DataGridView tableGrid;
        private Label tableTitle;

        public Admin()
        {
            InitializeComponent();

            if (!UiTheme.IsDesignMode)
            {
                PrepareInterface();
            }
        }

        private void PrepareInterface()
        {
            UiTheme.ApplyForm(this);
            ClientSize = new Size(900, 560);
            MinimumSize = new Size(820, 520);
            Text = "Электронное портфолио - администратор";

            Controls.Add(UiTheme.CreateTitle("Панель администратора", 24, 18, 460));

            tableTitle = new Label();
            tableTitle.Text = "Выберите раздел";
            tableTitle.Location = new Point(300, 86);
            tableTitle.Size = new Size(560, 28);
            UiTheme.StyleSectionLabel(tableTitle);
            Controls.Add(tableTitle);

            tableGrid = new DataGridView();
            tableGrid.Location = new Point(300, 126);
            tableGrid.Size = new Size(560, 380);
            tableGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UiTheme.StyleGrid(tableGrid);
            Controls.Add(tableGrid);

            button1.Text = "Пользователи";
            button2.Text = "Ученики";
            button3.Text = "Учителя";
            button4.Text = "Классы";
            button5.Text = "Успеваемость";
            button6.Text = "Достижения";

            Button[] buttons = { button1, button2, button3, button4, button5, button6 };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Location = new Point(24, 86 + i * 48);
                buttons[i].Size = new Size(240, 36);
                UiTheme.StyleSecondaryButton(buttons[i]);
            }

            button1.Click += (sender, e) => LoadTable(
                "Пользователи",
                "SELECT user_id AS 'ID', login AS 'Логин', role AS 'Роль' FROM users");

            button2.Click += (sender, e) => LoadTable(
                "Ученики",
                "SELECT student_id AS 'ID', surname AS 'Фамилия', name AS 'Имя', patronymic AS 'Отчество', birth_date AS 'Дата рождения', address AS 'Адрес' FROM students");

            button3.Click += (sender, e) => LoadTable(
                "Учителя",
                "SELECT teacher_id AS 'ID', surname AS 'Фамилия', name AS 'Имя', patronymic AS 'Отчество' FROM teachers");

            button4.Click += (sender, e) => LoadTable(
                "Классы",
                @"SELECT c.class_id AS 'ID', c.class_name AS 'Класс', t.surname AS 'Фамилия учителя', t.name AS 'Имя учителя', t.patronymic AS 'Отчество учителя'
                  FROM classes c
                  LEFT JOIN teachers t ON c.class_teacher_id = t.teacher_id");

            button5.Click += (sender, e) => LoadTable(
                "Успеваемость",
                @"SELECT s.surname AS 'Фамилия', s.name AS 'Имя', d.discipline_name AS 'Предмет', g.grade_name AS 'Оценка', ap.assessment_date AS 'Дата'
                  FROM academic_performance ap
                  INNER JOIN students s ON ap.student_id = s.student_id
                  INNER JOIN disciplines d ON ap.discipline_id = d.discipline_id
                  INNER JOIN grades g ON ap.grade_id = g.grade_id");

            button6.Click += (sender, e) => LoadTable(
                "Достижения",
                @"SELECT s.surname AS 'Фамилия', s.name AS 'Имя', a.title AS 'Название', a.achievement_type AS 'Тип', a.achievement_date AS 'Дата'
                  FROM achievements a
                  INNER JOIN students s ON a.student_id = s.student_id");
        }

        private void LoadTable(string title, string query)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    tableTitle.Text = title;
                    tableGrid.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки данных");
            }
        }
    }
}
