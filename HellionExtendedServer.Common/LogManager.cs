using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Common
{
    public class LogManager
    {
        private static LogManager m_instance;

        public LogManager Instance {  get { return m_instance; } }

        public LogManager()
        {
            m_instance = this;
        }
    }

    public class LogInstance
    {

    }
}
