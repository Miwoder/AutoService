using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoService.DataModels;
using AutoService.ViewModel;

namespace AutoService
{
    public partial class EditPrice : Window
    {
        ResourceDictionary res;
        EditPriceViewModel editPriceViewModel;
        public EditPrice(int Id, User user, ResourceDictionary resource)
        {
            res = resource;
            InitializeComponent();
            editPriceViewModel = new EditPriceViewModel(Id, user, res);
            DataContext = editPriceViewModel;
        }
    }
}
