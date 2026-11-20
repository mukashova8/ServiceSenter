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
        }

        //подтягивает связанные таблицы (клиентов, устройства, статусы)
        private void LoadData()
        {
            GridLastRequests.ItemsSource = _context.Requests.Include(r => r.Client).Include(r => r.Device).Include(r => r.Status).OrderByDescending(r => r.Id).Take(5).ToList();
            GridAllRequests.ItemsSource = _context.Requests.Include(r => r.Client).Include(r => r.Device).Include(r => r.Status).Include(r => r.Employee).OrderByDescending(r => r.Id).ToList();

            GridClients.ItemsSource = _context.Clients.ToList();
            GridDevices.ItemsSource = _context.Devices.Include(d => d.DeviceType).ToList();
            GridEmployees.ItemsSource = _context.Employees.Include(e => e.Role).ToList();
            GridHistory.ItemsSource = _context.StatusHistories.Include(h => h.Request).ThenInclude(r => r.Client).Include(h => h.Status).OrderByDescending(h => h.ChangeDatetime).ToList();
        }

        //навигация по меню
        private void BtnTabDashboard_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabDashboard;
        private void BtnTabRequests_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabRequests;
        private void BtnTabClients_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabClients;
        private void BtnTabDevices_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabDevices;
        private void BtnTabEmployees_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabEmployees;
        private void BtnTabHistory_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedItem = TabHistory;

        //переключение заголовка при смене вкладки
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtHeader == null) return;
            if (MainTabControl.SelectedItem == TabDashboard) { TxtHeader.Text = "Главная"; SetActiveButton(BtnTabDashboard); }
            else if (MainTabControl.SelectedItem == TabRequests) { TxtHeader.Text = "Все заявки"; SetActiveButton(BtnTabRequests); }
            else if (MainTabControl.SelectedItem == TabClients) { TxtHeader.Text = "Клиенты"; SetActiveButton(BtnTabClients); }
            else if (MainTabControl.SelectedItem == TabDevices) { TxtHeader.Text = "Устройства"; SetActiveButton(BtnTabDevices); }
            else if (MainTabControl.SelectedItem == TabEmployees) { TxtHeader.Text = "Сотрудники"; SetActiveButton(BtnTabEmployees); }
            else if (MainTabControl.SelectedItem == TabHistory) { TxtHeader.Text = "История статусов"; SetActiveButton(BtnTabHistory); }
        }

        //визуально показывает, какая вкладка открыта
        private void SetActiveButton(Button activeButton)
        {
            BtnTabDashboard.Background = Brushes.Transparent; BtnTabDashboard.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabRequests.Background = Brushes.Transparent; BtnTabRequests.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabClients.Background = Brushes.Transparent; BtnTabClients.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabDevices.Background = Brushes.Transparent; BtnTabDevices.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabEmployees.Background = Brushes.Transparent; BtnTabEmployees.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            BtnTabHistory.Background = Brushes.Transparent; BtnTabHistory.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));

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
            if (GridAllRequests.SelectedItem is Request d && !_context.StatusHistories.Any(r => r.Request.Id == d.Id))
            {
                if (MessageBox.Show("Удалить заявку?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                { _context.Requests.Remove(d); _context.SaveChanges(); LoadData(); }
            }
            else MessageBox.Show("Нельзя удалить заявку, так как у неё есть история статусов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        //возвращает на окно авторизации
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow(_context).Show();
            this.Close();
        }
    }
}