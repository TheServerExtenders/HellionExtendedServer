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
        
        protected readonly string SaveFile = "HES/PlayerPerms.json";
        protected Dictionary<long,Permission> PermissionFactory = new Dictionary<long, Permission>();
        protected Dictionary<string, PermissionAttribute> PermsDictionary = new Dictionary<string, PermissionAttribute>();

        public PermissionManager()
        {
            LoadFromFile();
        }

        public void AddPermissionAttribute(PermissionAttribute value)
        {
            PermsDictionary.Add(value.PermissionName.ToLower(),value);
        }

        public bool PlayerHasPerm(Player p, string perm)
        {
            Permission permission = GetPlayerPermission(p);
            if(permission.HasPerm(perm))return true;
            return false;
        }
        public bool CheckPerm(string perm)
        {
            if (PermsDictionary.ContainsKey(perm.ToLower()))
            {
                PermissionAttribute pa = PermsDictionary[perm.ToLower()];
                if (pa.Default.ToLower() == "default") return false;
            }
            return true;
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

        public void SetPlayerPermission(Permission p)
        {
            PermissionFactory.Add(p.GUID,p);
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
