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
    public class InfoUserServices : INotifyPropertyChanged
    {
        public string ServiceName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
        public int Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
