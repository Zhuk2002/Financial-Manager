using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public decimal Balance { get; set; } = 0;
}
