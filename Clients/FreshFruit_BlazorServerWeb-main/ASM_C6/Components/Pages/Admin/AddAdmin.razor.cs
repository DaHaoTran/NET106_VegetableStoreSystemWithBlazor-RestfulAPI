using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Microsoft.JSInterop.Implementation;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Microsoft.Extensions.Options;

namespace ASM_C6.Components.Pages.Admin
{
    public partial class AddAdmin : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference jmodule;

        private ApiSetting _apiSetting;
        private ASM_C6.Model.Admin admin = new Model.Admin();
        private bool _isRenderCompleted;
        public string apiUrl;
        public string repw;

        private IEnumerable<ASM_C6.Model.Admin> admins { get; set; }


        private async Task CreateAdmin()
        {
            if (!_isRenderCompleted)
                return;
            try
            {
                _apiSetting = ApiSettingOptions.Value;
                if (admin.Password != repw)
                {
                    await jmodule.InvokeVoidAsync("show", "Passwords do not match");
                    return;
                }
                admin.AdminCode = Guid.NewGuid();
                apiUrl = $"{_apiSetting.BaseUrl}/admins";

                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    admins = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.Admin>>();
                    while (admins.Any(e => e.AdminCode == admin.AdminCode))
                    {
                        admin.AdminCode = Guid.NewGuid();
                    }
                }

                StringContent content = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
                var response1 = await HttpClient.PostAsync(apiUrl, content);

                if (response1.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Add admin successfully.");
                    NavigationManager.NavigateTo("/adm/adsmn", true);

                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Add admin failed.");
                    NavigationManager.NavigateTo("/admin/admwelcome", true);
                }
            }
            catch
            {
                await jmodule.InvokeVoidAsync("show", "Add admin failed.");
                NavigationManager.NavigateTo("/admin/admwelcome", true);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
            }
        }
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/admin/admsmn", true);
        }
    }
}
