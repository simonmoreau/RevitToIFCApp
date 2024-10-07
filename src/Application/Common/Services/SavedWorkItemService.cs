using Autodesk.Forge.DesignAutomation.Model;
using Azure.Data.Tables;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public class SavedWorkItemService : ISavedWorkItemService
    {

        private readonly TableServiceClient _tableServiceClient;
        private const string _partitionKey = "workItems";

        public SavedWorkItemService(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        public async Task CreateSavedWorkItemStatus(WorkItemStatus workItemStatus, string userId, string revitVersion)
        {
            SavedWorkItem savedWorkItem = new SavedWorkItem();
            savedWorkItem.PartitionKey = _partitionKey;
            savedWorkItem.RowKey = workItemStatus.Id;
            savedWorkItem.UserId = userId;
            savedWorkItem.Version = revitVersion;
            savedWorkItem.Created = DateTime.UtcNow;
            savedWorkItem.LastModified = DateTime.UtcNow;
            savedWorkItem.UpdateStatus(workItemStatus);

            TableClient tableClient = _tableServiceClient.GetTableClient(_partitionKey);
            tableClient.CreateIfNotExists();

            Azure.Response response = await tableClient.AddEntityAsync<SavedWorkItem>(savedWorkItem);

            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task UpdateSavedWorkItemStatus(WorkItemStatus workItemStatus)
        {
            TableClient tableClient = _tableServiceClient.GetTableClient(_partitionKey);
            tableClient.CreateIfNotExists();

            SavedWorkItem savedWorkItem = await tableClient.GetEntityAsync<SavedWorkItem>(_partitionKey, workItemStatus.Id);
            savedWorkItem.UpdateStatus(workItemStatus);
            savedWorkItem.LastModified = DateTime.UtcNow;

            Azure.Response response = await tableClient.UpdateEntityAsync(savedWorkItem, Azure.ETag.All );

            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
