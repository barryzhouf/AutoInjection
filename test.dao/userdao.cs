using System;
using System.Collections.Generic;
using System.Text;

namespace test.dao
{
    public class userdao : Iuserdao
    {
        public List<string> getList()
        {
            List<string> retValue = new List<string>();
            retValue.Add("test1");
            retValue.Add("test2");
            retValue.Add("test3");

            return retValue;
        }
    }
}
