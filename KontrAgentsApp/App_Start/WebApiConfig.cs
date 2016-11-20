using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Routing.Constraints;

namespace KontrAgentsApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Конфигурация и службы Web API
            // Настройка Web API для использования только проверки подлинности посредством маркера-носителя.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Маршруты Web API
            config.MapHttpAttributeRoutes();
                                 
            //т.к. два запроса GET без параметра id - указываем дополнительный маршрут по названию метода
            config.Routes.MapHttpRoute(
            name: "ActionRoute",
            routeTemplate: "api/{controller}/{action}",
            defaults: new { },
            constraints: new
            {
                action = new AlphaRouteConstraint() //накладываем ограничение, что только буквы (иначе сюда же попадет GET с id)
            }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute("DefaultApiGet", "api/{controller}", new { action = "getkontragents" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //config.Routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}");                                
        }
    }
}
