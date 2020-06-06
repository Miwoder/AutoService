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
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Collections.ObjectModel;
using AutoService.Repository;

namespace AutoService.ViewModel
{
    class ServiceViewModel : BaseViewModel
    {
        ResourceDictionary res;
        UnitOfWork unitOfWork;

        DataModels.Service service;
        User user;
        UserContext userContext;
        UserContext userContext2;
        public ObservableCollection<User> PersonsList { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<string> UserComboList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> UserStatusCombo { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> UserOrderIdCombo { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int> AllOrderIdCombo { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<string> OrderStatusCombo { get; set; } = new ObservableCollection<string>();


        public ObservableCollection<InfoUserServices> PersonOrderList { get; set; } = new ObservableCollection<InfoUserServices>();
        public ObservableCollection<AllOrders> PersonOrderAllList { get; set; } = new ObservableCollection<AllOrders>();


        public ServiceViewModel(User user, ResourceDictionary resource)
        {
            res = resource;
            unitOfWork = new UnitOfWork();

            userContext = new UserContext();
            userContext2 = new UserContext();
            service = new DataModels.Service();
            this.user = user;

            //Person info page
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Phone = user.Phone;

            // List of users & list of logins(ComboBox) in PersonList page
            foreach (User userlist in userContext.Users)
            {
                PersonsList.Add(userlist);
                UserComboList.Add(userlist.Login);
            }

            // list of orders id in combo
            foreach(UsersServices services in userContext.UsersServices)
            {
                if(user.Id == services.UserId)
                    UserOrderIdCombo.Add(services.UserServicesId);
            }

            // status ComboBox PersonList page
            UserStatusCombo.Add("user");
            UserStatusCombo.Add("admin");

            // order status. All Orders page
            OrderStatusCombo.Add("Not ready");
            OrderStatusCombo.Add("Ready");
            OrderStatusCombo.Add("Denied");


            // Order list of each user
            using (UserContext db = new UserContext())
            {
                foreach(UsersServices ser in db.UsersServices)
                {
                    foreach (DataModels.Service serv in userContext.Services)
                    {
                        if (user.Id == ser.UserId && ser.ServiceId == serv.Id)
                        {
                            InfoUserServices infoUserServices = new InfoUserServices {Id=ser.UserServicesId, Date = ser.Date, Price = serv.Price, ServiceName = serv.ServiceName,
                                Status = ser.Status };
                            PersonOrderList.Add(infoUserServices);
                        }
                    }
                }
            }

            // page with all orders
            using (UserContext db = new UserContext())
            {
                foreach (UsersServices ser in db.UsersServices)
                {
                    foreach (DataModels.Service serv in userContext.Services)
                    {
                        foreach (User userr in userContext2.Users)
                        {
                            if (ser.ServiceId==serv.Id && ser.UserId==userr.Id)
                            {
                                AllOrders all = new AllOrders { Id = ser.UserServicesId, Date = ser.Date, Email = userr.Email, Name = userr.FirstName, Phone = userr.Phone, Price = serv.Price, ServiceName = serv.ServiceName, Status = ser.Status };
                                PersonOrderAllList.Add(all);
                            }

                        }
                    }
                    AllOrderIdCombo.Add(ser.UserServicesId);
                }
            }

        }




        private DelegateCommand logOut;
        public DelegateCommand LogOut
        {
            get
            {
                return logOut ?? (logOut = new DelegateCommand(LogOutBT));
            }
        }
        private void LogOutBT(object arg)
        {
            MainWindow main = new MainWindow();
            main.Show();
            unitOfWork.Dispose(); //завершаем работу с пользователем
            foreach (Window item in App.Current.Windows)
            {
               // if (item != main)
                    item.Close();
            }
        }
        private DelegateCommand deleteacc;
        public DelegateCommand DeleteAccount
        {
            get
            {
                return deleteacc ?? (deleteacc = new DelegateCommand(DeleteAcc));
            }
        }
        private void DeleteAcc(object arg)
        {
            MessageBoxResult sure = MessageBox.Show("Are you sure?", "Delete account?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            using (UserContext db = new UserContext())
            {
                foreach (UsersServices services in db.UsersServices)
                {
                    if (services.UserId == user.Id && sure == MessageBoxResult.Yes)
                    {
                        unitOfWork.Orders.Delete(services.UserServicesId);
                    }
                }
                unitOfWork.Users.Delete(user.Id);
                unitOfWork.Save();
            }
            if (sure == MessageBoxResult.Yes)
            {
                MainWindow main = new MainWindow();
                main.Show();
                foreach (Window item in App.Current.Windows)
                {
                    if (item != main)
                        item.Close();
                }
            }
        }



        //Person page
        private DelegateCommand editInfo;
        public DelegateCommand EditInfo
        {
            get
            {
                return editInfo ?? (editInfo = new DelegateCommand(EditUserInfo));
            }
        }
        private void EditUserInfo(object arg)
        {
            //EditProfile editProfile = new EditProfile(user);
            //editProfile.Show();
        }

        private DelegateCommand cancelServiceByUser;
        public DelegateCommand CancelServiceByUser
        {
            get
            {
                return cancelServiceByUser ?? (cancelServiceByUser = new DelegateCommand(CancelServiceByUserBT));
            }
        }
        private void CancelServiceByUserBT(object arg)
        {
            MessageBoxResult sure;
            if (UserServicesId != 0)
            {
                sure = MessageBox.Show("Cancel this order?", "Cancel?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (sure == MessageBoxResult.Yes)
                {
                    unitOfWork.Orders.Delete(UserServicesId);
                    unitOfWork.Save();
                    Service serviceView = new Service(user,res);
                    serviceView.Show();
                    foreach (Window item in App.Current.Windows)
                    {
                        if (item != serviceView)
                        item.Close();
                    }
                }
            }

        }


        /// <summary>
        /// Page with services(buy & edit smth)
        /// </summary>
        private DelegateCommand buyEngine;
        public DelegateCommand BuyEngine
        {
            get
            {
                return buyEngine ?? (buyEngine = new DelegateCommand(BuyEngineBT));
            }
        }
        private void BuyEngineBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 2, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт двигателя. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);

            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand enginePriceEdit;
        public DelegateCommand EnginePriceEdit
        {
            get
            {
                return enginePriceEdit ?? (enginePriceEdit = new DelegateCommand(enginePriceEditBT));
            }
        }
        private void enginePriceEditBT(object arg)
        {
            int serviceId = 2;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }

        private DelegateCommand buySuspension;
        public DelegateCommand BuySuspension
        {
            get
            {
                return buySuspension ?? (buySuspension = new DelegateCommand(BuySuspensionBT));
            }
        }
        private void BuySuspensionBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 3, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт подвески. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);
            
            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand suspensionPriceEdit;
        public DelegateCommand SuspensionPriceEdit
        {
            get
            {
                return suspensionPriceEdit ?? (suspensionPriceEdit = new DelegateCommand(suspensionPriceEditBT));
            }
        }
        private void suspensionPriceEditBT(object arg)
        {
            int serviceId = 3;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }

        private DelegateCommand buyGrip;
        public DelegateCommand BuyGrip
        {
            get
            {
                return buyGrip ?? (buyGrip = new DelegateCommand(BuyGripBT));
            }
        }
        private void BuyGripBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 7, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт сцепления. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);

            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand gripPriceEdit;
        public DelegateCommand GripPriceEdit
        {
            get
            {
                return gripPriceEdit ?? (gripPriceEdit = new DelegateCommand(gripPriceEditBT));
            }
        }
        private void gripPriceEditBT(object arg)
        {
            int serviceId = 7;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }

        private DelegateCommand buySteering;
        public DelegateCommand BuySteering
        {
            get
            {
                return buySteering ?? (buySteering = new DelegateCommand(BuySteeringBT));
            }
        }
        private void BuySteeringBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 4, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт рулевого управления. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);
            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand steeringPriceEdit;
        public DelegateCommand SteeringPriceEdit
        {
            get
            {
                return steeringPriceEdit ?? (steeringPriceEdit = new DelegateCommand(steeringPriceEditBT));
            }
        }
        private void steeringPriceEditBT(object arg)
        {
            int serviceId = 4;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }

        private DelegateCommand buyElectric;
        public DelegateCommand BuyElectric
        {
            get
            {
                return buyElectric ?? (buyElectric = new DelegateCommand(BuyElectricBT));
            }
        }
        private void BuyElectricBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 8, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт электрики. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);
            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand electricPriceEdit;
        public DelegateCommand ElectricPriceEdit
        {
            get
            {
                return electricPriceEdit ?? (electricPriceEdit = new DelegateCommand(electricPriceEditBT));
            }
        }
        private void electricPriceEditBT(object arg)
        {
            int serviceId = 8;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }

        private DelegateCommand buyCapiler;
        public DelegateCommand BuyCapiler
        {
            get
            {
                return buyCapiler ?? (buyCapiler = new DelegateCommand(BuyCapilerBT));
            }
        }
        private void BuyCapilerBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 6, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт суппортов. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);
            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand capilerPriceEdit;
        public DelegateCommand CapilerPriceEdit
        {
            get
            {
                return capilerPriceEdit ?? (capilerPriceEdit = new DelegateCommand(capilerPriceEditBT));
            }
        }
        private void capilerPriceEditBT(object arg)
        {
            int serviceId = 6;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }

