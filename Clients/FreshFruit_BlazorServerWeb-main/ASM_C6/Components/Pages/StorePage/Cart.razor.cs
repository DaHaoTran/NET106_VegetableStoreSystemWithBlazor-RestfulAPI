using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ASM_C6.Components.Pages.StorePage
{
    public partial class Cart : ComponentBase
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
        public List<OrderItem> items = new List<OrderItem>();
        public Food food = new Food();
        private Dictionary<Guid, (string ImageUrl, string FoodName)> foodDetails = new Dictionary<Guid, (string, string)>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
                items = await sessionStorageService.GetItemListAsync<OrderItem>("cart");
                await LoadFoodDetails();
                StateHasChanged(); // Yêu cầu render lại với dữ liệu mới
            }
        }

        private async Task LoadFoodDetails()
        {
            foreach (var item in items)
            {
                var imageUrl = await GetImgByFoodCode(item.FoodCode);
                var foodName = await GetName(item.FoodCode);
                foodDetails[item.FoodCode] = (imageUrl, foodName);
            }
        }
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
        }
        private async Task<string> GetImgByFoodCode(Guid id)
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";
                var response = await HttpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var food = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
                    string rootPath = @"wwwroot\";
                    int rootIndex = food.Image.IndexOf(rootPath);

                    // Kiểm tra xem rootPath có tồn tại trong chuỗi không
                    if (rootIndex >= 0)
                    {
                        // Tìm thấy rootPath, chuyển đổi thành đường dẫn tương đối
                        string relativePath = food.Image.Substring(rootIndex + rootPath.Length).Replace("\\", "/");
                        food.Image = relativePath;
                    }
                    Console.WriteLine(food.Image);
                    return food.Image;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data." + errorContent);
                    NavigationManager.NavigateTo("/cart", true);
                    return null; // Hoặc trả về chuỗi rỗng nếu không cần thiết
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/cart", true);
                return null; // Hoặc trả về chuỗi rỗng nếu không cần thiết
            }
        }
        private async Task UpdateQuantity()
        {
            await sessionStorageService.SaveItemAsModelAsync<List<OrderItem>>("cart", items);
            StateHasChanged();
        }
        private int SubTotal => items.Sum(x => x.Quantity * x.UnitPrice);
        private int Shipping = 3;
        private int Total => SubTotal + Shipping;

        private async Task<string> GetName(Guid id)
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";
                var response = await HttpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var food = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
                    Console.WriteLine(food.FoodName);
                    return food.FoodName;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data." + errorContent);
                    NavigationManager.NavigateTo("/cart", true);
                    return null; // Hoặc trả về chuỗi rỗng nếu không cần thiết
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/cart", true);
                return null; // Hoặc trả về chuỗi rỗng nếu không cần thiết
            }
        }
        private async Task ConfirmDelete(Guid id)
        {
            try
            {
                bool confirmed = await jmodule.InvokeAsync<bool>("showConfirmAlert", "Are you sure you want to delete?");
                if (confirmed)
                {
                    await RemoveItem(id);
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
        private async Task RemoveItem(Guid id)
        {
            var delitem = items.FirstOrDefault(x=>x.FoodCode == id);
            items.Remove(delitem);
            await sessionStorageService.SaveItemAsModelAsync<List<OrderItem>>("cart", items);
            StateHasChanged();
        }
        
    }
}
