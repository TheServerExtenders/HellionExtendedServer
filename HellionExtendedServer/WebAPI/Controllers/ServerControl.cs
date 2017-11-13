using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web.Http;
using System.Web.Http.SelfHost;

namespace HellionExtendedServer.WebAPI.Controllers
{
    public class ServerControlController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "World" };
        }

        public string Get(int id)
        {
            return "Hello, World!";
        }

        public void Post([FromBody]string value)
        {
        }
    }
}
