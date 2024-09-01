using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Forge.Core;
using MediatR;

namespace Application.ForgeApplications.Queries.ListForgeApplications
{
    public class ListForgeApplicationsQueryHandler : IRequestHandler<ListForgeApplicationsQuery, ListForgeApplicationsVm>
    {
        private readonly IActivitiesApi _activitiesApi;
        private readonly IAppBundlesApi _appBundlesApi;
        private readonly IEnginesApi _enginesApi;

        public ListForgeApplicationsQueryHandler(IActivitiesApi activitiesApi, IAppBundlesApi appBundlesApi, IEnginesApi enginesApi)
        {
            _activitiesApi = activitiesApi;
            _appBundlesApi = appBundlesApi;
            _enginesApi = enginesApi;
        }

        public async Task<ListForgeApplicationsVm> Handle(ListForgeApplicationsQuery request, CancellationToken cancellationToken)
        {
            ListForgeApplicationsVm listForgeApplicationsVm = new ListForgeApplicationsVm();


            ApiResponse<Page<string>> activitesIds = await _activitiesApi.GetActivitiesAsync();

            List<Task<Activity>> activityTasks = new List<Task<Activity>>();

            foreach (string activityId in activitesIds.Content.Data)
            {
                activityTasks.Add(GetActivity(activityId));
            }

            Activity[] activities = await Task.WhenAll(activityTasks);
            listForgeApplicationsVm.Activities.AddRange(activities.OrderBy(a => a.Description));

            ApiResponse<Page<string>> enginesResponse = await _enginesApi.GetEnginesAsync();
            listForgeApplicationsVm.Engines.AddRange(enginesResponse.Content.Data.Order());

            ApiResponse<Page<string>> appBundlesIds = await _appBundlesApi.GetAppBundlesAsync();

            List<Task<AppBundle>> appBundlesTasks = new List<Task<AppBundle>>();

            foreach (string appBundleId in appBundlesIds.Content.Data)
            {
                appBundlesTasks.Add(GetAppBundle(appBundleId));
            }

            AppBundle[] appBundles = await Task.WhenAll(appBundlesTasks);
            listForgeApplicationsVm.AppBundles.AddRange(appBundles.OrderBy(a => a.Description));

            return listForgeApplicationsVm;
        }

        private async Task<Activity> GetActivity(string activityId)
        {
            try
            {
                ApiResponse<Activity> activityResponse = await _activitiesApi.GetActivityAsync(activityId);
                return activityResponse.Content;
            }
            catch (Exception ex)
            {
                return new Activity() { Id = activityId, Description = ex.Message };
            }
        }

        private async Task<AppBundle> GetAppBundle(string appBundleId)
        {
            try
            {
                ApiResponse<AppBundle> appBundleResponse = await _appBundlesApi.GetAppBundleAsync(appBundleId);
                return appBundleResponse.Content;
            }
            catch (Exception ex)
            {
                return new AppBundle() { Id  = appBundleId , Description = ex.Message};
            }

        }

        private async Task<AppBundleView> CreateAppBundleView(string appBundleId)
        {
            AppBundleView appBundleView = new AppBundleView();
            appBundleView.Id = appBundleId;

            ApiResponse<Page<Alias>> aliasesResponse = await _appBundlesApi.GetAppBundleAliasesAsync(appBundleId);

            appBundleView.Aliases.AddRange(aliasesResponse.Content.Data);

            ApiResponse<Page<int>> versionsResponses = await _appBundlesApi.GetAppBundleVersionsAsync(appBundleId);
            appBundleView.Versions.AddRange(versionsResponses.Content.Data);

            return appBundleView;
        }

    }
}
