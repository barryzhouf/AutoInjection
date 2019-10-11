using AutoInjection;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using test.common;
using test.dao;

namespace test.service
{
    public class userservice : Iuserservice
    {
        [Autowired]
        private Iuserdao udao { set; get; }

        public List<string> getList()
        {
            return udao.getList();
        }
    }
}
