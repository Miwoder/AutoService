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
using System.Configuration;


namespace AutoService
{

    public partial class MainWindow : Window
    {
        ResourceDictionary rus = new ResourceDictionary() { Source = new Uri(@"C:\Users\bestb\OneDrive\Рабочий стол\КурсачООП\AutoService\Lang\LangRU.xaml") };
        ResourceDictionary eng = new ResourceDictionary() { Source = new Uri(@"C:\Users\bestb\OneDrive\Рабочий стол\КурсачООП\AutoService\Lang\LangEN.xaml") };
        ResourceDictionary res;

        MainWindowViewModel mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel(res);
            DataContext = mainWindowViewModel;
        }


        private void ChangeLangRU(object sender, RoutedEventArgs e)
        {
            this.Resources = rus;
            res = rus;
        }

        private void ChangeLangEN(object sender, RoutedEventArgs e)
        {
            this.Resources = eng;
            res = eng;
        }

    }
}
