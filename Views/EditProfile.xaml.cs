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
using AutoService.ViewModel;
using AutoService.DataModels;

namespace AutoService
{

    public partial class EditProfile : Window
    {
        EditProfileViewModel editProfileViewModel;
        public EditProfile(User user, ResourceDictionary res)
        {
            InitializeComponent();
            editProfileViewModel = new EditProfileViewModel(user,res);
            DataContext = editProfileViewModel;
            this.Resources = res;
        }
    }
}
