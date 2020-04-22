using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessIMC.Models;
using ServerlessIMC.Services;

namespace ServerlessIMC
{
    public static class FunctionIMC
    {
        [FunctionName("FunctionIMC")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function procesando petición.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data != null)
            {
                double peso = Convert.ToDouble(data.peso);
                double altura = Convert.ToDouble(data.altura);
                double result = (peso / (altura * altura));
                string nombre = data.nombre;
                log.LogInformation($"Nombre: {nombre}");

                // Insert en Cosmos DB
                var cosmosDBService = new CosmosServiceAzure();

                var item = new Item()
                {
                    Id = $"{Guid.NewGuid()}",
                    Nombre = nombre,
                    IMC = result
                };

                await cosmosDBService.GetStartedDemoAsync(item);

                return (ActionResult)new OkObjectResult(new { Result = Math.Truncate(result) });
            }
            else
                return new BadRequestObjectResult("Por favor envie un nombre, altura en metros y peso en kilogramos en el cuerpo de la solicitud");
        }
    }
}
