using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoService.DataModels
{
    public class User : INotifyPropertyChanged
    {

        private string login;
        [Required(ErrorMessage = "Login is required")]
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }

        private string passwrd;
        [Required(ErrorMessage = "Password is required")]
        //[RegularExpression("/^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{8,}/",ErrorMessage = "Must contain at least 8 characters, 1 uppercase, 1 lowercase, 1 number")]
        public string Passwrd
        {
            get { return passwrd; }
            set
            {
                passwrd = value;
                OnPropertyChanged("Password");
            }
        }


        private string email;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        private string firstName;
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string lastName;
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string phone;
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\+[1-9]\d{2}-\d{2}-\d{3}-\d{2}-\d{2}$", ErrorMessage = "The number must be in the format +xxx-xx-xxx-xx-xx")]
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
            }
        }

        [Key]
        public int Id { get; set; }

        private string userrole { get; set; }
        public string UserRole
        {
            get { return userrole; }
            set
            {
                userrole = value;
                OnPropertyChanged("UserRole");
            }
        }

        public ICollection<UsersServices> UserServices { get; set; }
        public User()
        {
            UserServices = new List<UsersServices>();
        }

        public User(string login, string passwrd)
        {
            this.login = login;
            this.passwrd = passwrd;
        }

        public User(string login, string passwrd, string email, string first, string phone, string role, string last)
        {
            this.lastName = last;
            this.UserRole = role;
            this.login = login;
            this.passwrd = passwrd;
            this.email = email;
            this.firstName = first;
            this.phone = phone;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
