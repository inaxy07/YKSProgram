using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace YKSProgram
{
    public partial class VerificationForm : Form
    {
        private readonly string _email;
        private readonly string _verificationCode;
        private readonly MainForm _mainForm; // MainForm referansı
        private DatabaseHelper dbHelper = new DatabaseHelper();

        // MainForm referansını doğrulama işlemi için alıyoruz
        public VerificationForm(string email, string verificationCode, MainForm mainForm)
        {
            InitializeComponent();
            _email = email;
            _verificationCode = verificationCode;
            _mainForm = mainForm; // MainForm referansını sakla
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            string userCode = txtVerificationCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(userCode))
            {
                MessageBox.Show("Doğrulama kodu boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (userCode == _verificationCode)
            {
                try
                {
                    // EmailVerified sütununu güncelle
                    string query = "UPDATE Users SET EmailVerified = 1 WHERE Email = @Email";
                    SQLiteParameter[] parameters = new SQLiteParameter[] {
                    new SQLiteParameter("@Email", _email)
                };

                    dbHelper.ExecuteQuery(query, parameters);

                    MessageBox.Show("E-posta doğrulandı. Artık giriş yaptınız.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // MainForm'u güncelle
                    _mainForm.LoadUserInfo(); // Kullanıcı bilgilerini güncelle
                    _mainForm.LoadUserTasks(); // Kullanıcı görevlerini güncelle

                    // MainForm'u aç ve bu formu kapat
                    this.Hide();
                    _mainForm.Show(); // MainForm'u göster
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Doğrulama kodu hatalı. Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}