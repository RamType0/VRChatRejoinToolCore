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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VRChatRejoinToolCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel Model => (MainWindowViewModel)DataContext;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectNewer(object sender, RoutedEventArgs e)
        {
            Model.SelectNewer();
        }

        private void SelectOlder(object sender, RoutedEventArgs e)
        {
            Model.SelectOlder();
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            VRChatApp.Launch(Model.Selected.Instance,false);
        }

        private void OpenInstanceDetail(object sender, RoutedEventArgs e)
        {
            VRChatWeb.OpenInstanceDetail(Model.Selected.Instance);
        }

        private void OpenUserDetail(object sender, RoutedEventArgs e)
        {
            VRChatWeb.OpenUserDetail(Model.Selected.Instance.OwnerId);
        }
    }
}
