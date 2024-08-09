using ASM_C6.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Net.Mail;
using System.Net;

namespace ASM_C6.Components.Pages.CustomerPage
{
    public partial class CustomerMn : ComponentBase
    {
        private IEnumerable<ASM_C6.Model.Customer> customers { get; set; }
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IOptions<ApiSetting> ApiSettingOptions { get; set; }

        private ApiSetting _apiSetting;
        private List<ASM_C6.Model.Customer> paginatedAdmins { get; set; }
        private bool _isRenderCompleted;
        public string apiUrl;

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference jmodule;
        private int currentPage = 1;
        private int pageSize = 4;
        private int totalPages;

        protected override async Task OnInitializedAsync()
        {
            _apiSetting = ApiSettingOptions.Value;
            await LoadDb();
        }


        private async Task LoadDb()
        {
            try
            {
                var apiUrl = $"{_apiSetting.BaseUrl}/customers";
                var response = await HttpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    customers = await response.Content.ReadFromJsonAsync<IEnumerable<ASM_C6.Model.Customer>>();
                    UpdatePaginatedAdmins();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load foods. Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                    NavigationManager.NavigateTo("/admin/cusmn", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await jmodule.InvokeVoidAsync("show", "Fail to upload data.");
                NavigationManager.NavigateTo("/admin/cusmn", true);
            }
        }

        private void UpdatePaginatedAdmins()
        {
            totalPages = (int)Math.Ceiling((double)customers.Count() / pageSize);
            paginatedAdmins = customers.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
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
        private async Task ResetPw(string email)
        {
            try
            {
                string pw = GeneratePassword(); 
                await Task.Run(() => SendPasswordEmail(email, pw));
                await jmodule.InvokeVoidAsync("show", "Email sent successfully");
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", $"Error during password reset: {ex.Message}");
            }
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
        private async Task ConfirmReset(string email)
        {
            try
            {
                bool confirmed = await jmodule.InvokeAsync<bool>("showConfirmAlert", "Are you sure you want to reset password?");
                if (confirmed)
                {
                    await ResetPw(email);
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
        private async Task ConfirmDelete(string email)
        {
            try
            {
                bool confirmed = await jmodule.InvokeAsync<bool>("showConfirmAlert", "Are you sure you want to delete?");
                if (confirmed)
                {
                    await DeleteAdm(email);
                }
            }
            catch (JSException jsEx)
            {
                await jmodule.InvokeVoidAsync("show", "An error occurred while confirming deletion.");
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "An unexpected error occurred." + ex.Message);
            }
        }

        private async Task DeleteAdm(string email)
        {
            try
            {
                string apiUrl = $"{_apiSetting.BaseUrl}/customers/{email}";

                var response = await HttpClient.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    await jmodule.InvokeVoidAsync("show", "Deleted successfully");
                    customers = customers.Where(a => a.Email != email).ToList();
                    UpdatePaginatedAdmins();
                }
                else
                {
                    await jmodule.InvokeVoidAsync("show", "Failed to delete data");
                }
            }
            catch (Exception ex)
            {
                await jmodule.InvokeVoidAsync("show", "Failed to delete data");
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
