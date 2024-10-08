﻿using Application.Common.Interfaces;
using Autodesk.Forge.DesignAutomation.Http;
using Azure.Data.Tables;
using Domain.Entities;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem
{
    public class GetWorkItemQueryHandler : IRequestHandler<GetWorkItemQuery, SavedWorkItem>
    {
        private readonly IAppDbContext _context;
        private readonly TableServiceClient _tableServiceClient;

        public GetWorkItemQueryHandler(IAppDbContext context, TableServiceClient tableServiceClient, IWorkItemsApi workItemsApi)
        {
            _context = context;
            _tableServiceClient = tableServiceClient;
        }

        public async Task<SavedWorkItem> Handle(GetWorkItemQuery request, CancellationToken cancellationToken)
        {
            TableClient tableClient = _tableServiceClient.GetTableClient("workItems");

            // Read a single item from container
            SavedWorkItem workItem = await tableClient.GetEntityAsync<SavedWorkItem>(
                rowKey: "68719518388",
                partitionKey: "workItems"
            );

            return workItem;
        }
    }
}
