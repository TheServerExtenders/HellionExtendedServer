using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers
{
    public class PermissionManager
    {

        protected readonly string SaveFile = "HES/Perms.json";
        protected Dictionary<long,Permission> PermissionFactory = new Dictionary<long, Permission>();

        public PermissionManager()
        {
            LoadFromFile();
        }

        public void LoadFromFile()
        {
            if (File.Exists(SaveFile))
            {
                string json = System.IO.File.ReadAllText(SaveFile);
                List<Permission> melist = JsonConvert.DeserializeObject<List<Permission>>(json);
                foreach (Permission me in melist)
                {
                    PermissionFactory.Add(me.GUID, me);
                }
            }
        }

        public void Save()
        {
            List<Permission> melist = PermissionFactory.Values.ToList();
            string json = JsonConvert.SerializeObject(melist, Formatting.Indented);
            //BUG might have error here since I dont have '@' at the begining of the name :/
            System.IO.File.WriteAllText(SaveFile, json);
        }

        public Permission GetPlayerPermission(Player p)
        {
            return GetPermission(p.GUID);
        }

        public Permission GetPermission(long guid)
        {
            if (PermissionFactory.ContainsKey(guid))
            {
                return PermissionFactory[guid];
            }
            Permission p = new Permission(guid);
            PermissionFactory.Add(guid,p);
            return p;
        }

    }
}
