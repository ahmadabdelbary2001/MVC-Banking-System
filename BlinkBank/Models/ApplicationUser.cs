using Microsoft.AspNetCore.Identity;

namespace BlinkBank.Models
{
    public class ApplicationUser : IdentityUser

    {
        public string Name { get; set; }
        public Accounts Account { get; set; }
    }
}
