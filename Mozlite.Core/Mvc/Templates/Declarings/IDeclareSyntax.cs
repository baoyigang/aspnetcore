﻿using System.IO;

namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 声明语法。
    /// </summary>
    public interface IDeclareSyntax : IServices
    {
        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 解析当前声明，并写入到实例对象中。
        /// </summary>
        /// <param name="writer">字符串写入器。</param>
        /// <param name="declare">声明的字符串。</param>
        void Write(TextWriter writer, string declare);
    }
}