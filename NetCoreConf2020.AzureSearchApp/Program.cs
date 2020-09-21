using NetCoreConf2020.AzureSearchApp.Services;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreConf2020.AzureSearchApp
{
    class Program
    {
         static async Task Main(string[] args)
        {
           var searchServiceName =  ConfigurationManager.AppSettings.Get("searchServiceName");
           var searchServiceKey = ConfigurationManager.AppSettings.Get("searchServiceKey");
            

            var searchServiceClient = new AzureSearchService(searchServiceName, searchServiceKey);

            //first we delete existing search structures if exist in order to create them later from zero
            await searchServiceClient.DeleteDataSourceAsync("jsonaudiosdatasource");
            await searchServiceClient.DeleteIndexerAsync("netcoreconf2020-indexer");
            await searchServiceClient.DeleteSkillsetAsync("jsonskillsets");
            await searchServiceClient.DeleteIndexAsync("netcoreconf2020index");

            //then we create our azure search structures
            var json = ReadJsonResource("jsonaudiosdatasource");
            await searchServiceClient.CreateorUpdateDataSource("jsonaudiosdatasource", json);

            json = ReadJsonResource("netcoreconf2020index");
            await searchServiceClient.CreateorUpdateAzureIndex("netcoreconf2020index", json);

            json = ReadJsonResource("jsonskillsets");
            await searchServiceClient.CreateorUpdateSkillset("jsonskillsets", json);

            json = ReadJsonResource("netcoreconf2020indexer");
            await searchServiceClient.CreateorUpdateIndexer("netcoreconf2020indexer", json);
        }

        static  string ReadJsonResource(string resourceName)
        {
            string jsonIndex = null;

            using (var originalstream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"NetCoreConf2020.AzureSearchApp.Resources.{resourceName}.json"))
            {
                using (StreamReader reader = new StreamReader(originalstream))
                {
                    jsonIndex = reader.ReadToEnd();
                }

            }
            var azureSearchUrl = ConfigurationManager.AppSettings.Get("azureSearchUrl");
            var azureStorageConnectionString = ConfigurationManager.AppSettings.Get("azureStorageConnectionString");
            return jsonIndex.Replace("{azureSearchUrl}", azureSearchUrl).Replace("{azureStorageConnectioString}", azureStorageConnectionString);
        }
    }
}
