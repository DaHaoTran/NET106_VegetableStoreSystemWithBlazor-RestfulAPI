using ASM_C6.Components.Pages.Admin;
using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.IO;
using System.Net.Http.Json;

namespace ASM_C6.Components.Pages.FoodPage
{
    public partial class FoodsMn : ComponentBase
    {
        private IEnumerable<ASM_C6.Model.Food> foods { get; set; }
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
        private int currentPage = 1;
        private int pageSize = 4;
        private int totalPages;
        private IEnumerable<FoodCategory> foodCategories { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            await LoadDb();
            await LoadCate();
        }

        private async Task LoadCate()
        {
            try
            {
                var cateUrl = $"{_apiSetting.BaseUrl}/categories";
                var caterespone = await HttpClient.GetAsync(cateUrl);
                if (caterespone.IsSuccessStatusCode)
                {
                    foodCategories = await caterespone.Content.ReadFromJsonAsync<IEnumerable<FoodCategory>>();
                }
                else
                {
                    var errorContent = await caterespone.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load categories. Status Code: {caterespone.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task LoadDb()
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    foods = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.Food>>();
                    string rootPath = @"wwwroot\";

                    foreach (var item in foods)
                    {
                        int rootIndex = item.Image.IndexOf(rootPath);
                        string relativePath = item.Image.Substring(rootIndex + rootPath.Length - 1).Replace("\\", "/");
                        item.Image = relativePath;
                    }
                    UpdatePaginatedAdmins();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                    NavigationManager.NavigateTo("/admin/foodsmn", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/admin/foodsmn", true);
            }
        }

        private string GetCategoryName(Guid categoryCode)
        {
            var category = foodCategories?.FirstOrDefault(c => c.FCategoryCode == categoryCode);
            return category != null ? category.CategoryName : "Unknown";
        }

        private void UpdatePaginatedAdmins()
        {
            totalPages = (int)Math.Ceiling((double)foods.Count() / pageSize);
            paginatedAdmins = foods.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

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
                    await DeleteFood(id);
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

        private async Task DeleteFood(Guid id)
        {
            try
            {
                ASM_C6.Model.Food food = new Food();
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";

                // Lấy thông tin thực phẩm từ API
                var response1 = await HttpClient.GetAsync(apiUrl);
                if (response1.IsSuccessStatusCode)
                {
                    food = await response1.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
                    string rootPath = food.Image;
                    if (File.Exists(rootPath))
                    {
                        File.Delete(rootPath);
                    }
                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Failed to delete root image.");
                    return;
                }

                // Xóa thực phẩm từ API
                var response = await HttpClient.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    // Xóa tệp ảnh nếu tồn tại
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", food.Image.Replace("/", "\\"));
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    await jmodule.InvokeVoidAsync("show", "Food deleted successfully.");
                    foods = foods.Where(f => f.FoodCode != id).ToList();
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
            }
        }
    }
}

