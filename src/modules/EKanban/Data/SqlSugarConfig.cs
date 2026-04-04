using SqlSugar;
using Microsoft.Extensions.Configuration;
using EKanban.Models;

namespace EKanban.Data;

public static class SqlSugarConfig
{
    /// <summary>
    /// 获取 SqlSugar 客户端
    /// </summary>
    public static SqlSugarClient GetSqlSugarClient(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        
        var db = new SqlSugarClient(
            new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
        
        // 自动建表
        db.CodeFirst.InitTables(
            typeof(BoardWorkItem),
            typeof(ExecutionCard),
            typeof(ExecutionTask),
            typeof(ExecutionRun),
            typeof(Spec),
            typeof(SpecEvaluation)
        );
        
        return db;
    }
}
