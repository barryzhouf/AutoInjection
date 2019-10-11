using AutoInjection;
using System;
using System.Collections.Generic;
using System.Text;
using test.common;

namespace test.service
{
    public interface Iuserservice
    {
        [Transactional(typeof(TestInterceptor))]
        List<string> getList();
    }
}
