using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageApp
{
    class BlobManager
    {
        IConfiguration configuration;
        CloudStorageAccount storageAccount;
        CloudBlobClient blobClient;
        CloudBlobContainer container;

        public BlobManager()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("local.settings.json");

            configuration = builder.Build();
            storageAccount = CloudStorageAccount.Parse(configuration["StorageAccountConnectionString"]);
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference(configuration["ContainerName"]);
        }
      
        public async Task<String> InitContainer()
        {
            bool created = await container.CreateIfNotExistsAsync();
            return(created ? "Created the Blob container" : "Blob container already exists.");
        }

        public async Task<IEnumerable<string>> BlobList()
        {
            List<string> names = new List<string>();
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;
            do
            {
                resultSegment = await container.ListBlobsSegmentedAsync(continuationToken);

                // Get the name of each blob.
                names.AddRange(resultSegment.Results.OfType<ICloudBlob>().Select(b => b.Name));

                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

            return names;
        }

        public Task Save(Stream fileStream, string name)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            return blockBlob.UploadFromStreamAsync(fileStream);
        }

        public Task<Stream> Load(string name)
        {
            return container.GetBlobReference(name).OpenReadAsync();
        }

    }

    
}
