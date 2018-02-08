﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装管理实现类基类。
    /// </summary>
    public abstract class InstallerManagerBase : IInstallerManager
    {
        private readonly IDbContext<Lisence> _context;
        /// <summary>
        /// 初始化类<see cref="InstallerManagerBase"/>。
        /// </summary>
        /// <param name="context">数据库操作接口。</param>
        protected InstallerManagerBase(IDbContext<Lisence> context)
        {
            _context = context;
        }

        /// <summary>
        /// 是否已经安装。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回判断结果。</returns>
        public async Task<InstallerResult> InstalledAsync(CancellationToken cancellationToken)
        {
            //等待数据迁移
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Installer.Current == InstallerResult.New)
                    break;
                await Task.Delay(100, cancellationToken);
                await InstalledAsync(cancellationToken);
            }

            var lisence = (await _context.FetchAsync(cancellationToken: cancellationToken)).FirstOrDefault();
            var registration = lisence?.Registration;
            if (string.IsNullOrWhiteSpace(registration))
            {
                registration = await SetupAsync(cancellationToken);
                if (string.IsNullOrEmpty(registration))
                    return InstallerResult.Failured;
                if (await SaveLisenceAsync(registration))
                    return InstallerResult.New;
            }
            try
            {
                registration = Cores.Decrypto(registration);
                return await IsValidAsync(registration, cancellationToken);
            }
            catch { }
            return InstallerResult.Failured;
        }

        /// <summary>
        /// 验证是否合法。
        /// </summary>
        /// <param name="registration">注册码。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回验证结果。</returns>
        protected virtual Task<InstallerResult> IsValidAsync(string registration, CancellationToken cancellationToken)
        {
            return Task.FromResult(InstallerResult.Success);
        }

        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        public virtual async Task<bool> SaveLisenceAsync(string registration)
        {
            registration = registration.Trim();
            var lisence = new Lisence { Registration = Cores.Encrypto(registration) };
            if (!await _context.AnyAsync())
                return await _context.CreateAsync(lisence);
            if (await _context.UpdateAsync(lisence))
            {//验证
                Installer.Current = await IsValidAsync(registration, CancellationToken.None);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <typeparam name="TRegistration">注册码类型。</typeparam>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        public virtual Task<bool> SaveLisenceAsync<TRegistration>(TRegistration registration)
        {
            return SaveLisenceAsync(JsonConvert.SerializeObject(registration));
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回执行后的注册码实例。</returns>
        protected abstract Task<string> SetupAsync(CancellationToken cancellationToken);
    }
}