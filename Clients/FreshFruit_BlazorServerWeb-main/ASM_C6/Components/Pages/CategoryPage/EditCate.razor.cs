using ASM_C6.Components.Pages.Admin;
using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ASM_C6.Components.Pages.CategoryPage
{
    public partial class EditCate :ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private ASM_C6.Model.FoodCategory category = new ASM_C6.Model.FoodCategory();
        private bool _isRenderCompleted;
        public string apiUrl;

        [Parameter]
        public Guid id { get; set; }
        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
            }
        }
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            await LoadData();
        }
        private async Task LoadData()
        {
            var apiUrl = $"{_apiSetting.BaseUrl}/categories/{id}";
            var response = await HttpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                category = await response.Content.ReadFromJsonAsync<ASM_C6.Model.FoodCategory>();
            }
        }
        private async Task EditCategory()
        {
            try
            {
                    var apiUrl = $"{_apiSetting.BaseUrl}/categories/{category.FCategoryCode}";
                    var response = await HttpClient.PutAsJsonAsync(apiUrl, category);
                    if (response.IsSuccessStatusCode)
                    {
                        await jmodule.InvokeVoidAsync("show", "Change name of category successfully.");
                        NavigationManager.NavigateTo("/admin/categoriesmn", true);
                    }
                    else
                    {
                        await jmodule.InvokeVoidAsync("show", "Change name of categoryfailed.");
                        NavigationManager.NavigateTo($"/admin/editcate/{id}", true);
                    }

            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "An error occurred: " + ex.Message);
                NavigationManager.NavigateTo("/admin/categoriesmn", true);
            }
        }
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/admin/categoriesmn", true);
        }
    }
}
