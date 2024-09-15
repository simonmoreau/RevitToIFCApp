using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    internal class AdditionalParameterRetriever
    {
        internal static string GetConversionCreditsAttributeName(string b2cExtensionAppClientId)
        {
            string attributeName = "ConversionCredits";

            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new System.ArgumentException("Parameter cannot be null", nameof(attributeName));
            }

            return $"extension_{b2cExtensionAppClientId}_{attributeName}";
        }
    }
}
