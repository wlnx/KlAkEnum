// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using KlAkAut;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace KlAkEnum
{
    /// <summary>
    /// Логика взаимодействия для KSCBrowser.xaml
    /// </summary>
    /// 
    class TreeViewItemGrp : TreeViewItem
    {
        public int GrpId;
        KlAkGroups fGrps = new KlAkGroups();

        TreeView Root
        {
            get
            {
                object result = Parent;
                while (!(result is TreeView))
                    result = ((TreeViewItemGrp)result).Parent;
                return (TreeView)result;
            }
        }



        public TreeViewItemGrp(int GroupId, dynamic Pxy)
        {
            GrpId = GroupId;
            fGrps.Object.AdmServer = Pxy;
        }
    }

    public partial class KSCBrowser : Window
    {
        KlAkGroups fGrps = new KlAkGroups();

        void CreateChildren(TreeViewItemGrp Current, dynamic ChindrenInfo)
        {
            if (ChindrenInfo != null)
            {
                for (int i = 0; i < ChindrenInfo.Count; i++)
                {
                    TreeViewItemGrp Tmp = new TreeViewItemGrp(ChindrenInfo.Item(i).Item("id"), fGrps.Object.AdmServer)
                    {
                        Header = ChindrenInfo.Item(i).Item("name")
                    };
                    CreateChildren(Tmp, ChindrenInfo.Item(i).Item("groups"));
                    Current.Items.Add(Tmp);
                }
            }
        }

        internal bool? ShowDialog(KlAkProxy Pxy)
        {
            fGrps.AdmSrv = Pxy;
            KSCTree.Items.Clear();
            TreeViewItemGrp TmpItem;
            TmpItem = new TreeViewItemGrp((int)fGrps.Object.GroupIdSuper, fGrps.Object.AdmServer)
            {
                Header = "Сервер администрирования"
            };
            CreateChildren(TmpItem, fGrps.Object.GetSubgroups(TmpItem.GrpId, 0));
            KSCTree.Items.Add(TmpItem);
            TmpItem = new TreeViewItemGrp((int)fGrps.Object.GroupIdGroups, fGrps.Object.AdmServer)
            {
                Header = "Управляемые компьютеры"
            };
            CreateChildren(TmpItem, fGrps.Object.GetSubgroups(TmpItem.GrpId, 0));
            KSCTree.Items.Add(TmpItem);
            TmpItem = new TreeViewItemGrp((int)fGrps.Object.GroupIdUnassigned, fGrps.Object.AdmServer)
            {
                Header = "Нераспределённые компьютеры"
            };
            CreateChildren(TmpItem, fGrps.Object.GetSubgroups(TmpItem.GrpId, 0));
            KSCTree.Items.Add(TmpItem);
            return ShowDialog();
        }

        public KSCBrowser()
        {
            InitializeComponent();
        }
    }
}
