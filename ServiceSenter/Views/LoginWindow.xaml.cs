using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceSenter.Data;
using ServiceSenter.Models;

namespace ServiceSenter.Views
{
    public partial class LoginWindow : Window
    {
        public readonly AppDbContext _context;
        public LoginWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
        }

        public void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text;
            string password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                TxtError.Text = "Заполните все поля!";
                return;
            }
            var user = _context.Employees.Include(e => e.Role).FirstOrDefault(u => u.Login == login);
            if (user != null && user.PasswordHash == HashPassword(password))
            {
                UserSession.Id = user.Id;
                UserSession.FullName = user.FullName;
                UserSession.Role = user.Role.RoleName;
                OpenRoleWindow();
            }
            else
            {
                TxtError.Text = "Неверный логин или пароль!";
            }
        }

        public void OpenRoleWindow()
        {
            Window nextWindow = null;

            if (nextWindow != null)
            {
                nextWindow.Show();
                this.Close();
            }
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}