using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Drawing.Printing;
using System.Reflection;

namespace ASM_C6.Components.Pages.LoginPage
{
    public partial class CustomerHistory : ComponentBase
    {
        private IEnumerable<ASM_C6.Model.Order> orders = new List<ASM_C6.Model.Order>();
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private List<ASM_C6.Model.Order> paginatedAdmins { get; set; }
        private bool _isRenderCompleted;
        public string apiUrl;
        public string email;
        public ASM_C6.Model.Customer temp = new Model.Customer();

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private int currentPage = 1;
        private int pageSize = 4;
        private int totalPages;
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
                await LoadDb();
                StateHasChanged();
            }
        }
        private async Task LoadDb()
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/orders";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    orders = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.Order>>();
                    temp = await sessionStorageService.GetItemAsModelAsync<Model.Customer>("Login");
                    email = temp.Email;
                    orders = orders.Where(x => x.CustomerEmail == email);
                    UpdatePaginatedAdmins();
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

        private void UpdatePaginatedAdmins()
        {
            totalPages = (int)Math.Ceiling((double)orders.Count() / pageSize);
            paginatedAdmins = orders.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
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
    }
}
