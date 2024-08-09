using ASM_C6.Model;
using Elfie.Serialization;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;


namespace ASM_C6.Components.Pages.FoodPage
{
     public partial class AddFood :ComponentBase
    {
        [Inject]
        private IWebHostEnvironment _evironment { get; set; }
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference jmodule;

        private ApiSetting _apiSetting;
        private ASM_C6.Model.Food food = new Model.Food();
        private bool _isRenderCompleted;
        private IEnumerable<ASM_C6.Model.Admin> admins = new List<ASM_C6.Model.Admin>();
        private IEnumerable<FoodCategory> foodCategories = new List<FoodCategory>();
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            food.BrowserFile = e.File;
        }

        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            LoadAdm();
            LoadCate();
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
        private async Task CreateFood()
        {
            try
            {
                if (food.BrowserFile == null)
                {
                    await jmodule.InvokeVoidAsync("show", "Please select an image.");
                    return;
                }
                // Gọi EncryptFileNameAsync để mã hóa tên file
                var encryptedFileName = await EncryptFileNameAsync(food.BrowserFile);
                // Đường dẫn lưu trữ ảnh
                string filePath = Path.Combine(_evironment.WebRootPath, "images");
                //Path 2: string filePath = @"D:\FPoly\CNTT\NET106_LapTrinhC#6_SU24\Labs\NET106_PS30117_TranGiaHao_ASM\Clients\FreshFruit_BlazorServerWeb-main\ASM_C6\wwwroot\images";
                // Gọi phương thức SaveImageAsync để lưu ảnh
                var imageSaveResponse = await SaveImageAsync(food.BrowserFile, encryptedFileName, filePath);

                // Cập nhật URL ảnh cho food
                food.Image = imageSaveResponse;

                // Tạo mới food
                food.FoodCode = Guid.NewGuid();
                food.Sold = 0;
                food.PreviousPrice = 0;
                
                // Tạo nội dung JSON cho food
                var apiUrl = $"{_apiSetting.BaseUrl}/foods";
                StringContent content = new StringContent(JsonConvert.SerializeObject(food), Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST tới API
                var response = await HttpClient.PostAsync(apiUrl, content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Add food successfully.");
                    NavigationManager.NavigateTo("/admin/foodsmn", true);
                }
               
                else
                {
                    // Đọc và in ra nội dung phản hồi lỗi từ API
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", $"Add food failed. Please check the product name again, the product name already exists.");
                    NavigationManager.NavigateTo("/admin/admwelcome", true);
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync($"An error occurred: {ex.Message}");
                NavigationManager.NavigateTo("/admin/admwelcome", true);
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
                Console.WriteLine("content:"+content);
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
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/admin/foodsmn", true);
        }
    }
}
