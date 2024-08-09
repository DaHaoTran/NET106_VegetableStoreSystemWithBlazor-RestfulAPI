using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ASM_C6.Components.Pages.ComboPage
{
    public partial class ComboMn : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private IEnumerable<ASM_C6.Model.Combo> combos { get; set; }
        private List<ASM_C6.Model.Combo> paginatedAdmins { get; set; }
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
                apiUrl = $"{_apiSetting.BaseUrl}/combos";

                var response = await HttpClient.GetAsync(apiUrl);
                if(response.IsSuccessStatusCode)
                {
                    combos = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.Combo>>();
                    string rootPath = @"wwwroot\";

                    foreach (var item in combos)
                    {
                        int rootIndex = item.Image.IndexOf(rootPath);
                        string relativePath = item.Image.Substring(rootIndex + rootPath.Length - 1).Replace("\\", "/");
                        item.Image = relativePath;
                    }
                    UpdatePaginatedAdmins();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load comboees. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
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
            totalPages = (int)Math.Ceiling((double)combos.Count() / pageSize);
            paginatedAdmins = combos.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
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
                    await DeleteAdm(id);
                }
            }
            catch (JSException jsEx)
            {
                await jmodule.InvokeVoidAsync("show", "An error occurred while confirming deletion.");
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "An unexpected error occurred." +ex.Message);
            }
        }

        private async Task DeleteAdm(Guid id)
        {
            try
            {
                string apiUrl = $"{_apiSetting.BaseUrl}/combos/{id}";

                var response = await HttpClient.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Deleted successfully");
                    combos = combos.Where(a => a.ComboCode != id).ToList();
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
