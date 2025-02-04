using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace YKSProgram
{
    public partial class RegisterForm : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public RegisterForm()
        {
            InitializeComponent();
        }

        public static class SessionManager
        {
            public static int CurrentUserID { get; set; } = 0;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tüm alanları doldurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Parolalar eşleşmiyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO Users (Username, Email, Password, EmailVerified) VALUES (@Username, @Email, @Password, 0)";
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Username", username),
                    new SQLiteParameter("@Email", email),
                    new SQLiteParameter("@Password", password)
                };

                dbHelper.ExecuteQuery(query, parameters);
                MessageBox.Show("Kayıt başarılı! Şimdi giriş yapabilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide(); // RegisterForm'u kapat
                LoginForm loginForm = new LoginForm();
                loginForm.Show(); // LoginForm'u aç
                this.Close();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
            {
                MessageBox.Show("Veritabanı kilitli. Lütfen işlemi tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lgnBtn_Click(object sender, EventArgs e)
        {
            this.Hide(); // RegisterForm'u kapat
            LoginForm loginForm = new LoginForm();
            loginForm.Show(); // LoginForm'u aç
            this.Close();
        }
    }
}