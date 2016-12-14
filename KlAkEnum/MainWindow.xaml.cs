// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using KlAkAut;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace KlAkEnum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int CISResult;

        public MainWindow()
        {
            CISResult = AllowNWAuth.Init();
            if (CISResult != 0)
            {
                MessageBox.Show("Ошибка при вызове CoInitializeSecurity (0x" + CISResult.ToString("X") + ")", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            InitializeComponent();
        }

        string miGetSrvInfoGeneric(TreeViewItem Node)
        {
            string result = "";
            if (Node.Parent == SrvTree)
            {
                result = "Для корневого сервера общая информация недоступна\r\n";
            }
            else
            {
                int i = int.Parse(((string)Node.Header).Split(':')[0]);
                KlAkSlaveServers Neighbors = new KlAkSlaveServers();
                Neighbors.AdmSrv = (KlAkProxy)((TreeViewItem)((TreeViewItem)Node.Parent).Parent).Tag;
                foreach (string Param in Neighbors[i].Keys)
                {
                    result += Param + ": " + Neighbors[i][Param] + "\r\n";
                }
            }
            return result;
        }

        string miGetSrvInfo(TreeViewItem Node)
        {
            KlAkProxy Pxy = (KlAkProxy)Node.Tag;
            string result = "";
            if (!Pxy.Connected)
            {
                result = "Сервер не подключен\r\n";
            }
            else
            {
                foreach (string Param in Pxy.Props.Keys)
                {
                    result += Param + ": " + (Pxy.Props[Param] == null ? "<Информация отсутствует>" : Pxy.Props[Param]) + "\r\n";
                }
            }
            return result;
        }

        private void miAddSrv_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem Item = new TreeViewItem();
            KlAkProxy Pxy = new KlAkProxy();
            Item.Tag = Pxy;
            Item.Header = "KSCRoot";
            SrvTree.Items.Add(Item);
        }

        private void SrvTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem tvi = (TreeViewItem)SrvTree.SelectedItem;
            miConnect.IsEnabled = (tvi?.Tag != null) && (!((KlAkProxy)tvi.Tag).Connected);
            miDisconnect.IsEnabled = (tvi?.Tag != null) && (((KlAkProxy)tvi.Tag).Connected);

            tbData.Text = tvi.Tag == null ? "" : miGetSrvInfoGeneric(tvi) + "\r\n" + miGetSrvInfo(tvi);
        }

        private void miConnect_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)SrvTree.SelectedItem;

            if (tvi.Parent == SrvTree)
            {
                ((KlAkProxy)tvi.Tag).Connect((string)tvi.Header);
            }
            else
            {
                using (KlAkSlaveServers ssParent = new KlAkSlaveServers())
                {
                    ssParent.Object.Admserver = ((KlAkProxy)((TreeViewItem)((TreeViewItem)tvi.Parent).Parent).Tag).Object;
                    ((KlAkProxy)tvi.Tag).Connect(ssParent, int.Parse(((string)tvi.Header).Split(':')[0]));
                }
            }

            miConnect.IsEnabled = (tvi?.Tag != null) && (!((KlAkProxy)tvi.Tag).Connected);
            miDisconnect.IsEnabled = (tvi?.Tag != null) && (((KlAkProxy)tvi.Tag).Connected);

            TreeViewItem ChildrenItem = new TreeViewItem();
            ChildrenItem.Header = "Подчинённые серверы";
            tvi.Items.Add(ChildrenItem);
            KlAkSlaveServers Slaves = new KlAkSlaveServers();
            Slaves.AdmSrv = (KlAkProxy)tvi.Tag;
            foreach (int i in Slaves.Ids)
            {
                TreeViewItem NewSrv = new TreeViewItem();
                NewSrv.Header = i.ToString() + ": " + Slaves[i]["KLSRVH_SRV_DN"];
                ChildrenItem.Items.Add(NewSrv);
            }

            TreeViewItem VirtualsItem = new TreeViewItem();
            VirtualsItem.Header = "Виртуальные серверы";
            tvi.Items.Add(VirtualsItem);

            tbData.Text = tvi.Tag == null ? "" : miGetSrvInfoGeneric(tvi) + "\r\n" + miGetSrvInfo(tvi);
        }
    }
}
