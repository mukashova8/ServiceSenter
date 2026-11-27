using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using ServiceSenter.Data;
using ServiceSenter.Models;
using System;

namespace ServiceSenter.Views
{
    /// <summary>
    ///  класс окна Администратора (управление заявками, клиентами, устройствами и сотрудниками)
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly AppDbContext _context;

        public AdminWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            TxtUserInfo.Text = $"{UserSession.FullName} (Администратор)";
            LoadData();

            MainTabControl.SelectedItem = TabRequests;
        }

        //подтягивает связанные таблицы (клиентов, устройства, статусы)
        private void LoadData()
        {
            LoadFilters();

            GridAllRequests.ItemsSource = _context.Requests.Include(r => r.Client).Include(r => r.Device).Include(r => r.Status).Include(r => r.Employee).OrderByDescending(r => r.Id).ToList();
            GridClients.ItemsSource = _context.Clients.ToList();
            GridDevices.ItemsSource = _context.Devices.Include(d => d.DeviceType).ToList();
            GridEmployees.ItemsSource = _context.Employees.Include(e => e.Role).ToList();
            GridHistory.ItemsSource = _context.StatusHistories.Include(h => h.Request).ThenInclude(r => r.Client).Include(h => h.Status).OrderByDescending(h => h.ChangeDatetime).ToList();
        }

        //навигация по меню
        private void BtnTabRequests_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabRequests;
        private void BtnTabClients_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabClients;
        private void BtnTabDevices_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabDevices;
        private void BtnTabEmployees_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabEmployees;
        private void BtnTabHistory_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabHistory;
        private void BtnTabReports_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabReports;

        //переключение заголовка при смене вкладки
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtHeader == null) return;
            if (MainTabControl.SelectedItem == TabRequests) { TxtHeader.Text = "Все заявки"; SetActiveButton(BtnTabRequests); }
            else if (MainTabControl.SelectedItem == TabClients) { TxtHeader.Text = "Клиенты"; SetActiveButton(BtnTabClients); }
            else if (MainTabControl.SelectedItem == TabDevices) { TxtHeader.Text = "Устройства"; SetActiveButton(BtnTabDevices); }
            else if (MainTabControl.SelectedItem == TabEmployees) { TxtHeader.Text = "Сотрудники"; SetActiveButton(BtnTabEmployees); }
            else if (MainTabControl.SelectedItem == TabHistory) { TxtHeader.Text = "История статусов"; SetActiveButton(BtnTabHistory); }
            else if (MainTabControl.SelectedItem == TabReports) { TxtHeader.Text = "Отчёты"; SetActiveButton(BtnTabReports); }
        }

        //визуально показывает, какая вкладка открыта
        private void SetActiveButton(Button activeButton)
        {
            BtnTabRequests.Background = Brushes.Transparent; BtnTabRequests.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabClients.Background = Brushes.Transparent; BtnTabClients.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabDevices.Background = Brushes.Transparent; BtnTabDevices.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabEmployees.Background = Brushes.Transparent; BtnTabEmployees.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabHistory.Background = Brushes.Transparent; BtnTabHistory.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabReports.Background = Brushes.Transparent; BtnTabReports.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));

            if (activeButton != null) { activeButton.Background = new SolidColorBrush(Color.FromRgb(74, 74, 74)); activeButton.Foreground = Brushes.White; }
        }

        //методы для клиентов
        //добавление
        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            if (new ClientEditWindow(_context).ShowDialog() == true) LoadData();
        }
        //редактирование
        private void BtnEditClient_Click(object sender, RoutedEventArgs e)
        {
            if (GridClients.SelectedItem is Client c && new ClientEditWindow(_context, c).ShowDialog() == true) LoadData();
            else MessageBox.Show("Выберите клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        //удаление
        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (GridClients.SelectedItem is Client c && !_context.Requests.Any(r => r.Client.Id == c.Id))
            {
                if (MessageBox.Show("Удалить клиента?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                { _context.Clients.Remove(c); _context.SaveChanges(); LoadData(); }
            }
            else MessageBox.Show("Нельзя удалить клиента, так как у него есть заявки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //методы для устройств
        //добавление
        private void BtnAddDevice_Click(object sender, RoutedEventArgs e)
        {
            if (new DeviceEditWindow(_context).ShowDialog() == true) LoadData();
        }
        //редактирование
        private void BtnEditDevice_Click(object sender, RoutedEventArgs e)
        {
            if (GridDevices.SelectedItem is Device d && new DeviceEditWindow(_context, d).ShowDialog() == true) LoadData();
            else MessageBox.Show("Выберите устройство!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        //удаление
        private void BtnDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            if (GridDevices.SelectedItem is Device d && !_context.Requests.Any(r => r.Device.Id == d.Id))
            {
                if (MessageBox.Show("Удалить устройство?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                { _context.Devices.Remove(d); _context.SaveChanges(); LoadData(); }
            }
            else MessageBox.Show("Нельзя удалить устройство, так как оно используется в заявках!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //методы для сотрудников
        //добавление
        private void BtnAddEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (new EmployeeEditWindow(_context).ShowDialog() == true) LoadData();
        }

        //редактирование
        private void BtnEditEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (GridEmployees.SelectedItem is Employee d && new EmployeeEditWindow(_context, d).ShowDialog() == true) LoadData();
            else MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        //удаление
        private void BtnDeleteEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (GridEmployees.SelectedItem is Employee d)
            {
                if (d.Id == UserSession.Id) { MessageBox.Show("Вы не можете удалить самого себя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (!_context.Requests.Any(r => r.Employee.Id == d.Id) && !_context.StatusHistories.Any(h => h.Request.Employee.Id == d.Id))
                {
                    if (MessageBox.Show("Удалить сотрудника?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    { _context.Employees.Remove(d); _context.SaveChanges(); LoadData(); }
                }
                else MessageBox.Show("Нельзя удалить сотрудника, так как он используется в заявках или истории!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        //методы для заявок
        //добавление
        private void BtnAddRequest_Click(object sender, RoutedEventArgs e)
        {
            if (new RequestEditWindow(_context).ShowDialog() == true) LoadData();
        }

        //редактирование
        private void BtnEditRequest_Click(object sender, RoutedEventArgs e)
        {
            if (GridAllRequests.SelectedItem is Request d && new RequestEditWindow(_context, d).ShowDialog() == true) LoadData();
            else MessageBox.Show("Выберите заявку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        //удаление
        private void BtnDeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            //проверяем, выбрана ли заявка
            if (GridAllRequests.SelectedItem is Request d)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить заявку и всю её историю статусов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    //удаление в истории статусов 
                    var historyRecords = _context.StatusHistories.Where(h => h.Request.Id == d.Id).ToList();
                    if (historyRecords.Any())
                    {
                        _context.StatusHistories.RemoveRange(historyRecords);
                    }

                    _context.Requests.Remove(d);

                    //сохраняем изменения в БД (удалятся и история, и заявка)
                    _context.SaveChanges();

                    //обновляем таблицу
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //возвращает на окно авторизации
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow(_context).Show();
            this.Close();
        }

        //заполнение выпадающих списков для фильтров
        private void LoadFilters()
        {
            CmbFilterStatus.Items.Clear();
            foreach (var status in _context.RequestStatuses.ToList())
                CmbFilterStatus.Items.Add(status.StatusName);
            CmbFilterStatus.SelectedIndex = -1;

            CmbFilterMaster.Items.Clear();
            var masters = _context.Employees.Include(e => e.Role).Where(e => e.Role.RoleName == "Мастер").ToList();
            foreach (var master in masters)
                CmbFilterMaster.Items.Add(master.FullName);
            CmbFilterMaster.SelectedIndex = -1;
        }

        //обработка фильтров
        private void TxtSearchId_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void TxtSearchClient_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void DpFilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CmbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CmbFilterMaster_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        //кнопка поиск
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
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
            ApplyFilters();
        }

        //основной метод применения фильтров
        private void ApplyFilters()
        {
            var query = _context.Requests
                .Include(r => r.Client)
                .Include(r => r.Device).ThenInclude(d => d.DeviceType)
                .Include(r => r.Status)
                .Include(r => r.Employee)
                .AsQueryable();

            //1 поиск по номеру заявки
            if (!string.IsNullOrWhiteSpace(TxtSearchId.Text))
            {
                query = query.Where(r => r.Id.ToString().Contains(TxtSearchId.Text.Trim()));
            }

            //2 поиск по ФИО клиента
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

            //5 фильтр по статусу
            if (CmbFilterStatus.SelectedItem != null)
            {
                string selectedStatus = CmbFilterStatus.SelectedItem.ToString();
                query = query.Where(r => r.Status.StatusName == selectedStatus);
            }

            //6 фильтр по мастеру
            if (CmbFilterMaster.SelectedItem != null)
            {
                string selectedMaster = CmbFilterMaster.SelectedItem.ToString();
                query = query.Where(r => r.Employee.FullName == selectedMaster);
            }

            GridAllRequests.ItemsSource = query.OrderByDescending(r => r.Id).ToList();
        }

        //обработчик смены типа отчёта
        private void CmbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridReport != null) GridReport.ItemsSource = null;
        }

        //кнопка сформировать — выбирает нужный отчёт по типу
        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (CmbReportType.SelectedItem is not ComboBoxItem item)
            {
                MessageBox.Show("Выберите тип отчёта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reportType = item.Content.ToString();

            try
            {
                switch (reportType)
                {
                    case "Список всех заявок за период":
                        GenerateRequestsReport();
                        break;
                    case "Отчёт по загрузке мастеров":
                        GenerateMastersWorkloadReport();
                        break;
                    case "Отчёт по выполненным работам и стоимости":
                        GenerateCompletedWorksReport();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //отчёт1 — список всех заявок за период
        private void GenerateRequestsReport()
        {
            var query = _context.Requests
                .Include(r => r.Client)
                .Include(r => r.Device).ThenInclude(d => d.DeviceType)
                .Include(r => r.Status)
                .Include(r => r.Employee)
                .AsQueryable();

            if (DpReportDateStart.SelectedDate.HasValue)
                query = query.Where(r => r.DateReceived >= DpReportDateStart.SelectedDate.Value);

            if (DpReportDateEnd.SelectedDate.HasValue)
                query = query.Where(r => r.DateReceived <= DpReportDateEnd.SelectedDate.Value);

            var result = query
                .OrderByDescending(r => r.DateReceived)
                .Select(r => new
                {
                    Номер = r.Id,
                    Дата = r.DateReceived.ToString("dd.MM.yyyy"),
                    Клиент = r.Client.FullName,
                    Телефон = r.Client.Phone,
                    Устройство = r.Device.DisplayName,
                    Мастер = r.Employee != null ? r.Employee.FullName : "—",
                    Статус = r.Status.StatusName,
                    Стоимость = r.Cost
                })
                .ToList();

            GridReport.ItemsSource = result;

            if (result.Count == 0)
                MessageBox.Show("За выбранный период заявок не найдено", "Отчёт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //отчёт2 — загрузка мастеров (группировка по мастеру, подсчёт по статусам и сумма)
        private void GenerateMastersWorkloadReport()
        {
            var query = _context.Requests
                .Include(r => r.Employee)
                .Include(r => r.Status)
                .Where(r => r.Employee != null)
                .AsQueryable();

            if (DpReportDateStart.SelectedDate.HasValue)
                query = query.Where(r => r.DateReceived >= DpReportDateStart.SelectedDate.Value);

            if (DpReportDateEnd.SelectedDate.HasValue)
                query = query.Where(r => r.DateReceived <= DpReportDateEnd.SelectedDate.Value);

            var result = query
                .GroupBy(r => r.Employee.FullName)
                .Select(g => new
                {
                    Мастер = g.Key,
                    ВсегоЗаявок = g.Count(),
                    Принята = g.Count(r => r.Status.StatusName == "Принята"),
                    ВРаботе = g.Count(r => r.Status.StatusName == "ВРаботе"),
                    ОжидаетЗапчасти = g.Count(r => r.Status.StatusName == "ОжидаетЗапчасти"),
                    Готова = g.Count(r => r.Status.StatusName == "Готова"),
                    Выдана = g.Count(r => r.Status.StatusName == "Выдана"),
                    ОбщаяСтоимость = g.Sum(r => r.Cost)
                })
                .OrderByDescending(x => x.ВсегоЗаявок)
                .ToList();

            GridReport.ItemsSource = result;

            if (result.Count == 0)
                MessageBox.Show("За выбранный период данных по мастерам не найдено.", "Отчёт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //отчёт3 — выполненные работы и стоимость 
        private void GenerateCompletedWorksReport()
        {
            var query = _context.Requests
                .Include(r => r.Client)
                .Include(r => r.Device).ThenInclude(d => d.DeviceType)
                .Include(r => r.Employee)
                .Include(r => r.Status)
                .Where(r => r.Status.StatusName == "Готова" || r.Status.StatusName == "Выдана")
                .AsQueryable();

            if (DpReportDateStart.SelectedDate.HasValue)
                query = query.Where(r => r.DateReceived >= DpReportDateStart.SelectedDate.Value);

            if (DpReportDateEnd.SelectedDate.HasValue)
                query = query.Where(r => r.DateReceived <= DpReportDateEnd.SelectedDate.Value);

            var result = query
                .OrderByDescending(r => r.Cost)
                .Select(r => new
                {
                    Номер = r.Id,
                    Дата = r.DateReceived.ToString("dd.MM.yyyy"),
                    Клиент = r.Client.FullName,
                    Устройство = r.Device.DisplayName,
                    ТипУстройства = r.Device.DeviceType.TypeName,
                    Мастер = r.Employee != null ? r.Employee.FullName : "—",
                    Статус = r.Status.StatusName,
                    Стоимость = r.Cost
                })
                .ToList();

            GridReport.ItemsSource = result;

            if (result.Count == 0)
            {
                MessageBox.Show("За выбранный период выполненных работ не найдено.", "Отчёт", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            decimal total = result.Sum(x => x.Стоимость);
            MessageBox.Show($"Выполненных работ: {result.Count}\nОбщая стоимость: {total:C}", "Итоги отчёта", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}