using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using E_Kanban.Backend.Data;
using E_Kanban.Backend.DI;
using E_Kanban.Backend.Jobs;
using E_Kanban.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 配置 SqlSugar
builder.Services.AddSingleton<SqlSugarClient>(sp =>
{
    return SqlSugarConfig.GetSqlSugarClient(builder.Configuration);
});

// 配置 ASP.NET Core Identity (需要 EF Core 存储用户表)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 添加仓储和服务
builder.Services.AddRepositories();
builder.Services.AddServices();

// 配置 Hangfire
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSerializerSettings(new Newtonsoft.Json.JsonSerializerSettings())
    .UseSqlServerStorage(hangfireConnectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

// 配置 CORS 允许前端访问
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

// Hangfire 仪表盘 - 允许匿名访问（开发环境）
app.UseHangfireDashboard("/hangfire");

// 注册定时任务：从 Azure Boards 同步
RecurringJob.AddOrUpdate<SyncFromAzureBoardsJob>(
    "sync-from-azure-boards",
    job => job.RunAsync(),
    "*/15 * * * *"); // 每 15 分钟同步一次

app.MapControllers();
app.MapHangfireDashboard();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    
    // 创建默认角色和管理员用户（如果不存在）
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}

app.Run();
