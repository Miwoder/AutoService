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
using System.Data.SqlClient;
using System.Data;
using AutoService.ViewModel;
using AutoService.DataModels;
using AutoService.Context;

namespace AutoService
{

    public partial class Service : Window
    {
        ResourceDictionary res;
        ServiceViewModel serviceViewModel;
        ResourceDictionary rus = new ResourceDictionary() { Source = new Uri(@"C:\Users\bestb\OneDrive\Рабочий стол\КурсачООП\AutoService\Lang\LangRU.xaml") };
        ResourceDictionary eng = new ResourceDictionary() { Source = new Uri(@"C:\Users\bestb\OneDrive\Рабочий стол\КурсачООП\AutoService\Lang\LangEN.xaml") };
        public Service(User user, ResourceDictionary resource)
        {
            this.Resources = resource;
            res = resource;
            userr = user;
            InitializeComponent();
            serviceViewModel = new ServiceViewModel(user, res);
            DataContext = serviceViewModel;

            if (user.UserRole == "user")
            {
                AdminList.Visibility = Visibility.Hidden;
                AdminPersonsList.Visibility = Visibility.Hidden;
                editCaliper.Visibility = editElectric.Visibility = editEngine.Visibility = editGrip.Visibility = editmuffler.Visibility = editStarters.Visibility =
                    editSteering.Visibility = editSuspension.Visibility = Visibility.Hidden;
            }
        }

        private void ChangeLangRU(object sender, RoutedEventArgs e)
        {
            this.Resources = rus;
            res = rus;
            Service service = new Service(userr, res);
            service.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != service)
                    item.Close();
            }
        }

        private void ChangeLangEN(object sender, RoutedEventArgs e)
        {
            this.Resources = eng;
            res = eng;
            Service service = new Service(userr, res);
            service.Show();
            foreach (Window item in App.Current.Windows)
            {
                if (item != service)
                    item.Close();
            }
        }




        User userr;
        private void EditInfo(object sender, RoutedEventArgs e)
        {
            ResourceDictionary res = this.Resources;
            EditProfile editProfile = new EditProfile(userr, res);
            editProfile.Show();
        }
    }
}
