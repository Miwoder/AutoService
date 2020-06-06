using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Data.SqlClient;
using AutoService.DataModels;
using AutoService.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using AutoService.Repository;
using System.Text.RegularExpressions;

namespace AutoService
{
    class MainWindowViewModel : BaseViewModel
    {
        ResourceDictionary res;
        UserContext userContext;
        User user;
        UnitOfWork unitOfWork;


        public MainWindowViewModel(ResourceDictionary resource)
        {
            res = resource;
            userContext = new UserContext();
            user = new User();
            unitOfWork = new UnitOfWork();
        }

        [Required(ErrorMessage = "Login is required")]
        public string Login
        {
            get { return user.Login; }
            set
            {
                if (value != null && value.Length >= 8 && value.Length < 20)
                    user.Login = value;
                else
                    MessageBox.Show("Логин должен быть от 8 до 20 символов / Login must be between 8 and 20 characters");
                OnPropertyChanged("Login");
            }
        }

        string patternPass = @"^[0-9a-zA-Zа-яА-Я]{8,20}$";
        [Required(ErrorMessage = "Password is required")]
        public string Passwrd
        {
            get { return user.Passwrd; }
            set
            {
                if (Regex.IsMatch(value, patternPass, RegexOptions.IgnoreCase))
                    user.Passwrd = value;
                else
                    MessageBox.Show("Пароль должен содержать минимум 8 символов, 1 верхний, 1 нижний, 1 цифру и максимум 20 / Must contain at least 8 characters, 1 uppercase, 1 lowercase, 1 number and max 20");
                OnPropertyChanged("Password");
            }
        }

        public string PasswrdEnter
        {
            get { return user.Passwrd; }
            set
            {
                user.Passwrd = value;
                OnPropertyChanged("Password");
            }
        }


        string patternEmail = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email
        {
            get { return user.Email; }
            set
            {
                if (Regex.IsMatch(value, patternEmail, RegexOptions.IgnoreCase))
                    user.Email = value;
                 else
                    MessageBox.Show("Проверьте правильнность ввода email / Check the input format email");
                OnPropertyChanged("Email");
            }
        }

        string patternName = @"^[a-zA-Zа-яА-Я]{2,25}$";
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName
        {
            get { return user.FirstName; }
            set
            {
                if (Regex.IsMatch(value, patternName, RegexOptions.IgnoreCase))
                    user.FirstName = value;
                else
                    MessageBox.Show("Используйте русский или английский алфавит для ввода имени / Use the Russian or English alphabet to enter a name");
                OnPropertyChanged("FirstName");
            }
        }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName
        {
            get { return user.LastName; }
            set
            {
                if (Regex.IsMatch(value, patternName, RegexOptions.IgnoreCase))
                    user.LastName = value;
                else
                    MessageBox.Show("Используйте русский или английский алфавит для ввода фамилии / Use the Russian or English alphabet to enter a last name");

                OnPropertyChanged("LastName");
            }
        }

        public string patternPhone = @"^\+[0-9]\d{2}-\d{2}-\d{3}-\d{2}-\d{2}$";
        [Required(ErrorMessage = "Phone is required")]
        public string Phone
        {
            get { return user.Phone; }
            set
            {
                if (Regex.IsMatch(value, patternPhone, RegexOptions.IgnoreCase))
                    user.Phone = value;
                else
                    MessageBox.Show("Введен неверный формат телефона / The number must be in the format +xxx-xx-xxx-xx-xx");
                OnPropertyChanged("Phone");
            }
        }



        private DelegateCommand submit;
        public DelegateCommand Submit
        {
            get
            {
                return submit ?? (submit = new DelegateCommand(SubmitUser));
            }
        }

        private void SubmitUser(object arg)
        {
            int check = 0;
            foreach(User user in userContext.Users)
            {
                if (user.Login == Login && user.Passwrd == GetHash(Passwrd))
                {
                    Service service = new Service(user,res);
                    service.Show();
                    foreach (Window item in App.Current.Windows)
                    {
                        if (item != service)
                            item.Close();
                    }
                    check = 1;
                    break;
                }
            }
            if (check==0)
                (Application.Current.MainWindow as MainWindow).loginerror.Visibility = Visibility.Visible;

        }



