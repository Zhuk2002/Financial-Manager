using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Здесь можно добавить дополнительные поля, если потребуется (например, FirstName или LastName)
    public decimal Balance { get; set; } = 0;
}
