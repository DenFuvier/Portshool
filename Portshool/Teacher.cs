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
        private Label gradeSection;
        private ComboBox disciplineCombo;
        private ComboBox gradeCombo;
        private DateTimePicker gradeDate;
        private Button addGradeButton;

        private Label achievementSection;
        private TextBox achievementTitleBox;
        private TextBox achievementTypeBox;
        private DateTimePicker achievementDate;
        private Button addAchievementButton;

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
            LoadDisciplines();
            LoadGradeOptions();
        }

        private void LoadDisciplines()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT discipline_id, discipline_name FROM disciplines ORDER BY discipline_name", con);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    disciplineCombo.DisplayMember = "discipline_name";
                    disciplineCombo.ValueMember = "discipline_id";
                    disciplineCombo.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось загрузить предметы");
            }
        }

        private void LoadGradeOptions()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT grade_id, grade_name FROM grades ORDER BY grade_id", con);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gradeCombo.DisplayMember = "grade_name";
                    gradeCombo.ValueMember = "grade_id";
                    gradeCombo.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось загрузить оценки");
            }
        }

        private int? GetSelectedStudentId()
        {
            if (ViewsChildren.CurrentRow == null)
            {
                return null;
            }

            object value = ViewsChildren.CurrentRow.Cells["ID"].Value;

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(value);
        }

        private string GetSelectedStudentName()
        {
            if (ViewsChildren.CurrentRow == null)
            {
                return string.Empty;
            }

            object surname = ViewsChildren.CurrentRow.Cells["Фамилия"].Value;
            object name = ViewsChildren.CurrentRow.Cells["Имя"].Value;

            return (surname + " " + name).Trim();
        }

        private void AddGrade_Click(object sender, EventArgs e)
        {
            int? studentId = GetSelectedStudentId();

            if (studentId == null)
            {
                MessageBox.Show("Сначала выберите ученика в таблице.", "Не выбран ученик");
                return;
            }

            if (disciplineCombo.SelectedValue == null || gradeCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите предмет и оценку.", "Не заполнено");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    string stm =
                        "INSERT INTO academic_performance (student_id, discipline_id, grade_id, assessment_date) " +
                        "VALUES (@student, @discipline, @grade, @date)";

                    MySqlCommand cmd = new MySqlCommand(stm, con);
                    cmd.Parameters.AddWithValue("@student", studentId.Value);
                    cmd.Parameters.AddWithValue("@discipline", Convert.ToInt32(disciplineCombo.SelectedValue));
                    cmd.Parameters.AddWithValue("@grade", Convert.ToInt32(gradeCombo.SelectedValue));
                    cmd.Parameters.AddWithValue("@date", gradeDate.Value.Date);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Оценка «" + gradeCombo.Text + "» по предмету «" + disciplineCombo.Text +
                    "» выставлена ученику " + GetSelectedStudentName() + ".",
                    "Оценка добавлена",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось выставить оценку");
            }
        }

        private void AddAchievement_Click(object sender, EventArgs e)
        {
            int? studentId = GetSelectedStudentId();

            if (studentId == null)
            {
                MessageBox.Show("Сначала выберите ученика в таблице.", "Не выбран ученик");
                return;
            }

            string title = GetBoxValue(achievementTitleBox);
            string type = GetBoxValue(achievementTypeBox);

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Укажите название достижения.", "Не заполнено");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    string stm =
                        "INSERT INTO achievements (student_id, title, achievement_type, achievement_date) " +
                        "VALUES (@student, @title, @type, @date)";

                    MySqlCommand cmd = new MySqlCommand(stm, con);
                    cmd.Parameters.AddWithValue("@student", studentId.Value);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@type", string.IsNullOrWhiteSpace(type) ? (object)DBNull.Value : type);
                    cmd.Parameters.AddWithValue("@date", achievementDate.Value.Date);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Достижение «" + title + "» добавлено ученику " + GetSelectedStudentName() + ".",
                    "Достижение добавлено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ResetPlaceholder(achievementTitleBox);
                ResetPlaceholder(achievementTypeBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось добавить достижение");
            }
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
            ClientSize = new System.Drawing.Size(920, 720);
            MinimumSize = new System.Drawing.Size(880, 720);
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
            ViewsChildren.Size = new System.Drawing.Size(872, 224);
            ViewsChildren.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UiTheme.StyleGrid(ViewsChildren);

            BuildGradePanel();
            BuildAchievementPanel();
        }

        private void BuildGradePanel()
        {
            gradeSection = new Label();
            gradeSection.Text = "Поставить оценку выбранному ученику";
            gradeSection.Location = new System.Drawing.Point(24, 484);
            gradeSection.Size = new System.Drawing.Size(560, 24);
            gradeSection.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleSectionLabel(gradeSection);
            Controls.Add(gradeSection);

            disciplineCombo = new ComboBox();
            disciplineCombo.Location = new System.Drawing.Point(24, 514);
            disciplineCombo.Size = new System.Drawing.Size(244, 28);
            disciplineCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StyleCombo(disciplineCombo);
            Controls.Add(disciplineCombo);

            gradeCombo = new ComboBox();
            gradeCombo.Location = new System.Drawing.Point(280, 514);
            gradeCombo.Size = new System.Drawing.Size(150, 28);
            gradeCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StyleCombo(gradeCombo);
            Controls.Add(gradeCombo);

            gradeDate = new DateTimePicker();
            gradeDate.Location = new System.Drawing.Point(442, 514);
            gradeDate.Size = new System.Drawing.Size(140, 28);
            gradeDate.Format = DateTimePickerFormat.Short;
            gradeDate.Font = UiTheme.TextFont;
            gradeDate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(gradeDate);

            addGradeButton = new Button();
            addGradeButton.Text = "Поставить оценку";
            addGradeButton.Location = new System.Drawing.Point(594, 512);
            addGradeButton.Size = new System.Drawing.Size(220, 32);
            addGradeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            addGradeButton.Click += new EventHandler(AddGrade_Click);
            UiTheme.StylePrimaryButton(addGradeButton);
            Controls.Add(addGradeButton);
        }

        private void BuildAchievementPanel()
        {
            achievementSection = new Label();
            achievementSection.Text = "Добавить достижение выбранному ученику";
            achievementSection.Location = new System.Drawing.Point(24, 568);
            achievementSection.Size = new System.Drawing.Size(560, 24);
            achievementSection.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleSectionLabel(achievementSection);
            Controls.Add(achievementSection);

            achievementTitleBox = new TextBox();
            achievementTitleBox.Location = new System.Drawing.Point(24, 598);
            achievementTitleBox.Size = new System.Drawing.Size(244, 28);
            achievementTitleBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleTextBox(achievementTitleBox);
            SetPlaceholder(achievementTitleBox, "Название достижения");
            Controls.Add(achievementTitleBox);

            achievementTypeBox = new TextBox();
            achievementTypeBox.Location = new System.Drawing.Point(280, 598);
            achievementTypeBox.Size = new System.Drawing.Size(150, 28);
            achievementTypeBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleTextBox(achievementTypeBox);
            SetPlaceholder(achievementTypeBox, "Тип (грамота, диплом…)");
            Controls.Add(achievementTypeBox);

            achievementDate = new DateTimePicker();
            achievementDate.Location = new System.Drawing.Point(442, 598);
            achievementDate.Size = new System.Drawing.Size(140, 28);
            achievementDate.Format = DateTimePickerFormat.Short;
            achievementDate.Font = UiTheme.TextFont;
            achievementDate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(achievementDate);

            addAchievementButton = new Button();
            addAchievementButton.Text = "Добавить достижение";
            addAchievementButton.Location = new System.Drawing.Point(594, 596);
            addAchievementButton.Size = new System.Drawing.Size(220, 32);
            addAchievementButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            addAchievementButton.Click += new EventHandler(AddAchievement_Click);
            UiTheme.StylePrimaryButton(addAchievementButton);
            Controls.Add(addAchievementButton);
        }

        private static void StyleCombo(ComboBox combo)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.FlatStyle = FlatStyle.Flat;
            combo.Font = UiTheme.TextFont;
            combo.BackColor = UiTheme.Surface;
            combo.ForeColor = UiTheme.Text;
        }

        private void SetPlaceholder(TextBox box, string hint)
        {
            box.Tag = hint;
            ResetPlaceholder(box);

            box.GotFocus += (s, e) =>
            {
                if (box.Text == hint)
                {
                    box.Text = string.Empty;
                    box.ForeColor = UiTheme.Text;
                }
            };

            box.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(box.Text))
                {
                    ResetPlaceholder(box);
                }
            };
        }

        private void ResetPlaceholder(TextBox box)
        {
            box.Text = box.Tag as string ?? string.Empty;
            box.ForeColor = UiTheme.MutedText;
        }

        private string GetBoxValue(TextBox box)
        {
            string hint = box.Tag as string;

            if (box.Text == hint)
            {
                return string.Empty;
            }

            return box.Text.Trim();
        }
    }
}
