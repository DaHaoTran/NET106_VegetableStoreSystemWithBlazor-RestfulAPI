using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using NetcodeHub.Packages.Extensions.SessionStorage;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ASM_C6.Components.Pages.StorePage
{
    public partial class Shop : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private IEnumerable<Food> foods = new List<Food>();
        private IEnumerable<FoodCategory> foodCategories = new List<FoodCategory>();
        private IEnumerable<Food> show = new List<Food>();
        private bool _isRenderCompleted;
        public Guid selectid = Guid.Empty;
        private int currentPage = 1;
        private int pageSize = 4;
        private int totalPages;
        private List<ASM_C6.Model.Food> paginatedAdmins = new List<Food>();
        private List<Food> topsale = new List<Food>();
        public string searchkey;
        public List<ASM_C6.Model.Food> res = new List<ASM_C6.Model.Food>();
        public int price;

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private string activeTabId;
        private string searchQuery;

        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            await LoadCate();
            await LoadDb();
        }

        private async Task LoadCate()
        {
            try
            {
                var cateUrl = $"{_apiSetting.BaseUrl}/categories";
                var response = await HttpClient.GetAsync(cateUrl);
                if (response.IsSuccessStatusCode)
                {
                    foodCategories = await response.Content.ReadFromJsonAsync<IEnumerable<FoodCategory>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load categories. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }
        }
        private async Task LoadSearch()
        {
            if (string.IsNullOrWhiteSpace(searchkey))
            {
                res = []; // Hoặc bạn có thể để res là một danh sách rỗng tùy thuộc vào yêu cầu của bạn
            }
            else
            {
                res = foods
                    .Where(x => x.FoodName != null && x.FoodName.Contains(searchkey, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            StateHasChanged();
        }

        private async Task LoadDb()
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
                    UpdatePaginatedAdmins();
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
        private void UpdatePaginatedAdmins()
        {
            totalPages = (int)Math.Ceiling((double)foods.Count() / pageSize);
            paginatedAdmins = foods.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            StateHasChanged();
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

        private IEnumerable<Food> GetFoodsByCategory(Guid cateid)
        {
            return foods.Where(x => x.FCategoryCode == cateid);
        }
        public List<OrderItem> items = [];
        public OrderItem item = new OrderItem();
        private Food addfood = new Food();
        private async Task AddToCart(Guid id)
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";
                var response = await HttpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var addfood = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
                    if (addfood != null)
                    {
                        string rootPath = @"wwwroot\";
                        int rootIndex = addfood.Image.IndexOf(rootPath);

                        if (rootIndex >= 0)
                        {
                            string relativePath = addfood.Image.Substring(rootIndex + rootPath.Length).Replace("\\", "/");
                            addfood.Image = relativePath;
                        }
                        else
                        {
                            // Handle case where rootPath is not found in addfood.Image
                            addfood.Image = addfood.Image.Replace("\\", "/");
                        }
                    }
                    items = await sessionStorageService.GetItemListAsync<OrderItem>("cart");
                    if (items != null)
                    {
                        bool itemExists = false;

                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].FoodCode == id)
                            {
                                items[i].Quantity++;
                                itemExists = true;
                                break;
                            }
                        }

                        if (!itemExists)
                        {
                            var newItem = new OrderItem()
                            {
                                FoodCode = addfood.FoodCode,
                                UnitPrice = addfood.CurrentPrice,
                                Quantity = 1
                            };
                            items.Add(newItem);
                        }
                        await sessionStorageService.SaveItemAsModelAsync<List<OrderItem>>("cart", items);
                    }

                    else
                    {
                        item.FoodCode = addfood.FoodCode;
                        item.UnitPrice = addfood.CurrentPrice;
                        item.Quantity = 1;
                        await sessionStorageService.AddItemToListAsync<OrderItem>("cart", item);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi và điều hướng đến trang chính
                await jmodule.InvokeVoidAsync("show", "Fail to add product to cart. Please try later." + ex.Message);
                NavigationManager.NavigateTo("/", true);
            }
        }
        private void SearchByPrice()
        {
            if (price > 0)
            {
                res = foods.Where(x => x.CurrentPrice <= price).ToList();
            }
            else
            {
                res = []; // Nếu không có giá trị tìm kiếm, có thể hiển thị tất cả
            }
            StateHasChanged();
        }

        private async Task LoadTopSale()
        {
            topsale = foods
                .OrderByDescending(x => x.Sold)
                .Take(3)
                .ToList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                searchkey = await sessionStorageService.GetItemAsStringAsync("search");
                Console.WriteLine(searchkey);
                await LoadSearch();
                StateHasChanged();
            }
        }
    }
}
