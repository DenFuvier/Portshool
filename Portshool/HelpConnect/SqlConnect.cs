using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portshool.HelpConnect
{
    public static class SqlConnect
    {
        public static string GetConnect()
        {
            return "server=localhost;userid=Den;password=123;database=portschool";
        }
    }
}