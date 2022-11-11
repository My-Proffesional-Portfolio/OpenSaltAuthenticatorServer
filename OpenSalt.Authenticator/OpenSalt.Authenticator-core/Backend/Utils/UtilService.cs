using Microsoft.Extensions.Configuration;
using OpenSalt.Authenticator_infrastructure.Interfaces;


/*----------------------------------WARNING------------------------------------------
  * This is not the best way to get appsettings value in a separate
  * project library or outside startup config, but I do as a
  * demostrative example of how you could do it, try to avoid it;
  * the best way is using dependency injection with a service
  * initialization (or a simple class with properties), more information
  * //Google search: services AddScopped instanciate
  * Initialize the Instances within ConfigServices in Startup .NET
  * https://www.thecodebuzz.com/initialize-instances-within-configservices-in-startup/ 
  * Example : At starutp file or Program.cs
  * builder.Services.AddSingleton<ModelClass>(op =>
    {
     ModelClass obj = new ModelClass();
     obj.SomeProperty = "Your_value_here";
     return obj;
   }); */


namespace OpenSalt.Authenticator_core.Backend.Utils
{

    public class UtilService : IUtilService
    {
        //https://www.ttmind.com/techpost/How-to-read-appSettings-JSON-from-Class-Library-in-ASP-NET-Core
        //        Install the following Nuget Packege in Class library
        //        Microsoft.Extensions.Configuration
        //        Microsoft.Extensions.Configuration.Abstractions
        //        Microsoft.Extensions.Configuration.Json
        //        Microsoft.Extensions.Configuration.Binder

        public string GetAppSettingsConfiguration(string section, string property)
        {

            //IConfigurationBuilder builder = new ConfigurationBuilder();
            //builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            //https://stackoverflow.com/questions/31453495/how-to-read-appsettings-values-from-json-file-in-asp-net-core
            var configuration = new ConfigurationBuilder().AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")).Build()
                .GetSection(section)[property];
            return configuration;

        }

        public List<KeyValuePair<string, string>> GetAppSettingsConfiguration(string section)
        {
            //https://stackoverflow.com/questions/31453495/how-to-read-appsettings-values-from-json-file-in-asp-net-core
            var appSection = new ConfigurationBuilder().AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")).Build().GetSection(section);


            //foreach (var studentBirthdayObject in appSection.GetChildren())
            //{
            //    // your format is a bit weird here where each birthday is a key:value pair,
            //    // rather than something like { "name": "Anne", "birthday": "01/11/2000" }
            //    // so we need to get the children and take the first one
            //    var kv = studentBirthdayObject.GetChildren().First();
            //    string studentName = kv.Key;
            //    string studentBirthday = kv.Value;

            //}

            var result = appSection
            .Get<Dictionary<string, string>[]>()
            .SelectMany(i => i)
            .Select(ie => new KeyValuePair<string, string>(ie.Key, ie.Value))
            .ToList();


            return result;

        }
    }
}
