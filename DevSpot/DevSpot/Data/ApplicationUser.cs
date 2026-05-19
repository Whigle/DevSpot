using DevSpot.Models;
using Microsoft.AspNetCore.Identity;

namespace DevSpot.Data;

public class ApplicationUser : IdentityUser
{
    public Company? Company { get; set; }
}