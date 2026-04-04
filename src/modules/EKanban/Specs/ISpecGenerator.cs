using System.Threading.Tasks;
using EKanban.Models;

namespace EKanban.Specs
{
    public interface ISpecGenerator
    {
        Task<Spec> GenerateSpecAsync(ExecutionCard card);
    }
}
