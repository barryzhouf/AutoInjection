namespace AutoInjection
{
    using Castle.DynamicProxy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class AutowiredControllerActivator : IControllerActivator
    {
        public object Create(ControllerContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            Type serviceType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
            var target = actionContext.HttpContext.RequestServices.GetRequiredService(serviceType);

            //方法拦截
            List<Type> interceptorTypes = new List<Type>();
            var methods = serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo info in methods)
            {
                if (info.GetCustomAttribute<TransactionalAttribute>() != null)
                {
                    var type = info.GetCustomAttributesData().FirstOrDefault().ConstructorArguments[0].Value as Type;
                    interceptorTypes.Add(type);
                }
            }

            List<IInterceptor> interceptors = interceptorTypes
               .ConvertAll<IInterceptor>(interceptorType => actionContext.HttpContext.RequestServices.GetService(interceptorType) as IInterceptor);

            var proxy = new ProxyGenerator().CreateClassProxy(serviceType, interceptors.ToArray());

            //属性注入
            var properties = serviceType.GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo info in properties)
            {
                if (info.GetCustomAttribute<AutowiredAttribute>() != null)
                {
                    var propertyType = info.PropertyType;
                    var impl = actionContext.HttpContext.RequestServices.GetService(propertyType);
                    if (impl != null)
                    {
                        info.SetValue(proxy, impl);
                    }
                }
            }

            //字段注入
            var fields = serviceType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo info in fields)
            {
                if (info.GetCustomAttribute<AutowiredAttribute>() != null)
                {
                    var fieldType = info.FieldType;
                    var impl = actionContext.HttpContext.RequestServices.GetService(fieldType);
                    if (impl != null)
                    {
                        info.SetValue(proxy, impl);
                    }
                }
            }

            return proxy;
        }

        public void Release(ControllerContext context, object controller)
        {
            
        }
    }
}
