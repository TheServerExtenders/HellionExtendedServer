using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers.Plugins.Config;
using Newtonsoft.Json.Linq;

namespace HellionExtendedServer.Managers
{
    public class TestingClass
    {
        public TestingClass()
        {

        }

        public static void PreStartEvent()
        {
            //PreE1();
        }
        public static void PostStartEvent()
        {
            //PostE1();
        }

        public static void PostE1()
        {
            
        }
        public static void PreE1()
        {
            Console.WriteLine("Starting Test====111=");
            JObject cc = ConfigConstructor.GetJObject("Test.json");
            if (cc.GetInt("TESTING") == 50) Console.WriteLine("YOOOOOO111111111OOOOOO");
        }

    }
}
