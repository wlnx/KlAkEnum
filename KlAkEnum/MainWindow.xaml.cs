// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using KlAkAut;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace KlAkEnum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    class TreeViewItemPxy : TreeViewItem, IDisposable
    {
        public Hashtable PxyInfo = new Hashtable();
        public int PxyId = 0;
        public bool IsPxyVirtual;
        public KlAkProxy Pxy = new KlAkProxy();
        public TreeViewItem SlavesContainer = null;
        public TreeViewItem VirtualsContainer = null;
        private bool fDisposed;

        public TreeViewItemPxy ParentNode
        {
            get
            {
                return (Parent is TreeViewItem ? ((TreeViewItem)Parent).Parent : null) as TreeViewItemPxy;
            }
        }

        public ItemCollection SlaveItems
        {
            get
            {
                if (IsPxyVirtual)
                    throw new InvalidOperationException("Виртуальный сервер не может иметь подчинённых серверов");
                return SlavesContainer.Items;
            }
        }

        public ItemCollection VirtualItems
        {
            get
            {
                if (IsPxyVirtual)
                    throw new InvalidOperationException("Виртуальный сервер не может иметь виртуальных серверов");
                return VirtualsContainer.Items;
            }
        }

        public TreeViewItemPxy(bool IsVirtual = false) : base()
        {
            IsPxyVirtual = IsVirtual;
            if (!IsPxyVirtual)
            {
                SlavesContainer = new TreeViewItem
                {
                    Header = "Подчинённые серверы"
                };
                Items.Add(SlavesContainer);
                VirtualsContainer = new TreeViewItem
                {
                    Header = "Виртуальные серверы"
                };
                Items.Add(VirtualsContainer);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (fDisposed)
                return;

            if (disposing)
            {
                Pxy.Dispose();
            }

            fDisposed = true;
        }
    }
 

    public partial class MainWindow : Window
    {
        int CISResult;

        public MainWindow()
        {
            CISResult = NativeMethods.Init();
            if (CISResult != 0)
            {
                MessageBox.Show("Ошибка при вызове CoInitializeSecurity (0x" + CISResult.ToString("X") + ")", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            InitializeComponent();
        }

        void FixMIAvailability()
        {
            if (SrvTree.SelectedItem is TreeViewItemPxy Current)
            {
                miConnect.IsEnabled = !Current.Pxy.Connected;
                miDisconnect.IsEnabled = miBrowse.IsEnabled = Current.Pxy.Connected;
            }
            else { miConnect.IsEnabled = miDisconnect.IsEnabled = false; }
        }

        string GetSrvInfo()
        {
            string result = "";

            if (SrvTree.SelectedItem is TreeViewItemPxy Item)
            {
                foreach (string Key in Item.PxyInfo.Keys)
                {
                    result += Key + " = " + Item.PxyInfo[Key] + "\r\n";
                }

                if (result != "")
                    result += "\r\n";

                if (!Item.Pxy.Connected)
                {
                    result += "Подключение не установлено\r\n";
                }
                else
                {
                    foreach (string Key in Item.Pxy.Props.Keys)
                    {
                        result += Key + " = " + (Item.Pxy.Props[Key] ?? "<Не определено>") + "\r\n";
                    }
                }
            }

            return result;
        }

        private void miAddSrv_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItemPxy Item = new TreeViewItemPxy
            {
                Header = "KSCRoot"
            };
            SrvTree.Items.Add(Item);
        }

        private void SrvTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FixMIAvailability();

            tbData.Text = GetSrvInfo();
        }

        private void miConnect_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItemPxy tvi = (TreeViewItemPxy)SrvTree.SelectedItem;

            if (tvi.ParentNode == null)
            {
                tvi.Pxy.Connect((string)tvi.Header);
            }
            else
            {
                using (dynamic ssParent = tvi.IsPxyVirtual ? (object)(new KlAkVServers3()) : (new KlAkSlaveServers()))
                {
                    ssParent.AdmSrv = tvi.ParentNode.Pxy;
                    tvi.Pxy.Connect(ssParent, tvi.PxyId);
                }
            }

            FixMIAvailability();

            KlAkSlaveServers Slaves = new KlAkSlaveServers
            {
                AdmSrv = tvi.Pxy
            };
            foreach (int i in Slaves.Ids)
            {
                TreeViewItemPxy NewSrv = new TreeViewItemPxy
                {
                    Header = Slaves[i]["KLSRVH_SRV_DN"],
                    PxyId = i,
                    PxyInfo = Slaves[i]
                };
                tvi.SlaveItems.Add(NewSrv);
            }

            KlAkVServers3 Virtuals = new KlAkVServers3
            {
                AdmSrv = tvi.Pxy
            };
            foreach (int i in Virtuals.Ids)
            {
                TreeViewItemPxy NewSrv = new TreeViewItemPxy(true)
                {
                    Header = Virtuals[i]["KLVSRV_DN"],
                    PxyId = i,
                    PxyInfo = Virtuals[i]
                };
                tvi.VirtualItems.Add(NewSrv);
            }
            tbData.Text = GetSrvInfo();
        }

        private void miBrowse_Click(object sender, RoutedEventArgs e)
        {
            KSCBrowser KSCBrowserWnd = new KSCBrowser();
            KSCBrowserWnd.ShowDialog(((TreeViewItemPxy)SrvTree.SelectedItem).Pxy);
        }

        private void miTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Действий нет");
        }
    }
}
