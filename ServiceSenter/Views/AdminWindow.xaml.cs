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
    ///  класс окна Администратора: управление заявками, клиентами, устройствами и сотрудниками
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

        //возвращает на окно авторизации
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow(_context).Show();
            this.Close();
        }
    }
}