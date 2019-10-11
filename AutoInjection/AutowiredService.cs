namespace AutoInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class AutowiredService
    {
        #region 接口模式
        /// <summary>
        /// 瞬时
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSbTransient<TService, TImplementation>(this IServiceCollection services, params Type[] interceptorTypes) =>
            services.AddSbService(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);

        /// <summary>
        /// 请求级别
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSbScoped<TService, TImplementation>(this IServiceCollection services, params Type[] interceptorTypes) =>
            services.AddSbService(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);

        /// <summary>
        /// 单例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSbSingleton<TService, TImplementation>(this IServiceCollection services, params Type[] interceptorTypes) =>
            services.AddSbService(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);

        public static IServiceCollection AddSbService(this IServiceCollection services, Type serviceType, Type implementationType,
            ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));

            object Factory(IServiceProvider provider)

            {
                var target = provider.GetService(implementationType);

                //属性注入
                var properties = implementationType.GetTypeInfo().DeclaredProperties;
                foreach (PropertyInfo info in properties)
                {
                    if (info.GetCustomAttribute<AutowiredAttribute>() != null)
                    {
                        var propertyType = info.PropertyType;
                        var impl = provider.GetService(propertyType);
                        if (impl != null)
                        {
                            info.SetValue(target, impl);
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
                        var impl = provider.GetService(fieldType);
                        if (impl != null)
                        {
                            info.SetValue(target, impl);
                        }
                    }
                }

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
                    .ConvertAll<IInterceptor>(interceptorType => provider.GetService(interceptorType) as IInterceptor);

                var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(serviceType, target, interceptors.ToArray());

                return proxy;
            };

            var serviceDescriptor = new ServiceDescriptor(serviceType, Factory, lifetime);
            services.Add(serviceDescriptor);

            return services;
        }
        #endregion

        #region 实例类模式
        /// <summary>
        /// 瞬时
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSbTransient<TService>(this IServiceCollection services, params Type[] interceptorTypes) =>
            services.AddSbService(typeof(TService), ServiceLifetime.Transient);

        /// <summary>
        /// 请求级别
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSbScoped<TService>(this IServiceCollection services, params Type[] interceptorTypes) =>
            services.AddSbService(typeof(TService), ServiceLifetime.Scoped);

        /// <summary>
        /// 单例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSbSingleton<TService>(this IServiceCollection services, params Type[] interceptorTypes) => 
            services.AddSbService(typeof(TService), ServiceLifetime.Singleton);

        public static IServiceCollection AddSbService(this IServiceCollection services, Type serviceType,
            ServiceLifetime lifetime)
        {
            object Factory(IServiceProvider provider)
            {
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
                   .ConvertAll<IInterceptor>(interceptorType => provider.GetService(interceptorType) as IInterceptor);

                var proxy = new ProxyGenerator().CreateClassProxy(serviceType, interceptors.ToArray());

                //属性注入
                var properties = serviceType.GetTypeInfo().DeclaredProperties;
                foreach (PropertyInfo info in properties)
                {
                    if (info.GetCustomAttribute<AutowiredAttribute>() != null)
                    {
                        var propertyType = info.PropertyType;
                        var impl = provider.GetService(propertyType);
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
                        var impl = provider.GetService(fieldType);
                        if (impl != null)
                        {
                            info.SetValue(proxy, impl);
                        }
                    }
                }

                return proxy;
            };

            var serviceDescriptor = new ServiceDescriptor(serviceType, Factory, lifetime);
            services.Add(serviceDescriptor);

            return services;
        }
        #endregion

        /// <summary>
        /// 添加MvcBuilder扩展
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddAutowired(this IMvcBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            ControllerFeature feature = new ControllerFeature();
            builder.PartManager.PopulateFeature<ControllerFeature>(feature);
            foreach (Type type in feature.Controllers.Select(c => c.AsType()))
            {
                builder.Services.TryAddTransient(type, type);
            }
            builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, AutowiredControllerActivator>());

            return builder;
        }
    }
}
