using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Npgsql;

namespace Milestone3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserPage userPage;
        BusinessPage businessPage;

        /// <summary>
        /// Initialize the win forms app
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.userPage = new UserPage();
            this.PageFrame.Content = this.userPage;
        }

        private void UserPanelButtonClick(object sender, RoutedEventArgs e)
        {
            this.userInfoButton.Foreground = new SolidColorBrush(Colors.RoyalBlue);
            GridCursor.Width = 70;
            GridCursor.Margin = new Thickness(0, 0, 300, 0);
            this.businessSearchButton.Foreground = new SolidColorBrush(Colors.Gray);

            // Change Page
            this.PageFrame.Content = this.userPage;
        }

        private void BusinessPanelButtonClick(object sender, RoutedEventArgs e)
        {
            this.businessSearchButton.Foreground = new SolidColorBrush(Colors.RoyalBlue);
            GridCursor.Width = 118;
            GridCursor.Margin = new Thickness(0, 0, 120, 0);
            this.userInfoButton.Foreground = new SolidColorBrush(Colors.Gray);

            // Change Page
            //if (this.businessPage == null)
            //{
                //this.businessPage = new BusinessPage(this.userPage.userId);
            //}
            this.businessPage = new BusinessPage(this.userPage.userId);

            this.PageFrame.Content = this.businessPage;
        }

        private void PowerButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
