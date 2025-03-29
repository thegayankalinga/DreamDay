using Microsoft.EntityFrameworkCore;

namespace DreamDay.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    //You can  get a property by typing "prop"
    
    // public DbSet<AppUser> AppUsers { get; set; }
    
}