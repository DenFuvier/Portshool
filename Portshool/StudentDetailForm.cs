using MySql.Data.MySqlClient;
using Portshool.HelpConnect;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Portshool
{
    public partial class StudentDetailForm : Form
    {
        private readonly int _studentId;

        // Tab 1 – info
        private TextBox _surnameBox, _nameBox, _patronymicBox, _addressBox;
        private DateTimePicker _birthPicker;
        private ComboBox _classCombo;
        private PictureBox _studentPhotoBox;

        // Tab 2 – grades
        private DataGridView _gradesGrid;
        private ComboBox _editDiscCombo, _editGradeCombo;
        private DateTimePicker _editGradeDate;

        // Tab 3 – achievements
        private DataGridView _achGrid;
        private TextBox _editAchTitle, _editAchDesc;
        private ComboBox _editAchType;
        private DateTimePicker _editAchDate;

        // Tab 4 – files
        private DataGridView _filesGrid;
        private TextBox _renameBox;

        public StudentDetailForm(int studentId)
        {
            _studentId = studentId;
            InitializeComponent();
            BuildInterface();
            LoadAll();
        }

        // ────────────────────────────────────────────────────────────
        // BUILD UI
        // ────────────────────────────────────────────────────────────

        private void BuildInterface()
        {
            UiTheme.ApplyForm(this);

            var tabs = new TabControl();
            tabs.Location = new Point(12, 12);
            tabs.Size = new Size(896, 616);
            tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabs.Font = UiTheme.TextFont;
            Controls.Add(tabs);

            var t1 = new TabPage("Данные ученика");
            var t2 = new TabPage("Успеваемость");
            var t3 = new TabPage("Достижения");
            var t4 = new TabPage("Файлы портфолио");
            foreach (var t in new[] { t1, t2, t3, t4 })
                t.BackColor = UiTheme.Page;

            tabs.TabPages.AddRange(new[] { t1, t2, t3, t4 });

            BuildInfoTab(t1);
            BuildGradesTab(t2);
            BuildAchievementsTab(t3);
            BuildFilesTab(t4);
        }

        // ── TAB 1: ДАННЫЕ ──────────────────────────────────────────

        private void BuildInfoTab(TabPage tab)
        {
            L(tab, "Фамилия *",   16,  20); L(tab, "Имя *",    312, 20); L(tab, "Отчество", 608, 20);
            _surnameBox    = T(tab, 16,  42, 280);
            _nameBox       = T(tab, 312, 42, 280);
            _patronymicBox = T(tab, 608, 42, 248);

            L(tab, "Дата рождения *", 16, 88); L(tab, "Класс", 312, 88);
            _birthPicker = new DateTimePicker { Location = new Point(16, 110), Size = new Size(200, 28), Format = DateTimePickerFormat.Short, Font = UiTheme.TextFont };
            tab.Controls.Add(_birthPicker);

            _classCombo = new ComboBox { Location = new Point(312, 110), Size = new Size(200, 28) };
            StyleCb(_classCombo);
            tab.Controls.Add(_classCombo);

            L(tab, "Адрес", 16, 154);
            _addressBox = T(tab, 16, 176, 848);

            var save = B(tab, "Сохранить изменения", 16, 224, 220);
            save.Click += SaveInfo_Click;

            // Фото ученика
            L(tab, "Фото ученика", 16, 282);
            _studentPhotoBox = new PictureBox();
            _studentPhotoBox.Location = new Point(16, 304);
            _studentPhotoBox.Size = new Size(130, 155);
            UiTheme.StylePhotoBox(_studentPhotoBox);
            tab.Controls.Add(_studentPhotoBox);

            var changePhoto = BS(tab, "Изменить фото", 16, 468, 130);
            changePhoto.Click += ChangeStudentPhoto_Click;
        }

        private void ChangeStudentPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title  = "Выбрать фото ученика";
                dlg.Filter = "Фото|*.jpg;*.jpeg;*.png;*.bmp";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string s = _surnameBox.Text.Trim();
                    string n = _nameBox.Text.Trim();
                    string p = _patronymicBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(s) || string.IsNullOrWhiteSpace(n))
                    { MessageBox.Show("Сначала заполните Фамилию и Имя и сохраните данные."); return; }

                    PortfolioFileHelper.UploadPersonPhoto(dlg.FileName, s, n, p, PersonType.Student);
                    PortfolioFileHelper.LoadPersonPhoto(_studentPhotoBox, s, n, p, PersonType.Student);

                    MessageBox.Show("Фото ученика обновлено.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки фото"); }
            }
        }

        private void SaveInfo_Click(object sender, EventArgs e)
        {
            string s = _surnameBox.Text.Trim(), n = _nameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(s) || string.IsNullOrWhiteSpace(n))
            { MessageBox.Show("Фамилия и Имя обязательны.", "Не заполнено"); return; }

            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE students SET surname=@s,name=@n,patronymic=@p,birth_date=@b,address=@a,class_id=@c " +
                        "WHERE student_id=@id", con);
                    cmd.Parameters.AddWithValue("@s", s);
                    cmd.Parameters.AddWithValue("@n", n);
                    cmd.Parameters.AddWithValue("@p", NullIfEmpty(_patronymicBox.Text));
                    cmd.Parameters.AddWithValue("@b", _birthPicker.Value.Date);
                    cmd.Parameters.AddWithValue("@a", NullIfEmpty(_addressBox.Text));
                    cmd.Parameters.AddWithValue("@c", Convert.ToInt32(_classCombo.SelectedValue));
                    cmd.Parameters.AddWithValue("@id", _studentId);
                    cmd.ExecuteNonQuery();
                }
                Text = "Карточка: " + s + " " + n;
                MessageBox.Show("Данные сохранены.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        // ── TAB 2: УСПЕВАЕМОСТЬ ────────────────────────────────────

        private void BuildGradesTab(TabPage tab)
        {
            _gradesGrid = G(tab, 12, 12, 848, 290);
            _gradesGrid.SelectionChanged += (s, e) => OnGradeRowSelected();

            L(tab, "Редактировать выбранную запись:", 12, 316);

            _editDiscCombo = new ComboBox { Location = new Point(12, 338), Size = new Size(256, 28) };
            StyleCb(_editDiscCombo);
            tab.Controls.Add(_editDiscCombo);

            _editGradeCombo = new ComboBox { Location = new Point(280, 338), Size = new Size(160, 28) };
            StyleCb(_editGradeCombo);
            tab.Controls.Add(_editGradeCombo);

            _editGradeDate = new DateTimePicker { Location = new Point(452, 338), Size = new Size(140, 28), Format = DateTimePickerFormat.Short, Font = UiTheme.TextFont };
            tab.Controls.Add(_editGradeDate);

            var upd = B(tab, "Обновить", 604, 336, 120);
            upd.Click += UpdateGrade_Click;

            var del = BS(tab, "Удалить", 736, 336, 120);
            del.Click += DeleteGrade_Click;
        }

        private void OnGradeRowSelected()
        {
            if (_gradesGrid.CurrentRow == null) return;
            var row = _gradesGrid.CurrentRow;
            if (row.Cells["discipline_id"].Value != DBNull.Value)
                _editDiscCombo.SelectedValue = row.Cells["discipline_id"].Value;
            if (row.Cells["grade_id"].Value != DBNull.Value)
                _editGradeCombo.SelectedValue = row.Cells["grade_id"].Value;
            object d = row.Cells["Дата"].Value;
            if (d != null && d != DBNull.Value)
                _editGradeDate.Value = Convert.ToDateTime(d);
        }

        private void UpdateGrade_Click(object sender, EventArgs e)
        {
            if (_gradesGrid.CurrentRow == null) { MessageBox.Show("Выберите строку."); return; }
            if (_editDiscCombo.SelectedValue == null || _editGradeCombo.SelectedValue == null)
            { MessageBox.Show("Выберите предмет и оценку."); return; }

            int perfId = Convert.ToInt32(_gradesGrid.CurrentRow.Cells["performance_id"].Value);
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE academic_performance SET discipline_id=@d,grade_id=@g,assessment_date=@dt " +
                        "WHERE performance_id=@id", con);
                    cmd.Parameters.AddWithValue("@d", Convert.ToInt32(_editDiscCombo.SelectedValue));
                    cmd.Parameters.AddWithValue("@g", Convert.ToInt32(_editGradeCombo.SelectedValue));
                    cmd.Parameters.AddWithValue("@dt", _editGradeDate.Value.Date);
                    cmd.Parameters.AddWithValue("@id", perfId);
                    cmd.ExecuteNonQuery();
                }
                LoadGrades();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void DeleteGrade_Click(object sender, EventArgs e)
        {
            if (_gradesGrid.CurrentRow == null) { MessageBox.Show("Выберите строку."); return; }
            if (MessageBox.Show("Удалить эту оценку?", "Подтверждение", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int perfId = Convert.ToInt32(_gradesGrid.CurrentRow.Cells["performance_id"].Value);
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var cmd = new MySqlCommand("DELETE FROM academic_performance WHERE performance_id=@id", con);
                    cmd.Parameters.AddWithValue("@id", perfId);
                    cmd.ExecuteNonQuery();
                }
                LoadGrades();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        // ── TAB 3: ДОСТИЖЕНИЯ ──────────────────────────────────────

        private void BuildAchievementsTab(TabPage tab)
        {
            _achGrid = G(tab, 12, 12, 848, 240);
            _achGrid.SelectionChanged += (s, e) => OnAchRowSelected();

            L(tab, "Редактировать выбранное:", 12, 266);

            _editAchTitle = T(tab, 12, 288, 260);
            PH(_editAchTitle, "Название");

            _editAchType = new ComboBox { Location = new Point(284, 288), Size = new Size(140, 28) };
            StyleCb(_editAchType);
            _editAchType.Items.AddRange(new object[] { "Грамота", "Медаль", "Кубок", "Сертификат", "Диплом" });
            _editAchType.SelectedIndex = 0;
            tab.Controls.Add(_editAchType);

            _editAchDate = new DateTimePicker { Location = new Point(436, 288), Size = new Size(130, 28), Format = DateTimePickerFormat.Short, Font = UiTheme.TextFont };
            tab.Controls.Add(_editAchDate);

            var upd = B(tab, "Обновить", 578, 286, 130);
            upd.Click += UpdateAch_Click;

            var del = BS(tab, "Удалить", 720, 286, 140);
            del.Click += DeleteAch_Click;

            _editAchDesc = T(tab, 12, 328, 848);
            PH(_editAchDesc, "Описание (необязательно)");

            var photo = BS(tab, "Прикрепить фото / файл к выбранному достижению", 12, 370, 400);
            photo.Click += AttachPhoto_Click;
        }

        private void OnAchRowSelected()
        {
            if (_achGrid.CurrentRow == null) return;
            var row = _achGrid.CurrentRow;

            string title = row.Cells["Название"].Value?.ToString() ?? "";
            string type  = row.Cells["Тип"].Value?.ToString() ?? "";
            object date  = row.Cells["Дата"].Value;
            string desc  = row.Cells["Описание"].Value?.ToString() ?? "";

            _editAchTitle.Text = title;
            _editAchTitle.ForeColor = UiTheme.Text;

            int idx = _editAchType.Items.IndexOf(type);
            if (idx >= 0) _editAchType.SelectedIndex = idx;

            if (date != null && date != DBNull.Value)
                _editAchDate.Value = Convert.ToDateTime(date);

            if (string.IsNullOrWhiteSpace(desc))
                PH(_editAchDesc, "Описание (необязательно)");
            else { _editAchDesc.Text = desc; _editAchDesc.ForeColor = UiTheme.Text; }
        }

        private void UpdateAch_Click(object sender, EventArgs e)
        {
            if (_achGrid.CurrentRow == null) { MessageBox.Show("Выберите строку."); return; }
            string title = _editAchTitle.Text.Trim();
            if (string.IsNullOrWhiteSpace(title) || title == "Название")
            { MessageBox.Show("Укажите название."); return; }

            string desc = _editAchDesc.Text.Trim();
            if (desc == "Описание (необязательно)") desc = null;

            int achId = Convert.ToInt32(_achGrid.CurrentRow.Cells["achievement_id"].Value);
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE achievements SET title=@t,achievement_type=@at,achievement_date=@d,description=@desc " +
                        "WHERE achievement_id=@id", con);
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@at", _editAchType.SelectedItem?.ToString() ?? "Грамота");
                    cmd.Parameters.AddWithValue("@d", _editAchDate.Value.Date);
                    cmd.Parameters.AddWithValue("@desc", NullIfEmpty(desc));
                    cmd.Parameters.AddWithValue("@id", achId);
                    cmd.ExecuteNonQuery();
                }
                LoadAchievements();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void DeleteAch_Click(object sender, EventArgs e)
        {
            if (_achGrid.CurrentRow == null) { MessageBox.Show("Выберите строку."); return; }
            if (MessageBox.Show("Удалить это достижение?", "Подтверждение", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int achId = Convert.ToInt32(_achGrid.CurrentRow.Cells["achievement_id"].Value);
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var cmd = new MySqlCommand("DELETE FROM achievements WHERE achievement_id=@id", con);
                    cmd.Parameters.AddWithValue("@id", achId);
                    cmd.ExecuteNonQuery();
                }
                LoadAchievements();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void AttachPhoto_Click(object sender, EventArgs e)
        {
            if (_achGrid.CurrentRow == null)
            { MessageBox.Show("Сначала выберите достижение в таблице.", "Не выбрано"); return; }

            string achTitle = _achGrid.CurrentRow.Cells["Название"].Value?.ToString() ?? "файл";
            int achId = Convert.ToInt32(_achGrid.CurrentRow.Cells["achievement_id"].Value);

            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Выбрать фото или документ";
                dlg.Filter = "Фото и документы|*.jpg;*.jpeg;*.png;*.bmp;*.pdf;*.doc;*.docx|Все файлы|*.*";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string docDir = PortfolioFileHelper.GetPortfolioDirectory("Documents");
                    if (!Directory.Exists(docDir))
                        Directory.CreateDirectory(docDir);

                    string ext      = Path.GetExtension(dlg.FileName);
                    string safeName = SafeFileName(achTitle) + "_ach" + achId + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ext;
                    string destPath = Path.Combine(docDir, safeName);

                    File.Copy(dlg.FileName, destPath, overwrite: true);

                    using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                    {
                        con.Open();
                        var cmd = new MySqlCommand(
                            "INSERT INTO portfolio_files (student_id, file_name, file_path) VALUES (@s,@n,@p)", con);
                        cmd.Parameters.AddWithValue("@s", _studentId);
                        cmd.Parameters.AddWithValue("@n", safeName);
                        cmd.Parameters.AddWithValue("@p", destPath);
                        cmd.ExecuteNonQuery();
                    }

                    LoadFiles();
                    MessageBox.Show(
                        "Файл прикреплён к достижению «" + achTitle + "»:\n" + safeName,
                        "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
            }
        }

        // ── TAB 4: ФАЙЛЫ ПОРТФОЛИО ─────────────────────────────────

        private void BuildFilesTab(TabPage tab)
        {
            _filesGrid = G(tab, 12, 12, 848, 440);
            _filesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _filesGrid.SelectionChanged += (s, e) => OnFileRowSelected();

            // Кнопки действий
            var add = B(tab, "Добавить файл", 12, 466, 170);
            add.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            add.Click += AddFile_Click;

            var open = BS(tab, "Открыть", 194, 466, 120);
            open.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            open.Click += OpenFile_Click;

            var del = BS(tab, "Удалить запись", 326, 466, 150);
            del.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            del.Click += DeleteFile_Click;

            // Строка переименования
            L(tab, "Переименовать выбранный файл:", 12, 514);
            _renameBox = T(tab, 12, 534, 660);
            _renameBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            var renameBtn = B(tab, "Сохранить имя", 686, 532, 174);
            renameBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            renameBtn.Click += RenameFile_Click;
        }

        private void OnFileRowSelected()
        {
            if (_filesGrid.CurrentRow == null) return;
            string name = _filesGrid.CurrentRow.Cells["Имя файла"].Value?.ToString() ?? "";
            _renameBox.Text      = name;
            _renameBox.ForeColor = UiTheme.Text;
        }

        private void RenameFile_Click(object sender, EventArgs e)
        {
            if (_filesGrid.CurrentRow == null)
            { MessageBox.Show("Выберите файл в таблице.", "Не выбрано"); return; }

            string newName = _renameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            { MessageBox.Show("Введите новое имя файла.", "Не заполнено"); return; }

            int    fileId  = Convert.ToInt32(_filesGrid.CurrentRow.Cells["file_id"].Value);
            string oldPath = _filesGrid.CurrentRow.Cells["file_path"].Value?.ToString() ?? "";
            string oldName = _filesGrid.CurrentRow.Cells["Имя файла"].Value?.ToString() ?? "";

            // Сохраняем расширение, если пользователь его не указал
            string oldExt = Path.GetExtension(oldName);
            if (!string.IsNullOrEmpty(oldExt) && !newName.Contains("."))
                newName = newName + oldExt;

            // Новый путь = та же папка, новое имя
            string newPath = oldPath;
            if (!string.IsNullOrEmpty(oldPath))
            {
                string dir = Path.GetDirectoryName(oldPath);
                newPath = string.IsNullOrEmpty(dir)
                    ? newName
                    : Path.Combine(dir, newName);
            }

            try
            {
                // Переименовываем файл на диске, если он существует
                if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                {
                    if (!string.Equals(oldPath, newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        if (File.Exists(newPath))
                        {
                            if (MessageBox.Show(
                                "Файл «" + newName + "» уже существует. Заменить?",
                                "Файл существует", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return;
                            File.Delete(newPath);
                        }
                        File.Move(oldPath, newPath);
                    }
                }

                // Обновляем запись в БД
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE portfolio_files SET file_name=@n, file_path=@p WHERE file_id=@id", con);
                    cmd.Parameters.AddWithValue("@n", newName);
                    cmd.Parameters.AddWithValue("@p", newPath);
                    cmd.Parameters.AddWithValue("@id", fileId);
                    cmd.ExecuteNonQuery();
                }

                LoadFiles();
                MessageBox.Show(
                    "Файл переименован:\n«" + oldName + "»  →  «" + newName + "»",
                    "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка переименования"); }
        }

        private void AddFile_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Title = "Добавить файл в портфолио", Filter = "Все файлы|*.*" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    string docDir = PortfolioFileHelper.GetPortfolioDirectory("Documents");
                    if (!Directory.Exists(docDir))
                        Directory.CreateDirectory(docDir);

                    string origName = Path.GetFileName(dlg.FileName);
                    string destPath = Path.Combine(docDir, origName);
                    if (File.Exists(destPath))
                    {
                        string noExt = Path.GetFileNameWithoutExtension(origName);
                        string ext   = Path.GetExtension(origName);
                        origName = noExt + "_" + DateTime.Now.ToString("HHmmss") + ext;
                        destPath = Path.Combine(docDir, origName);
                    }
                    File.Copy(dlg.FileName, destPath);

                    using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                    {
                        con.Open();
                        var cmd = new MySqlCommand(
                            "INSERT INTO portfolio_files (student_id, file_name, file_path) VALUES (@s,@n,@p)", con);
                        cmd.Parameters.AddWithValue("@s", _studentId);
                        cmd.Parameters.AddWithValue("@n", origName);
                        cmd.Parameters.AddWithValue("@p", destPath);
                        cmd.ExecuteNonQuery();
                    }
                    LoadFiles();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (_filesGrid.CurrentRow == null) { MessageBox.Show("Выберите файл."); return; }
            string path = _filesGrid.CurrentRow.Cells["file_path"].Value?.ToString();
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            { MessageBox.Show("Файл не найден:\n" + path, "Ошибка"); return; }
            try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка открытия"); }
        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            if (_filesGrid.CurrentRow == null) { MessageBox.Show("Выберите файл."); return; }
            string name = _filesGrid.CurrentRow.Cells["Имя файла"].Value?.ToString();
            if (MessageBox.Show(
                "Удалить запись о файле «" + name + "»?\n(Файл на диске не удаляется)",
                "Подтверждение", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            int fileId = Convert.ToInt32(_filesGrid.CurrentRow.Cells["file_id"].Value);
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var cmd = new MySqlCommand("DELETE FROM portfolio_files WHERE file_id=@id", con);
                    cmd.Parameters.AddWithValue("@id", fileId);
                    cmd.ExecuteNonQuery();
                }
                LoadFiles();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        // ────────────────────────────────────────────────────────────
        // LOAD DATA
        // ────────────────────────────────────────────────────────────

        private void LoadAll()
        {
            LoadClasses();
            LoadDisciplines();
            LoadGradeOptions();
            LoadStudentInfo();
            LoadGrades();
            LoadAchievements();
            LoadFiles();
        }

        private void LoadStudentInfo()
        {
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var da = new MySqlDataAdapter(
                        "SELECT surname,name,patronymic,birth_date,address,class_id FROM students WHERE student_id=@id", con);
                    da.SelectCommand.Parameters.AddWithValue("@id", _studentId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return;

                    var row = dt.Rows[0];
                    _surnameBox.Text    = row["surname"].ToString();
                    _nameBox.Text       = row["name"].ToString();
                    _patronymicBox.Text = row["patronymic"].ToString();
                    _addressBox.Text    = row["address"].ToString();

                    object bd = row["birth_date"];
                    if (bd != null && bd != DBNull.Value)
                        _birthPicker.Value = Convert.ToDateTime(bd);

                    _classCombo.SelectedValue = Convert.ToInt32(row["class_id"]);
                    Text = "Карточка: " + row["surname"] + " " + row["name"];

                    PortfolioFileHelper.LoadPersonPhoto(
                        _studentPhotoBox,
                        row["surname"].ToString(),
                        row["name"].ToString(),
                        row["patronymic"].ToString(),
                        PersonType.Student);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки данных"); }
        }

        private void LoadClasses()
        {
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var da = new MySqlDataAdapter(
                        "SELECT c.class_id, c.class_name FROM classes c " +
                        "INNER JOIN teachers t ON c.class_teacher_id = t.teacher_id " +
                        "WHERE t.user_id = @uid ORDER BY c.class_name", con);
                    da.SelectCommand.Parameters.AddWithValue("@uid", Session.UserId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    _classCombo.DisplayMember = "class_name";
                    _classCombo.ValueMember   = "class_id";
                    _classCombo.DataSource    = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки классов"); }
        }

        private void LoadDisciplines()
        {
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var da = new MySqlDataAdapter("SELECT discipline_id, discipline_name FROM disciplines ORDER BY discipline_name", con);
                    var dt = new DataTable();
                    da.Fill(dt);
                    _editDiscCombo.DisplayMember = "discipline_name";
                    _editDiscCombo.ValueMember   = "discipline_id";
                    _editDiscCombo.DataSource    = dt;
                }
            }
            catch { }
        }

        private void LoadGradeOptions()
        {
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var da = new MySqlDataAdapter("SELECT grade_id, grade_name FROM grades ORDER BY grade_id", con);
                    var dt = new DataTable();
                    da.Fill(dt);
                    _editGradeCombo.DisplayMember = "grade_name";
                    _editGradeCombo.ValueMember   = "grade_id";
                    _editGradeCombo.DataSource    = dt;
                }
            }
            catch { }
        }

        private void LoadGrades()
        {
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var da = new MySqlDataAdapter(@"
                        SELECT ap.performance_id, ap.discipline_id, ap.grade_id,
                               d.discipline_name AS 'Предмет',
                               g.grade_name      AS 'Оценка',
                               ap.assessment_date AS 'Дата'
                        FROM academic_performance ap
                        INNER JOIN disciplines d ON ap.discipline_id = d.discipline_id
                        INNER JOIN grades g      ON ap.grade_id      = g.grade_id
                        WHERE ap.student_id = @id
                        ORDER BY ap.assessment_date DESC, d.discipline_name", con);
                    da.SelectCommand.Parameters.AddWithValue("@id", _studentId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    _gradesGrid.DataSource = dt;
                    HideCol(_gradesGrid, "performance_id");
                    HideCol(_gradesGrid, "discipline_id");
                    HideCol(_gradesGrid, "grade_id");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки оценок"); }
        }

        private void LoadAchievements()
        {
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var da = new MySqlDataAdapter(@"
                        SELECT achievement_id,
                               title            AS 'Название',
                               achievement_type AS 'Тип',
                               achievement_date AS 'Дата',
                               description      AS 'Описание'
                        FROM achievements WHERE student_id=@id ORDER BY achievement_date DESC", con);
                    da.SelectCommand.Parameters.AddWithValue("@id", _studentId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    _achGrid.DataSource = dt;
                    HideCol(_achGrid, "achievement_id");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки достижений"); }
        }

        private void LoadFiles()
        {
            try
            {
                using (var con = new MySqlConnection(SqlConnect.GetConnect()))
                {
                    con.Open();
                    var da = new MySqlDataAdapter(
                        "SELECT file_id, file_name AS 'Имя файла', file_path, upload_date AS 'Дата загрузки' " +
                        "FROM portfolio_files WHERE student_id=@id ORDER BY upload_date DESC", con);
                    da.SelectCommand.Parameters.AddWithValue("@id", _studentId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    _filesGrid.DataSource = dt;
                    HideCol(_filesGrid, "file_id");
                    HideCol(_filesGrid, "file_path");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки файлов"); }
        }

        // ────────────────────────────────────────────────────────────
        // HELPERS
        // ────────────────────────────────────────────────────────────

        private static void HideCol(DataGridView grid, string col)
        {
            if (grid.Columns.Contains(col))
                grid.Columns[col].Visible = false;
        }

        private static object NullIfEmpty(string s)
            => string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s.Trim();

        private static string SafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Replace(' ', '_').TrimEnd('.');
        }

        private static void PH(TextBox box, string hint)
        {
            box.Tag = hint;
            box.Text = hint;
            box.ForeColor = UiTheme.MutedText;
            box.GotFocus  += (s, e) => { if (box.Text == hint) { box.Text = ""; box.ForeColor = UiTheme.Text; } };
            box.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(box.Text)) { box.Text = hint; box.ForeColor = UiTheme.MutedText; } };
        }

        private static void StyleCb(ComboBox cb)
        {
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.FlatStyle = FlatStyle.Flat;
            cb.Font = UiTheme.TextFont;
            cb.BackColor = UiTheme.Surface;
            cb.ForeColor = UiTheme.Text;
        }

        private static void L(Control p, string text, int x, int y)
        {
            p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = UiTheme.SmallFont, ForeColor = UiTheme.MutedText, BackColor = UiTheme.Page });
        }

        private static TextBox T(Control p, int x, int y, int w)
        {
            var box = new TextBox { Location = new Point(x, y), Size = new Size(w, 28) };
            UiTheme.StyleTextBox(box);
            p.Controls.Add(box);
            return box;
        }

        private static Button B(Control p, string text, int x, int y, int w)
        {
            var btn = new Button { Text = text, Location = new Point(x, y), Size = new Size(w, 34) };
            UiTheme.StylePrimaryButton(btn);
            p.Controls.Add(btn);
            return btn;
        }

        private static Button BS(Control p, string text, int x, int y, int w)
        {
            var btn = new Button { Text = text, Location = new Point(x, y), Size = new Size(w, 34) };
            UiTheme.StyleSecondaryButton(btn);
            p.Controls.Add(btn);
            return btn;
        }

        private static DataGridView G(Control p, int x, int y, int w, int h)
        {
            var grid = new DataGridView { Location = new Point(x, y), Size = new Size(w, h) };
            UiTheme.StyleGrid(grid);
            p.Controls.Add(grid);
            return grid;
        }
    }
}
