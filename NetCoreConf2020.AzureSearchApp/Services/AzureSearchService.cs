using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreConf2020.AzureSearchApp.Services
{

    public class AzureSearchService
    {
        private SearchServiceClient serviceClient;
        public AzureSearchService(string searchServiceName, string searchServiceAdminApiKey)
        {
            this.serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(searchServiceAdminApiKey));
            serviceClient.HttpClient.DefaultRequestHeaders.Add("api-key", searchServiceAdminApiKey);

        }


        #region CreateorUpdate
        public async Task CreateorUpdateDataSource(string dataSourceName, string jsonDataSourceDescription)
        {

            try
            {
                var uri = $"https://{serviceClient.SearchServiceName}.{serviceClient.SearchDnsSuffix}/datasources/{dataSourceName}?api-version=2020-06-30";
                var content = new StringContent(jsonDataSourceDescription, Encoding.UTF8, "application/json");
                var response = await serviceClient.HttpClient.PutAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                throw;
            }
        }

        public async Task CreateorUpdateAzureIndex(string indexName, string jsonIndexDescription)
        {
            try
            {
                var uri = $"https://{serviceClient.SearchServiceName}.{serviceClient.SearchDnsSuffix}/indexes/{indexName}?api-version=2020-06-30";
                var content = new StringContent(jsonIndexDescription, Encoding.UTF8, "application/json");
                var response = await serviceClient.HttpClient.PutAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch(Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                throw;
            }

        }
        public async Task CreateorUpdateSkillset(string skillSetName, string jsonSkillset)
        {

            try
            {
                var uri = $"https://{serviceClient.SearchServiceName}.{serviceClient.SearchDnsSuffix}/skillsets/{skillSetName}?api-version=2020-06-30";
                var content = new StringContent(jsonSkillset, Encoding.UTF8, "application/json");
                var response = await serviceClient.HttpClient.PutAsync(uri, content);
                response.EnsureSuccessStatusCode();

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                throw;
            }

        }
        public async Task CreateorUpdateIndexer(string indexerName, string jsonIndexerDefinition)
        {

            try
            {
                var uri = $"https://{serviceClient.SearchServiceName}.{serviceClient.SearchDnsSuffix}/indexers/{indexerName}?api-version=2020-06-30";
                var content = new StringContent(jsonIndexerDefinition, Encoding.UTF8, "application/json");
                var response = await serviceClient.HttpClient.PutAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: '{e.Message}' ");
                throw;
            }

        }

        #endregion

        #region Delete
        public async Task DeleteDataSourceAsync(string dataSourceName)
        {
            Console.WriteLine($"Deleting '{dataSourceName}' data source...");
            await serviceClient.DataSources.DeleteAsync(dataSourceName);
        }

        public async Task DeleteIndexAsync(string indexName)
        {
            Console.WriteLine($"Deleting '{indexName}' index...");
            await serviceClient.Indexes.DeleteAsync(indexName);
        }

        public async Task DeleteSkillsetAsync(string skillsetName)
        {
            Console.WriteLine($"Deleting '{skillsetName}' skillset...");
            var uri = $"https://{serviceClient.SearchServiceName}.{serviceClient.SearchDnsSuffix}/skillsets/{skillsetName}?api-version=2020-06-30";
            var response = await serviceClient.HttpClient.DeleteAsync(uri);
        }

        public async Task DeleteIndexerAsync(string indexerName)
        {
            Console.WriteLine($"Deleting '{indexerName}' indexer...");
            await serviceClient.Indexers.DeleteAsync(indexerName);
        }
        #endregion

    

    }
}

