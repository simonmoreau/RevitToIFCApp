using Autodesk.Forge.DesignAutomation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.WorkItems.Commands.UpdateWorkItemStatus
{

    public class WorkItemStatusDTO
    {
        /// <summary>
        /// The current status of the workitem.
        /// </summary>
        /// <value>The current status of the workitem.</value>
        public string? Status { get; set; }

        /// <summary>
        /// The current status of the workitem.
        /// </summary>
        /// <value>The current status of the workitem.</value>
        public string? Progress { get; set; }

        /// <summary>
        /// The detailed report about the workitem, report url is valid for 1 hours from first receiving it.
        /// </summary>
        /// <value>The detailed report about the workitem, report url is valid for 1 hours from first receiving it.</value>
        public string? ReportUrl { get; set; }

        /// <summary>
        /// The debug information for the workitem, the url is valid for 1 hours from first receiving it.
        /// </summary>
        /// <value>The debug information for the workitem, the url is valid for 1 hours from first receiving it.</value>
        public Uri? DebugInfoUrl { get; private set; }

        /// <summary>
        /// Basic statistics about workitem processing.
        /// </summary>
        /// <value>Basic statistics about workitem processing.</value>
        public Statistics? Stats { get; set; }

        public string? Id { get; set; }

        public static T ToEnum<T>(string str)
        {
            Type enumType = typeof(T);
            foreach (string enumName in Enum.GetNames(enumType))
            {
                EnumMemberAttribute enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(enumName).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, enumName);
            }
            //throw exception or whatever handling you want or
            return default(T);
        }
    }
}
