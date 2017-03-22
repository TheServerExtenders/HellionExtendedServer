using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.API
{
    public class HesApi
    {

        public Chat Chat;


        private static HesApi m_instance;
        internal static HesApi API => m_instance;

        
        internal HesApi()
        {
            m_instance = this;

            Chat = new Chat();

        }
        

    }
}
