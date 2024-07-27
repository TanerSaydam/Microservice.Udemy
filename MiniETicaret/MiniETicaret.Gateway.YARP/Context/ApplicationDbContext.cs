using Microsoft.EntityFrameworkCore;
using MiniETicaret.Gateway.YARP.Models;

namespace MiniETicaret.Gateway.YARP.Context;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
}
