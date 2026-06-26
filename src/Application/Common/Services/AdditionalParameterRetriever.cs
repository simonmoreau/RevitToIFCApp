namespace Application.Common.Services
{
    internal class AdditionalParameterRetriever
    {
        internal static string GetConversionCreditsAttributeName(string b2cExtensionAppClientId)
        {
            string attributeName = "ConversionCredits";

            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(attributeName));
            }

            return $"extension_{b2cExtensionAppClientId}_{attributeName}";
        }
    }
}
