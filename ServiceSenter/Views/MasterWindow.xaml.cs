using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ServiceSenter.Data;
using ServiceSenter.Models;
using ServiceSenter.Patterns.State;

namespace ServiceSenter.Views
{
    /// <summary>
    /// класс окна Мастера (просмотр своих заявок и смена их статуса)
    /// </summary>
    public partial class MasterWindow : Window
    {
        private readonly AppDbContext _context;
        private Request _selectedRequest; //выбранная в таблице заявка

        //конструктор принимает бд
        public MasterWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            TxtUserInfo.Text = $"{UserSession.FullName} (Мастер)";
            LoadMyRequests();
            CmbNewStatus.ItemsSource = _context.RequestStatuses.ToList();
        }

        //загрузка заявок, назначенных только текущему мастеру
        private void LoadMyRequests()
        {
            var requests = _context.Requests
                .Include(r => r.Client)
                .Include(r => r.Device)
                    .ThenInclude(d => d.DeviceType) 
                .Include(r => r.Status)
                .Include(r => r.Employee)
                .Where(r => r.Employee.Id == UserSession.Id) 
                .OrderByDescending(r => r.Id)
                .ToList();

            GridMyRequests.ItemsSource = requests;
        }

        //обработка выбора заявки в таблице 
        private void GridMyRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //проверка выбранной заявки
            if (GridMyRequests.SelectedItem is Request request)
            {
                _selectedRequest = request;
                //включение панели с деталями
                DetailsPanel.IsEnabled = true; 

                //заполнение текстовых полей данными из заявки
                TxtClientName.Text = _selectedRequest.Client?.FullName ?? "Не указан";
                TxtPhone.Text = _selectedRequest.Client?.Phone ?? "Не указан";
                TxtDevice.Text = _selectedRequest.Device?.DeviceType?.TypeName ?? "Не указано";
                TxtModel.Text = _selectedRequest.Device?.Model ?? "Не указана";
                TxtSerial.Text = _selectedRequest.Device?.SerialNumber ?? "Не указан";
                TxtFault.Text = _selectedRequest.FaultDescription ?? "";
                TxtCurrentStatus.Text = _selectedRequest.Status?.StatusName ?? "Не указан";

                //установка текущего статуса 
                CmbNewStatus.SelectedItem = _selectedRequest.Status;
            }
        }

        //сохранение нового статуса заявки
        private void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
        {
            //проверка, выбрана ли заявка и новый статус
            if (_selectedRequest == null || CmbNewStatus.SelectedItem is not RequestStatus newStatus) return;

            //если статус не изменился — ничего не делаем
            if (_selectedRequest.Status != null && _selectedRequest.Status.Id == newStatus.Id) return;

            //применение паттерна состояние 
            //получение текущего статуса заявки в виде перечисления(Enum), если нет по умолчанию - принята
            if (!Enum.TryParse(_selectedRequest.Status?.StatusName, out RequestStatusEnum currentStatusEnum))
                currentStatusEnum = RequestStatusEnum.Принята;

            //получение новогой статуса, который выбрал мастер, тоже в виде Enum.
            if (!Enum.TryParse(newStatus.StatusName, out RequestStatusEnum newStatusEnum)) return;

            //создание Контекста — он автоматически подтянет нужный класс состояния 
            var stateContext = new RequestStateContext(currentStatusEnum);

            //проверка разрешен ли переход по правилам паттерна
            if (!stateContext.CanTransitionTo(newStatusEnum))
            {
                MessageBox.Show($"Недопустимый переход статуса: из '{currentStatusEnum}' нельзя перейти в '{newStatusEnum}'!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //1 замена статуса в самой заявке 
            _selectedRequest.Status = newStatus;

            //2 запись изменения в Историю статусов
            var historyRecord = new StatusHistory
            {
                Request = _selectedRequest,
                ChangeDatetime = DateTime.Now,
                Status = newStatus
            };

            _context.StatusHistories.Add(historyRecord);
            _context.SaveChanges(); //сохранение в бд

            MessageBox.Show($"Статус заявки №{_selectedRequest.Id} изменен на '{newStatus.StatusName}'!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            LoadMyRequests(); //обновление таблицы
            DetailsPanel.IsEnabled = false; //скрытие панели деталей
            _selectedRequest = null; //сброс выбора
        }

        //кнопка выхода из аккаунта
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow(_context).Show();
            this.Close();
        }
    }
}