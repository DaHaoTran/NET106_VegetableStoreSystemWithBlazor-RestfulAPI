using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ASM_C6.Components.Pages.ComboDetail
{
    public partial class ComboDetailsMn : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private IEnumerable<ASM_C6.Model.ComboDetail> combodetails { get; set; }
        private List<ASM_C6.Model.ComboDetail> paginatedAdmins { get; set; }
        private IEnumerable<Food> foods { get; set; }
        private IEnumerable<Combo> combos { get; set; }
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
            await LoadCombo();
            await LoadFood();
        }

        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");

            }
        }
        private async Task LoadFood()
        {
            try
            {
                apiUrl = $"{_apiSetting.BaseUrl}/foods";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    foods = await response.Content.ReadFromJsonAsync<IEnumerable<Food>>();
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                Console.WriteLine(ex.Message);
                NavigationManager.NavigateTo("/admin/admwelcome", true);
            }
        }
        private async Task LoadCombo()
        {
            try
            {
                apiUrl = $"{_apiSetting.BaseUrl}/combos";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    combos = await response.Content.ReadFromJsonAsync<IEnumerable<Combo>>();
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                Console.WriteLine(ex.Message);
                NavigationManager.NavigateTo("/admin/admwelcome", true);
            }
        }

        private async Task LoadDb()
        {
            try
            {
                apiUrl = $"{_apiSetting.BaseUrl}/combodetails";

                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    combodetails = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.ComboDetail>>();
                    UpdatePaginatedAdmins();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
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
         private string GetComboName(Guid comboCode)
        {
            var combo = combos?.FirstOrDefault(c => c.ComboCode == comboCode);
            return combo != null ? combo.ComboName : "Unknown";
        }
        private string GetFoodName(Guid foodCode)
        {
            var food = foods?.FirstOrDefault(c => c.FoodCode == foodCode);
            return food != null ? food.FoodName : "Unknown";
        }
        private void UpdatePaginatedAdmins()
        {
            totalPages = (int)Math.Ceiling((double)combodetails.Count() / pageSize);
            paginatedAdmins = combodetails.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
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

        private async Task ConfirmDelete(int id)
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
                await jmodule.InvokeVoidAsync("show", "An unexpected error occurred." + ex.Message);
            }
        }

        private async Task DeleteAdm(int id)
        {
            try
            {
                string apiUrl = $"{_apiSetting.BaseUrl}/combodetails/{id}";
                Console.WriteLine(apiUrl);
                var response = await HttpClient.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Delete data successfully");
                    combodetails = combodetails.Where(a => a.Id != id).ToList();
                    UpdatePaginatedAdmins();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", $"Failed to delete data. Error: {errorContent}");
                    Console.WriteLine($"Error deleting admin with ID {id}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "Failed to delete data");
            }
        }
    }
}
