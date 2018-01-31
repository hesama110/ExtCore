// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using ExtCore.Data.Abstractions;
using ExtCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;

namespace ExtCore.Data.EntityFramework
{
    /// <summary>
    /// Implements the <see cref="IStorage">IStorage</see> interface and represents implementation of the
    /// Unit of Work design pattern with the mechanism of getting the repositories to work with the underlying
    /// Entity Framework storage context and committing the changes made by all the repositories.
    /// </summary>
    public class Storage : IStorage
    {
        /// <summary>
        /// Gets the Entity Framework storage context.
        /// </summary>
        public IStorageContext StorageContext { get; private set; }

        public Storage(IStorageContext storageContext)
        {
            if (!(storageContext is DbContext))
                throw new ArgumentException("The storageContext object must be an instance of the Microsoft.EntityFrameworkCore.DbContext class.");

            this.StorageContext = storageContext;
        }

        /// <summary>
        /// Gets a repository of the given type.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <returns></returns>
        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            TRepository repository = ExtensionManager.GetInstance<TRepository>();

            if (repository != null)
                repository.SetStorageContext(this.StorageContext);

            return repository;
        }

        /// <summary>
        /// Commits the changes made by all the repositories.
        /// </summary>
        public void Save()
        {
            (this.StorageContext as DbContext).SaveChanges();
        }


        public int SaveAndReturn()
        {
            return (this.StorageContext as StorageContextBase).SaveChanges();
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (this.StorageContext as StorageContextBase).SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return (this.StorageContext as StorageContextBase).SaveChangesAsync(cancellationToken);
        }
    }
}