using ASM_C6.Components.Pages.CustomerPage;
using ASM_C6.Model;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Text;

namespace ASM_C6.Components.Pages.LoginPage
{
    public partial class CusInfomation : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private bool _isRenderCompleted;
        public string apiUrl;

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private ASM_C6.Model.CustomerInfomation customer = new ASM_C6.Model.CustomerInfomation();
        private List<ASM_C6.Model.CustomerInfomation> listcus = new List<Model.CustomerInfomation>();
        public string email;
        private ASM_C6.Model.Customer temp = new Model.Customer();
        public int id;


        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
                _isRenderCompleted = true;
                temp = await sessionStorageService.GetItemAsModelAsync<Model.Customer>("Login");
                email = temp.Email;
                await LoadDb();
                StateHasChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
        }
        private async Task EditCus()
        {
            var apiUrl = $"{_apiSetting.BaseUrl}/customerinformations/{id}";
            StringContent content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            var response = await HttpClient.PutAsync(apiUrl,content);
            if (response.IsSuccessStatusCode)
            {
                await jmodule.InvokeVoidAsync("show", "Your information has been changed");
                NavigationManager.NavigateTo("/cusinformation", true);
            }
            else
            {
                await jmodule.InvokeVoidAsync("show", "Fail to update your information. Please contact to admin");
                NavigationManager.NavigateTo("/cusinformation", true);
            }
        }
        private async Task LoadDb()
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/customerinformations/relatedinformation/{email}";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    listcus = await response.Content.ReadFromJsonAsync<List<ASM_C6.Model.CustomerInfomation>>();
                    customer = listcus.FirstOrDefault(x => x.CustomerEmail == email);
                    id = customer.CInforId;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                    NavigationManager.NavigateTo("/cusinformation", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/cusinformation", true);
            }
        }
    }
}
