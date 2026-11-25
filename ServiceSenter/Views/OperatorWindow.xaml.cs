using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using ServiceSenter.Data;
using ServiceSenter.Models;

namespace ServiceSenter.Views
{
    /// <summary>
    ///  класс окна Оператора (создание  и просмотр заявок, клиентов, устройств)
    /// </summary>
    public partial class OperatorWindow : Window
    {
        private readonly AppDbContext _context;

        //конструктор принимает бд
        public OperatorWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            TxtUserInfo.Text = $"{UserSession.FullName} (Оператор)";
            LoadData();
        }

        //загрузка данных из БД в таблицы
        private void LoadData()
        {
            //заполнение выпадающих списков для создания заявки
            CmbClient.ItemsSource = _context.Clients.ToList();
            CmbDevice.ItemsSource = _context.Devices.Include(d => d.DeviceType).ToList();
            CmbStatus.ItemsSource = _context.RequestStatuses.ToList();
            CmbMaster.ItemsSource = _context.Employees.Include(e => e.Role).Where(e => e.Role.RoleName == "Мастер").ToList();

            //загрузка заявок
            GridRequests.ItemsSource = _context.Requests
                .Include(r => r.Client)
                .Include(r => r.Device).ThenInclude(d => d.DeviceType)
                .Include(r => r.Status)
                .Include(r => r.Employee)
                .OrderByDescending(r => r.Id)
                .ToList();

            //загрузка клиентов
            GridClients.ItemsSource = _context.Clients.ToList();

            //загрузка устройств
            GridDevices.ItemsSource = _context.Devices.Include(d => d.DeviceType).ToList();

            //установка текущей даты и статуса по умолчанию
            DpDateReceived.SelectedDate = DateTime.Now;
            var defaultStatus = _context.RequestStatuses.FirstOrDefault(s => s.StatusName == "Принята");
            if (defaultStatus != null) CmbStatus.SelectedItem = defaultStatus;
        }

        //переключение вкладок
        private void BtnTabRequests_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabRequests;
        private void BtnTabClients_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabClients;
        private void BtnTabDevices_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabDevices;

        //смена заголовка 
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtHeader == null) return;
            if (MainTabControl.SelectedItem == TabRequests) 
            { 
                TxtHeader.Text = "Заявки"; 
                SetActiveButton(BtnTabRequests); 
            }
            else if (MainTabControl.SelectedItem == TabClients) 
            { 
                TxtHeader.Text = "Клиенты"; 
                SetActiveButton(BtnTabClients); 
            }
            else if (MainTabControl.SelectedItem == TabDevices) 
            { 
                TxtHeader.Text = "Устройства"; 
                SetActiveButton(BtnTabDevices); 
            }
        }

        //подстветка выбранной вкладки
        private void SetActiveButton(Button activeButton)
        {
            BtnTabRequests.Background = Brushes.Transparent; 
            BtnTabRequests.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabClients.Background = Brushes.Transparent; 
            BtnTabClients.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabDevices.Background = Brushes.Transparent; 
            BtnTabDevices.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            if (activeButton != null) 
            { 
                activeButton.Background = new SolidColorBrush(Color.FromRgb(74, 74, 74)); 
                activeButton.Foreground = Brushes.White; 
            }
        }

        //сохранение и проверка полей на пустату и добавление в бд
        private void BtnSaveRequest_Click(object sender, RoutedEventArgs e)
        {
            //проверка на заполнение
            if (CmbClient.SelectedItem == null || CmbDevice.SelectedItem == null || CmbMaster.SelectedItem == null || CmbStatus.SelectedItem == null || string.IsNullOrWhiteSpace(TxtFault.Text))
            { 
                TxtMsgRequest.Foreground = Brushes.Red; TxtMsgRequest.Text = "Заполните все поля!"; 
                return; 
            }

            if (!decimal.TryParse(TxtCost.Text, out decimal cost))
            { 
                TxtMsgRequest.Foreground = Brushes.Red; TxtMsgRequest.Text = "Ошибка в стоимости!"; 
                return; 
            }

            //создание новой заявки
            var newReq = new Request
            {
                DateReceived = DpDateReceived.SelectedDate ?? DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                FaultDescription = TxtFault.Text,
                Cost = cost,
                Client = _context.Clients.Find(((Client)CmbClient.SelectedItem).Id),
                Device = _context.Devices.Find(((Device)CmbDevice.SelectedItem).Id),
                Employee = _context.Employees.Find(((Employee)CmbMaster.SelectedItem).Id),
                Status = _context.RequestStatuses.Find(((RequestStatus)CmbStatus.SelectedItem).Id)
            };

            //добавление в бд
            _context.Requests.Add(newReq);
            _context.SaveChanges();

            //очищение поля ввода и обновление таблиц
            TxtMsgRequest.Foreground = Brushes.Green;
            TxtMsgRequest.Text = "Заявка создана!";
            TxtFault.Clear();
            TxtCost.Text = "0";
            LoadData();
        }

        //кнопки для работы с клиентами
        //добавление
        private void BtnAddClient_Click(object sender, RoutedEventArgs e) 
        { 
            if (new ClientEditWindow(_context).ShowDialog() == true) 
                LoadData(); //обновление таблицы
        }

        //редактирование (берет выбранного клиента)
        private void BtnEditClient_Click(object sender, RoutedEventArgs e) 
        { 
            if (GridClients.SelectedItem is Client c && new ClientEditWindow(_context, c).ShowDialog() == true) 
                LoadData(); //обновление таблицы
        }

        //удаление (проверяет, есть ли у клиента заявки, и только потом удаляет с подтверждением)
        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            //если клиент выбран и у него нет заявок
            if (GridClients.SelectedItem is Client c && !_context.Requests.Any(r => r.Client.Id == c.Id))
            {
                if (MessageBox.Show("Удалить?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                { 
                    _context.Clients.Remove(c); //удаление из бд
                    _context.SaveChanges(); //сохранение в бд
                    LoadData(); //обновление таблицы
                }
            }
            else MessageBox.Show("Нельзя удалить клиента с заявками!");
        }

        //кнопки для работы с устройствами
        //добавление
        private void BtnAddDevice_Click(object sender, RoutedEventArgs e) 
        { 
            if (new DeviceEditWindow(_context).ShowDialog() == true) 
                LoadData(); //обновление таблицы
        }

        //редактирование (берет выбранное устройство)
        private void BtnEditDevice_Click(object sender, RoutedEventArgs e) 
        { 
            if (GridDevices.SelectedItem is Device d && new DeviceEditWindow(_context, d).ShowDialog() == true) 
                LoadData(); //обновление таблицы
        }

        //удаление (проверяет, есть ли у устрова заявки, и только потом удаляет с подтверждением)
        private void BtnDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            //если устрово выбрано и у него нет заявок
            if (GridDevices.SelectedItem is Device d && !_context.Requests.Any(r => r.Device.Id == d.Id))
            {
                if (MessageBox.Show("Удалить?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                { 
                    _context.Devices.Remove(d); //удаление из бд
                    _context.SaveChanges(); //сохранение в бд
                    LoadData(); //обновление таблицы
                }
            }
            else MessageBox.Show("Нельзя удалить устройство с заявками!");
        }

        //выход из аккаунта
        private void BtnLogout_Click(object sender, RoutedEventArgs e) 
        { 
            new LoginWindow(_context).Show(); 
            this.Close(); 
        }
    }
}