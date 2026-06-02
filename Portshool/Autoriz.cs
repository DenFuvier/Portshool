using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
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
        }

        private void InVhod_Click(object sender, EventArgs e)
        {
            string cs = SqlConnect.GetConnect();

            try
            {
                using (MySqlConnection con = new MySqlConnection(cs))
                {
                    con.Open();

                    string stm = String.Format(
                        "SELECT user_id, role FROM users WHERE login = '{0}' AND password_hash = '{1}'",
                        LoginBox.Text,
                        PasswordBox.Text
                    );

                    MySqlCommand cmd = new MySqlCommand(stm, con);

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