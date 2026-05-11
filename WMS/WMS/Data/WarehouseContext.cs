using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

namespace WMS.Data
{
    class WarehouseContext { }
    //class WarehouseContext : DbContext
    //{
    //    public WarehouseContext(DbContextOptions<WarehouseContext> options) : base(options) { }

    //    public DbSet<Item> Item { get; set; }
    //    public DbSet<User> User { get; set; }
    //    public DbSet<Mapping> Mapping { get; set; }
    //    public DbSet<PackingHeader> PackingHeader { get; set; }
    //    public DbSet<PackingLine> PackingLine { get; set; }
    //    public DbSet<ScanLabelString> ScanLabelString { get; set; }
    //    public DbSet<Prescan> Prescan { get; set; }
    //    public DbSet<OuterCarton> OuterCarton { get; set; }
    //    public DbSet<InnerCarton> InnerCarton { get; set; }
    //    public DbSet<PrescanOuterCarton> PrescanOuterCarton { get; set; }
    //    public DbSet<PrescanInnerCarton> PrescanInnerCarton { get; set; }
    //    public DbSet<PackingMapping> PackingMapping { get; set; }
    //    public DbSet<Printer> Printer { get; set; }
    //    public DbSet<CustomerGroup> CustomerGroups { get; set; }
    //    public DbSet<ODataSettings> ODataSettings { get; set; }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<InnerCarton>()
    //            .HasKey(ic => new { ic.DocumentNo, ic.DocumentLineNo, ic.OuterCartonLineNo, ic.LineNo }); // 替换为实际属性名称
    //        modelBuilder.Entity<OuterCarton>()
    //            .HasKey(oc => new { oc.DocumentNo, oc.DocumentLineNo, oc.LineNo }); // 定义 OuterCarton 的复合主键
    //        modelBuilder.Entity<PrescanOuterCarton>()
    //            .HasKey(poc => new { poc.DocumentNo, poc.LineNo });
    //        modelBuilder.Entity<PrescanInnerCarton>()
    //            .HasKey(pic => new { pic.DocumentNo, pic.OuterCartonLineNo, pic.LineNo }); // 定义 PrescanInnerCarton 的复合主键
    //        modelBuilder.Entity<PackingLine>().ToTable("Packing Line")
    //            .HasKey(pl => new { pl.DocumentNo, pl.LineNo }); // 定义 PackingLine 的复合主键
    //        modelBuilder.Entity<PackingHeader>().ToTable("Packing Header"); // 根據實際表名調整
    //        modelBuilder.Entity<ScanLabelString>().ToTable("Scan Label String"); // 根據實際表名調整
    //        modelBuilder.Entity<OuterCarton>().ToTable("Outer Carton"); // 根據實際表名調整
    //        modelBuilder.Entity<InnerCarton>().ToTable("Inner Carton"); // 根據實際表名調整
    //        modelBuilder.Entity<PrescanOuterCarton>().ToTable("Prescan Outer Carton"); // 根據實際表名調整
    //        modelBuilder.Entity<PrescanInnerCarton>().ToTable("Prescan Inner Carton"); // 根據實際表名調整
    //        modelBuilder.Entity<PackingMapping>().ToTable("Packing Mapping"); // 根據實際表名調整
    //        modelBuilder.Entity<CustomerGroup>().ToTable("Customer Group"); // 根據實際表名調整
    //        modelBuilder.Entity<ODataSettings>().ToTable("OData Settings");

    //        modelBuilder.Entity<Prescan>()
    //            .Property(p => p.Suspend)
    //            .HasConversion<byte>(v => (bool)v ? (byte)1 : (byte)0, v => v == 1);
    //        modelBuilder.Entity<Prescan>()
    //            .Property(p => p.Finish)
    //            .HasConversion<byte>(v => (bool)v ? (byte)1 : (byte)0, v => v == 1);
    //        modelBuilder.Entity<PrescanOuterCarton>()
    //            .Property(p => p.Closed)
    //            .HasConversion<byte>(v => (bool)v ? (byte)1 : (byte)0, v => v == 1);
    //        modelBuilder.Entity<PrescanInnerCarton>()
    //            .Property(p => p.Closed)
    //            .HasConversion<byte>(v => (bool)v ? (byte)1 : (byte)0, v => v == 1);
    //        modelBuilder.Entity<PrescanInnerCarton>()
    //            .Property(p => p.Selected)
    //            .HasConversion<byte>(v => (bool)v ? (byte)1 : (byte)0, v => v == 1);
    //        modelBuilder.Entity<ScanLabelString>()
    //            .Property(p => p.Prescan)
    //            .HasConversion<byte>(v => (bool)v ? (byte)1 : (byte)0, v => v == 1);
    //        modelBuilder.Entity<ScanLabelString>()
    //            .Property(p => p.Closed)
    //            .HasConversion<byte>(v => (bool)v ? (byte)1 : (byte)0, v => v == 1);

    //    }
    //}
}
