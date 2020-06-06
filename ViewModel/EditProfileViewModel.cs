using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using AutoService.DataModels;
using AutoService.Context;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AutoService.ViewModel
{
    class EditProfileViewModel : BaseViewModel
    {
        ResourceDictionary res;
        UserContext userContext;
        User user;
        public EditProfileViewModel(User user, ResourceDictionary resource)
        {
            res = resource;
            userContext = new UserContext();
            this.user = user;
        }

        private DelegateCommand save;
        public DelegateCommand Save
        {
            get
            {
                return save ?? (save = new DelegateCommand(SaveBT));
            }
        }

        private void SaveBT(object arg)
        {
            int check = 1;
            using (UserContext db = new UserContext())
            {
                foreach (User newUser in db.Users)
                {
                    if (newUser.Login == user.Login && newUser.Passwrd == user.Passwrd)
                    {
                        string temppas = user.Passwrd;
                        newUser.LastName = LastName;
                        newUser.FirstName = FirstName;
                        newUser.Email = Email;
                        newUser.Phone = Phone;
                        if (NewPasswrd == ConfirmPasswrd && NewPasswrd != null && NewPasswrd != EditPasswrd)
                            newUser.Passwrd = GetHash(NewPasswrd);
                        else
                            MessageBox.Show("Пароль не изменился (введены неправильные значения или вы не стали его менять) / Password has not changed (incorrect values entered or you did not begin to change it)");
                        check = 0;
                        if (NewPasswrd == ConfirmPasswrd  && GetHash(EditPasswrd) == temppas )
                        {
                            Service service = new Service(newUser,res);
                            service.Show();
                            foreach (Window item in App.Current.Windows)
                            {
                                if (item != service)
                                    item.Close();
                            }
                        }
                        else
                            MessageBox.Show("Проверьте правильность заполненных данных! / Check that the data is correct!");
                    }
                }
                if (check == 1)
                    MessageBox.Show("Проверьте правильность заполненных данных! / Check that the data is correct!");
                else
                    db.SaveChanges();
            }


        }


        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }






        string patternPass = @"^[0-9a-zA-Zа-яА-Я]{8,20}$";
        private string newpas;
        public string NewPasswrd
        {
            get { return newpas ; }
            set
            {
                if (Regex.IsMatch(value, patternPass, RegexOptions.IgnoreCase))
                    newpas = value;
                else
                    MessageBox.Show("Пароль должен содержать минимум 8 символов, 1 верхний, 1 нижний, 1 цифру и максимум 20 / Must contain at least 8 characters, 1 uppercase, 1 lowercase, 1 number and max 20");
                OnPropertyChanged("NewPasswrd");
            }
        }

        private string connewpas;
        public string ConfirmPasswrd
        {
            get { return connewpas; }
            set
            {
                connewpas = value;
                OnPropertyChanged("ConfirmPasswrd");
            }
        }

        private string pas;
        public string EditPasswrd
        {
            get { return pas; }
            set
            {
                pas = value;
                OnPropertyChanged("EditPassword");
            }
        }


        string patternEmail = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
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
        public string FirstName
        {
            get { return user.FirstName; }
            set
            {
                if (Regex.IsMatch(value, patternName, RegexOptions.IgnoreCase))
                    user.FirstName = value;
                else
                    MessageBox.Show("Используйте русский или английский алфавит для ввода имени (2-25 букв) / Use the Russian or English alphabet to enter a name (2-25 letters)");
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get { return user.LastName; }
            set
            {
                if (Regex.IsMatch(value, patternName, RegexOptions.IgnoreCase))
                    user.LastName = value;
                else
                    MessageBox.Show("Используйте русский или английский алфавит для ввода фамилии (2-25 букв) / Use the Russian or English alphabet to enter a last name (2-25 letters)");
                OnPropertyChanged("LastName");
            }
        }

        public string patternPhone = @"^\+[1-9]\d{2}-\d{2}-\d{3}-\d{2}-\d{2}$";
        public string Phone
        {
            get { return user.Phone; }
            set
            {
                if (Regex.IsMatch(value, patternPhone, RegexOptions.IgnoreCase))
                    user.Phone = value;
                else
                    MessageBox.Show("Допустимый формат телефона +xxx-xx-xxx-xx-xx / The number must be in the format +xxx-xx-xxx-xx-xx");
                OnPropertyChanged("Phone");
            }
        }
    }
}
