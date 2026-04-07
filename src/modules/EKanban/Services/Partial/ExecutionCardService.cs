using EKanban.Models;
using EKanban.IServices;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using EKanban.IRepositories;

namespace EKanban.Services
{
    public partial class ExecutionCardService : IExecutionCardService
    {
        public async Task<List<EKanban.Models.ExecutionCard>> GetInProgressAiCardsAsync()
        {
            return await ((IExecutionCardRepository)repository).GetInProgressAiCardsAsync();
        }

        public async Task TriggerReExecuteAsync(int cardId)
        {
            var card = await ((IExecutionCardRepository)repository).FindFirstAsync(c => c.Id == cardId);
            if (card == null)
            {
                throw new System.ArgumentException($"Card {cardId} not found");
            }

            // Reset failure count if it was needing manual intervention
            if (card.NeedsManualIntervention)
            {
                card.FailureCount = 0;
                card.NeedsManualIntervention = false;
            }

            // Transition back to Ready to trigger another execution
            if (card.Status == EKanban.Models.ExecutionCardStatus.InProgress)
            {
                // Already in progress, just reset the start time
                card.InProgressStartTime = System.DateTime.UtcNow;
                ((IExecutionCardRepository)repository).Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
                await ((IExecutionCardRepository)repository).SaveChangesAsync();
            }
            else if (card.Status != EKanban.Models.ExecutionCardStatus.Completed)
            {
                // Go back to Ready
                card.Status = EKanban.Models.ExecutionCardStatus.Ready;
                card.LastUpdated = System.DateTime.UtcNow;
                ((IExecutionCardRepository)repository).Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
                await ((IExecutionCardRepository)repository).SaveChangesAsync();
            }
        }

        /// <summary>
        /// 创建手动看板卡片
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建的卡片</returns>
        public async Task<EKanban.Models.ExecutionCard> CreateManualCardAsync(ManualCardCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new System.ArgumentException("标题不能为空");
            }
            
            if (request.Title.Length > 500)
            {
                throw new System.ArgumentException("标题长度不能超过500个字符");
            }
            
            var card = new EKanban.Models.ExecutionCard
            {
                Title = request.Title,
                Description = request.Description,
                Status = EKanban.Models.ExecutionCardStatus.New,
                ExecutorType = EKanban.Models.ExecutorType.Human,
                BoardWorkItemId = 0, // 手动创建的卡片没有Azure WorkItem ID
                BoardId = request.BoardId ?? "Manual", // 手动创建的卡片使用指定或默认BoardId
                IsManualCreated = true,
                ProjectRepositoryId = request.ProjectRepositoryId,
                SpecId = request.SpecId,
                CreatedAt = System.DateTime.UtcNow,
                LastUpdated = System.DateTime.UtcNow
            };
            
            ((IExecutionCardRepository)repository).Add(card);
            await ((IExecutionCardRepository)repository).SaveChangesAsync();
            return card;
        }

        /// <summary>
        /// 更新手动看板卡片
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的卡片</returns>
        public async Task<EKanban.Models.ExecutionCard> UpdateManualCardAsync(ManualCardUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new System.ArgumentException("标题不能为空");
            }
            
            if (request.Title.Length > 500)
            {
                throw new System.ArgumentException("标题长度不能超过500个字符");
            }
            
            var card = await ((IExecutionCardRepository)repository).FindFirstAsync(c => c.Id == request.Id);
            if (card == null)
            {
                throw new System.ArgumentException("卡片不存在");
            }
            
            if (!card.IsManualCreated)
            {
                throw new System.ArgumentException("只能编辑手动创建的卡片");
            }
            
            card.Title = request.Title;
            card.Description = request.Description;
            card.BoardId = request.BoardId ?? card.BoardId;
            card.ProjectRepositoryId = request.ProjectRepositoryId;
            card.SpecId = request.SpecId;
            card.LastUpdated = System.DateTime.UtcNow;
            
            ((IExecutionCardRepository)repository).Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
            await ((IExecutionCardRepository)repository).SaveChangesAsync();
            return card;
        }

        /// <summary>
        /// 删除手动看板卡片
        /// </summary>
        /// <param name="id">卡片ID</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> DeleteManualCardAsync(int id)
        {
            var card = await ((IExecutionCardRepository)repository).FindFirstAsync(c => c.Id == id);
            if (card == null)
            {
                throw new System.ArgumentException("卡片不存在");
            }
            
            if (!card.IsManualCreated)
            {
                throw new System.ArgumentException("只能删除手动创建的卡片");
            }
            
            ((IExecutionCardRepository)repository).Delete(card);
            await ((IExecutionCardRepository)repository).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 获取手动看板卡片列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="search">搜索关键词</param>
        /// <param name="status">状态筛选</param>
        /// <returns>卡片列表和总数</returns>
        public async Task<(List<EKanban.Models.ExecutionCard> Items, int Total)> GetManualCardsAsync(int page, int pageSize, string? search, int? status)
        {
            // 获取所有卡片
            var allCards = await ((IExecutionCardRepository)repository).FindAsync(x => x.IsManualCreated);
            
            // 搜索筛选
            if (!string.IsNullOrWhiteSpace(search))
            {
                allCards = allCards.Where(c => c.Title.Contains(search)).ToList();
            }
            
            // 状态筛选
            if (status.HasValue)
            {
                allCards = allCards.Where(c => c.Status == (EKanban.Models.ExecutionCardStatus)status.Value).ToList();
            }
            
            // 计算总数
            var total = allCards.Count;
            
            // 分页
            var cards = allCards
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return (cards, total);
        }
    }
}
