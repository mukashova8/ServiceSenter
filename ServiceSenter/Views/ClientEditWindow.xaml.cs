using System;
using System.Linq;
using System.Windows;
using ServiceSenter.Data;
using ServiceSenter.Models;
using System.Text.RegularExpressions;

namespace ServiceSenter.Views
{
    /// <summary>
    /// класс окна добавления, редактирования и удаления клиентов 
    /// </summary>
    public partial class ClientEditWindow : Window
    {
        private readonly AppDbContext _context;
        private Client _client;

        //конструктор(принимает бд и клиента)
        public ClientEditWindow(AppDbContext context, Client client = null)
        {
            InitializeComponent();
            _context = context;
            _client = client;

            //редактирование клиента, если есть такой клиент, если нет то создание нового(дата первого приема ставиться автоматически нового клиаента)
            if (_client != null)
            {
                TxtFullName.Text = _client.FullName;
                TxtPhone.Text = _client.Phone;
                TxtEmail.Text = _client.Email;
                DpFirstContact.SelectedDate = _client.FirstContactDate;
                this.Title = "Редактирование клиента";
            }
            else
            {
                DpFirstContact.SelectedDate = DateTime.Now;
                this.Title = "Новый клиент";
            }
        }
        //сохранение и проверка полей на пустату и добавление в бд
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //проверка на пустоту
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                TxtError.Text = "Введите ФИО клиента!";
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                TxtError.Text = "Введите номер телефона!";
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                TxtError.Text = "Введите Email!";
                return;
            }

            //проверка телефона
            //очищение ввода от пробелов, скобок и дефисов 
            string cleanPhone = Regex.Replace(TxtPhone.Text, @"[^\d+]", "");

            if (cleanPhone.Length < 11 || cleanPhone.Length > 12)
            {
                TxtError.Text = "Неверный формат телефона! Введите 11 цифр";
                return;
            }

            //проверка email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(TxtEmail.Text, emailPattern))
            {
                TxtError.Text = "Неверный формат Email!";
                return;
            }

            //сохранение в бд
            if (_client == null)
            {
                _client = new Client
                {
                    FullName = TxtFullName.Text,
                    Phone = TxtPhone.Text,
                    Email = TxtEmail.Text,
                    FirstContactDate = DpFirstContact.SelectedDate ?? DateTime.Now
                };
                _context.Clients.Add(_client);
            }
            else
            {
                _client.FullName = TxtFullName.Text;
                _client.Phone = TxtPhone.Text;
                _client.Email = TxtEmail.Text;
                _client.FirstContactDate = DpFirstContact.SelectedDate ?? _client.FirstContactDate;
            }

            //сохранение в бд
            _context.SaveChanges();
            this.DialogResult = true;
            this.Close();
        }
        //отмена кнопка
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}