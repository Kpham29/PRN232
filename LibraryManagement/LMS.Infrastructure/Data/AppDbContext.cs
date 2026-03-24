using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Reader> Readers { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BorrowSlip> BorrowSlips { get; set; }
    public DbSet<BorrowSlipDetail> BorrowSlipDetails { get; set; }
    public DbSet<FineSlip> FineSlips { get; set; }
    public DbSet<BorrowRequest> BorrowRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Account>().HasIndex(a => a.Email).IsUnique();
        mb.Entity<Reader>().HasIndex(r => r.CardNumber).IsUnique();
        mb.Entity<FineSlip>().HasIndex(f => f.BorrowSlipId).IsUnique();
        mb.Entity<BorrowRequest>().HasIndex(r => new { r.ReaderId, r.BookId, r.Status }).HasFilter("[Status] = 'Pending'");

        mb.Entity<Book>().Property(b => b.Price).HasColumnType("decimal(18,2)");
        mb.Entity<FineSlip>().Property(f => f.FineAmount).HasColumnType("decimal(18,2)");
        mb.Entity<FineSlip>().Property(f => f.AdjustedAmount).HasColumnType("decimal(18,2)");

        // Restrict để tránh lỗi multiple cascade paths (BorrowSlip có 2 FK đến Account)
        mb.Entity<BorrowSlip>()
            .HasOne(b => b.Librarian).WithMany()
            .HasForeignKey(b => b.LibrarianId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<FineSlip>()
            .HasOne(f => f.BorrowSlip).WithOne(b => b.FineSlip)
            .HasForeignKey<FineSlip>(f => f.BorrowSlipId);
    }
}
