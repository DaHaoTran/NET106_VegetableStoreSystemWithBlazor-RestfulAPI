using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ASM_C6.Model;
using Microsoft.Extensions.Options;

namespace ASM_C6.Components.Pages.Admin
{
    public partial class AdminMn
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private IEnumerable<ASM_C6.Model.Admin> admins { get; set; }
        private List<ASM_C6.Model.Admin> paginatedAdmins { get; set; }
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
                apiUrl = $"{_apiSetting.BaseUrl}/admins";

                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    admins = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.Admin>>();
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
                if (_isRenderCompleted) {
                    await jmodule.InvokeVoidAsync("show", $"Error loading data: {ex.Message}");
                    NavigationManager.NavigateTo("/admin/admwelcome", true);
                }
            }
        }

        private void UpdatePaginatedAdmins()
        {
            totalPages = (int)Math.Ceiling((double)admins.Count() / pageSize);
            paginatedAdmins = admins.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
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
                await jmodule.InvokeVoidAsync("show", "An unexpected error occurred.");
            }
        }

        private async Task DeleteAdm(Guid id)
        {
            try
            {
                string apiUrl = $"{_apiSetting.BaseUrl}/admins/{id}";

                var response = await HttpClient.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Deleted successfully");
                    admins = admins.Where(a => a.AdminCode != id).ToList();
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
