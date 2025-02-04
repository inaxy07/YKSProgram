using System;
using System.Data;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Windows.Forms;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static YKSProgram.RegisterForm;

namespace YKSProgram
{
    public partial class MainForm : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadUserInfo(); // Kullanıcı bilgilerini yükle
            LoadUserTasks(); // Kullanıcı görevlerini yükle
            LoadUsers(); // Form yüklendiğinde kullanıcıları getir
            LoadWeekly(); // Form yüklendiğinde kullanıcıların konularını getir

            // Şartı kontrol et
            if (SessionManager.CurrentUser != null &&
                SessionManager.CurrentUser.Email == "talanisa1905@gmail.com")
            {
                dataGridViewUsers.Visible = true; // Görünür yap
                dataGridViewWeekly.Visible = true; // Görünür yap
                LoadWeekly(); // Verileri yükle
                LoadUsers(); // Verileri yükle
            }
            else
            {
                dataGridViewUsers.Visible = false; // Gizle
            }
        }

        public void LoadUserTasks()
        {
            try
            {
                string query = "SELECT Day, Task FROM WeeklySchedules WHERE UserID = @UserID ORDER BY Day ASC";
                SQLiteParameter[] parameters = new SQLiteParameter[]
                {
            new SQLiteParameter("@UserID", SessionManager.CurrentUserID)
                };

                lstTasks.Items.Clear(); // Listeyi temizle
                using (var reader = dbHelper.ExecuteReader(query, parameters))
                {
                    while (reader.Read())
                    {
                        string day = reader["Day"].ToString();
                        string task = reader["Task"].ToString();
                        lstTasks.Items.Add($"{day}: {task}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadUserInfo()
        {
            if (SessionManager.CurrentUser != null)
            {
                lblUserID.Text = $"UserID: {SessionManager.CurrentUser.UserID}";
                lblUsername.Text = $"Username: {SessionManager.CurrentUser.Username}";
                lblEmail.Text = $"Email: {SessionManager.CurrentUser.Email}";
                lblPassword.Text = $"Password: {SessionManager.CurrentUser.Password}";
                lblEmailVerified.Text = $"Email Verified: {(SessionManager.CurrentUser.EmailVerified ? "Yes" : "No")}";
            }
            else
            {
                // Kullanıcı bilgisi bulunamadığında
                lblUserID.Text = "UserID: Kullanıcı bilgisi bulunamadı.";
                lblUsername.Text = "Username: Kullanıcı bilgisi bulunamadı.";
                lblEmail.Text = "Email: Kullanıcı bilgisi bulunamadı.";
                lblPassword.Text = "Password: Kullanıcı bilgisi bulunamadı.";
                lblEmailVerified.Text = "Email Verified: Kullanıcı bilgisi bulunamadı.";
            }
        }

        private void LoadUsers()
        {
            try
            {
                string query = "SELECT UserID, Username, Email, EmailVerified FROM Users"; // Verileri getirme sorgusu
                DataTable usersTable = new DataTable();

                using (SQLiteConnection conn = dbHelper.GetConnection())
                {
                    conn.Open();
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn))
                    {
                        adapter.Fill(usersTable); // Verileri DataTable'a doldur
                    }
                }

                dataGridViewUsers.DataSource = usersTable; // DataGridView'e bağla
                dataGridViewUsers.Columns["UserID"].HeaderText = "Kullanıcı ID";
                dataGridViewUsers.Columns["Username"].HeaderText = "Kullanıcı Adı";
                dataGridViewUsers.Columns["Email"].HeaderText = "E-posta";
                dataGridViewUsers.Columns["EmailVerified"].HeaderText = "E-posta Doğrulandı";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWeekly()
        {
            try
            {
                string query = "SELECT ID, UserID, Day, Task FROM WeeklySchedules"; // Verileri getirme sorgusu
                DataTable weeklyTable = new DataTable();

                using (SQLiteConnection conn = dbHelper.GetConnection())
                {
                    conn.Open();
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn))
                    {
                        adapter.Fill(weeklyTable); // Verileri DataTable'a doldur
                    }
                }

                dataGridViewWeekly.DataSource = weeklyTable; // DataGridView'e bağla
                dataGridViewWeekly.Columns["ID"].HeaderText = "ID";
                dataGridViewWeekly.Columns["UserID"].HeaderText = "Kullanıcı ID";
                dataGridViewWeekly.Columns["Day"].HeaderText = "Tarih";
                dataGridViewWeekly.Columns["Task"].HeaderText = "Konu";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Çıkış işlemi
            SessionManager.CurrentUserID = 0;
            MessageBox.Show("Çıkış yaptınız.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Hide(); // Mevcut MainForm'u gizle
            LoginForm loginForm = new LoginForm(this); // Mevcut MainForm örneğini geçiriyoruz
            loginForm.Show(); // Giriş formunu göster
        }

        private void btnCreateTable_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcının girdiği değerleri al
                string taskDescription = txtTask.Text.Trim();
                string selectedDate = dtpDate.Value.ToShortDateString();

                if (string.IsNullOrWhiteSpace(taskDescription))
                {
                    MessageBox.Show("Görev açıklaması boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Veriyi veritabanına ekle
                string insertQuery = @"
            INSERT INTO WeeklySchedules (UserID, Day, Task) 
            VALUES (@UserID, @Day, @Task)";
                SQLiteParameter[] parameters = new SQLiteParameter[]
                {
            new SQLiteParameter("@UserID", SessionManager.CurrentUserID),
            new SQLiteParameter("@Day", selectedDate),
            new SQLiteParameter("@Task", taskDescription)
                };

                dbHelper.ExecuteQuery(insertQuery, parameters);

                MessageBox.Show("Görev başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Görevleri listele
                LoadUserTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show(); // RegisterForm'u aç
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show(); // LoginForm'u aç
        }
    }
}
