using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Solid.Testing.AspNetCore.Abstractions.Factories
{
    public abstract class WebHostFactory : IWebHostFactory
    {
        protected IWebHostOptionsProvider Provider { get; }

        protected WebHostFactory(IWebHostOptionsProvider provider)
        {
            Provider = provider;
        }

        public virtual IWebHost CreateWebHost(Type startup, string hostname)
        {
            var builder = InitializeWebHostBuilder(startup, hostname);
            builder.UseConfiguration(GenerateApplicationConfiguration(new ConfigurationBuilder()));
            Provider.Configure(builder);
            return builder.Start();
        }

        protected abstract IWebHostBuilder InitializeWebHostBuilder(Type startup, string hostname);

        protected virtual IConfiguration GenerateApplicationConfiguration(IConfigurationBuilder builder)
        {
            foreach (var path in GetAppSettingsPaths())
                builder.AddJsonFile(path);
            return builder.Build();
        }

        protected virtual IEnumerable<string> GetAppSettingsPaths()
        {
            var current = AppDomain.CurrentDomain.BaseDirectory;
            var settings = new[]
            {
                Path.Combine(current, "appsettings.json")
            }
            .Concat(GetProjectFilePaths()
                .Distinct()
                .Select(p => Path.GetDirectoryName(p))
                .Select(p => Path.Combine(p, "appsettings.json"))
            )
            .Where(p => File.Exists(p))
            .ToArray();

            return settings;
        }

        protected virtual IEnumerable<string> GetProjectFilePaths()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var directory = new DirectoryInfo(path);
            while (!ContainsProject(directory.FullName))
            {
                if (directory.Parent == directory.Root) break;
                directory = directory.Parent;
            }

            var projects = Directory.EnumerateFiles(directory.FullName, "*.csproj");
            foreach (var project in projects)
            {
                foreach (var reference in GetProjectReferences(project))
                    yield return reference;
                yield return project;
            }
        }

        protected virtual IEnumerable<string> GetProjectReferences(string project)
        {
            using (var file = File.OpenRead(project))
            {
                var doc = XDocument.Load(file);
                var references = doc.Descendants("ProjectReference");
                foreach (var reference in references)
                {
                    var path = reference.Attribute("Include").Value;
                    path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project), path));
                    yield return path;
                }
            }

            yield break;
        }

        protected virtual bool ContainsProject(string path) =>
            Directory.EnumerateFiles(path, "*.csproj").Any();
    }
}
