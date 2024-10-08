﻿using Microsoft.AspNetCore.Components;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Components.Admin
{
    public partial class CreateForgeApplication
    {
        [Inject]
        public IDataService _dataService { get; set; }

        ForgeActivityForm forgeActivityForm = new ForgeActivityForm()
        {
            Name = "RVTToIFC",
            AppbundleFile = "",
            Description = "Convert Revit model To IFC",
            Engine = "Autodesk.Revit+2024"
        };

        private void CreateForgeApp()
        {
            _dataService.CreateApplication(forgeActivityForm);
        }
    }
}
