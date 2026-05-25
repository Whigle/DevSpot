using Microsoft.AspNetCore.Identity;

namespace DevSpot.Models;

public class ApplicationUser : IdentityUser
{
    public Company? Company { get; set; }
}