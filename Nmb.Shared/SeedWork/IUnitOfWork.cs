﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nmb.Shared.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}