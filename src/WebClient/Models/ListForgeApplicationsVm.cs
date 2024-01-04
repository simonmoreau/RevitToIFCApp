using Autodesk.Forge.DesignAutomation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class ListForgeApplicationsVm
    {
        public List<string> Engines { get; set; } = new List<string>();
        public List<AppBundle> AppBundles { get; set; } = new List<AppBundle>();
        public List<Activity> Activities { get; set; } = new List<Activity>();

    }
}
