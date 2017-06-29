using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public interface ITableEntityMapper
    {
        string GetTable(Type type);
        string GetTable<TEntity>() where TEntity : ITableEntity;
        string GetTable<TEntity>(TEntity entity) where TEntity : ITableEntity;
    }
}