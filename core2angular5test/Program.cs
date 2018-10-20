using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace core2angular5test
{
    public class Program
    {
        public static void Main(string[] args)
        {    
            //Build & Run on the created WebHost
            CreateWebHostBuilder(args).Build().Run();
        }

        /*
         * What is a Web Host? In a very few words, a host is the execution context of any ASP.NET Core app.
         *
         * In a web-based application, the host must implement the IWebHost interface, which exposes a collection of
         * web-related features and services and also a Start method. The Web Host references the server that will
         * handle requests.
         * 
         * The preceeding statement can lead to thinking that the web host and the web server are the same thing;
         * however, it's very important to understand that they're not, as they serve very different purposes.
         * 
         * The following excerpt from the .NET Core GitHub project does a great job explaining the key difference
         * between them:
         * The host is responsible for application startup and lifetime management. The server is responsible for
         * accepting HTTP requests.
         * Part of the host's responsibility includes ensuring that the application's services and the server are
         * available and properly configured. We could think of the host as being a wrapper around the server.
         * The host is configured to use a particular server; the server is unaware of its host.
         *
         * 
         */
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
         /*
          * Sets up Kestrel server, content root folder, where to look for appsettings.json and other configuration files
          * IIS integration (what about mac os?), logging
          * Which Startup class to use
          * 
          * https:/​/​github.​com/​aspnet/​MetaPackages/​blob/​rel/ 2.​0.​0/​src/​Microsoft.​AspNetCore/​WebHost.​cs.
          */
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}