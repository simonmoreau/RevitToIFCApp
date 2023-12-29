using Domain.Entities;

namespace Application.Sites.Queries.GetSiteList
{
    public class SiteListVm
    {
        public IList<Site>? Sites { get; set; }
    }
}
