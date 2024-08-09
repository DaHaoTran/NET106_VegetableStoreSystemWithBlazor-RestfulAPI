using ASM_C6.Components.Pages.CustomerPage;
using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace ASM_C6.Components.Pages.LoginPage
{
    public partial class ChangePw : ComponentBase
    {
        private ASM_C6.Model.Customer customer = new ASM_C6.Model.Customer();
        private ASM_C6.Model.Customer temp = new ASM_C6.Model.Customer();
        public string repw;
        public string email;
        public string username;
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
        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
            }
        }
        private static string CutStringBeforeAt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string cannot be null or empty", nameof(input));
            }

            int atIndex = input.IndexOf('@');
            if (atIndex == -1)
            {
                throw new ArgumentException("Input string does not contain '@' character", nameof(input));
            }

            return input.Substring(0, atIndex);
        }
        private async Task Changepw()
        {
            if(repw == customer.PassWord)
            {
               
                temp = await sessionStorageService.GetItemAsModelAsync<Model.Customer>("Login");
                email = temp.Email;
                username = CutStringBeforeAt(temp.Email);
                customer.Email = email;
                customer.UserName = username;
                try
                {
                    var apiUrl = $"{_apiSetting.BaseUrl}/customers/{customer.Email}";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                    var response = await HttpClient.PutAsync(apiUrl,content);
                    if (response.IsSuccessStatusCode)
                    {
                        await jmodule.InvokeVoidAsync("show", "Your password has been changed. Please log in again to continue");
                        await sessionStorageService.ClearAllItemsAsync();
                        NavigationManager.NavigateTo("/", true);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                        Console.WriteLine($"Response Content: {errorContent}");
                        await jmodule.InvokeVoidAsync("show", "Fail to change password. Please try later.");
                        NavigationManager.NavigateTo("/cusinformation", true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    await jmodule.InvokeVoidAsync("show", "Fail to change password. Please try later.");
                    NavigationManager.NavigateTo("/cusinformation", true);
                }
            }
        }
    }
}
