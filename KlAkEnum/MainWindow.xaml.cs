// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using KLAKAUTLib;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace KlAkEnum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    static class NativeMethods
    {
        [DllImport("ole32.dll")]
        static extern int CoInitializeSecurity(
            IntPtr pVoid,
            int cAuthSvc, 
            IntPtr asAuthSvc, 
            IntPtr pReserved1, 
            RpcAuthnLevel level,
            RpcImpLevel impers,
            IntPtr pAuthList,
            EoAuthnCap dwCapabilities,
            IntPtr pReserved3
        );


        enum RpcAuthnLevel
        {
            Default = 0,
            None = 1,
            Connect = 2,
            Call = 3,
            Pkt = 4,
            PktIntegrity = 5,
            PktPrivacy = 6
        }


        enum RpcImpLevel
        {
            Default = 0,
            Anonymous = 1,
            Identify = 2,
            Impersonate = 3,
            Delegate = 4
        }


        [Flags]
        enum EoAuthnCap
        {
            None = 0x00,
            MutualAuth = 0x01,
            StaticCloaking = 0x20,
            DynamicCloaking = 0x40,
            AnyAuthority = 0x80,
            MakeFullSIC = 0x100,
            Default = 0x800,
            SecureRefs = 0x02,
            AccessControl = 0x04,
            AppID = 0x08,
            Dynamic = 0x10,
            RequireFullSIC = 0x200,
            AutoImpersonate = 0x400,
            NoCustomMarshal = 0x2000,
            DisableAAA = 0x1000
        }

        public static int Init()
        {
            return
                CoInitializeSecurity(
                    IntPtr.Zero,
                    -1,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    RpcAuthnLevel.Default,
                    RpcImpLevel.Impersonate,
                    IntPtr.Zero,
                    EoAuthnCap.None,
                    IntPtr.Zero
                );
        }
    }

    static class FetchInfo
    {
        public static KlAkParams PxyProps(KlAkProxy Pxy)
        {
            // Эти поля взяты из klakaut.chm

            return (Pxy == null) ? null : new KlAkParams()
            {
                { "IsAlive", Pxy.GetProp("IsAlive") },
                { "KLADMSRV_VS_LICDISABLED", Pxy.GetProp("KLADMSRV_VS_LICDISABLED") },
                { "KLADMSRV_VSID", Pxy.GetProp("KLADMSRV_VSID") },
                { "KLADMSRV_USERID", Pxy.GetProp("KLADMSRV_USERID")??"<null>" },
                { "KLADMSRV_SAAS_BLOCKED", Pxy.GetProp("KLADMSRV_SAAS_BLOCKED") },
                { "KLADMSRV_SERVER_HOSTNAME", Pxy.GetProp("KLADMSRV_SERVER_HOSTNAME") }
            };
        }

        public static TreeViewItem KlAkView(string Caption, object Item)
        {
            var result = new TreeViewItem() { Header = Caption };

            if (Item == null)
            {
                result.Items.Add(new TreeViewItem() { Header = "Нет данных" });
            }
            else if (Item is KlAkParams Params)
            {
                foreach (string Name in Params)
                {
                    var Value = Params.get_Item(Name);
                    if (Marshal.IsComObject(Value))
                    {
                        result.Items.Add(KlAkView(Name, Value));
                    }
                    else
                    {
                        result.Items.Add(new TreeViewItem() { Header = Name + ": " + Value.ToString() });
                    }
                }
            }
            else if (Item is KlAkCollection Coll)
            {
                for (int i = 0; i < Coll.Count; i++)
                {
                    object Value = Coll.get_Item(i);
                    if (Marshal.IsComObject(Value))
                    {
                        result.Items.Add(KlAkView(i.ToString(), Value));
                    }
                    else
                    {
                        result.Items.Add(new TreeViewItem() { Header = i.ToString() + ": " + Value.ToString() });
                    }
                }
            }
            else if (Item is IKlAkSettingsStorage Settings)
            {
                var Storages = Settings.Enum();
                for (int i = 0; i < Storages.Count; i++)
                {
                    KlAkParams Storage = Storages.get_Item(i);
                    var Data = Settings.Read(Storage.get_Item("PRODUCT"), Storage.get_Item("VERSION"), Storage.get_Item("SECTION"));
                    result.Items.Add(FetchInfo.KlAkView(Storage.get_Item("PRODUCT") + "/" + Storage.get_Item("VERSION") + "/" + Storage.get_Item("SECTION"), Data));
                }
            }
            else { throw new NotImplementedException("Визуализация типа " + Item.GetType().ToString() + " не реализована."); }
            return result;
        }
    }

    class TVISlaveSrvs : TreeViewItem
    {
        public KlAkSlaveServers fSlaves = null;

        public TVISlaveSrvs(KlAkSlaveServers SlavesInfo)
        {
            Header = "Подчинённые серверы";
            fSlaves = SlavesInfo;
            foreach (KlAkParams Slave in fSlaves.GetServers(-1))
            {
                Items.Add(new TVISrvSlave(Slave));
            }
        }
    }

    class TVIVSrvs : TreeViewItem
    {
        public KlAkVServers3 fVirtuals = null;

        public TVIVSrvs(KlAkVServers3 VirtualsInfo)
        {
            Header = "Виртуальные серверы";
            fVirtuals = VirtualsInfo;
            foreach (KlAkParams Virtual in VirtualsInfo.GetVServers(-1))
            {
                Items.Add(new TVISrvVirtual(Virtual));
            }
        }
    }

    class TVISrvRoot : TreeViewItem
    {
        public KlAkProxy fPxy = null;
        KlAkParams fParams = null;

        public KlAkParams ConnectionParameters
        {
            get { return fParams; }
            set
            {
                fParams = value;
                Header = value.get_Item("Address");
            }
        }
        
        public delegate void ConnectEventHandler(object sender);
        public event ConnectEventHandler OnConnect;

        public delegate void DisconnectEventHandler(object sender);
        public event DisconnectEventHandler OnDisconnect;

        public TVISrvRoot()
        {
            Header = "<Не заданы параметры подключения>";
        }

        public void Connect()
        {
            fPxy = new KlAkProxy();
            fPxy.Connect(fParams);

            Items.Add(new TVISlaveSrvs(new KlAkSlaveServers() { AdmServer = fPxy }));
            Items.Add(new TVIVSrvs(new KlAkVServers3() { AdmServer = fPxy }));
            OnConnect?.Invoke(this);
        }

        public void Disconnect()
        {
            Items.Clear();
            fPxy.Disconnect();
            fPxy = null;
            OnDisconnect?.Invoke(this);
        }

        public TreeViewItem ViewPxyProps()
        {
            return (fPxy == null) ? new TreeViewItem() { Header = "Подключение не установлено" } : FetchInfo.KlAkView("Данные о подключении", FetchInfo.PxyProps(fPxy));
        }
    }

    class TVISrvSlave : TreeViewItem
    {
        public KlAkProxy fPxy = null;
        public KlAkParams fSrvInfo = null;

        public delegate void ConnectEventHandler(object sender);
        public event ConnectEventHandler OnConnect;

        public delegate void DisconnectEventHandler(object sender);
        public event DisconnectEventHandler OnDisconnect;

        public TVISrvSlave(KlAkParams SrvInfo)
        {
            fSrvInfo = SrvInfo;
            Header = fSrvInfo.get_Item("KLSRVH_SRV_DN");
        }

        public void Connect()
        {
            fPxy = ((TVISlaveSrvs)Parent).fSlaves.Connect(fSrvInfo.get_Item("KLSRVH_SRV_ID"), -1);
            Items.Add(new TVISlaveSrvs(new KlAkSlaveServers() { AdmServer = fPxy }));
            Items.Add(new TVIVSrvs(new KlAkVServers3() { AdmServer = fPxy }));
            OnConnect?.Invoke(this);
        }

        public void Disconnect()
        {
            Items.Clear();
            fPxy.Disconnect();
            fPxy = null;
            OnDisconnect?.Invoke(this);
        }

        public TreeViewItem ViewInfo()
        {
            return FetchInfo.KlAkView("Информация о сервере", fSrvInfo);
        }

        public TreeViewItem ViewPxyProps()
        {
            return (fPxy == null) ? new TreeViewItem() { Header = "Подключение не установлено" } : FetchInfo.KlAkView("Данные о подключении", FetchInfo.PxyProps(fPxy));
        }
    }

    class TVISrvVirtual : TreeViewItem
    {
        public KlAkParams fSrvInfo = null;

        public TVISrvVirtual(KlAkParams SrvInfo)
        {
            fSrvInfo = SrvInfo;
            Header = fSrvInfo.get_Item("KLVSRV_DN");
        }

        public TreeViewItem[] ViewInfo()
        {
            return new[]
            {
                FetchInfo.KlAkView("Информация о сервере", fSrvInfo),
                FetchInfo.KlAkView("Статистика", ((TVIVSrvs)Parent).fVirtuals.GetVServerStatistic((int)fSrvInfo.get_Item("KLVSRV_ID"))),
                FetchInfo.KlAkView("Разрешения", ((TVIVSrvs)Parent).fVirtuals.GetPermissions((int)fSrvInfo.get_Item("KLVSRV_ID")))
            };
        }
    }

    public partial class MainWindow : Window
    {
        int CISResult;
        bool fCredsRequired = false;

        void ViewSrvInfo(TVISrvVirtual Item)
        {
            SrvInfo.Items.Clear();
            foreach (var VItem in Item.ViewInfo())
            {
                SrvInfo.Items.Add(VItem);
            }
        }

        void ViewSrvInfo(TVISrvSlave Item)
        {
            SrvInfo.Items.Clear();
            SrvInfo.Items.Add(Item.ViewInfo());
            SrvInfo.Items.Add(Item.ViewPxyProps());
        }

        void ViewSrvInfo(TVISrvRoot Item)
        {
            SrvInfo.Items.Clear();
            SrvInfo.Items.Add(Item.ViewPxyProps());
        }

        public MainWindow()
        {
            CISResult = NativeMethods.Init();
            if (CISResult != 0)
            {
                MessageBox.Show("Ошибка при вызове CoInitializeSecurity (0x" + CISResult.ToString("X") + "). Используем явную аутентификацию.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                fCredsRequired = true;
            }
            InitializeComponent();
        }

        private void MenuTest_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuAddSrv_Click(object sender, RoutedEventArgs e)
        {
            var ConnParamsWnd = new ConnParams(fCredsRequired) { Owner = this } ;
            if (fCredsRequired)
            {
                
            }
            if ((bool)ConnParamsWnd.ShowDialog())
            {
                var Params = new KlAkParams()
                {
                    { "Address", ConnParamsWnd.tbAddress.Text + ":" + ConnParamsWnd.tbPort.Text }
                };
                if (ConnParamsWnd.cbUseSSL.IsChecked.HasValue)
                {
                    Params.Add("UseSSL", ConnParamsWnd.cbUseSSL.IsChecked);
                }
                if (ConnParamsWnd.cbCompressTraffic.IsChecked.HasValue)
                {
                    Params.Add("CompressTraffic", ConnParamsWnd.cbUseSSL.IsChecked);
                }
                if ((bool)ConnParamsWnd.cbIsAuthenticating.IsChecked)
                {
                    Params.Add("User", ConnParamsWnd.tbUser.Text);
                    if (ConnParamsWnd.tbDomain.Text != "") { Params.Add("Domain", ConnParamsWnd.tbDomain.Text); }
                    Params.Add("Password", ConnParamsWnd.tbPassword.Password);
                }
                if ((bool)ConnParamsWnd.cbIsUsingProxy.IsChecked)
                {
                    Params.Add("ProxyAddress", ConnParamsWnd.tbProxyAddress.Text + ":" + ConnParamsWnd.tbProxyPort.Text);
                    if (ConnParamsWnd.tbProxyUser.Text != "")
                    {
                        Params.Add("ProxyLogin", ConnParamsWnd.tbProxyUser.Text);
                        Params.Add("ProxyPassword", ConnParamsWnd.tbProxyPassword.Password);
                    }
                }
                if (ConnParamsWnd.cbThroughGw.IsChecked.HasValue)
                {
                    Params.Add("ThroughGw", ConnParamsWnd.cbThroughGw.IsChecked);
                }

                var Root = new TVISrvRoot() { ConnectionParameters = Params } ;
                SrvTree.Items.Add(Root);
            }
        }

        private void SrvTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RefreshSrvInfo(e.NewValue);
        }

        private void RefreshSrvInfo(object Node)
        {
            MenuConnect.IsEnabled = (((Node is TVISrvRoot) && ((TVISrvRoot)Node).fPxy == null) || ((Node is TVISrvSlave) && ((TVISrvSlave)Node).fPxy == null));
            MenuDisconnect.IsEnabled = (((Node is TVISrvRoot) && ((TVISrvRoot)Node).fPxy != null) || ((Node is TVISrvSlave) && ((TVISrvSlave)Node).fPxy != null));
            MenuBrowse.IsEnabled = MenuDisconnect.IsEnabled || (Node is TVISrvVirtual);
            if (Node is TVISrvVirtual VSrv)
            {
                ViewSrvInfo(VSrv);
            }
            else if (Node is TVISrvSlave SSrv)
            {
                ViewSrvInfo(SSrv);
            }
            else if (Node is TVISrvRoot RSrv)
            {
                ViewSrvInfo(RSrv);
            }
            else
            {
                SrvInfo.Items.Clear();
            }
        }

        private void MenuConnect_Click(object sender, RoutedEventArgs e)
        {
            if (SrvTree.SelectedItem is TVISrvRoot Root)
            {
                Root.Connect();
            }
            else if (SrvTree.SelectedItem is TVISrvSlave Slave)
            {
                Slave.Connect();
            }
            RefreshSrvInfo(SrvTree.SelectedItem);
        }

        private void MenuDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (SrvTree.SelectedItem is TVISrvRoot Root)
            {
                Root.Disconnect();
            }
            else if (SrvTree.SelectedItem is TVISrvSlave Slave)
            {
                Slave.Disconnect();
            }
            RefreshSrvInfo(SrvTree.SelectedItem);
        }

        private void MenuBrowse_Click(object sender, RoutedEventArgs e)
        {
            KSCBrowser Browser = null;

            if (SrvTree.SelectedItem is TVISrvRoot RSrv)
            {
                Browser = new KSCBrowser(RSrv.fPxy);
            }
            else if (SrvTree.SelectedItem is TVISrvSlave SSrv)
            {
                Browser = new KSCBrowser(SSrv.fPxy);
            }
            else if (SrvTree.SelectedItem is TVISrvVirtual VSrv)
            {
                Browser = new KSCBrowser(((TVIVSrvs)VSrv.Parent).fVirtuals.AdmServer, VSrv.fSrvInfo.get_Item("KLVSRV_ID"));
            }

            Browser.ShowDialog();
        }
    }
}
