using System.Linq;
using System;
/*
 *Author：jxx
 *Contact：283591387@qq.com
 *代码由框架生成,此处任何更改都可能导致被代码生成器覆盖
 *所有业务编写全部应在Partial文件夹下ExecutionCardService与IExecutionCardService中编写
 */
using EKanban.IRepositories;
using EKanban.IServices;
using VOL.Core.BaseProvider;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public partial class ExecutionCardService : ServiceBase<ExecutionCard, IExecutionCardRepository>
    , IExecutionCardService, IDependency
    {
        public ExecutionCardService(IExecutionCardRepository repository)
        : base(repository)
        {
            Init(repository);
        }
        public static IExecutionCardService Instance
        {
            get { return AutofacContainerModule.GetService<IExecutionCardService>(); }
        }
    }
}
