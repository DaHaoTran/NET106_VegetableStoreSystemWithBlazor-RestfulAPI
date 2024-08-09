using ASM_C6.Model;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Text;

namespace ASM_C6.Components.Pages.StorePage
{
    public partial class Checkout : ComponentBase
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

        private int SubTotal => items.Sum(x => x.Quantity * x.UnitPrice);
        private int Shipping = 3;
        private int Total => SubTotal + Shipping;
        public ASM_C6.Model.Customer customerlogin = new Model.Customer();

        public CustomerInfomation cusinfo = new CustomerInfomation();
        public string comment;
        public string paymentmethod;
        public class PaymentMethod
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public List<PaymentMethod> status = new List<PaymentMethod>
    {
        new PaymentMethod { Id = 1, Name = "Banking" },
        new PaymentMethod { Id = 2, Name = "Payment Upon Delivery" },
        new PaymentMethod { Id = 3, Name = "Paypal" }
    };
        private void Paymentmethod(ChangeEventArgs e)
        {
            paymentmethod = e.Value.ToString();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
                items = await sessionStorageService.GetItemListAsync<OrderItem>("cart");
                await LoadFoodDetails();
                if (await sessionStorageService.GetItemsCountAsync<Customer>("login")>0)
                {
                    customerlogin = await sessionStorageService.GetItemAsModelAsync<ASM_C6.Model.Customer>("login");
                    if (customerlogin != null)
                    {
                        cusinfo = await GetCusInfo(customerlogin.Email);
                    }
                }
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
                using (var response = await HttpClient.GetAsync(apiUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var food = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
                        string rootPath = @"wwwroot\";

                        if (!string.IsNullOrEmpty(food?.Image))
                        {
                            int rootIndex = food.Image.IndexOf(rootPath);

                            // Kiểm tra xem rootPath có tồn tại trong chuỗi không
                            if (rootIndex >= 0)
                            {
                                // Tìm thấy rootPath, chuyển đổi thành đường dẫn tương đối
                                string relativePath = food.Image.Substring(rootIndex + rootPath.Length).Replace("\\", "/");
                                food.Image = relativePath;
                            }
                        }
                        return food?.Image;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        await jmodule.InvokeVoidAsync("show", "Fail to upload data. " + errorContent);
                        NavigationManager.NavigateTo("/cart", true);
                        return null; // Hoặc trả về chuỗi rỗng nếu không cần thiết
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Task was canceled: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Operation was canceled.");
                NavigationManager.NavigateTo("/cart", true);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/cart", true);
                return null; // Hoặc trả về chuỗi rỗng nếu không cần thiết
            }
        }

        private async Task<string> GetName(Guid id)
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";
                var response = await HttpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var food = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
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
        private async Task<CustomerInfomation> GetCusInfo(string email)
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/customerinfomations/relatedinfomation/{email}";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var findinfo = await response.Content.ReadFromJsonAsync<CustomerInfomation>();
                    return findinfo;
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve customer information. Status Code: {response.StatusCode}");
                    return null;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching customer information: {ex.Message}");
                return null;
            }
            
        }
        ASM_C6.Model.Order neworder = new ASM_C6.Model.Order();
        List<CustomerInfomation> listcus = new List<CustomerInfomation>();
        private async Task SaveOrder()
        {
            try
            {
                
                // Lấy thông tin khách hàng
                var cusUrl = $"{_apiSetting.BaseUrl}/customerinformations";
                var cusresponse = await HttpClient.GetAsync(cusUrl);

                // Kiểm tra phản hồi từ API
                if (!cusresponse.IsSuccessStatusCode)
                {
                    // Xử lý khi phản hồi không thành công
                    var errorMessage = await cusresponse.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get customer information. Status Code: {cusresponse.StatusCode}. Error Message: {errorMessage}");
                }

                // Đọc danh sách khách hàng
                listcus = await cusresponse.Content.ReadFromJsonAsync<List<CustomerInfomation>>();

                // Kiểm tra xem khách hàng đã tồn tại hay chưa
                var temp = listcus.FirstOrDefault(x => x.CustomerEmail == cusinfo.CustomerEmail);
                if (temp == null)
                {
                    // Nếu khách hàng chưa tồn tại, thêm mới
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(cusinfo), Encoding.UTF8, "application/json");
                    var addCustomerResponse = await HttpClient.PostAsync(cusUrl, content1);
                    if (addCustomerResponse.IsSuccessStatusCode)
                    {
                        // Yêu cầu thành công, có thể xử lý phản hồi ở đây nếu cần
                        Console.WriteLine($"Customer email: {cusinfo.CustomerEmail}");
                    }
                    else
                    {
                        // Yêu cầu không thành công, xử lý thông báo lỗi
                        var errorMessage = await addCustomerResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {errorMessage}");
                        // Hoặc bạn có thể throw exception hoặc xử lý lỗi theo cách khác
                        throw new Exception($"Failed to add customer. Status code: {addCustomerResponse.StatusCode}, Error message: {errorMessage}");
                    }

                    // Kiểm tra phản hồi từ API
                    if (!addCustomerResponse.IsSuccessStatusCode)
                    {
                        // Xử lý khi phản hồi không thành công
                        var errorMessage = await addCustomerResponse.Content.ReadAsStringAsync();
                        throw new Exception($"Failed to add customer information. Status Code: {addCustomerResponse.StatusCode}. Error Message: {errorMessage}");
                    }
                }
                // Tạo đối tượng đơn hàng mới
                neworder = new Order()
                {
                    Comment = comment,
                    OrderCode = Guid.NewGuid(),
                    DeliveryDate = DateTime.Now.AddDays(12),
                    Total = Total,
                    CustomerEmail = cusinfo.CustomerEmail,
                    State = "Not deliveried",
                    PaymentMethod = paymentmethod,
                    CInforId = temp.CInforId
                };
                // Gửi đơn hàng đến API
                var apiUrl = $"{_apiSetting.BaseUrl}/orders";
                StringContent content = new StringContent(JsonConvert.SerializeObject(neworder), Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync(apiUrl, content);

                // Kiểm tra phản hồi từ API
                if (!response.IsSuccessStatusCode)
                {
                    // Xử lý khi phản hồi không thành công
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to save order. Status Code: {response.StatusCode}. Error Message: {errorMessage}");
                }
                await jmodule.InvokeVoidAsync("show", "Your order has been saved in the system, please wait for confirmation from the store. Thank you!!!");
                await sessionStorageService.DeleteItemAsync("cart");
                await sessionStorageService.DeleteItemAsync("search");
                NavigationManager.NavigateTo("/", true);

            }
            catch (HttpRequestException httpRequestException)
            {
                // Xử lý lỗi liên quan đến yêu cầu HTTP
                Console.WriteLine($"HttpRequestException: {httpRequestException.Message}");
                // Có thể thêm logic để thông báo lỗi cho người dùng
            }
            catch (JsonSerializationException jsonSerializationException)
            {
                // Xử lý lỗi liên quan đến việc phân tích JSON
                Console.WriteLine($"JsonSerializationException: {jsonSerializationException.Message}");
                // Có thể thêm logic để thông báo lỗi cho người dùng
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi không mong muốn khác
                Console.WriteLine($"Exception: {ex.Message}");
                // Có thể thêm logic để thông báo lỗi cho người dùng
            }
        }


    }
}
