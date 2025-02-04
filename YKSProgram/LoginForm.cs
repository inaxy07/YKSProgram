using System;
using System.Data.SQLite;
using System.Windows.Forms;
using static YKSProgram.RegisterForm;

namespace YKSProgram
{
    public partial class LoginForm : Form
    {
        private readonly Form _previousForm; // Önceki formu tutmak için alan
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private MainForm _mainForm;
        public LoginForm(Form previousForm = null)
        {
            InitializeComponent();
            _previousForm = previousForm;

            if (_mainForm != null)
            {
                _mainForm.Close(); // Eski MainForm'u kapat
                _mainForm = null; // Referansı temizle
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("E-posta ve şifre alanları boş bırakılamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
                SQLiteParameter[] parameters = {
            new SQLiteParameter("@Email", email),
            new SQLiteParameter("@Password", password)
        };

                using (var reader = dbHelper.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        int userId = Convert.ToInt32(reader["UserID"]);
                        string username = reader["Username"].ToString();
                        string userEmail = reader["Email"].ToString();
                        string passwordDb = reader["Password"].ToString();
                        bool emailVerified = Convert.ToBoolean(reader["EmailVerified"]);
                        string verificationCode = GenerateVerificationCode();

                        UpdateVerificationCode(userId, verificationCode);
                        SendVerificationEmail(email, verificationCode);

                        // Kullanıcı bilgilerini sakla
                        SessionManager.CurrentUserID = userId;
                        SessionManager.CurrentUser = new User
                        {
                            UserID = userId,
                            Username = username,
                            Email = userEmail,
                            Password = passwordDb,
                            EmailVerified = emailVerified,
                        };

                        MessageBox.Show("Doğrulama kodu gönderildi. Lütfen e-posta adresinizi kontrol edin.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();
                        MainForm mainForm = new MainForm(); // Yeni MainForm oluştur
                        VerificationForm verificationForm = new VerificationForm(email, verificationCode, mainForm); // MainForm referansını geç
                        verificationForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz e-posta veya şifre.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 haneli kod
        }

        private void UpdateVerificationCode(int userId, string code)
        {
            string query = "UPDATE Users SET VerificationCode = @VerificationCode WHERE UserID = @UserID";
            SQLiteParameter[] parameters = {
                new SQLiteParameter("@VerificationCode", code),
                new SQLiteParameter("@UserID", userId)
            };

            dbHelper.ExecuteQuery(query, parameters);
        }

        private void SendVerificationEmail(string email, string verificationCode)
        {
            EmailService emailService = new EmailService();
            string subject = "E-Posta Doğrulama Kodu";
            string body = $"Doğrulama kodunuz: {verificationCode}";
            emailService.SendEmail(email, subject, body);
        }

        private void rgstrBtn_Click(object sender, EventArgs e)
        {
            LoginForm lgnForm = new LoginForm();
            // Kayıt formunu aç
            this.Hide(); // LoginForm'u kapat
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show(); // RegisterForm'u aç
            lgnForm.Close();
        }
    }
}