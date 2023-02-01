using System;
using System.Collections.Generic;
using Autofac;
using HotelManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inside.API.v2.Startups
{
 //   public static class AutofacModuleExtensions
 //   {
 //       public static ContainerBuilder ConfigureDbContexts(this ContainerBuilder builder)
 //       {
 //           builder.Register(componentContext =>
 //{
 //    var serviceProvider = componentContext.Resolve<IServiceProvider>();
 //    var configuration = componentContext.Resolve<IConfiguration>();
 //    var dbContextOptions =
 //        new DbContextOptions<HotelDbContext>(new Dictionary<Type, IDbContextOptionsExtension>());
 //    var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>(dbContextOptions)
 //        .UseApplicationServiceProvider(serviceProvider)
 //        .UseSqlServer(configuration.GetConnectionString("DefaultConnection"), serverOptions =>
 //            serverOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null))
 //        .EnableSensitiveDataLogging()
 //        .EnableDetailedErrors()
 //        .ConfigureWarnings(w =>
 //            w.Ignore(RelationalEventId.BoolWithDefaultWarning));

 //    return optionsBuilder.Options;
 //}).As<DbContextOptions<HotelDbContext>>().InstancePerDependency();

 //           builder.Register(context => context.Resolve<DbContextOptions<HotelDbContext>>()).As<DbContextOptions>()
 //               .InstancePerLifetimeScope();
 //           builder.RegisterType<HotelDbContext>().AsSelf().AsImplementedInterfaces().InstancePerDependency();
 //           builder.Register(context => context.Resolve<HotelDbContext>()).As<DbContext>().InstancePerDependency();

 //           return builder;
 //       }

 //   }
}