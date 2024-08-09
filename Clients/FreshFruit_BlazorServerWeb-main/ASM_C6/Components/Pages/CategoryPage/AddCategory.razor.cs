using ASM_C6.Components.Pages.Admin;
using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Text;

namespace ASM_C6.Components.Pages.CategoryPage
{
    public partial class AddCategory : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference jmodule;

        private ApiSetting _apiSetting;
        private ASM_C6.Model.FoodCategory category = new ASM_C6.Model.FoodCategory();
        private bool _isRenderCompleted;
        public string apiUrl;
        private IEnumerable<FoodCategory> categories { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
            }
        }

        private async Task CreateCate()
        {
            if (!_isRenderCompleted)
                return;

            try
            {
                _apiSetting = ApiSettingOptions.Value;
                category.FCategoryCode = Guid.NewGuid();
                apiUrl = $"{_apiSetting.BaseUrl}/categories";

                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    categories = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.FoodCategory>>();
                    while (categories.Any(e => e.FCategoryCode == category.FCategoryCode))
                    {
                        category.FCategoryCode = Guid.NewGuid();
                    }
                }

                StringContent content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                var response1 = await HttpClient.PostAsync(apiUrl, content);

                if (response1.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Add category successfully.");
                    NavigationManager.NavigateTo("/admin/categoriesmn", true);
                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Add category failed.");
                    NavigationManager.NavigateTo("/admin/admwelcome", true);
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", $"{ex.Message}");
                NavigationManager.NavigateTo("/admin/admwelcome", true);
            }
        }
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/admin/categoriesmn", true);
        }
    }
}
