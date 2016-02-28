using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;

namespace AddressBook
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();

			// GET /api/{resource}/{action}
			config.Routes.MapHttpRoute(
				name: "Web API RPC",
				routeTemplate: "api/{controller}/{action}",
				defaults: new { },
				constraints: new { action = @"[A-Za-z]+", httpMethod = new HttpMethodConstraint("GET") }
				);

			// GET|PUT|DELETE /api/{resource}/id
			config.Routes.MapHttpRoute(
				name: "Web API Resource",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { },
				constraints: new { id = @"\d+" }
				);

			// GET /api/{resource}
			config.Routes.MapHttpRoute(
				name: "Web API Get All",
				routeTemplate: "api/{controller}",
				defaults: new { action = "Get" },
				constraints: new { httpMethod = new HttpMethodConstraint("GET") }
				);

			// POST /api/{resource}
			config.Routes.MapHttpRoute(
				name: "Web API Post",
				routeTemplate: "api/{controller}",
				defaults: new { action = "Post" },
				constraints: new { httpMethod = new HttpMethodConstraint("POST") }
				);

			//Remove application/xml so that it defaults to returning JSON data.
			var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
			config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
		}
	}
}
