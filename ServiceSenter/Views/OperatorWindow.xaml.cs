using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using ServiceSenter.Data;
using ServiceSenter.Models;
using System.IO;

namespace ServiceSenter.Views
{
    /// <summary>
    ///  класс окна Оператора (создание  и просмотр заявок, клиентов, устройств)
    /// </summary>
    public partial class OperatorWindow : Window
    {
        private readonly AppDbContext _context;
        private Request _lastCreatedRequest;

        //конструктор принимает бд
        public OperatorWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            TxtUserInfo.Text = $"{UserSession.FullName} (Оператор)";
            LoadData();
        }

        //загрузка данных из бд в таблицы
        private void LoadData()
        {
            //заполнение выпадающих списков для создания заявки
            CmbClient.ItemsSource = _context.Clients.ToList();
            CmbDevice.ItemsSource = _context.Devices.Include(d => d.DeviceType).ToList();
            CmbStatus.ItemsSource = _context.RequestStatuses.ToList();
            CmbMaster.ItemsSource = _context.Employees.Include(e => e.Role).Where(e => e.Role.RoleName == "Мастер").ToList();

            //заполнение выпадающих списков для фильтров
            CmbFilterStatus.Items.Clear();
            foreach (var status in _context.RequestStatuses.ToList())
                CmbFilterStatus.Items.Add(status.StatusName);
            CmbFilterStatus.SelectedIndex = -1;

            CmbFilterMaster.Items.Clear();
            var masters = _context.Employees.Include(e => e.Role).Where(e => e.Role.RoleName == "Мастер").ToList();
            foreach (var master in masters)
                CmbFilterMaster.Items.Add(master.FullName);
            CmbFilterMaster.SelectedIndex = -1;

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
        private void BtnTabAllRequests_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabAllRequests; //добавлено
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
            else if (MainTabControl.SelectedItem == TabAllRequests)
            {
                TxtHeader.Text = "Все заявки";
                SetActiveButton(BtnTabAllRequests);
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
            BtnTabAllRequests.Background = Brushes.Transparent;
            BtnTabAllRequests.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
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
            //сохранение, чтобы у заявки появился Id перед записью в историю
            _context.SaveChanges();

            //запись начального статуса новой заявки в историю статусов
            _context.StatusHistories.Add(new StatusHistory
            {
                Request = newReq,
                Status = newReq.Status,
                ChangeDatetime = DateTime.Now
            });

            //сохранение записи в историю
            _context.SaveChanges(); 

            //сохранение созданной заявки для печати квитанции
            _lastCreatedRequest = newReq;

            //очищение поля ввода и обновление таблиц
            TxtMsgRequest.Foreground = Brushes.Green;
            TxtMsgRequest.Text = "Заявка создана!";
            TxtFault.Clear();
            TxtCost.Text = "0";
            LoadData();
        }

        //кнопка сохранения квитанции в блокнот
        private void BtnPrintReceiptFromCreate_Click(object sender, RoutedEventArgs e)
        {
            if (_lastCreatedRequest == null)
            {
                MessageBox.Show("Сначала создайте заявку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var req = _lastCreatedRequest;

            //создание диалогового окна для сохранения файла
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Текстовый файл (*.txt)|*.txt|Все файлы (*.*)|*.*";
            saveFileDialog.FileName = "Квитанция.txt"; 

            //если нажали отмена в окне сохранения — ничего не произойдет
            if (saveFileDialog.ShowDialog() != true) return;

            //формирование текста квитанции
            string text = $"Сервисный центр\n\n" +
                          $"Квитанция\n" +
                          $"Номер заявки:  № {req.Id}\n" +
                          $"Дата приёма:   {req.DateReceived:dd.MM.yyyy HH:mm}\n" +
                          $"Клиент:        {req.Client?.FullName}\n" +
                          $"Телефон:       {req.Client?.Phone}\n" +
                          $"Устройство:    {req.Device?.DisplayName}\n" +
                          $"Серийный номер: {req.Device?.SerialNumber}\n" +
                          $"Неисправность: {req.FaultDescription}\n" +
                          $"Мастер:        {req.Employee?.FullName ?? "Не назначен"}\n" +
                          $"Статус:        {req.Status?.StatusName}\n" +
                          $"Стоимость:     {req.Cost:C}\n" +
                          $"Подпись клиента: ___________________\n\n" +
                          $"Подпись оператора: _________________";

            try
            {
                //запись текста в файл с кодировкой UTF-8 
                File.WriteAllText(saveFileDialog.FileName, text, System.Text.Encoding.UTF8);

                MessageBox.Show("Квитанция успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //редактирование выбранной заявки
        private void BtnEditRequest_Click(object sender, RoutedEventArgs e)
        {
            if (GridRequests.SelectedItem is Request d && new RequestEditWindow(_context, d).ShowDialog() == true)
                LoadData();
            else
                MessageBox.Show("Выберите заявку для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        //кнопка поиска заявок по всем параметрам
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var query = _context.Requests
                .Include(r => r.Client)
                .Include(r => r.Device).ThenInclude(d => d.DeviceType)
                .Include(r => r.Status)
                .Include(r => r.Employee)
                .AsQueryable();

            //1 поиск по номеру
            if (!string.IsNullOrWhiteSpace(TxtSearchId.Text))
            {
                query = query.Where(r => r.Id.ToString().Contains(TxtSearchId.Text.Trim()));
            }

            //2 поиск по клиенту
            if (!string.IsNullOrWhiteSpace(TxtSearchClient.Text))
            {
                string search = TxtSearchClient.Text.Trim().ToLower();
                query = query.Where(r => r.Client.FullName.ToLower().Contains(search));
            }

            //3 фильтр по дате (с какой)
            if (DpFilterDateStart.SelectedDate.HasValue)
            {
                query = query.Where(r => r.DateReceived >= DpFilterDateStart.SelectedDate.Value);
            }

            //4 фильтр по дате (по какую)
            if (DpFilterDateEnd.SelectedDate.HasValue)
            {
                query = query.Where(r => r.DateReceived <= DpFilterDateEnd.SelectedDate.Value);
            }

            //5 поиск по статусу
            if (CmbFilterStatus.SelectedItem != null)
            {
                string selectedStatus = CmbFilterStatus.SelectedItem.ToString();
                query = query.Where(r => r.Status.StatusName == selectedStatus);
            }

            //6 поиск по мастеру
            if (CmbFilterMaster.SelectedItem != null)
            {
                string selectedMaster = CmbFilterMaster.SelectedItem.ToString();
                query = query.Where(r => r.Employee.FullName == selectedMaster);
            }

            GridRequests.ItemsSource = query.OrderByDescending(r => r.Id).ToList();
        }

        //кнопка сброса
        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            TxtSearchId.Text = "";
            TxtSearchClient.Text = "";
            DpFilterDateStart.SelectedDate = null;
            DpFilterDateEnd.SelectedDate = null;
            CmbFilterStatus.SelectedIndex = -1;
            CmbFilterMaster.SelectedIndex = -1;
            LoadData(); // Восстанавливаем полный список
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