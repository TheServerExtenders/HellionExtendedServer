using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers
{
    public class Permission
    {
        public long GUID { get; set; }
        public String name { get; set; }
        public Boolean OP{ get; set; }
        public List<String> Perms{ get; set; }
        public List<String> Groups{ get; set; }

        public Permission(long guid)
        {
            GUID = guid;
            name = null;
            OP = false;
            Perms = new List<string>();
        }

        public Boolean HasPerm(String perm)
        {
            return Perms.Contains(perm);
        }
        public Boolean IsOP()
        {
            return OP;
        }

        public void SetOP(bool op = true)
        {
            OP = op;
        }

        public void AddPerm(String perm)
        {
            if (!Perms.Contains(perm)) Perms.Add(perm);
        }

        public void DelPerm(String perm)
        {
            if (Perms.Contains(perm)) Perms.Remove(perm);
        }
        public void AddGroup(String group)
        {
            if (!Groups.Contains(group)) Groups.Add(group);
        }

        public void DelGroup(String groupo)
        {
            if (Groups.Contains(group)) Groups.Remove(group);
        }
    

}
}