        private DelegateCommand register;
        public DelegateCommand Register
        {
            get
            {
                return register ?? (register = new DelegateCommand(RegisterUser));
            }
        }
        private void RegisterUser(object arg)
        {
            User user;
            string userrole = "user";

            string firstName = FirstName;
            string phone = Phone;
            string lastName = LastName;
            string email = Email;
            string login = Login;
            string password = GetHash(Passwrd);
            int x = 0;
            foreach (User us in userContext.Users)
            {
                if (us.Login == login || us.Email == email)
                {
                    x = 1;
                }
            }
            if (x == 1)
                MessageBox.Show("Такой логин или email уже существует / Login or email exist");
            if (x == 0)
            {
                if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Passwrd))
                {
                    user = new User { FirstName = firstName, LastName = lastName, UserRole = userrole, Email = email, Phone = phone, Login = login, Passwrd = password };
                    unitOfWork.Users.Create(user);
                    unitOfWork.Save();
                    SendEmail("Вы успешно зарегистрировались в нашем приложении. Ваша информация будет отображаться в профиле. Теперь вы можете использовать наше приложение.", user.Email);
                    (Application.Current.MainWindow as MainWindow).Registration.Visibility = Visibility.Hidden;
                    (Application.Current.MainWindow as MainWindow).Login.Visibility = Visibility.Visible;
                    (Application.Current.MainWindow as MainWindow).emailRegTB.Text = (Application.Current.MainWindow as MainWindow).firstNameRegTB.Text = (Application.Current.MainWindow as MainWindow).loginRegTB.Text = (Application.Current.MainWindow as MainWindow).PhoneTextBox.Text = (Application.Current.MainWindow as MainWindow).lastNameRegTB.Text = (Application.Current.MainWindow as MainWindow).passwrdRegTB.Text = "";

                }
                else
                {
                    MessageBox.Show("Заполните все поля! Fill all fields!");
                }
            }
        }



        private DelegateCommand forgetPas;
        public DelegateCommand ForgetPas
        {
            get
            {
                return forgetPas ?? (forgetPas = new DelegateCommand(ForgetPasBT));
            }
        }
        private void ForgetPasBT(object arg)
        {
            string newpas = Newpas();
            int check = 0;
            foreach (User user in userContext.Users)
            {
                if (user.Login == Login)
                {
                    SendEmail("Система восстановления пароля. Ваш новый пароль: " + newpas + ". Если это были не вы, смените пароль в личном кабинете.", user.Email);
                    user.Passwrd = GetHash(newpas);
                    MessageBox.Show("Письмо отправлено на указанный email / Email sent to specified email");
                    check = 1;
                    break;
                }
                
            }
            userContext.SaveChanges();
            if (check == 0)
                MessageBox.Show("Проверьте правильность ввода логина или его не существует / Verify that the login is correct or it does not exist");

        }

        private DelegateCommand goToReg;
        public DelegateCommand GoToReg
        {
            get
            {
                return goToReg ?? (goToReg = new DelegateCommand(GoToRegBT));
            }
        }
        private void GoToRegBT(object arg)
        {
            (Application.Current.MainWindow as MainWindow).Login.Visibility = Visibility.Hidden;
            (Application.Current.MainWindow as MainWindow).Registration.Visibility = Visibility.Visible;
        }

        private DelegateCommand goToLog;
        public DelegateCommand GoToLog
        {
            get
            {
                return goToLog ?? (goToLog = new DelegateCommand(GoToLogBT));
            }
        }
        private void GoToLogBT(object arg)
        {
            (Application.Current.MainWindow as MainWindow).Login.Visibility = Visibility.Visible;
            (Application.Current.MainWindow as MainWindow).Registration.Visibility = Visibility.Hidden;
        }





        private void SendEmail(string body, string email)
        {
            try
            {
                MailAddress from = new MailAddress("ivanivanoki@mail.ru", "AutoService");
                MailAddress to = new MailAddress(email);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Auto Service";
                m.Body = body;
                m.IsBodyHtml = true;
                //Simple Mail Transfer Protocol
                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                smtp.Credentials = new NetworkCredential("ivanivanoki@mail.ru", "oop2020SEM");
                smtp.EnableSsl = true;
                smtp.Send(m);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Произошла ошибка отправки сообщения. Приносим свои извинения. Возможно ошибка связана с несуществующим email. /" +
                    "An error occurred while sending the message. We apologize. Perhaps the error is due to a nonexistent email. ");
            }
        }

        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        private string Newpas()
        {
            string str = "qwertyuiopasdfghjklzxcvbnm123456789QWERTYUIOPASDFGHJKLZXCVBNM";
            string pass = "";

            Random rnd = new Random();
            int lng = str.Length;
            for (int i = 0; i <= 12; i++)
                pass += str[rnd.Next(lng)];
            
            return pass;
        }
    }
}
