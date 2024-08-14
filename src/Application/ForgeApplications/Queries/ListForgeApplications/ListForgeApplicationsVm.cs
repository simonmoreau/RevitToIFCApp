using Autodesk.Forge.DesignAutomation.Model;

namespace Application.ForgeApplications.Queries.ListForgeApplications
{
    public class ListForgeApplicationsVm
    {
        public List<string> Engines { get; set; } = new List<string>();
        public List<AppBundle> AppBundles { get; set; } = new List<AppBundle>();
        public List<Activity> Activities { get; set; } = new List<Activity>();

    }

    public class AppBundleView
    {
        public string Id { get; set; }
        public readonly List<Alias> Aliases = new List<Alias>();
        public readonly List<int> Versions = new List<int>();
    }
}
