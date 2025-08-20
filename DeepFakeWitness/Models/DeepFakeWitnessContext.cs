using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DeepFakeWitness.Models; // Use your correct namespace

public class DeepFakeWitnessContext : IdentityDbContext
{
    public DeepFakeWitnessContext(DbContextOptions<DeepFakeWitnessContext> options)
        : base(options)
    {
    }

    public DbSet<UserImage> UserImage { get; set; }
    public DbSet<Contact> Contact { get; set; }
}
