using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Avocado.Web.Utilities;
using Avocado.Domain.Abstract;
using Avocado.Domain.Concrete;
using System.Configuration;

namespace Avocado.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Browse", // Route name
                "Browse/Category/{category}",
                new { controller = "Browse", action = "Category", category = (string)null }// Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            SetupDependencyInjection();
        }

        public void SetupDependencyInjection()
        {
            //create Ninject DI kernel
            IKernel kernel = new StandardKernel();

            //register services with Ninject DI container
            kernel.Bind<IAccountMembershipService>().To<AccountMembershipService>().WithConstructorArgument("connectionString", ConfigurationManager.ConnectionStrings["AvocadoEntities"].ConnectionString);
            kernel.Bind<IFormsAuthenticationService>().To<FormsAuthenticationService>();
            kernel.Bind<IBrowseService>().To<BrowseService>().WithConstructorArgument("connectionString", ConfigurationManager.ConnectionStrings["AvocadoEntities"].ConnectionString);
            
            //tell ASP.net MVC to use Ninject DI container
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}