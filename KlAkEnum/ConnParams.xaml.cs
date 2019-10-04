// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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

namespace KlAkEnum
{
    /// <summary>
    /// Логика взаимодействия для ConnParams.xaml
    /// </summary>
    public partial class ConnParams : Window
    {
        public ConnParams()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ErrMsg = "";
            if ((tbAddress.Text == "") || (tbPort.Text == ""))
            {
                ErrMsg += "Необходимо указать адрес и порт для подключения к серверу.\r\n";
            }
            if ((bool)cbIsAuthenticating.IsChecked)
            {
                if (tbUser.Text == "")
                {
                    ErrMsg += "Необходимо указать имя пользователя для подключения к серверу администрирования.\r\n";
                }
                if (tbPassword.Password == "")
                {
                    ErrMsg += "Необходимо указать пароль для подключения к серверу администрирования.\r\n";
                }
            }
            if ((bool)cbIsUsingProxy.IsChecked)
            {
                if ((tbProxyAddress.Text == "") || (tbProxyPort.Text == ""))
                {
                    ErrMsg += "Необходимо указать адрес и порт для подключения к прокси-серверу.\r\n";
                }
                if ((tbProxyUser.Text == "") ^ (tbProxyPassword.Password == ""))
                {
                    ErrMsg += "Имя пользователя и пароль для прокси-сервера должны быть указаны или не указаны одновременно.\r\n";
                }
            }
            if (ErrMsg != "")
            {
                MessageBox.Show(this, ErrMsg, "Ошибка заполнения данных подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                DialogResult = true;
            }
        }

        private void cbIsAuthenticating_Checked(object sender, RoutedEventArgs e)
        {
            if (cbUseSSL.IsChecked != true)
            {
                cbIsAuthenticating.IsChecked = false;
            }
            cbIsAuthenticating.IsEnabled = (cbUseSSL.IsChecked == true);
            tbUser.IsEnabled = tbPassword.IsEnabled = tbDomain.IsEnabled = (bool)cbIsAuthenticating.IsChecked;
        }

        private void cbIsUsingProxy_Checked(object sender, RoutedEventArgs e)
        {
            tbProxyAddress.IsEnabled = tbProxyPort.IsEnabled = tbProxyUser.IsEnabled = tbProxyPassword.IsEnabled = (bool)cbIsUsingProxy.IsChecked;
        }
    }
}
