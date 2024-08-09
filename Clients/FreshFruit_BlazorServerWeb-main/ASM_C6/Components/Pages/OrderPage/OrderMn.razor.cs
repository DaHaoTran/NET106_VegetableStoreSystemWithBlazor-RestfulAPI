using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ASM_C6.Components.Pages.OrderPage
{
    public partial class OrderMn : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private List<ASM_C6.Model.Order> paginatedAdmins { get; set; }
        private bool _isRenderCompleted;
        public string apiUrl;
        private List<ASM_C6.Model.Order> orders = new List<Order>();
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

        private async Task LoadDb()
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/orders";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    orders = await response.Content.ReadFromJsonAsync<List<ASM_C6.Model.Order>>();
                   
                    UpdatePaginatedAdmins();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                    NavigationManager.NavigateTo("/adm/admwelcome", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/adm/admwelcome", true);
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

        private async Task ConfirmDelete(Guid id)
        {
            try
            {
                bool confirmed = await jmodule.InvokeAsync<bool>("showConfirmAlert", "Are you sure you want to delete?");
                if (confirmed)
                {
                    await Delete(id);
                }
            }
            catch (JSException jsEx)
            {
                await jmodule.InvokeVoidAsync("show", "An error occurred while confirming deletion." + jsEx.Message);
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "An unexpected error occurred." + ex.Message);
            }
        }

        private async Task Delete(Guid id)
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";

                
                var response = await HttpClient.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    
                    orders = orders.Where(f => f.OrderCode != id).ToList();
                    UpdatePaginatedAdmins();
                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Failed to delete food.");
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", $"Failed to delete food: {ex.Message}");
            }
        }

        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
                await LoadDb();
                StateHasChanged();
            }
        }
    }
}

