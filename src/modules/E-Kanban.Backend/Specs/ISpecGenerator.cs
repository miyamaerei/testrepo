using System.Threading.Tasks;
using E_Kanban.Backend.Models;

namespace E_Kanban.Backend.Specs
{
    public interface ISpecGenerator
    {
        Task<Spec> GenerateSpecAsync(ExecutionCard card);
    }
}
