using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Text;

namespace ASM_C6.Components.Pages.AdminLoginPage
{
    public partial class AdminLogin : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private ASM_C6.Model.Admin admin = new Model.Admin();
        private bool _isRenderCompleted;
        public string apiUrl;
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
            }
        }
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
        }
        private List<Model.Customer> listcus = new List<Model.Customer>();
        private async Task CheckLogin()
        {
            try
            {
                
                var apiUrl = $"{_apiSetting.BaseUrl}/admins/login";
                StringContent content = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    bool loginSuccess = JsonConvert.DeserializeObject<bool>(result);
                    Model.Admin temp = new Model.Admin()
                    {
                        Email = admin.Email,
                        Level = admin.Level
                        
                    };
                    if (loginSuccess)
                    {
                        await sessionStorageService.SaveItemAsModelAsync<ASM_C6.Model.Admin>("AdmLogin", temp);
                        await jmodule.InvokeVoidAsync("show", "Login successfully");
                        NavigationManager.NavigateTo("/admin/admwelcome");
                    }
                    else
                    {
                        await jmodule.InvokeVoidAsync("show", "Login failed. Please check your email and password.");
                        return;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to login. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");

                    await jmodule.InvokeVoidAsync("show", "An error occurred. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during login: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "An error occurred. Please try again later.");
            }
        }
       


    }
}
