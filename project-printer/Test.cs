using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClasses
{
    class Test
    {
        public Test(string name, Action test)
        {
            testName = name;
            this.test = test;
        }

        public readonly string testName;
        public Action test;
        public void TestImplement()
        {
            try
            {
                Provider.DataOutputMethod(new string[] { $"{DateTime.Now.ToString()} : {testName} Start" });
                test();
                Provider.DataOutputMethod(new string[] { $"{DateTime.Now.ToString()} : {testName} Complete", "\n" });
            }
            catch (Exception e)
            {
                Provider.DataOutputMethod(new string[] { $"{DateTime.Now.ToString()} : {testName} failed", e.Message, e.StackTrace, "\n" });
            }
        }
    }
}
