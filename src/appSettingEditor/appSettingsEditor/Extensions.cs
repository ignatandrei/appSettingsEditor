using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using RoslynSettingEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appSettingsEditor
{
    public static class Extensions
    {
        private static void MapFile(string file, IEndpointRouteBuilder endpoint)
        {
            //use ManifestEmbeddedFileProvider
            endpoint.MapGet(file, async cnt =>
            {
                var extension = Path.GetExtension(file);
                switch (extension)
                {
                    case ".svg":
                        extension = "image/svg+xml";
                        break;
                    case ".js":
                        extension = "application/javascript";
                        break;
                    case ".css":
                        extension = "text/css";
                        break;
                    default:
                        throw new ArgumentException("cannot know content from " + extension);
                }
                cnt.Response.ContentType = extension;
                var res = EmbeddedResource.GetContent(file);

                byte[] result = Encoding.UTF8.GetBytes(res);

                var m = new ReadOnlyMemory<byte>(result);
                await cnt.Response.BodyWriter.WriteAsync(m);

            });

        }
        public static IEndpointRouteBuilder MapSettingsView<T>(this IEndpointRouteBuilder endpoint, IConfiguration configuration, string nameFile = "appsettings.json", string endpointAPISettings = "/api/appsettings")
     where T : IAppSettingsConfig<T>, new()
        {
            MapFile("settingsUI/jsoneditor.js", endpoint);
            MapFile("settingsUI/jsoneditor.css", endpoint);
            MapFile("settingsUI/img/jsoneditor-icons.svg", endpoint);
            endpoint.MapGet("settingsUI", async cnt =>
            {
                cnt.Response.Redirect("settingsUI/index.html");
            });

            endpoint.MapGet("settingsUI/index.html", async cnt =>
            {
                var res = EmbeddedResource.GetContent("settingsUI/index.html");
                res = res.Replace("appSettingsClass.ClassName", Path.GetFileNameWithoutExtension(nameFile));
                res = res.Replace("endpointAPISettings", endpointAPISettings);
                byte[] result = Encoding.UTF8.GetBytes(res);

                var m = new ReadOnlyMemory<byte>(result);


                cnt.Response.ContentType = "text/html";
                await cnt.Response.BodyWriter.WriteAsync(m);

            });
            return endpoint;
        }

    }
}
