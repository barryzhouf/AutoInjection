using AutoInjection;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace test.common
{
    public class TestInterceptor : TransactionalInterceptor
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="invocation"></param>
        public override void BeforeExecute(IInvocation invocation)
        {
            Console.WriteLine("{0}拦截前，参数个数", invocation.Method.Name, invocation.Arguments.Length);
            
        }

        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="invocation"></param>
        public override void Executed(IInvocation invocation)
        {
            Console.WriteLine("{0}拦截后，返回值是{1}", invocation.Method.Name, invocation.ReturnValue);
        }
    }
}
