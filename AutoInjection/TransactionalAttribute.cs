namespace AutoInjection
{
    using Castle.DynamicProxy;
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalAttribute : Attribute
    {
        public TransactionalAttribute(Type type)
        {

        }
    }
}
