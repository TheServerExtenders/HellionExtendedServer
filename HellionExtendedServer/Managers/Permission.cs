using System;
using System.Collections.Generic;

namespace HellionExtendedServer.Managers
{
    public class Permission
    {
        public long GUID { get; set; }
        public String name { get; set; }
        public Boolean OP { get; set; }
        public List<String> Perms { get; set; }
        public List<String> Groups { get; set; }

        public Permission(long guid)
        {
            GUID = guid;
            name = null;
            OP = false;
            Perms = new List<string>();
        }

        //Super smart now! Allows for HES.* and HES.I.Am.Testing.This.Very.Long.Name.That.I.Can.Can.Access.With HES.* or HES.I.* ect....
        public Boolean HasPerm(String perm)
        {
            String[] b = perm.ToLower().Split(".".ToCharArray());
            foreach (String p in Perms)
            {
                bool valid = false;
                String[] a = p.Split(".".ToCharArray());
                if (b.Length < a.Length) continue;
                for (int z = 0; z < b.Length; z++)
                {
                    if (b[z].ToLower() == a[z].ToLower())
                    {
                        valid = true;
                        continue;
                    }
                    if (a[z] == "*") return true; //Insta Valid!
                    break;
                }
                if (valid) return true;
            }
            return false;
            //Basic
            //return Perms.Contains(perm.ToLower());
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
            if (!Perms.Contains(perm.ToLower())) Perms.Add(perm.ToLower());
        }

        public void DelPerm(String perm)
        {
            if (Perms.Contains(perm.ToLower())) Perms.Remove(perm.ToLower());
        }

        public void AddGroup(String group)
        {
            if (!Groups.Contains(group.ToLower())) Groups.Add(group.ToLower());
        }

        public void DelGroup(String group)
        {
            if (Groups.Contains(group.ToLower())) Groups.Remove(group.ToLower());
        }
    }
}