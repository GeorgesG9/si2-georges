using si2.dal.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace si2.dal.UnitOfWork
{
    public interface IUnitOfWork
    {
        IDataflowRepository Dataflows { get; }
        IStudentRepository Students { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);

        int SaveChanges();

        public void Dispose();
    }
}
