using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;

namespace EKanban.IServices
{
    public partial interface IBoardWorkItemService : IService<EKanban.Models.BoardWorkItem>
    {
    }
}
