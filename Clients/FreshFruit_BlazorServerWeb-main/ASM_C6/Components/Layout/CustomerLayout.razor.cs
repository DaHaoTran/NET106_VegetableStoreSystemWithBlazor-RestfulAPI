using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NetcodeHub.Packages.Extensions.LocalStorage;
using NetcodeHub.Packages.Extensions.SessionStorage;
using Newtonsoft.Json;

namespace ASM_C6.Components.Layout
{
    public partial class CustomerLayout : LayoutComponentBase
    {
        [Inject]
        public ILocalStorageService localStorageService { get; set; }

        [Inject]
        public ISessionStorageService sessionStorageService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private Model.Customer customer = new Model.Customer();
        public string email;
        private IJSObjectReference jmodule;

        private async void Logout()
        {
            await sessionStorageService.ClearAllItemsAsync();
            NavigationManager.NavigateTo("/", true);
        }

        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first) // Chỉ chạy khi lần đầu render
            {
                jmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");

                customer = await sessionStorageService.GetItemAsModelAsync<Model.Customer>("Login");

                if (customer != null)
                {
                    Console.WriteLine("Customer object: " + JsonConvert.SerializeObject(customer));
                    email = customer.Email;
                    Console.WriteLine("Customer email: " + email);

                    if (string.IsNullOrEmpty(email))
                    {
                        await ShowLoginAlertAndRedirect();
                    }
                }
                else
                {
                    Console.WriteLine("No customer information found in session.");
                    await ShowLoginAlertAndRedirect();
                }
            }
        }

        private async Task ShowLoginAlertAndRedirect()
        {
            await jmodule.InvokeVoidAsync("show", "You need to login first.");
            NavigationManager.NavigateTo("/login", true);
        }
    }
}
