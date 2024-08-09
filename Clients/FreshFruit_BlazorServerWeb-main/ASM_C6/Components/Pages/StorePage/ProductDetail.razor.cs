using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ASM_C6.Components.Pages.StorePage
{
    public partial class ProductDetail : ComponentBase
    {
        [Parameter]
        public Guid id { get; set; }
        private ASM_C6.Model.Food food = new ASM_C6.Model.Food();
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private List<ASM_C6.Model.Food> paginatedAdmins { get; set; }
        private bool _isRenderCompleted;
        public string apiUrl;

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private IEnumerable<FoodCategory> foodCategories { get; set; }
        private IEnumerable<ASM_C6.Model.Food> foods = new List<ASM_C6.Model.Food>();
        private IEnumerable<ASM_C6.Model.Food> topsale = new List<ASM_C6.Model.Food>();


        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            await LoadDb();
            await LoadDbList();
            await LoadTopSale();
        }

       
        private async Task LoadDb()
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    food = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
                    string rootPath = @"wwwroot\";
                        int rootIndex = food.Image.IndexOf(rootPath);
                        string relativePath = food.Image.Substring(rootIndex + rootPath.Length - 1).Replace("\\", "/");
                        food.Image = relativePath;
                    
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                    NavigationManager.NavigateTo("/foodsmn", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/foodsmn", true);
            }
        }
        private string GetCategoryName(Guid categoryCode)
        {
            var category = foodCategories?.FirstOrDefault(c => c.FCategoryCode == categoryCode);
            return category != null ? category.CategoryName : "Unknown";
        }
        private async Task LoadTopSale()
        {
            topsale = foods
                .OrderByDescending(x => x.Sold)
                .Take(6)
                .ToList();
        }
        private async Task LoadDbList()
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    foods = await response.Content.ReadFromJsonAsync<IEnumerable<Food>>();
                    string rootPath = @"wwwroot\";

                    foreach (var item in foods)
                    {
                        int rootIndex = item.Image.IndexOf(rootPath);
                        if (rootIndex != -1)
                        {
                            string relativePath = item.Image.Substring(rootIndex + rootPath.Length).Replace("\\", "/");
                            item.Image = relativePath;
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                    NavigationManager.NavigateTo("/", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/", true);
            }
        }

    }
}
