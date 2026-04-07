using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using VOL.Core.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using EKanban.IRepositories;

namespace EKanban.IServices
{
    /// <summary>
    /// 手动看板创建请求
    /// </summary>
    public class ManualCardCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BoardId { get; set; }
        public int? ProjectRepositoryId { get; set; }
        public int? SpecId { get; set; }
    }

    /// <summary>
    /// 手动看板更新请求
    /// </summary>
    public class ManualCardUpdateRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BoardId { get; set; }
        public int? ProjectRepositoryId { get; set; }
        public int? SpecId { get; set; }
    }

    /// <summary>
    /// 卡片详情响应结构
    /// </summary>
    public class ExecutionCardDetailResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int ExecutorType { get; set; }
        public int BoardWorkItemId { get; set; }
        public string BoardId { get; set; }
        public int? ProjectRepositoryId { get; set; }
        public int? SpecId { get; set; }
        public int FailureCount { get; set; }
        public bool NeedsManualIntervention { get; set; }
        public DateTime? InProgressStartedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsManualCreated { get; set; }
        public List<PhaseProgressResponse> PhaseProgressList { get; set; }
        public List<FileChangeResponse> FileChangeList { get; set; }
        public ProjectRepositoryResponse ProjectRepository { get; set; }
    }

    /// <summary>
    /// 阶段进度响应结构
    /// </summary>
    public class PhaseProgressResponse
    {
        public int Id { get; set; }
        public int ExecutionCardId { get; set; }
        public int Phase { get; set; }
        public int Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string PhaseLog { get; set; }
    }

    /// <summary>
    /// 文件变更响应结构
    /// </summary>
    public class FileChangeResponse
    {
        public int Id { get; set; }
        public int ExecutionCardId { get; set; }
        public string FilePath { get; set; }
        public int ChangeType { get; set; }
        public string CommitHash { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    /// <summary>
    /// 项目仓库响应结构
    /// </summary>
    public class ProjectRepositoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalWorkingDir { get; set; }
        public string GitRemoteUrl { get; set; }
        public string DefaultBranch { get; set; }
        public string Description { get; set; }
    }

    public partial interface IExecutionCardService : IService<EKanban.Models.ExecutionCard>
    {
        Task<List<EKanban.Models.ExecutionCard>> GetInProgressAiCardsAsync();
        Task TriggerReExecuteAsync(int cardId);

        /// <summary>
        /// 创建手动看板卡片
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建的卡片</returns>
        Task<EKanban.Models.ExecutionCard> CreateManualCardAsync(ManualCardCreateRequest request);

        /// <summary>
        /// 更新手动看板卡片
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的卡片</returns>
        Task<EKanban.Models.ExecutionCard> UpdateManualCardAsync(ManualCardUpdateRequest request);

        /// <summary>
        /// 删除手动看板卡片
        /// </summary>
        /// <param name="id">卡片ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteManualCardAsync(int id);

        /// <summary>
        /// 获取手动看板卡片列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="search">搜索关键词</param>
        /// <param name="status">状态筛选</param>
        /// <returns>卡片列表和总数</returns>
        Task<(List<EKanban.Models.ExecutionCard> Items, int Total)> GetManualCardsAsync(int page, int pageSize, string? search, int? status);

        /// <summary>
        /// 获取卡片详情（包含阶段进度和文件变更）
        /// </summary>
        /// <param name="id">卡片ID</param>
        /// <returns>卡片详情</returns>
        Task<ExecutionCardDetailResponse> GetDetailAsync(int id);
    }
}
