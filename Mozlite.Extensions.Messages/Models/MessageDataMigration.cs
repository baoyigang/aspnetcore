﻿using Mozlite.Data.Migrations;

namespace Mozlite.Extensions.Messages.Models
{
    public class MessageDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<Message>(table => table
                .Column(x => x.Id)
                .Column(x => x.UserId)
                .Column(x => x.To)
                .Column(x => x.MessageType)
                .Column(x => x.Title)
                .Column(x => x.Content)
                .Column(x => x.Status)
                .Column(x => x.TryTimes)
                .Column(x => x.CreatedDate)
                .Column(x => x.ConfirmDate)
            );
        }
    }
}