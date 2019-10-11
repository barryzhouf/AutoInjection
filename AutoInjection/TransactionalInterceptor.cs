namespace AutoInjection
{
    using Castle.DynamicProxy;
    using System.Reflection;

    public abstract class TransactionalInterceptor : StandardInterceptor
    {
        protected override void PreProceed(IInvocation invocation)
        {
            BeforeExecute(invocation);
        }

        protected override void PerformProceed(IInvocation invocation)
        {
            invocation.Proceed();
        }

        protected override void PostProceed(IInvocation invocation)
        {
            Executed(invocation);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="invocation"></param>
        abstract public void BeforeExecute(IInvocation invocation);

        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="invocation"></param>
        abstract public void Executed(IInvocation invocation);
    }
}
