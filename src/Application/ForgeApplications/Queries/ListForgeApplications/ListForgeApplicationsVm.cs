using Autodesk.Forge.DesignAutomation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
