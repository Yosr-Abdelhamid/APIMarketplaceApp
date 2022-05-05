/*using System.Web.Http;
using Unity;
using APIMarketplaceApp.Resolver;

namespace APIMarketplaceApp
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services  
            var container = new UnityContainer();
            //container.RegisterType<IEmployeeRepository, EmployeeSqlRepository>();  
            //container.RegisterType<IEmployeeRepository, EmployeeMongoRepository>();  
            //container.RegisterType<IEmployeeRepository, EmployeeMySqlRepository>();
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes  
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
*/