using ASM_C6.Components.Pages.Admin;
using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ASM_C6.Components.Pages.FoodPage
{
    public partial class EditFood : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private ASM_C6.Model.Food food = new Model.Food();
        private bool _isRenderCompleted;
        private IEnumerable<ASM_C6.Model.Admin> admins = new List<ASM_C6.Model.Admin>();
        private IEnumerable<FoodCategory> foodCategories = new List<FoodCategory>();
        [Parameter]
        public Guid id { get; set; }
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
           food.BrowserFile = e.File;
        }
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            LoadAdm();
            LoadCate();
            LoadFoodData();
        }
        private async Task LoadAdm()
        {
            var admUrl = $"{_apiSetting.BaseUrl}/admins";
            var admresponse = await HttpClient.GetAsync(admUrl);
            if (admresponse.IsSuccessStatusCode)
            {
                admins = await admresponse.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.Admin>>();
            }
        }
        private async Task LoadCate()
        {
            var cateUrl = $"{_apiSetting.BaseUrl}/categories";
            var cateresponse = await HttpClient.GetAsync(cateUrl);
            if (cateresponse.IsSuccessStatusCode)
            {
                foodCategories = await cateresponse.Content.ReadFromJsonAsync<IEnumerable<FoodCategory>>();
            }
        }
        private async Task LoadFoodData()
        {
            var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";
            var response = await HttpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                food = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Food>();
            }
        }
        public async Task<string> EncryptFileNameAsync(IBrowserFile file)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.OpenReadStream()), "file", file.Name);
            var apiUrl = $"{_apiSetting.BaseUrl}/images/name/encrypt";
            try
            {
                var response = await HttpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Ghi lại thông tin lỗi chi tiết để debug
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to encrypt file name: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                // Ghi lại thông tin lỗi chi tiết để debug
                throw new Exception($"Exception occurred while encrypting file name: {ex.Message}", ex);
            }
        }


        public async Task<string> SaveImageAsync(IBrowserFile file, string encryptedFileName, string filePath)
        {
            // Tạo StreamContent từ stream của tệp
            using (var streamContent = new StreamContent(file.OpenReadStream()))
            {
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                // Tạo nội dung multipart/form-data
                var content = new MultipartFormDataContent();
                content.Add(streamContent, "file", file.Name);

                // Tạo URL của API và mã hóa tham số
                var encodedFilePath = Uri.EscapeDataString(filePath);
                var encodedFileName = Uri.EscapeDataString(encryptedFileName);
                var apiUrl = $"{_apiSetting.BaseUrl}/images/savetosystem/{encodedFileName}/{encodedFilePath}";
                // Gửi yêu cầu POST tới API
                var response = await HttpClient.PostAsync(apiUrl, content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("API Response:");
                    Console.WriteLine(responseContent);
                    return responseContent;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to save image: {errorMessage}");
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
            }
        }
        private async Task EditFd()
        {
            try
            {
                // Đường dẫn lưu trữ ảnh
                string filePath = @"C:\Users\Dong\Downloads\ASM_C6\ASM_C6\wwwroot\images";

                // Nếu không có ảnh mới, sử dụng lại ảnh cũ
                if (food.BrowserFile == null)
                {
                    // Không thay đổi đường dẫn ảnh nếu không có ảnh mới
                    food.Image = this.food.Image;
                }
                else
                {
                    // Gọi EncryptFileNameAsync để mã hóa tên file
                    var encryptedFileName = await EncryptFileNameAsync(food.BrowserFile);

                    // Gọi phương thức SaveImageAsync để lưu ảnh
                    var imageSaveResponse = await SaveImageAsync(food.BrowserFile, encryptedFileName, filePath);

                    // Cập nhật URL ảnh cho food
                    food.Image = imageSaveResponse;
                }

                // Tạo nội dung JSON cho food
                var apiUrl = $"{_apiSetting.BaseUrl}/foods/{id}";
                StringContent content = new StringContent(JsonConvert.SerializeObject(food), Encoding.UTF8, "application/json");

                // Gửi yêu cầu PUT tới API
                var response = await HttpClient.PutAsync(apiUrl, content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Edit food successfully.");
                    NavigationManager.NavigateTo("/admin/foodsmn", true);
                }
                else
                {
                    // Đọc và in ra nội dung phản hồi lỗi từ API
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", $"Edit food failed. Please check the product name again, the product name already exists.");
                    Console.WriteLine(errorMessage);
                    NavigationManager.NavigateTo($"/admin/editfood/{id}", true);
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", $"An error occurred: {ex.Message}");
                NavigationManager.NavigateTo($"/admin/editfood/{id}", true);
            }
        }
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/admin/foodsmn", true);
        }
    }
}
