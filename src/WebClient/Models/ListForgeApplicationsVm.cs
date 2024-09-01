using Autodesk.Forge.DesignAutomation.Model;

namespace WebClient.Models
{
    public class ListForgeApplicationsVm
    {
        public List<string> Engines { get; set; } = new List<string>();
        public List<AppBundle> AppBundles { get; set; } = new List<AppBundle>();
        public List<Activity> Activities { get; set; } = new List<Activity>();

    }
}
