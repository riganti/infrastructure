using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Riganti.Utils.Infrastructure.Services.Storage;

namespace Riganti.Utils.Infrastructure.Services.Azure.Storage
{
    public class AzureBlobStorageFolder : IStorageFolder
    {
        private readonly CloudBlobContainer containerReference;

        private AzureBlobStorageFolder(CloudBlobContainer containerReference)
        {
            this.containerReference = containerReference;
        }


        public static AzureBlobStorageFolder UseExistingContainer(CloudBlobContainer containerReference)
        {
            return new AzureBlobStorageFolder(containerReference);
        }

        public static AzureBlobStorageFolder UseExistingContainer(CloudStorageAccount storageAccount, string containerName)
        {
            var containerReference = storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
            return UseExistingContainer(containerReference);
        }

        public static AzureBlobStorageFolder CreateContainerIfNotExists(CloudBlobContainer containerReference, BlobContainerPublicAccessType blobContainerPublicAccessType)
        {
            containerReference.CreateIfNotExists(blobContainerPublicAccessType);
            return new AzureBlobStorageFolder(containerReference);
        }

        public static AzureBlobStorageFolder CreateContainerIfNotExists(CloudStorageAccount storageAccount, string containerName, BlobContainerPublicAccessType blobContainerPublicAccessType)
        {
            var containerReference = storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
            return CreateContainerIfNotExists(containerReference, blobContainerPublicAccessType);
        }



        public Task<bool> FileExists(string name)
        {
            return containerReference.GetBlockBlobReference(name).ExistsAsync();
        }

        public Task SaveFile(string name, Stream stream)
        {
            return containerReference.GetBlockBlobReference(name).UploadFromStreamAsync(stream);
        }

        public Task<Stream> LoadFile(string name)
        {
            return containerReference.GetBlockBlobReference(name).OpenReadAsync();
        }
    }
}
