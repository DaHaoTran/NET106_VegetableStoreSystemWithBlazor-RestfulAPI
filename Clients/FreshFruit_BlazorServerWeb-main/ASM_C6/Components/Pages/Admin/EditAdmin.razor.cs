using ASM_C6.Model;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ASM_C6.Components.Pages.Admin
{
    public partial class EditAdmin
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private IEnumerable<ASM_C6.Model.Admin> admins { get; set; }
        private ASM_C6.Model.Admin admin = new ASM_C6.Model.Admin();
        private bool _isRenderCompleted;
        public string apiUrl;
        public string repw;
        private ASM_C6.Model.Admin upadmin = new ASM_C6.Model.Admin();

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
            await LoadAdminData();
        }

        private async Task LoadAdminData()
        {
            var apiUrl = $"{_apiSetting.BaseUrl}/admins/{id}";
            var response = await HttpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                admin = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Admin>();
            }
        }

        private async Task EditAdm()
        {
            try
            {
                string message = string.Empty;
                if (repw == null || repw == admin.Password)
                {
                    var apiUrl = $"{_apiSetting.BaseUrl}/admins/{admin.AdminCode}";
                    var response = await HttpClient.PutAsJsonAsync(apiUrl, admin);
                    if (response.IsSuccessStatusCode)
                    {
                        await jmodule.InvokeVoidAsync("show", "Change password successfully.");
                        NavigationManager.NavigateTo("/admin/admsmn", true);
                    }
                    else
                    {
                        await jmodule.InvokeVoidAsync("show", "Change password is failed.");
                        NavigationManager.NavigateTo($"/admin/editadm/{id}", true);
                    }
                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Passwords do not match");
                    return;
                }

                
            }
            catch (Exception ex)
            {
                NavigationManager.NavigateTo("/admin/admwelcome", true);
                if (jmodule != null)
                {
                    await jmodule.InvokeVoidAsync("show", "An error occurred: " + ex.Message);
                }
            }
        }
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/admin/adsmn", true);
        }
    }
}
