using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreConf2020.Functions.Services
{
    public class BlobStorageService
    {
        private CloudStorageAccount cloudStorageAccount;
        public BlobStorageService(string blobStorageConnectionString)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
        }

        public async Task<string> UploadFile(string containerReference, string filepath, string contentType)
        {
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerReference);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(Path.GetFileName(filepath));
            cloudBlockBlob.Properties.ContentType = contentType;

            await cloudBlockBlob.UploadFromFileAsync(filepath);
            return cloudBlockBlob.Uri.AbsoluteUri;
        }


    }
}
