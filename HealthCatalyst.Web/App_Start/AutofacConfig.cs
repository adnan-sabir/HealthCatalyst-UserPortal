using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using HealthCatalyst.Service.UserService;
using HealthCatalyst.DataAccess.Repository;
using HealthCatalyst.Domain.Data;
using HealthCatalyst.DataAccess;

namespace HealthCatalyst.Web.App_Start
{
    public static class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // Register API controllers using assembly scanning.
            builder.RegisterControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();

            // Register Service layer this way if only few Service classes.
            builder.RegisterType<UserService>().As<IService<User>>()
                    .InstancePerRequest();
            // Register DataAccess layer this way if only few DAO classes.
            builder.RegisterType<Repository<User>>().As<IRepository<User>>()
                    .InstancePerRequest();
            builder.RegisterType<UserContext>().As<IContext>()
                    .InstancePerRequest();

            // Register Service layer this way if has many Service classes with same ending.
            //builder.RegisterAssemblyTypes(Assembly.Load("Service"))
            //        .Where(t => t.Name.EndsWith("Service"))
            //        .AsImplementedInterfaces()
            //        .InstancePerLifetimeScope();

            // Register DataAccess layer this way if has many DAO classes with same ending.
            //builder.RegisterAssemblyTypes(Assembly.Load("DataAccess"))
            //        .Where(t => t.Name.EndsWith("DAO"))
            //        .AsImplementedInterfaces()
            //        .InstancePerLifetimeScope();


            var container = builder.Build();
            // Set MVC DI resolver to use our Autofac container
            var resolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);
        }
    }
}