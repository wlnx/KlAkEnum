// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace KlAkAut
{
    enum KlAkTypes
    {
        IKlAkParams = 1,
        IKlAkCollection = 2,
        IKlAkProxy = 3,
        IKlAkGroups = 4,
        IKlAkHosts = 5,
        IKlAkIpSubnets = 6,
        IKlAkSlaveServers = 7,
        IKlAkHstRulesEx = 8,
        IKlAkUpdateAgents = 9,
        IKlAkTasks = 10,
        IKlAkAdHosts = 11,
        IKlAkLicense = 12,
        IKlAkEvents = 13,
        IKlAkReports = 14,
        IKlAkPolicies = 15,
        IKlAkLicHst = 17,
        IKlAkVServers = 18,
        IKlAkUsers = 19,
        IKlAkPackages = 20,
        IKlAkDeployment = 21,
        IKlAkTasks2 = 22,
        IKlAkStatistics = 23,
        IKlAkUserDevices = 24,
        IKlAkHstIncidents = 25,
        IKlAkPackages2 = 26,
        IKlAkTagsControl = 27,
        IKlAkUserList = 28,
        IKlAkSettingsStorage = 29,
        IKlAkPolicyProfiles = 30,
        IKlAkPolicies2 = 31,
        IKlAkSrvStatistics2 = 32,
        IKlAkHosts2 = 33,
        IKlAkTunnels = 34,
        IKlAkPackages3 = 35,
        IKlAkSecurityPolicy = 36,
        IKlAkSecurityPolicy2 = 37,
        IKlAkSecurityGroups = 38,
        IKlAkUserDevices2 = 39,
        IKlAkNotificationProperties = 40,
        IKlAkSrvView = 41,
        IKlAkLicense2 = 42,
        KlAkHostAffectivePolicy = 43,
        KlAkWebUsers = 44,
        KlAkGcm = 45,
        IKlAkCertPoolCtrl = 46,
        IKlAkPackages4 = 47,
        IKlAkVapmCtrl = 53
    }

    class NativeMethods
    {
        [DllImport("ole32.dll")]
        static extern int CoInitializeSecurity(IntPtr pVoid, int
        cAuthSvc, IntPtr asAuthSvc, IntPtr pReserved1, RpcAuthnLevel level,
        RpcImpLevel impers, IntPtr pAuthList, EoAuthnCap dwCapabilities, IntPtr
        pReserved3);

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

    class KlAkObject : IDisposable
    {
        bool fInitObj = true;
        protected bool fDisposed = false;
        protected dynamic fObject;
        protected string fKlAkTypeName;

        public dynamic Object
        {
            get
            {
                return fObject;
            }
        }

        KlAkTypes KlAkType
        {
            get
            {
                return (KlAkTypes)fObject.Type;
            }
        }

        public KlAkObject(string KlAkTypeName, bool InitObj = true)
        {
            fInitObj = InitObj;
            fKlAkTypeName = KlAkTypeName;
            if (fInitObj)
            {
                Type _KlAkType = Type.GetTypeFromProgID(fKlAkTypeName, true);
                fObject = (dynamic)Activator.CreateInstance(_KlAkType);
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
                // NA yet
            }

            if (fInitObj)
            {
                Marshal.ReleaseComObject(fObject);
            }

            fDisposed = true;
        }
    }

    class KlAkParams : KlAkObject
    {
        public object[] Keys
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkParams", "Объект уже освобождён");
                object[] result = new object[Object.Count];
                for (int i = 0; i < Object.Count; i++)
                {
                    result[i] = Object[i];
                }

                return result;
            }
        }

        public object this[object i]
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkParams", "Объект уже освобождён");
                return Object.Item(i);
            }
        }

        public KlAkParams() : base("klakaut.KlAkParams")
        {

        }

        public KlAkParams(dynamic Params) : base("klakaut.KlAkParams")
        {
            Marshal.ReleaseComObject(fObject);
            fObject = Params;
        }

        public void Add(dynamic index, dynamic Val)
        {
            if (fDisposed)
                throw new ObjectDisposedException("KlAkParams", "Объект уже освобождён");
            Object.Add(index, Val);
        }
    }

    class KlAkCollection : KlAkObject
    {
        public int Count
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkCollection", "Объект уже освобождён");
                return Object.Count;
            }
        }

        public dynamic this[int i]
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkCollection", "Объект уже освобождён");
                return (i >= Object.Count) ? Object.Item(i) : null;
            }
            set
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkCollection", "Объект уже освобождён");
                if (i >= Object.Count)
                    Object.SetSize(i + 1);
                Object.SetAt(i, value);
            }
        }

        public KlAkCollection(params object[] Items) : base("klakaut.KlAkCollection")
        {
            foreach (object Item in Items)
                this[Count] = Item;
        }
    }

    class KlAkProxy : KlAkObject
    {
        bool fConnected = false;
        string fAddr = "localhost";
        ushort fPort = 13291;
        
        public string Build
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkProxy", "Объект уже освобождён");
                return Object.Build;
            }
        }

        public int VersionId
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkProxy", "Объект уже освобождён");
                return Object.VersionId;
            }
        }

        public Hashtable Props
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkProxy", "Объект уже освобождён");
                Hashtable result = new Hashtable()
                {
                    { "IsAlive", Object.GetProp("IsAlive") },
                    { "KLADMSRV_VS_LICDISABLED", Object.GetProp("KLADMSRV_VS_LICDISABLED") },
                    { "KLADMSRV_VSID", Object.GetProp("KLADMSRV_VSID") },
                    { "KLADMSRV_USERID", Object.GetProp("KLADMSRV_USERID") },
                    { "KLADMSRV_SAAS_BLOCKED", Object.GetProp("KLADMSRV_SAAS_BLOCKED") },
                    { "KLADMSRV_SERVER_HOSTNAME", Object.GetProp("KLADMSRV_SERVER_HOSTNAME") }
                };
                return result;
            }
        }

        public bool Connected
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkProxy", "Объект уже освобождён");
                return fConnected;
            }
        }

        public KlAkProxy() : base("klakaut.KlAkProxy", false)
        {
            
        }

        public void Disconnect()
        {
            if (fDisposed)
                throw new ObjectDisposedException("KlAkProxy", "Объект уже освобождён");
            Object.Disconnect();
            Marshal.ReleaseComObject(fObject);
            fConnected = false;
        }

        public void Connect(string Address = "", ushort Port = 0, bool Virtual = false)
        {
            if (fDisposed)
                throw new ObjectDisposedException("KlAkProxy", "Объект уже освобождён");

            Type _KlAkType = Type.GetTypeFromProgID(fKlAkTypeName, true);
            fObject = (dynamic)Activator.CreateInstance(_KlAkType);
            using (KlAkParams Params = new KlAkParams())
            {
                if (Address != "")
                {
                    fAddr = Address;
                }
                if (Port != 0)
                {
                    fPort = Port;
                }
                Params.Add("Address", fAddr + ":" + fPort.ToString());
                Object.Connect(Params.Object);
            }

            fConnected = true;
        }

        public void Connect(dynamic Parent, int Id)
        {
            if (fDisposed)
                throw new ObjectDisposedException("KlAkProxy", "Объект уже освобождён");
            if (Parent is KlAkVServers3)
                throw new NotImplementedException("Подключение к виртуальному серверу не реализовано");
            fAddr = "";
            fPort = 0;
            fObject = Parent.Object.Connect(Id, -1);
            fConnected = true;
        }
    }

    class KlAkHosts2 : KlAkObject
    {
        public KlAkProxy AdmSrv
        {
            set
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkHosts2", "Объект уже освобождён");
                Object.AdmServer = value.Object;
            }
        }

        public KlAkHosts2() : base("klakaut.KlAkHosts2")
        {

        }

        public Hashtable GetHostInfo(string UId, params string[] Props)
        {
            if (fDisposed)
                throw new ObjectDisposedException("KlAkHosts2", "Объект уже освобождён");
            Hashtable result = new Hashtable();
            dynamic Data = Object.GetHostInfo(UId, (new KlAkCollection(Props)).Object);
            foreach (object i in Data)
                result.Add(i, Data.Item(i));
            return result;
        }
    }

    class KlAkSlaveServers : KlAkObject
    {
        public KlAkProxy AdmSrv
        {
            set
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkSlaveServers", "Объект уже освобождён");
                Object.AdmServer = value.Object;
            }
        }

        public int[] Ids
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkSlaveServers", "Объект уже освобождён");
                int[] result = new int[Object.GetServers(-1).Count];
                int i = 0;
                foreach (dynamic Srv in Object.GetServers(-1))
                {
                    result[i] = Srv["KLSRVH_SRV_ID"];
                }
                return result;
            }
        }

        public Hashtable this[int i]
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkSlaveServers", "Объект уже освобождён");
                Hashtable result = new Hashtable();
                foreach (dynamic Server in Object.GetServers(-1))
                {
                    if (Server.Item("KLSRVH_SRV_ID") != i)
                        continue;
                    foreach (string Param in Server)
                        result[Param] = Server.Item(Param);
                    using (KlAkGroups Grps = new KlAkGroups())
                    {
                        Grps.Object.AdmServer = Object.AdmServer;
                        result["grp_full_name"] = Grps[(int)result["KLSRVH_SRV_GROUPID"]]["grp_full_name"];
                    }
                    break;
                }
                return result;
            }
        }

        public KlAkSlaveServers() : base("klakaut.KlAkSlaveServers")
        {

        }
    }

    class KlAkVServers3 : KlAkObject
    {
        public KlAkProxy AdmSrv
        {
            set
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkVServers3", "Объект уже освобождён");
                Object.AdmServer = value.Object;
            }
        }

        public int[] Ids
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkVServers3", "Объект уже освобождён");
                int[] result = new int[Object.GetVServers(-1).Count];
                int i = 0;
                foreach (dynamic Srv in Object.GetVServers(-1))
                {
                    result[i] = Srv["KLVSRV_ID"];
                }
                return result;
            }
        }

        public Hashtable this[int i]
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkVServers3", "Объект уже освобождён");
                Hashtable result = new Hashtable();
                foreach (dynamic Server in Object.GetVServers(-1))
                {
                    if (Server.Item("KLVSRV_ID") != i)
                        continue;
                    foreach (string Param in Server)
                        result[Param] = Server.Item(Param);
                    using (KlAkGroups Grps = new KlAkGroups())
                    {
                        Grps.Object.AdmServer = Object.AdmServer;
                        result["grp_full_name"] = Grps[(int)result["KLVSRV_GRP"]]["grp_full_name"];
                    }
                    break;
                }
                return result;
            }
        }

        public KlAkVServers3() : base("klakaut.KlAkVServers3")
        {

        }
    }

    class KlAkGroups : KlAkObject
    {
        public KlAkProxy AdmSrv
        {
            set
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkGroups", "Объект уже освобождён");
                Object.AdmServer = value.Object;
            }
        }

        public Hashtable this[int i]
        {
            get
            {
                if (fDisposed)
                    throw new ObjectDisposedException("KlAkGroups", "Объект уже освобождён");
                Hashtable result = new Hashtable();
                dynamic GroupInfo = Object.GetGroupInfo(i);
                result["grp_full_name"] = GroupInfo.Item("grp_full_name");
                return result;
            }
        }

        public KlAkGroups() : base("klakaut.KlAkGroups")
        {

        }
    }
}