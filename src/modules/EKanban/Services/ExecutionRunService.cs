using System.Linq;
using System;
/*
 *Author：jxx
 *Contact：283591387@qq.com
 *代码由框架生成,此处任何更改都可能导致被代码生成器覆盖
 *所有业务编写全部应在Partial文件夹下ExecutionRunService与IExecutionRunService中编写
 */
using EKanban.Models;
using EKanban.IRepositories;
using EKanban.IServices;
using VOL.Core.BaseProvider;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public partial class ExecutionRunService : ServiceBase<EKanban.Models.ExecutionRun, IExecutionRunRepository>
    , IExecutionRunService, IDependency
    {
        public ExecutionRunService(IExecutionRunRepository repository)
        : base(repository)
        {
            Init(repository);
        }
        public static IExecutionRunService Instance
        {
            get { return AutofacContainerModule.GetService<IExecutionRunService>(); }
        }
    }
}
