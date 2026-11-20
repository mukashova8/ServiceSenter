using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using ServiceSenter.Data;
using ServiceSenter.Models;

namespace ServiceSenter.Views
{
    /// <summary>
    /// класс окна добавления, редактирования и удаления заявки 
    /// </summary>
    public partial class RequestEditWindow : Window
    {
        private readonly AppDbContext _context;
        private Request _request;

        //конструктор(принимает бд и заявку)
        public RequestEditWindow(AppDbContext context, Request request = null)
        {
            InitializeComponent();
            _context = context;
            _request = request;

            //загрузка данных в ComboBox
            CmbClient.ItemsSource = _context.Clients.ToList();
            CmbDevice.ItemsSource = _context.Devices.Include(d => d.DeviceType).ToList();
            CmbMaster.ItemsSource = _context.Employees.Include(e => e.Role).Where(e => e.Role.RoleName == "Мастер").ToList();
            CmbStatus.ItemsSource = _context.RequestStatuses.ToList();

            //редактирование заявки, если есть такая заявка
            if (_request != null)
            {
                CmbClient.SelectedValue = _request.Client.Id;
                CmbDevice.SelectedValue = _request.Device.Id;
                CmbMaster.SelectedValue = _request.Employee.Id;
                CmbStatus.SelectedValue = _request.Status.Id;
                TxtFault.Text = _request.FaultDescription;
                TxtCost.Text = _request.Cost.ToString();
                DpDateReceived.SelectedDate = _request.DateReceived;
                this.Title = "Редактирование заявки";
            }
            else
            {
                DpDateReceived.SelectedDate = DateTime.Now;
            }
        }

        //сохранение и проверка полей на пустату и добавление в бд
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClient.SelectedValue == null || CmbDevice.SelectedValue == null || CmbMaster.SelectedValue == null || CmbStatus.SelectedValue == null || string.IsNullOrWhiteSpace(TxtFault.Text))
            { TxtError.Text = "Заполните все поля!"; return; }

            if (!decimal.TryParse(TxtCost.Text, out decimal cost)) { TxtError.Text = "Ошибка в стоимости!"; return; }

            //создание новой заявки
            if (_request == null)
            {
                _request = new Request
                {
                    DateReceived = DpDateReceived.SelectedDate ?? DateTime.Now,
                    DueDate = DateTime.Now.AddDays(7),
                    FaultDescription = TxtFault.Text,
                    Cost = cost,
                    Client = _context.Clients.FirstOrDefault(c => c.Id == (int)CmbClient.SelectedValue),
                    Device = _context.Devices.FirstOrDefault(d => d.Id == (int)CmbDevice.SelectedValue),
                    Employee = _context.Employees.FirstOrDefault(emp => emp.Id == (int)CmbMaster.SelectedValue),
                    Status = _context.RequestStatuses.FirstOrDefault(s => s.Id == (int)CmbStatus.SelectedValue)
                };
                _context.Requests.Add(_request);
            }
            //обновление старой
            else
            {
                _request.Client = _context.Clients.FirstOrDefault(c => c.Id == (int)CmbClient.SelectedValue);
                _request.Device = _context.Devices.FirstOrDefault(d => d.Id == (int)CmbDevice.SelectedValue);
                _request.Employee = _context.Employees.FirstOrDefault(emp => emp.Id == (int)CmbMaster.SelectedValue);
                _request.Status = _context.RequestStatuses.FirstOrDefault(s => s.Id == (int)CmbStatus.SelectedValue);
                _request.FaultDescription = TxtFault.Text;
                _request.Cost = cost;
                _request.DateReceived = DpDateReceived.SelectedDate ?? _request.DateReceived;
            }

            _context.SaveChanges();
            this.DialogResult = true;
            this.Close();
        }

        //кнопка отмена
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}