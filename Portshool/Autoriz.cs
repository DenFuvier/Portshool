using MySql.Data.MySqlClient;
using Portshool.HelpConnect;
using System;
using System.Windows.Forms;

namespace Portshool
{
    public partial class Autoriz : Form
    {
       

        public Autoriz()
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
            MinimumSize = new System.Drawing.Size(420, 300);
            ClientSize = new System.Drawing.Size(420, 300);
            Text = "Электронное портфолио - вход";

            Controls.Add(UiTheme.CreateTitle("Электронное портфолио", 32, 24, 360));

            label1.Text = "Логин";
            label1.BackColor = UiTheme.Page;
            label1.ForeColor = UiTheme.MutedText;
            label1.Font = UiTheme.SmallFont;
            label1.Location = new System.Drawing.Point(32, 86);

            LoginBox.Location = new System.Drawing.Point(32, 110);
            LoginBox.Size = new System.Drawing.Size(356, 28);
            UiTheme.StyleTextBox(LoginBox);

            label2.Text = "Пароль";
            label2.BackColor = UiTheme.Page;
            label2.ForeColor = UiTheme.MutedText;
            label2.Font = UiTheme.SmallFont;
            label2.Location = new System.Drawing.Point(32, 150);

            PasswordBox.Location = new System.Drawing.Point(32, 174);
            PasswordBox.Size = new System.Drawing.Size(356, 28);
            PasswordBox.UseSystemPasswordChar = true;
            UiTheme.StyleTextBox(PasswordBox);

            InVhod.Text = "Войти в портфолио";
            InVhod.Location = new System.Drawing.Point(32, 226);
            InVhod.Size = new System.Drawing.Size(356, 38);
            UiTheme.StylePrimaryButton(InVhod);
        }

        private void InVhod_Click(object sender, EventArgs e)
        {
            string cs = SqlConnect.GetConnect();

            try
            {
                using (MySqlConnection con = new MySqlConnection(cs))
                {
                    con.Open();

                    string stm = "SELECT user_id, role FROM users WHERE login = @login AND password_hash = @password";

                    MySqlCommand cmd = new MySqlCommand(stm, con);
                    cmd.Parameters.AddWithValue("@login", LoginBox.Text);
                    cmd.Parameters.AddWithValue("@password", PasswordBox.Text);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int userId = Convert.ToInt32(reader["user_id"]);
                        string role = reader["role"].ToString();

                        Session.UserId = userId;
                        Session.Role = role;

                        if (role == "Ученик")
                        {
                            Child frm = new Child();
                            frm.Show();
                            this.Hide();
                        }
                        else if (role == "Учитель")
                        {
                            Teacher frm = new Teacher();
                            frm.Show();
                            this.Hide();
                        }
                        else if (role == "Администратор")
                        {
                            Admin frm = new Admin();
                            frm.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Неверный логин или пароль",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка подключения"
                );
            }
        }
    }
}
