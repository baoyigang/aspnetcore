﻿using System;
using System.Linq;
using Mozlite.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Mozlite.Extensions.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 缓存分类管理实现类基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CachableCategoryManager<TCategory> : CategoryManager<TCategory>,
        ICachableCategoryManager<TCategory>
        where TCategory : CategoryBase
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化类<see cref="CachableCategoryManager{TCategory}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected CachableCategoryManager(IDbContext<TCategory> db, IMemoryCache cache)
            : base(db)
        {
            _cache = cache;
        }

        /// <summary>
        /// 根据结果刷新缓存。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <returns>返回当前结果实例。</returns>
        protected virtual DataResult RefreshCache(DataResult result)
        {
            if (result.Succeed())
                _cache.Remove(typeof(TCategory));
            return result;
        }

        /// <summary>
        /// 保存分类。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回保存结果。</returns>
        public override DataResult Save(TCategory category)
        {
            return RefreshCache(base.Save(category));
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="category">模型实例对象。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回保存结果。</returns>
        public override async Task<DataResult> SaveAsync(TCategory category, CancellationToken cancellationToken = default)
        {
            return RefreshCache(await base.SaveAsync(category, cancellationToken));
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(TCategory category)
        {
            var categories = Fetch();
            return categories.Any(x => x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<bool> IsDuplicatedAsync(TCategory category, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var categories = await FetchAsync(cancellationToken: cancellationToken);
            return categories.Any(x => x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回删除结果。</returns>
        public override DataResult Delete(int id)
        {
            return RefreshCache(base.Delete(id));
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<DataResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            return RefreshCache(await base.DeleteAsync(id, cancellationToken));
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="ids">分类Id集合，以“,”分隔。</param>
        /// <returns>返回删除结果。</returns>
        public override DataResult Delete(string ids)
        {
            return RefreshCache(base.Delete(ids));
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回分类列表。</returns>
        public override IEnumerable<TCategory> Fetch(Expression<Predicate<TCategory>> expression = null)
        {
            var categories = _cache.GetOrCreate(typeof(TCategory), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return Context.Fetch();
            });
            return categories.Filter(expression);
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分类列表。</returns>
        public override async Task<IEnumerable<TCategory>> FetchAsync(Expression<Predicate<TCategory>> expression = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var categories = await _cache.GetOrCreateAsync(typeof(TCategory), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return await Context.FetchAsync(cancellationToken: cancellationToken);
            });
            return categories.Filter(expression);
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        public override TCategory Find(int id)
        {
            var categories = Fetch();
            return categories.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分类实例。</returns>
        public override async Task<TCategory> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            var categories = await FetchAsync(cancellationToken: cancellationToken);
            return categories.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 当前分类实例。
        /// </summary>
        public override IEnumerable<TCategory> Categories => Fetch();
    }
}