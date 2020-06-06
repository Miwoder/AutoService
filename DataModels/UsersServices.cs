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
    public class UsersServices : INotifyPropertyChanged
    {

        public int ServiceId { get; set; }

        public DateTime Date { get; set; }


        public int UserId { get; set; }

        [Key]
        public int UserServicesId { get; set; }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("OrderStatus");
            }
        }

        public UsersServices() { }

        public UsersServices(string name, string customer, DateTime date, string status)
        {
            //this.orderName = name;
            //this.customer = customer;
            this.Date = date;
            this.status = status;
        }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
