using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Text;

namespace ASM_C6.Components.Pages.ComboDetail
{
    public partial class AddComboDetail : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference jmodule;

        private ApiSetting _apiSetting;
        private ASM_C6.Model.ComboDetail combodetail = new Model.ComboDetail();
        private IEnumerable<Food> foods = new List<Food>();
        private IEnumerable<Combo> combos = new List<Combo>();
        private bool _isRenderCompleted;
        public string apiUrl;


        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
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
        private async Task CreateDetail()
        {
            try
            {
                // Tạo nội dung JSON cho food
                var apiUrl = $"{_apiSetting.BaseUrl}/combodetails";
                StringContent content = new StringContent(JsonConvert.SerializeObject(combodetail), Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST tới API
                var response = await HttpClient.PostAsync(apiUrl, content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Add detail successfully.");
                    NavigationManager.NavigateTo("/combodetailsmn", true);
                }
                else
                {
                    // Đọc và in ra nội dung phản hồi lỗi từ API
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", $"Add detail failed. Please check that you have filled in all the information");
                    Console.WriteLine($"Error: {errorMessage}");
                    NavigationManager.NavigateTo("/admin/admwelcome", true);
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync($"An error occurred: {ex.Message}");
                NavigationManager.NavigateTo("/admin/admwelcome", true);
            }
        }
        
        private async Task Backto()
        {
            NavigationManager.NavigateTo("/admin/combodetailsmn", true);
        }

    }
}
