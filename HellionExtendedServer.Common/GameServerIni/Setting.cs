using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Common.GameServerIni
{
    public class Setting : CollectionBase
    {
        public string Name;
        public bool Enabled;
        public bool Required;
        public string Category;
        public Type Type;
        public object DefaultValue;
        public object Value;
        public string Description;
        public string Line;
        public bool Valid;


        public GameServerProperty this[int index]
        {
            get
            {
                return (GameServerProperty)base.List[index];
            }
            set
            {
                base.List[index] = (GameServerProperty)value;
            }
        }

        public void Add(GameServerProperty Value)
        {
            base.List.Add(Value);
        }

        public Setting()
        {

        }
    }
}
