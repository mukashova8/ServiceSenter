using System;
using System.Linq;
using System.Windows;
using ServiceSenter.Data;
using ServiceSenter.Models;

namespace ServiceSenter.Views
{
    /// <summary>
    /// класс окна добавления, редактирования и удаления сотрудников 
    /// </summary>
    public partial class EmployeeEditWindow : Window
    {
        private readonly AppDbContext _context;
        private Employee _employee;

        //конструктор(принимает бд и сотрудника)
        public EmployeeEditWindow(AppDbContext context, Employee employee = null)
        {
            InitializeComponent();
            _context = context;
            _employee = employee;

            //заполнение ComboBox значениями из перечисления
            CmbRole.ItemsSource = Enum.GetValues(typeof(EmployeeRoleEnum)).Cast<EmployeeRoleEnum>().ToList();

            //редактирование сотрудника, если есть такой сотрудник, если нет то создание нового
            if (_employee != null)
            {
                TxtFullName.Text = _employee.FullName;
                TxtLogin.Text = _employee.Login;

                //установление текущей должности
                if (_employee.Role != null && Enum.TryParse(_employee.Role.RoleName, out EmployeeRoleEnum parsedRole))
                {
                    CmbRole.SelectedItem = parsedRole;
                }

                this.Title = "Редактирование сотрудника";
            }
            else
            {
                this.Title = "Новый сотрудник";
                CmbRole.SelectedIndex = 0;
            }
        }

        //сохранение и проверка полей на пустату и добавление в бд
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                TxtError.Text = "Введите ФИО сотрудника!";
                return;
            }

            if (CmbRole.SelectedItem == null)
            {
                TxtError.Text = "Выберите должность!";
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtLogin.Text))
            {
                TxtError.Text = "Введите логин!";
                return;
            }

            if (_employee == null && string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                TxtError.Text = "Введите пароль!";
                return;
            }

            string selectedRoleName = CmbRole.SelectedItem.ToString();

            //поиск выбранной должности
            var role = _context.EmployeesRole.FirstOrDefault(r => r.RoleName == selectedRoleName);

            if (role == null)
            {
                TxtError.Text = "Выбранная должность не найдена! Добавление новых запрещено.";
                return;
            }

            //сохранение
            if (_employee == null)
            {
                _employee = new Employee
                {
                    FullName = TxtFullName.Text,
                    Login = TxtLogin.Text,
                    PasswordHash = LoginWindow.HashPassword(TxtPassword.Password),
                    Role = role
                };
                _context.Employees.Add(_employee);
            }
            else
            {
                _employee.FullName = TxtFullName.Text;
                _employee.Login = TxtLogin.Text;
                _employee.Role = role;

                if (!string.IsNullOrWhiteSpace(TxtPassword.Password))
                {
                    _employee.PasswordHash = LoginWindow.HashPassword(TxtPassword.Password);
                }
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

        //кнопка отмена 
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}