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
    public class Service : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }


        private string serviceName;
        public string ServiceName
        {
            get { return serviceName; }
            set
            {
                serviceName = value;
                OnPropertyChanged("ServiceName");
            }
        }

        private string price;
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }

        private string descript;
        public string Descript
        {
            get { return descript; }
            set
            {
                descript = value;
                OnPropertyChanged("Descript");
            }
        }

        public ICollection<UsersServices> UserServices { get; set; }
        public Service()
        {
            UserServices = new List<UsersServices>();
        }

        public Service(string serviceName, string price, string descript)
        {
            this.serviceName = serviceName;
            this.price = price;
            this.descript = descript;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
