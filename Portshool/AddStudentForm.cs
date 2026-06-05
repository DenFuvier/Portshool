using MySql.Data.MySqlClient;
using Portshool.HelpConnect;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Portshool
{
    public partial class AddStudentForm : Form
    {
        private TextBox surnameBox;
        private TextBox nameBox;
        private TextBox patronymicBox;
        private DateTimePicker birthDatePicker;
        private ComboBox classCombo;
        private TextBox addressBox;
        private TextBox loginBox;
        private TextBox passwordBox;
        private Button saveButton;
        private Button cancelButton;

        public AddStudentForm()
        {
            InitializeComponent();
            BuildInterface();
            LoadTeacherClasses();
        }

        private void BuildInterface()
        {
            UiTheme.ApplyForm(this);
            BackColor = UiTheme.Page;

            Controls.Add(UiTheme.CreateTitle("Добавить ученика", 24, 18, 400));

            // --- Строка 1: Фамилия, Имя, Отчество ---
            AddLabel("Фамилия *", 24, 72);
            AddLabel("Имя *", 196, 72);
            AddLabel("Отчество", 368, 72);

            surnameBox = AddTextBox(24, 96, 160);
            nameBox = AddTextBox(196, 96, 160);
            patronymicBox = AddTextBox(368, 96, 168);

            // --- Строка 2: Дата рождения, Класс ---
            AddLabel("Дата рождения *", 24, 140);
            AddLabel("Класс *", 240, 140);

            birthDatePicker = new DateTimePicker();
            birthDatePicker.Location = new Point(24, 162);
            birthDatePicker.Size = new Size(200, 28);
            birthDatePicker.Format = DateTimePickerFormat.Short;
            birthDatePicker.Font = UiTheme.TextFont;
            birthDatePicker.Value = new DateTime(2009, 9, 1);
            Controls.Add(birthDatePicker);

            classCombo = new ComboBox();
            classCombo.Location = new Point(240, 162);
            classCombo.Size = new Size(296, 28);
            classCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            classCombo.FlatStyle = FlatStyle.Flat;
            classCombo.Font = UiTheme.TextFont;
            classCombo.BackColor = UiTheme.Surface;
            classCombo.ForeColor = UiTheme.Text;
            Controls.Add(classCombo);

            // --- Строка 3: Адрес ---
            AddLabel("Адрес", 24, 206);
            addressBox = AddTextBox(24, 228, 512);

            // --- Строка 4: Логин, Пароль ---
            AddLabel("Логин *", 24, 272);
            AddLabel("Пароль *", 280, 272);

            loginBox = AddTextBox(24, 294, 244);
            passwordBox = AddTextBox(280, 294, 256);
            passwordBox.UseSystemPasswordChar = true;

            // --- Кнопки ---
            saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Location = new Point(290, 358);
            saveButton.Size = new Size(120, 36);
            saveButton.Click += new EventHandler(Save_Click);
            UiTheme.StylePrimaryButton(saveButton);
            Controls.Add(saveButton);

            cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Location = new Point(424, 358);
            cancelButton.Size = new Size(112, 36);
            cancelButton.Click += (s, e) => Close();
            UiTheme.StyleSecondaryButton(cancelButton);
            Controls.Add(cancelButton);
        }

        private void LoadTeacherClasses()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    string query =
                        "SELECT c.class_id, c.class_name " +
                        "FROM classes c " +
                        "INNER JOIN teachers t ON c.class_teacher_id = t.teacher_id " +
                        "WHERE t.user_id = @userId " +
                        "ORDER BY c.class_name";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@userId", Session.UserId);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    classCombo.DisplayMember = "class_name";
                    classCombo.ValueMember = "class_id";
                    classCombo.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось загрузить классы");
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            string surname   = surnameBox.Text.Trim();
            string name      = nameBox.Text.Trim();
            string patronymic = patronymicBox.Text.Trim();
            string address   = addressBox.Text.Trim();
            string login     = loginBox.Text.Trim();
            string password  = passwordBox.Text;

            if (string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Заполните обязательные поля: Фамилия, Имя, Логин, Пароль.",
                    "Не заполнено");
                return;
            }

            if (classCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите класс.", "Не заполнено");
                return;
            }

            int classId = Convert.ToInt32(classCombo.SelectedValue);

            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();

                    // Проверяем, не занят ли логин
                    MySqlCommand checkCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM users WHERE login = @login", con);
                    checkCmd.Parameters.AddWithValue("@login", login);
                    long exists = (long)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        MessageBox.Show(
                            "Логин «" + login + "» уже занят. Придумайте другой.",
                            "Логин занят");
                        return;
                    }

                    // Создаём пользователя
                    MySqlCommand userCmd = new MySqlCommand(
                        "INSERT INTO users (login, password_hash, role) " +
                        "VALUES (@login, @password, 'Ученик')", con);
                    userCmd.Parameters.AddWithValue("@login", login);
                    userCmd.Parameters.AddWithValue("@password", password);
                    userCmd.ExecuteNonQuery();

                    long newUserId = userCmd.LastInsertedId;

                    // Создаём ученика
                    MySqlCommand studentCmd = new MySqlCommand(
                        "INSERT INTO students (user_id, class_id, surname, name, patronymic, birth_date, address) " +
                        "VALUES (@userId, @classId, @surname, @name, @patronymic, @birthDate, @address)", con);
                    studentCmd.Parameters.AddWithValue("@userId", newUserId);
                    studentCmd.Parameters.AddWithValue("@classId", classId);
                    studentCmd.Parameters.AddWithValue("@surname", surname);
                    studentCmd.Parameters.AddWithValue("@name", name);
                    studentCmd.Parameters.AddWithValue("@patronymic",
                        string.IsNullOrWhiteSpace(patronymic) ? (object)DBNull.Value : patronymic);
                    studentCmd.Parameters.AddWithValue("@birthDate", birthDatePicker.Value.Date);
                    studentCmd.Parameters.AddWithValue("@address",
                        string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address);
                    studentCmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    surname + " " + name + " добавлен(а) в класс " + classCombo.Text + ".\n" +
                    "Логин: " + login,
                    "Ученик добавлен",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось добавить ученика");
            }
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            lbl.Font = UiTheme.SmallFont;
            lbl.ForeColor = UiTheme.MutedText;
            lbl.BackColor = UiTheme.Page;
            Controls.Add(lbl);
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            TextBox box = new TextBox();
            box.Location = new Point(x, y);
            box.Size = new Size(width, 28);
            UiTheme.StyleTextBox(box);
            Controls.Add(box);
            return box;
        }
    }
}
