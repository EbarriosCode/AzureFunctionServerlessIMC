using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using Microsoft.Azure.Cosmos;
using ServerlessIMC.Models;

namespace ServerlessIMC.Services
{
    public class CosmosServiceAzure
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "<endpoint>";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "<primarykey>";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "pesoidealimc";
        private string containerId = "Items";

        public async Task GetStartedDemoAsync(Item items)
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await this.CreateDatabaseAsync();

            //ADD THIS PART TO YOUR CODE
            await this.CreateContainerAsync();

            //ADD THIS PART TO YOUR CODE
            await this.AddItemsToContainerAsync(items);
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Nombre");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        private async Task AddItemsToContainerAsync(Item items)
        {            
            //var items = new Item
            //{
            //    Id = $"{Guid.NewGuid()}",
            //    Nombre = "Esteban",
            //    IMC = 21
            //};

            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Item> eduardoimc = await this.container.ReadItemAsync<Item>(items.Id, new PartitionKey(items.Nombre));
                Console.WriteLine("Item in database with id: {0} already exists\n", eduardoimc.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<Item> andersenFamilyResponse = await this.container.CreateItemAsync<Item>(items, new PartitionKey(items.Nombre));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
            }
        }
    }
}
