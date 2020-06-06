using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using AutoService.Context;
using AutoService.DataModels;

namespace AutoService.ViewModel
{
    class EditPriceViewModel : BaseViewModel
    {
        ResourceDictionary res;
        public int serviceId;
        public User user;
        public EditPriceViewModel(int id, User user, ResourceDictionary resource)
        {
            res = resource;
            this.user = user;
            serviceId = id;
        }

        private DelegateCommand savePrice;
        public DelegateCommand SavePrice
        {
            get
            {
                return savePrice ?? (savePrice = new DelegateCommand(SavePriceBT));
            }
        }
        private void SavePriceBT(object arg)
        {
            int check = 0;
            using (UserContext db = new UserContext())
            {
                foreach (DataModels.Service service in db.Services)
                {
                    if (service.Id == serviceId)
                    {
                        service.Price = editPrice + "$";
                       // MessageBox.Show(service.ServiceName + " = " + service.Price);
                        break;
                    }
                }
                db.SaveChanges();
            }
                Service serviceView = new Service(user, res);
                serviceView.Show();
                foreach (Window item in App.Current.Windows)
                {
                    if (item != serviceView)
                        item.Close();
                }
        }

        public string pattern = @"^[0-9]{1,20}$";
        private string pri;
        public string editPrice
        {
            get { return pri; }
            set
            {
                if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
                    pri = value;
                else
                    MessageBox.Show("Введен неверный формат (1-20 цифр) / Uncorrect format(1-20 figures)");
                OnPropertyChanged("Price");
            }
        }

    }
}
