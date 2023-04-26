using System;
using System.Collections.Generic;
using BloodNet.Models.Auth;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodNet.Models;

public partial class BloodNetContext : IdentityDbContext<User,Role,Guid>
{
    public BloodNetContext()
    {
    }

    public BloodNetContext(DbContextOptions<BloodNetContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
