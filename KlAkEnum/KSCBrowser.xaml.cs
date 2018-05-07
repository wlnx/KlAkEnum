// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using KLAKAUTLib;
using System.Windows;
using System.Windows.Controls;

namespace KlAkEnum
{
    /// <summary>
    /// Логика взаимодействия для KSCBrowser.xaml
    /// </summary>
    /// 
    class TVIGroup : TreeViewItem
    {
        KlAkGroups fGroups;
        int fGroupId;

        public int Id { get => fGroupId; }

        void CreateSubItems()
        {
            var GroupsHier = fGroups.GetSubgroups(fGroupId, 1);
            foreach (IKlAkParams Group in GroupsHier)
            {
                Items.Add(new TVIGroup(Group.get_Item("id"), fGroups));
            }
        }

        public TVIGroup(int GroupId, KlAkGroups Groups)
        {
            fGroupId = GroupId;
            fGroups = Groups;
            Header = fGroupId.ToString() + ": " + fGroups.GetGroupInfo(fGroupId).get_Item("name");
            CreateSubItems();
        }

        public TreeViewItem ViewInfo()
        {
            return FetchInfo.KlAkView("Информация о группе", fGroups.GetGroupInfo(fGroupId));
        }
    }

    class TVIHost : TreeViewItem
    {
        public TVIHost(string HostName, KlAkHosts2 Hosts)
        {
            var HostParamsList = new KlAkCollection();
            HostParamsList.SetSize(1);
            HostParamsList.SetAt(0, "KLHST_WKS_DN");
            var HostParams = Hosts.GetHostInfo(HostName, HostParamsList);
            Header = HostParams.get_Item("KLHST_WKS_DN");
            Items.Add(FetchInfo.KlAkView("Настройки узла", Hosts.GetHostSettings(HostName, "SS_SETTINGS")));
        }
    }

    public partial class KSCBrowser : Window
    {
        int? fVServerId = null;
        KlAkGroups fGroups = new KlAkGroups();
        KlAkHosts2 fHosts = new KlAkHosts2();

        int idGroups, idSuper, idUnassigned;
   
        public KSCBrowser(KlAkProxy Proxy, int? VServerId = null)
        {
            fVServerId = VServerId;
            fGroups.AdmServer = Proxy;
            fHosts.AdmServer = Proxy;

            if (fVServerId.HasValue)
            {
                var GrpList = new KlAkCollection();
                GrpList.SetSize(3);
                GrpList.SetAt(0, "KLVSRV_GROUPS");
                GrpList.SetAt(0, "KLVSRV_SUPER");
                GrpList.SetAt(0, "KLVSRV_UNASSIGNED");
                var VSrvs = (new KlAkVServers3() { AdmServer = fGroups.AdmServer }).GetVServerInfo((int)fVServerId, GrpList);
                idGroups = VSrvs.get_Item("KLVSRV_GROUPS");
                idSuper = VSrvs.get_Item("KLVSRV_SUPER");
                idUnassigned = VSrvs.get_Item("KLVSRV_UNASSIGNED");
            }
            else
            {
                idGroups = fGroups.GroupIdGroups;
                idSuper = fGroups.GroupIdSuper;
                idUnassigned = fGroups.GroupIdUnassigned;
            }

            InitializeComponent();

            RefreshGroupsTree();
        }

        private void GroupsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GroupInfo.Items.Clear();
            GroupInfo.Items.Add(((TVIGroup)e.NewValue).ViewInfo());

            var HostFieldsList = new KlAkCollection();
            HostFieldsList.SetSize(2);
            HostFieldsList.SetAt(0, "KLHST_WKS_DN");
            HostFieldsList.SetAt(1, "KLHST_WKS_HOSTNAME");
            var HostList = fHosts.FindHosts("(KLHST_WKS_GROUPID = " + ((TVIGroup)e.NewValue).Id.ToString() + ")", HostFieldsList, HostFieldsList);
            foreach (KlAkParams HostInfo in HostList)
            {
                GroupInfo.Items.Add(new TVIHost(HostInfo.get_Item("KLHST_WKS_HOSTNAME"), fHosts)) ;
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void RefreshGroupsTree()
        {
            GroupsTree.Items.Clear();
            GroupsTree.Items.Add(new TVIGroup(idGroups, fGroups));
            GroupsTree.Items.Add(new TVIGroup(idSuper, fGroups));
            GroupsTree.Items.Add(new TVIGroup(idUnassigned, fGroups));
        }
    }
}
