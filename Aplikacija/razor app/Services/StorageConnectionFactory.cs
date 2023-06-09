﻿using System.Threading.Tasks;
using backend.ViewComponents;
using backend.Services.Interfaces;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace backend.Services
{
    public class StorageConnectionFactory : IStorageConnectionFactory
    {
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;
        private readonly CloudStorageOptionsDTO _storageOptions;

        public StorageConnectionFactory(CloudStorageOptionsDTO storageOptions)
        {
            _storageOptions = storageOptions;

        }
        public async Task<CloudBlobContainer> GetContainer()
        {
            if (_blobContainer != null)
            {
                return _blobContainer;
            }
            else 
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageOptions.ConnectionString);

                _blobClient = storageAccount.CreateCloudBlobClient();
                _blobContainer = _blobClient.GetContainerReference(_storageOptions.ProfilePicsContainer);
                await _blobContainer.CreateIfNotExistsAsync();

                await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                return _blobContainer;
            }
        }
    }
}
