using MySql.Data.MySqlClient;
using Portshool.HelpConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private ComboBox achievementTypeCombo;
        private DateTimePicker achievementDate;
        private Button addAchievementButton;
        private TextBox achievementDescBox;
        private Button attachPhotoToAchBtn;
        private Label pendingPhotoLabel;
        private string _pendingAchievementPhotoPath;

        private Label _studentsSection;
        private Label _teacherClassLabel;

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
            string type = achievementTypeCombo.SelectedItem != null
                ? achievementTypeCombo.SelectedItem.ToString()
                : string.Empty;
            string desc = GetBoxValue(achievementDescBox);

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Укажите название достижения.", "Не заполнено");
                return;
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("Выберите тип достижения.", "Не заполнено");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO achievements (student_id, title, achievement_type, achievement_date, description) " +
                        "VALUES (@student, @title, @type, @date, @desc)", con);
                    cmd.Parameters.AddWithValue("@student", studentId.Value);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@date", achievementDate.Value.Date);
                    cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(desc) ? (object)DBNull.Value : desc);
                    cmd.ExecuteNonQuery();

                    long newAchId = cmd.LastInsertedId;

                    // Сохраняем фото, если было выбрано
                    if (!string.IsNullOrEmpty(_pendingAchievementPhotoPath) &&
                        File.Exists(_pendingAchievementPhotoPath))
                    {
                        string docDir = PortfolioFileHelper.GetPortfolioDirectory("Documents");
                        if (!Directory.Exists(docDir))
                            Directory.CreateDirectory(docDir);

                        string ext      = Path.GetExtension(_pendingAchievementPhotoPath);
                        string safeName = AchSafeFileName(title) + "_ach" + newAchId + "_" +
                                          DateTime.Now.ToString("yyyyMMdd_HHmmss") + ext;
                        string destPath = Path.Combine(docDir, safeName);

                        File.Copy(_pendingAchievementPhotoPath, destPath, overwrite: true);

                        MySqlCommand fileCmd = new MySqlCommand(
                            "INSERT INTO portfolio_files (student_id, file_name, file_path) VALUES (@s,@n,@p)", con);
                        fileCmd.Parameters.AddWithValue("@s", studentId.Value);
                        fileCmd.Parameters.AddWithValue("@n", safeName);
                        fileCmd.Parameters.AddWithValue("@p", destPath);
                        fileCmd.ExecuteNonQuery();
                    }
                }

                string photoNote = string.IsNullOrEmpty(_pendingAchievementPhotoPath)
                    ? string.Empty
                    : "\nФото: " + Path.GetFileName(_pendingAchievementPhotoPath);

                MessageBox.Show(
                    "Достижение «" + title + "» добавлено ученику " + GetSelectedStudentName() + "." + photoNote,
                    "Достижение добавлено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ResetPlaceholder(achievementTitleBox);
                ResetPlaceholder(achievementDescBox);
                achievementTypeCombo.SelectedIndex = 0;
                _pendingAchievementPhotoPath = null;
                pendingPhotoLabel.Text      = "Фото не выбрано";
                pendingPhotoLabel.ForeColor = UiTheme.MutedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось добавить достижение");
            }
        }
        private void ChangeTeacherPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title  = "Выбрать фото учителя";
                dlg.Filter = "Фото|*.jpg;*.jpeg;*.png;*.bmp";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string surname    = StripPrefix(Surname_L.Text);
                    string name       = StripPrefix(name_L.Text);
                    string patronymic = StripPrefix(Ochag_L.Text);

                    PortfolioFileHelper.UploadPersonPhoto(dlg.FileName, surname, name, patronymic, PersonType.Teacher);
                    PortfolioFileHelper.LoadPersonPhoto(pictureBox1, surname, name, patronymic, PersonType.Teacher);

                    MessageBox.Show("Фото обновлено.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка загрузки фото");
                }
            }
        }

        private static string StripPrefix(string labelText)
        {
            int i = labelText.IndexOf(':');
            return i >= 0 ? labelText.Substring(i + 1).Trim() : labelText.Trim();
        }

        private void AddStudent_Click(object sender, EventArgs e)
        {
            using (AddStudentForm form = new AddStudentForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadStudents();
            }
        }

        private void OpenStudentCard()
        {
            int? studentId = GetSelectedStudentId();
            if (studentId == null)
            { MessageBox.Show("Выберите ученика в таблице.", "Не выбрано"); return; }

            using (StudentDetailForm form = new StudentDetailForm(studentId.Value))
            {
                form.ShowDialog(this);
                LoadStudents();
            }
        }

        private void LoadTeacherInfo()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    string surname = string.Empty, name = string.Empty, patronymic = string.Empty;

                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT surname, name, patronymic FROM teachers WHERE user_id = @userId", con);
                    cmd.Parameters.AddWithValue("@userId", Session.UserId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            surname    = reader["surname"].ToString();
                            name       = reader["name"].ToString();
                            patronymic = reader["patronymic"].ToString();

                            Surname_L.Text = "Фамилия: " + surname;
                            name_L.Text    = "Имя: "      + name;
                            Ochag_L.Text   = "Отчество: " + patronymic;
                        }
                    }

                    // Загружаем классы этого учителя
                    MySqlCommand classCmd = new MySqlCommand(
                        "SELECT GROUP_CONCAT(c.class_name ORDER BY c.class_name SEPARATOR ', ') " +
                        "FROM classes c " +
                        "INNER JOIN teachers t ON c.class_teacher_id = t.teacher_id " +
                        "WHERE t.user_id = @uid", con);
                    classCmd.Parameters.AddWithValue("@uid", Session.UserId);

                    object classResult = classCmd.ExecuteScalar();
                    string classesStr  = (classResult != null && classResult != DBNull.Value)
                        ? classResult.ToString() : "—";

                    _teacherClassLabel.Text = "Класс: " + classesStr;
                    _studentsSection.Text   = "Ученики класса " + classesStr;

                    if (!string.IsNullOrEmpty(surname))
                        PortfolioFileHelper.LoadPersonPhoto(pictureBox1, surname, name, patronymic, PersonType.Teacher);
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
            ClientSize = new System.Drawing.Size(920, 758);
            MinimumSize = new System.Drawing.Size(880, 758);
            Text = "Электронное портфолио - учитель";

            Controls.Add(UiTheme.CreateTitle("Куратор портфолио", 24, 18, 460));

            pictureBox1.Location = new System.Drawing.Point(24, 82);
            pictureBox1.Size = new System.Drawing.Size(130, 150);
            UiTheme.StylePhotoBox(pictureBox1);

            Button changePhotoBtn = new Button();
            changePhotoBtn.Text = "Изменить фото";
            changePhotoBtn.Location = new System.Drawing.Point(24, 240);
            changePhotoBtn.Size = new System.Drawing.Size(130, 28);
            changePhotoBtn.Click += new EventHandler(ChangeTeacherPhoto_Click);
            UiTheme.StyleSecondaryButton(changePhotoBtn);
            Controls.Add(changePhotoBtn);

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

            _studentsSection = new Label();
            _studentsSection.Text = "Ученики класса";
            _studentsSection.Location = new System.Drawing.Point(24, 244);
            _studentsSection.Size = new System.Drawing.Size(580, 24);
            UiTheme.StyleSectionLabel(_studentsSection);
            Controls.Add(_studentsSection);

            _teacherClassLabel = new Label();
            _teacherClassLabel.Text = "Класс: —";
            _teacherClassLabel.Location = new System.Drawing.Point(534, 82);
            _teacherClassLabel.Size = new System.Drawing.Size(356, 38);
            UiTheme.StyleInfoLabel(_teacherClassLabel);
            Controls.Add(_teacherClassLabel);

            Button openCardButton = new Button();
            openCardButton.Text = "Открыть карточку";
            openCardButton.Location = new System.Drawing.Point(420, 240);
            openCardButton.Size = new System.Drawing.Size(216, 32);
            openCardButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            openCardButton.Click += (s, e) => OpenStudentCard();
            UiTheme.StyleSecondaryButton(openCardButton);
            Controls.Add(openCardButton);

            Button addStudentButton = new Button();
            addStudentButton.Text = "Добавить ученика";
            addStudentButton.Location = new System.Drawing.Point(648, 240);
            addStudentButton.Size = new System.Drawing.Size(248, 32);
            addStudentButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            addStudentButton.Click += new EventHandler(AddStudent_Click);
            UiTheme.StylePrimaryButton(addStudentButton);
            Controls.Add(addStudentButton);

            ViewsChildren.Location = new System.Drawing.Point(24, 278);
            ViewsChildren.Size = new System.Drawing.Size(872, 190);
            ViewsChildren.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UiTheme.StyleGrid(ViewsChildren);
            ViewsChildren.CellDoubleClick += (s, e) => OpenStudentCard();

            BuildGradePanel();
            BuildAchievementPanel();
        }

        private void AttachPhotoToAchievement_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title  = "Выбрать фото для достижения";
                dlg.Filter = "Фото|*.jpg;*.jpeg;*.png;*.bmp";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                _pendingAchievementPhotoPath = dlg.FileName;
                pendingPhotoLabel.Text       = Path.GetFileName(dlg.FileName);
                pendingPhotoLabel.ForeColor  = UiTheme.Primary;
            }
        }

        private static string AchSafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Replace(' ', '_').TrimEnd('.');
        }

        private void BuildGradePanel()
        {
            gradeSection = new Label();
            gradeSection.Text = "Поставить оценку выбранному ученику";
            gradeSection.Location = new System.Drawing.Point(24, 484 + 34);
            gradeSection.Size = new System.Drawing.Size(560, 24);
            gradeSection.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleSectionLabel(gradeSection);
            Controls.Add(gradeSection);

            disciplineCombo = new ComboBox();
            disciplineCombo.Location = new System.Drawing.Point(24, 514 + 34);
            disciplineCombo.Size = new System.Drawing.Size(244, 28);
            disciplineCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StyleCombo(disciplineCombo);
            Controls.Add(disciplineCombo);

            gradeCombo = new ComboBox();
            gradeCombo.Location = new System.Drawing.Point(280, 514 + 34);
            gradeCombo.Size = new System.Drawing.Size(150, 28);
            gradeCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StyleCombo(gradeCombo);
            Controls.Add(gradeCombo);

            gradeDate = new DateTimePicker();
            gradeDate.Location = new System.Drawing.Point(442, 514 + 34);
            gradeDate.Size = new System.Drawing.Size(140, 28);
            gradeDate.Format = DateTimePickerFormat.Short;
            gradeDate.Font = UiTheme.TextFont;
            gradeDate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(gradeDate);

            addGradeButton = new Button();
            addGradeButton.Text = "Поставить оценку";
            addGradeButton.Location = new System.Drawing.Point(594, 512 + 34);
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
            achievementSection.Location = new System.Drawing.Point(24, 568 + 34);
            achievementSection.Size = new System.Drawing.Size(560, 24);
            achievementSection.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleSectionLabel(achievementSection);
            Controls.Add(achievementSection);

            // Строка 1: название, тип, дата, кнопка
            achievementTitleBox = new TextBox();
            achievementTitleBox.Location = new System.Drawing.Point(24, 632);
            achievementTitleBox.Size = new System.Drawing.Size(244, 28);
            achievementTitleBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UiTheme.StyleTextBox(achievementTitleBox);
            SetPlaceholder(achievementTitleBox, "Название достижения");
            Controls.Add(achievementTitleBox);

            achievementTypeCombo = new ComboBox();
            achievementTypeCombo.Location = new System.Drawing.Point(280, 632);
            achievementTypeCombo.Size = new System.Drawing.Size(150, 28);
            achievementTypeCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StyleCombo(achievementTypeCombo);
            achievementTypeCombo.Items.AddRange(new object[]
            {
                "Грамота", "Медаль", "Кубок", "Сертификат", "Диплом"
            });
            achievementTypeCombo.SelectedIndex = 0;
            Controls.Add(achievementTypeCombo);

            achievementDate = new DateTimePicker();
            achievementDate.Location = new System.Drawing.Point(442, 632);
            achievementDate.Size = new System.Drawing.Size(140, 28);
            achievementDate.Format = DateTimePickerFormat.Short;
            achievementDate.Font = UiTheme.TextFont;
            achievementDate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(achievementDate);

            addAchievementButton = new Button();
            addAchievementButton.Text = "Добавить достижение";
            addAchievementButton.Location = new System.Drawing.Point(594, 630);
            addAchievementButton.Size = new System.Drawing.Size(220, 32);
            addAchievementButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            addAchievementButton.Click += new EventHandler(AddAchievement_Click);
            UiTheme.StylePrimaryButton(addAchievementButton);
            Controls.Add(addAchievementButton);

            // Строка 2: описание (необязательно)
            achievementDescBox = new TextBox();
            achievementDescBox.Location = new System.Drawing.Point(24, 672);
            achievementDescBox.Size = new System.Drawing.Size(872, 28);
            achievementDescBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UiTheme.StyleTextBox(achievementDescBox);
            SetPlaceholder(achievementDescBox, "Описание (необязательно): например, муниципальный этап");
            Controls.Add(achievementDescBox);

            attachPhotoToAchBtn = new Button();
            attachPhotoToAchBtn.Text = "Прикрепить фото";
            attachPhotoToAchBtn.Location = new System.Drawing.Point(24, 712);
            attachPhotoToAchBtn.Size = new System.Drawing.Size(170, 32);
            attachPhotoToAchBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            attachPhotoToAchBtn.Click += new EventHandler(AttachPhotoToAchievement_Click);
            UiTheme.StyleSecondaryButton(attachPhotoToAchBtn);
            Controls.Add(attachPhotoToAchBtn);

            pendingPhotoLabel = new Label();
            pendingPhotoLabel.Text      = "Фото не выбрано";
            pendingPhotoLabel.Location  = new System.Drawing.Point(206, 720);
            pendingPhotoLabel.Size      = new System.Drawing.Size(600, 18);
            pendingPhotoLabel.Font      = UiTheme.SmallFont;
            pendingPhotoLabel.ForeColor = UiTheme.MutedText;
            pendingPhotoLabel.BackColor = UiTheme.Page;
            pendingPhotoLabel.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(pendingPhotoLabel);
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
