using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace ASM_C6.Components.Pages.CustomerPage
{
    public partial class AddCustomer : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference jmodule;

        private ApiSetting _apiSetting;
        private ASM_C6.Model.Customer customer = new Model.Customer();
        private bool _isRenderCompleted;
       
        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
        }

        public string GeneratePassword()
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string allChars = upperCase + numbers;
            Random rand = new Random();

            char upper = upperCase[rand.Next(upperCase.Length)];
            char number = numbers[rand.Next(numbers.Length)];

            int passwordLength = rand.Next(5, 9);
            char[] password = new char[passwordLength];

            password[0] = upper;
            password[1] = number;

            for (int i = 2; i < passwordLength; i++)
            {
                password[i] = allChars[rand.Next(allChars.Length)];
            }

            return new string(password.OrderBy(x => rand.Next()).ToArray());
        }

        public void SendPasswordEmail(string Email, string password)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("Ttphong0910@gmail.com");
                mail.To.Add(Email);
                mail.Subject = "Mật khẩu mới của bạn";
                mail.Body = $"Mật khẩu mới của bạn là: {password}";

                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential("kddk0910.kt@gmail.com", "mgcdlqxvrmbhypyx");
                smtpServer.EnableSsl = true;
                smtpServer.UseDefaultCredentials = false;
                smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
        private async Task CreateCus()
        {
            try
            {
                customer.PassWord = GeneratePassword();
                SendPasswordEmail(customer.Email, customer.PassWord);
                customer.UserName = "username";
                Console.WriteLine(customer.Email);
                Console.WriteLine(customer.PassWord);
                // Tạo nội dung JSON cho food
                var apiUrl = $"{_apiSetting.BaseUrl}/customers";
                StringContent content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST tới API
                var response = await HttpClient.PostAsync(apiUrl, content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Password has been sent to your registered email");
                    NavigationManager.NavigateTo("/admin/cusmn", true);
                }
                else
                {
                    // Đọc và in ra nội dung phản hồi lỗi từ API
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await jmodule.InvokeVoidAsync("show", $"Add new customer failed: {errorMessage}");
                    NavigationManager.NavigateTo("/admin/addcus", true);
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync($"An error occurred: {ex.Message}");
                NavigationManager.NavigateTo("/admin/addcus", true);
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
            NavigationManager.NavigateTo("/admin/cusmn", true);
        }
    }
}
