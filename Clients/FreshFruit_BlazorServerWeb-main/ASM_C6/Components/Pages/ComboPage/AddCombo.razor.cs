using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ASM_C6.Components.Pages.ComboPage
{
    public partial class AddCombo : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference jmodule;

        private ApiSetting _apiSetting;
        private ASM_C6.Model.Combo combo = new Model.Combo();
        private bool _isRenderCompleted;
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            combo.BrowserFile = e.File;
        }

        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            combo.ExpDate = DateTime.Now;
        }
       
        private async Task CreateCombo()
        {
            try
            {
                Console.WriteLine("fucntion started");
                if(combo.ExpDate <= DateTime.Now)
                {
                    await jmodule.InvokeVoidAsync("show", "Please choose another date, the expiration date must be after the creation date.");
                    return;
                }
                if (combo.BrowserFile == null)
                {
                    await jmodule.InvokeVoidAsync("show", "Please select an image.");
                    return;
                }

                // Gọi EncryptFileNameAsync để mã hóa tên file
                var encryptedFileName = await EncryptFileNameAsync(combo.BrowserFile);
                Console.WriteLine("Encry:" + encryptedFileName);
                // Đường dẫn lưu trữ ảnh
                string filePath = @"C:\Users\Dong\Downloads\ASM_C6\ASM_C6\wwwroot\images";
                // Gọi phương thức SaveImageAsync để lưu ảnh
                var imageSaveResponse = await SaveImageAsync(combo.BrowserFile, encryptedFileName, filePath);
                // Cập nhật URL ảnh cho food
                combo.Image = imageSaveResponse;
                Console.WriteLine("Image: " + combo.Image);
                // Tạo mới food
                combo.ComboCode = Guid.NewGuid();

                // Tạo nội dung JSON cho food
                var apiUrl = $"{_apiSetting.BaseUrl}/combos";
                StringContent content = new StringContent(JsonConvert.SerializeObject(combo), Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST tới API
                var response = await HttpClient.PostAsync(apiUrl, content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Add combo successfully.");
                    NavigationManager.NavigateTo("/admin/combosmn", true);
                }
                else
                {
                    // Đọc và in ra nội dung phản hồi lỗi từ API
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", $"Add combo failed: {errorMessage}");
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
                Console.WriteLine("content:" + content);
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
            NavigationManager.NavigateTo("/admin/combosmn", true);
        }
    }
}
