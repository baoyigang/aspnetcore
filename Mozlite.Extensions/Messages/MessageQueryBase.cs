﻿using Mozlite.Data;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 查询基类。
    /// </summary>
    public abstract class MessageQueryBase : QueryBase<Message>
    {
        /// <summary>
        /// 消息类型。
        /// </summary>
        protected abstract MessageType MessageType { get; }

        /// <summary>
        /// 标题。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 接受对象。
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>
        public MessageStatus? Status { get; set; }

        /// <summary>
        /// 返回结果状态。
        /// </summary>
        public int? Result { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<Message> context)
        {
            context.Where(x => x.MessageType == MessageType)
                .WithNolock();
            if (!string.IsNullOrWhiteSpace(Title))
                context.Where(x => x.Title.Contains(Title));
            if (!string.IsNullOrWhiteSpace(To))
                context.Where(x => x.To.Contains(To));
            if (UserId > 0)
                context.Where(x => x.UserId == UserId);
            if (Status != null)
                context.Where(x => x.Status == Status);
            if (Result != null)
                context.Where(x => x.Result == Result);
        }
    }
}