        private DelegateCommand buyStarters;
        public DelegateCommand BuyStarters
        {
            get
            {
                return buyStarters ?? (buyStarters = new DelegateCommand(BuyStartersBT));
            }
        }
        private void BuyStartersBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 5, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт стартеров. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);
            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand startersPriceEdit;
        public DelegateCommand StartersPriceEdit
        {
            get
            {
                return startersPriceEdit ?? (startersPriceEdit = new DelegateCommand(startersPriceEditBT));
            }
        }
        private void startersPriceEditBT(object arg)
        {
            int serviceId = 5;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }

        private DelegateCommand buyMuffler;
        public DelegateCommand BuyMuffler
        {
            get
            {
                return buyMuffler ?? (buyMuffler = new DelegateCommand(BuyMufflerBT));
            }
        }
        private void BuyMufflerBT(object arg)
        {
            DateTime dateTime = DateTime.Now;
            UsersServices usersServices = new UsersServices { ServiceId = 1, Date = dateTime, Status = "not ready", UserId = user.Id };
            unitOfWork.Orders.Create(usersServices);
            unitOfWork.Save();
            SendEmail("Вы успешно заказали ремонт глушителя. Администратор скоро свяжется с вами. Подробную информацию можно посмотреть в личном кабинете.", user.Email);
            Service serviceView = new Service(user,res);
            serviceView.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != serviceView)
                    item.Close();
            }
        }

        private DelegateCommand mufflerPriceEdit;
        public DelegateCommand MufflerPriceEdit
        {
            get
            {
                return mufflerPriceEdit ?? (mufflerPriceEdit = new DelegateCommand(mufflerPriceEditBT));
            }
        }
        private void mufflerPriceEditBT(object arg)
        {
            int serviceId = 1;
            EditPrice editPrice = new EditPrice(serviceId, user, res);
            editPrice.Show();
        }


        ///page with all orders
        private DelegateCommand changeOrderStatusTo;
        public DelegateCommand ChangeOrderStatusTo
        {
            get
            {
                return changeOrderStatusTo ?? (changeOrderStatusTo = new DelegateCommand(ChangeOrderStatusToBT));
            }
        }
        private void ChangeOrderStatusToBT(object arg)
        {
            MessageBoxResult sure = MessageBox.Show("Change status?", "Change?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            using (UserContext db = new UserContext())
            {
                foreach (User userr in userContext.Users)
                {
                    foreach (UsersServices us in db.UsersServices)
                    {
                        if (SelectOrderId != 0 && OrderStatusSelect != null && us.UserServicesId == SelectOrderId && userr.Id==us.UserId && sure == MessageBoxResult.Yes)
                        {
                            us.Status = OrderStatusSelect;
                            SendEmail("Ваш заказ №" + SelectOrderId + " теперь имеет статус: " + OrderStatusSelect, userr.Email);
                        }
                    }
                }
                db.SaveChanges();
            }
            if (sure == MessageBoxResult.Yes)
            {
                Service serviceView = new Service(user,res);
                serviceView.Show();
                foreach (Window item in App.Current.Windows)
                {
                    if (item != serviceView)
                        item.Close();
                }
            }
        }


        ///page with all users
        private DelegateCommand changeStatusTo;
        public DelegateCommand ChangeStatusTo
        {
            get
            {
                return changeStatusTo ?? (changeStatusTo = new DelegateCommand(ChangeStatusToBT));
            }
        }
        private void ChangeStatusToBT(object arg)
        {
            MessageBoxResult sure = MessageBox.Show("Change status?", "Change?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            using (UserContext db = new UserContext())
            {
                foreach (User newUser in db.Users)
                {
                    if (UserStatusSelect != null && UserLoginComboSelect != null && newUser.Login == UserLoginComboSelect && sure == MessageBoxResult.Yes)
                    {
                        newUser.UserRole = UserStatusSelect;
                        SendEmail("Ваш статус пользователя был изменен на: " + UserStatusSelect, newUser.Email);
                    }
                }
                db.SaveChanges();
            }
            if (sure == MessageBoxResult.Yes)
            {
                Service serviceView = new Service(user,res);
                serviceView.Show();
                foreach (Window item in App.Current.Windows)
                {
                    if (item != serviceView)
                        item.Close();
                }
            }
        }



        //Bindings
        ///User
        public string Email
        {
            get { return user.Email; }
            set
            {
                user.Email = value;
                OnPropertyChanged("Email");
            }
        }
        public string FirstName
        {
            get { return user.FirstName; }
            set
            {
                user.FirstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        public string LastName
        {
            get { return user.LastName; }
            set
            {
                user.LastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public string Phone
        {
            get { return user.Phone; }
            set
            {
                user.Phone = value;
                OnPropertyChanged("Phone");
            }
        }
        public string Userrole
        {
            get { return user.UserRole; }
            set
            {
                user.UserRole = value;
                OnPropertyChanged("UserRole");
            }
        }

        ///Service
        public string ServiceName
        {
            get { return service.ServiceName; }
            set
            {
                service.ServiceName = value;
                OnPropertyChanged("ServiceName");
            }
        }
        public string Price
        {
            get { return service.Price; }
            set
            {
                service.Price = value;
                OnPropertyChanged("Price");
            }
        }
        public string Descript
        {
            get {
                return service.Descript; }
            set
            {
                service.Descript = value;
                OnPropertyChanged("Descript");
            }
        }

        //Price
        public string priceee;
        public string EnginePrice
        {
            
            get {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 2)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee; }
            set
            {
                service.Price = value;
                OnPropertyChanged("EnginePrice");
            }
        }
        public string SuspensionPrice
        {

            get
            {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 3)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee;
            }
            set
            {
                service.Price = value;
                OnPropertyChanged("SuspensionPrice");
            }
        }
        public string GripPrice
        {

            get
            {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 7)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee;
            }
            set
            {
                service.Price = value;
                OnPropertyChanged("GripPrice");
            }
        }
        public string SteeringPrice
        {

            get
            {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 4)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee;
            }
            set
            {
                service.Price = value;
                OnPropertyChanged("SteeringPrice");
            }
        }
        public string ElectricPrice
        {

            get
            {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 8)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee;
            }
            set
            {
                service.Price = value;
                OnPropertyChanged("ElectricPrice");
            }
        }
        public string CapilerPrice
        {

            get
            {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 6)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee;
            }
            set
            {
                service.Price = value;
                OnPropertyChanged("CapilerPrice");
            }
        }
        public string StartersPrice
        {

            get
            {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 5)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee;
            }
            set
            {
                service.Price = value;
                OnPropertyChanged("StartersPrice");
            }
        }
        public string MufflerPrice
        {

            get
            {
                using (UserContext db = new UserContext())
                {
                    foreach (DataModels.Service service in db.Services)
                    {
                        if (service.Id == 1)
                        {
                            priceee = service.Price;
                        }
                    }
                }
                return priceee;
            }
            set
            {
                service.Price = value;
                OnPropertyChanged("MufflerPrice");
            }
        }
        
        //Comboboxes
        private int userServiceId;
        public int UserServicesId
        {
            get { return userServiceId; }
            set
            {
                userServiceId = value;
                OnPropertyChanged("UserServiceId");
            }
        }
        private int selectOrderId;
        public int SelectOrderId
        {
            get { return selectOrderId; }
            set
            {
                selectOrderId = value;
                OnPropertyChanged("UserServiceId");
            }
        }


        private string userLoginComboSelect;
        public string UserLoginComboSelect
        {
            get { return userLoginComboSelect; }
            set
            {
                userLoginComboSelect = value;
                OnPropertyChanged("UserLoginComboSelect");
            }
        }
        private string userStatusSelect;
        public string UserStatusSelect
        {
            get { return userStatusSelect; }
            set
            {
                userStatusSelect = value;
                OnPropertyChanged("UserStatusSelect");
            }
        }

        private string orderStatusSelect;
        public string OrderStatusSelect
        {
            get { return orderStatusSelect; }
            set
            {
                orderStatusSelect = value;
                OnPropertyChanged("OrderStatusSelect");
            }
        }


        //Auto email
        private void SendEmail(string body, string email)
        {
            MailAddress from = new MailAddress("ivanivanoki@mail.ru", "AutoService");
            MailAddress to = new MailAddress(email);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Auto Service";
            m.Body = body;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.Credentials = new NetworkCredential("ivanivanoki@mail.ru", "oop2020SEM");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
