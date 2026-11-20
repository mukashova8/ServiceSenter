using System;
using System.Linq;
using System.Windows;
using ServiceSenter.Data;
using ServiceSenter.Models;

namespace ServiceSenter.Views
{
    /// <summary>
    /// класс окна добавления, редактирования и удаления устройство 
    /// </summary>
    public partial class DeviceEditWindow : Window
    {
        private readonly AppDbContext _context;
        private Device _device;

        //конструктор(принимает бд и устройство)
        public DeviceEditWindow(AppDbContext context, Device device = null)
        {
            InitializeComponent();
            _context = context;
            _device = device;

            //заполнение ComboBox значениями из перечисления
            CmbType.ItemsSource = Enum.GetValues(typeof(DeviceTypeEnum)).Cast<DeviceTypeEnum>().ToList();

            //редактирование устройство, если есть такое устройство, если нет то создание нового
            if (_device != null)
            {
                TxtManufacturer.Text = _device.Manufacturer;
                TxtModel.Text = _device.Model;
                TxtSerial.Text = _device.SerialNumber;

                //установление текущего типа
                if (_device.DeviceType != null && Enum.TryParse(_device.DeviceType.TypeName, out DeviceTypeEnum parsedType))
                {
                    CmbType.SelectedItem = parsedType;
                }
                else if (_device.DeviceType != null)
                {
                    //если тип есть в БД, но его нет в Enum, просто пишем его текстом
                    CmbType.Text = _device.DeviceType.TypeName;
                }

                this.Title = "Редактирование устройства";
            }
            else
            {
                this.Title = "Новое устройство";
                CmbType.SelectedIndex = 0;
            }
        }

        //сохранение и проверка полей на пустату и добавление в бд
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(CmbType.Text) || string.IsNullOrWhiteSpace(TxtManufacturer.Text) || string.IsNullOrWhiteSpace(TxtModel.Text) || string.IsNullOrWhiteSpace(TxtSerial.Text))
            {
                TxtError.Text = "Заполните все поля!";
                return;
            }

            //работа с выбранным типом или введенный вручную)
            string selectedTypeName = CmbType.Text.Trim();

            //поиск выбранного типа
            var type = _context.DeviceTypes.FirstOrDefault(t => t.TypeName == selectedTypeName);

            //если нет, создание нового типа
            if (type == null)
            {
                type = new DeviceType { TypeName = selectedTypeName };
                _context.DeviceTypes.Add(type);
            }

            //сохранение
            if (_device == null)
            {
                _device = new Device
                {
                    Manufacturer = TxtManufacturer.Text,
                    Model = TxtModel.Text,
                    SerialNumber = TxtSerial.Text,
                    DeviceType = type
                };
                _context.Devices.Add(_device);
            }
            else
            {
                _device.Manufacturer = TxtManufacturer.Text;
                _device.Model = TxtModel.Text;
                _device.SerialNumber = TxtSerial.Text;
                _device.DeviceType = type;
            }

            //сохранение в бд
            try
            {
                _context.SaveChanges();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка сохранения");
            }
        }

        //кнопка отмены
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}