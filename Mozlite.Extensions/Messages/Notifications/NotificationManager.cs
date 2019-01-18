﻿using Microsoft.AspNetCore.Http;
using Mozlite.Data;
using Mozlite.Extensions.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 通知管理实现类。
    /// </summary>
    public class NotificationManager : ObjectManager<Notification>, INotificationManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 初始化类<see cref="NotificationManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="httpContextAccessor">Http上下文访问接口。</param>
        public NotificationManager(IDbContext<Notification> context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 当前用户ID。
        /// </summary>
        protected int UserId => _httpContextAccessor.HttpContext.User.GetUserId();

        /// <summary>
        /// 加载当前用户最新得通知。
        /// </summary>
        /// <param name="size">通知记录数。</param>
        /// <returns>返回通知列表。</returns>
        public virtual IEnumerable<Notification> Load(int size)
        {
            return Context.AsQueryable().WithNolock().Where(x => x.UserId == UserId).AsEnumerable(size);
        }

        /// <summary>
        /// 加载当前用户最新得通知。
        /// </summary>
        /// <param name="size">通知记录数。</param>
        /// <returns>返回通知列表。</returns>
        public virtual Task<IEnumerable<Notification>> LoadAsync(int size)
        {
            return Context.AsQueryable().WithNolock().Where(x => x.UserId == UserId).AsEnumerableAsync(size);
        }

        /// <summary>
        /// 获取当前用户得通知数量。
        /// </summary>
        /// <returns>返回通知得数量。</returns>
        public virtual int GetSize()
        {
            return Context.Count(x => x.UserId == UserId);
        }

        /// <summary>
        /// 获取当前用户得通知数量。
        /// </summary>
        /// <returns>返回通知得数量。</returns>
        public virtual Task<int> GetSizeAsync()
        {
            return Context.CountAsync(x => x.UserId == UserId);
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="userId">用户列表。</param>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public virtual DataResult Save(int[] userId, Notification model)
        {
            if (Context.BeginTransaction(db =>
            {
                foreach (var id in userId)
                {
                    model.Id = 0;
                    model.UserId = id;
                    db.Create(model);
                }
                return true;
            }))
                return DataAction.Created;
            return DataAction.CreatedFailured;
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="userId">用户列表。</param>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public virtual async Task<DataResult> SaveAsync(int[] userId, Notification model)
        {
            if (await Context.BeginTransactionAsync(async db =>
            {
                foreach (var id in userId)
                {
                    model.Id = 0;
                    model.UserId = id;
                    await db.CreateAsync(model);
                }
                return true;
            }))
                return DataAction.Created;
            return DataAction.CreatedFailured;
        }

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="id">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        public virtual bool SetStatus(int id, NotificationStatus status)
        {
            return Context.Update(id, new { status });
        }

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="id">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        public virtual Task<bool> SetStatusAsync(int id, NotificationStatus status)
        {
            return Context.UpdateAsync(id, new { status });
        }

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="ids">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        public virtual bool SetStatus(int[] ids, NotificationStatus status)
        {
            return Context.Update(x => x.Id.Included(ids), new { status });
        }

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="ids">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        public virtual Task<bool> SetStatusAsync(int[] ids, NotificationStatus status)
        {
            return Context.UpdateAsync(x => x.Id.Included(ids), new { status });
        }
    }
}