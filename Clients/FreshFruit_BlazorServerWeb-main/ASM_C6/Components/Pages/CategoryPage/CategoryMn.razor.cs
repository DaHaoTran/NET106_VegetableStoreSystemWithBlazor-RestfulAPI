using ASM_C6.Components.Pages.Admin;
using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ASM_C6.Components.Pages.CategoryPage
{
    public partial class CategoryMn :ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private IEnumerable<ASM_C6.Model.FoodCategory> categories { get; set; }
        private List<ASM_C6.Model.FoodCategory> paginatedAdmins { get; set; }
        private bool _isRenderCompleted;
        public string apiUrl;

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private int currentPage = 1;
        private int pageSize = 4;
        private int totalPages;

        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            await LoadDb();
        }

        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");

            }
        }
        private async Task LoadDb()
        {
            try
            {
                apiUrl = $"{_apiSetting.BaseUrl}/categories";

                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    categories = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.FoodCategory>>();
                    UpdatePaginatedAdmins();
                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data");
                    NavigationManager.NavigateTo("/admin/admwelcome", true);
                }
            }

            catch (Exception ex)
            {
                if (_isRenderCompleted)
                {
                    await jmodule.InvokeVoidAsync("show", $"Error loading data: {ex.Message}");
                    NavigationManager.NavigateTo("/admin/admwelcome", true);
                }
            }
        }
        private void UpdatePaginatedAdmins()
        {
            totalPages = (int)Math.Ceiling((double)categories.Count() / pageSize);
            paginatedAdmins = categories.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                UpdatePaginatedAdmins();
            }
        }

        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePaginatedAdmins();
            }
        }
        private async Task ConfirmDelete(Guid id)
        {
            try
            {
                bool confirmed = await jmodule.InvokeAsync<bool>("showConfirmAlert", "Are you sure you want to delete?");
                if (confirmed)
                {
                    await DeleteCate(id);
                }
            }
            catch (JSException jsEx)
            {
                await jmodule.InvokeVoidAsync("show", "An error occurred while confirming deletion.");
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "An unexpected error occurred."+ex.Message);
            }
        }

        private async Task DeleteCate(Guid id)
        {
            try
            {
                string apiUrl = $"{_apiSetting.BaseUrl}/categories/{id}";

                var response = await HttpClient.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Deleted successfully");
                    categories = categories.Where(a => a.FCategoryCode != id).ToList();
                    UpdatePaginatedAdmins();
                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Failed to delete data");
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "Failed to delete data");
            }
        }

    }
}
