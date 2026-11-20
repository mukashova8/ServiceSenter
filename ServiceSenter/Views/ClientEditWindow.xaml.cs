using System;
using System.Linq;
using System.Windows;
using ServiceSenter.Data;
using ServiceSenter.Models;

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