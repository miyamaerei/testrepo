using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EKanban.Data;

/// <summary>
/// ASP.NET Core Identity 所需的 DbContext
/// 只有用户表和角色表用这个，业务表都用 SqlSugar
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
