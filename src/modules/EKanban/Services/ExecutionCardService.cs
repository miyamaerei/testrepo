using System.Linq;
using System;
/*
 *Author：jxx
 *Contact：283591387@qq.com
 *代码由框架生成,此处任何更改都可能导致被代码生成器覆盖
 *所有业务编写全部应在Partial文件夹下ExecutionCardService与IExecutionCardService中编写
 */
using EKanban.Models;
using EKanban.IRepositories;
using EKanban.IServices;
using VOL.Core.BaseProvider;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public partial class ExecutionCardService : ServiceBase<EKanban.Models.ExecutionCard, IExecutionCardRepository>
    , IExecutionCardService, IDependency
    {
        protected readonly ITaskPhaseProgressRepository _phaseProgressRepository;
        protected readonly ITaskFileChangeRepository _fileChangeRepository;
        protected readonly IProjectRepositoriesRepository _projectRepository;

        public ExecutionCardService(IExecutionCardRepository repository)
        : base(repository)
        {
            _phaseProgressRepository = AutofacContainerModule.GetService<ITaskPhaseProgressRepository>();
            _fileChangeRepository = AutofacContainerModule.GetService<ITaskFileChangeRepository>();
            _projectRepository = AutofacContainerModule.GetService<IProjectRepositoriesRepository>();
            Init(repository);
        }
        
        // 用于依赖注入的构造函数
        public ExecutionCardService(
            IExecutionCardRepository repository,
            ITaskPhaseProgressRepository phaseProgressRepository,
            ITaskFileChangeRepository fileChangeRepository,
            IProjectRepositoriesRepository projectRepository)
            : base(repository)
        {
            _phaseProgressRepository = phaseProgressRepository;
            _fileChangeRepository = fileChangeRepository;
            _projectRepository = projectRepository;
            Init(repository);
        }
        
        public static IExecutionCardService Instance
        {
            get { return AutofacContainerModule.GetService<IExecutionCardService>(); }
        }
    }
}
