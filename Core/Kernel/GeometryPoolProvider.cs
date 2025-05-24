using Core.Kernel.Drawing.Geometry;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Concurrent;

namespace Core.Kernel;
/// <summary>
/// 创建不同的池来管理不同类型的几何对象。
/// </summary>
public class GeometryPoolProvider
{
    private interface IGeometryPool
    {
        DrawnGeometry Rent();
        void Return(DrawnGeometry geometry);
    }

    private class GenericGeometryPool<T>(ObjectPool<T> pool) : IGeometryPool
        where T : DrawnGeometry, new()
    {
        public DrawnGeometry Rent()
        {
            return pool.Get();
        }

        public void Return(DrawnGeometry geometry)
        {
            pool.Return((T)geometry);
        }
    }

    private readonly ConcurrentDictionary<Type, IGeometryPool> _pools = new();

    public T Rent<T>() where T : DrawnGeometry, new()
    {
        // GetOrAdd
        var pool = (GenericGeometryPool<T>)_pools.GetOrAdd(typeof(T),
            _ => new GenericGeometryPool<T>(ObjectPool.Create<T>()));
        return (T)pool.Rent();
    }

    public void Return(DrawnGeometry geometry)
    {
        if (geometry is null) return;

        var type = geometry.GetType();
        if (_pools.TryGetValue(type, out var pool))
        {
            pool.Return(geometry);
        }
    }
}
