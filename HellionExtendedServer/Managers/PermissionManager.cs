using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers
{
    public class PermissionManager
    {
        private readonly string SaveFile = "HES/PlayerPerms.json";
        private readonly Dictionary<long, Permission> PermissionFactory = new Dictionary<long, Permission>();
        private readonly Dictionary<string, PermissionAttribute> PermsDictionary = new Dictionary<string, PermissionAttribute>();

        public PermissionManager()
        {
            LoadFromFile();
        }

        public void AddPermissionAttribute(PermissionAttribute value)
        {
            PermsDictionary[value.PermissionName.ToLower()] = value;
        }

        public void DelPermissionAttribute(PermissionAttribute value)
        {
            PermsDictionary.Remove(value.PermissionName.ToLower());
        }

        public bool PlayerHasPerm(Player p, string perm)
        {
            if (!CheckPerm(perm, p)) return true;
            Permission permission = GetPlayerPermission(p);
            if (permission.HasPerm(perm)) return true;
            return false;
        }

        public bool CheckPerm(string perm, Player p = null)
        {
            PermissionAttribute pa;
            if (PermsDictionary.TryGetValue(perm.ToLower(), out pa))
            {
                if (pa.Default.ToLower() == "default") return false;
                if (p != null)
                {
                    Permission permission = GetPlayerPermission(p);
                    if (permission.OP && pa.Default.ToLower() == "OP") return true;
                }
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
            PermissionFactory[p.GUID] = p;
        }

        public void AddToPlayerPermission(Player p, string perm)
        {
            Permission perms = GetPlayerPermission(p);
            perms.AddPerm(perm);
        }

        public void RemovePermissionFromPlayer(Player p, string perm)
        {
            Permission perms = GetPlayerPermission(p);
            perms.DelPerm(perm);
        }

        public void AddPlayerToGroup(Player p, string group)
        {
            Permission perms = GetPlayerPermission(p);
            perms.AddGroup(group);
        }

        public void RemovePlayerFromGroup(Player p, string group)
        {
            Permission perms = GetPlayerPermission(p);
            perms.DelGroup(group);
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
            PermissionFactory.Add(guid, p);
            return p;
        }
    }
}