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
    public partial class EditCombo : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private ASM_C6.Model.Combo combo = new ASM_C6.Model.Combo();
        private bool _isRenderCompleted;
        public string apiUrl;

        [Parameter]
        public Guid id { get; set; }
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            combo.BrowserFile = e.File;
        }
        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                _isRenderCompleted = true;
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
            }
        }
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            await LoadData();
        }
        private async Task LoadData()
        {
            var apiUrl = $"{_apiSetting.BaseUrl}/combos/{id}";
            var response = await HttpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                combo = await response.Content.ReadFromJsonAsync<ASM_C6.Model.Combo>();
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
        private async Task EditCb()
        {
            try
            {
                if (combo.BrowserFile == null)
                {
                    await jmodule.InvokeVoidAsync("show", "Please select an image.");
                    return;
                }
                if(combo.ExpDate <= combo.ApplyDate)
                {
                    await jmodule.InvokeVoidAsync("show", "Please choose another date, the expiration date must be after the creation date.");
                    return;
                }

                // Gọi EncryptFileNameAsync để mã hóa tên file
                var encryptedFileName = await EncryptFileNameAsync(combo.BrowserFile);
                // Đường dẫn lưu trữ ảnh
                string filePath = @"C:\Users\Dong\Downloads\ASM_C6\ASM_C6\wwwroot\images";
                // Gọi phương thức SaveImageAsync để lưu ảnh
                var imageSaveResponse = await SaveImageAsync(combo.BrowserFile, encryptedFileName, filePath);

                // Cập nhật URL ảnh cho food
                combo.Image = imageSaveResponse;

                // Tạo nội dung JSON cho food
                var apiUrl = $"{_apiSetting.BaseUrl}/combos/{id}";
                StringContent content = new StringContent(JsonConvert.SerializeObject(combo), Encoding.UTF8, "application/json");
                // Gửi yêu cầu POST tới API
                var response = await HttpClient.PutAsync(apiUrl, content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Edit combo successfully.");
                    NavigationManager.NavigateTo("/admin/combosmn", true);
                }
                else
                {
                    // Đọc và in ra nội dung phản hồi lỗi từ API
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", $"Edit combo failed: {errorMessage}");
                    Console.WriteLine(errorMessage);
                    NavigationManager.NavigateTo($"/admin/editcombo/{id}", true);
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync($"An error occurred: {ex.Message}");
                NavigationManager.NavigateTo($"/admin/editcombo/{id}", true);
            }
        }
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/combosmn", true);
        }
    }
}
