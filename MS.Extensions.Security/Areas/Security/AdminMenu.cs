﻿using Mozlite.Extensions.Security;
using Mozlite.Mvc.AdminMenus;

namespace MS.Areas.Security
{
    /// <summary>
    /// 管理菜单。
    /// </summary>
    public class AdminMenu : MenuProvider
    {
        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            root.AddMenu("users", item => item.Texted("用户管理", "fa fa-users")
                .AddMenu("index", it => it.Texted("用户列表").Page("/Admin/Index", area: SecuritySettings.ExtensionName))
                .AddMenu("roles", it => it.Texted("角色列表").Page("/Admin/Roles/Index", area: SecuritySettings.ExtensionName))
                .AddMenu("permissions", it => it.Texted("权限列表").Page("/Admin/Permissions/Index", area: SecuritySettings.ExtensionName))
                .AddMenu("logs", it => it.Texted("日志管理").Page("/Admin/Logs/Index", area: SecuritySettings.ExtensionName))
                .AddMenu("settings", it => it.Texted("用户配置").Roled(DefaultRole.OwnerName).Page("/Admin/Settings", area: SecuritySettings.ExtensionName))
            );
        }
    }
}