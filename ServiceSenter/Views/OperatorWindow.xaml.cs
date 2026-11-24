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
            LoadData(); 
        }

        //загрузка данных из БД в таблицы
        private void LoadData()
        {
            //загрузка заявкок
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
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnTabRequests_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabRequests;
        }

        private void BtnTabClients_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabClients;
        }

        private void BtnTabDevices_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabDevices;
        }

        private void BtnSaveRequest_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnEditClient_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnAddDevice_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnEditDevice_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            //пока нет логики
        }
    }
}