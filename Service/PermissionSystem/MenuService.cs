﻿// =================================================================== 
// 项目说明
//====================================================================
// YXL @ CopyRight 2006-2010。
// 文件： MenuRespository.cs
// 项目名称： 
// 创建时间：2017-05-18
// 负责人：YXL
// ===================================================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Sakura.AspNetCore;
using Core;
using Core.Repository.Ef;
using IService;
using PermissionSystem.Models;
using PermissionSystem;
using ViewModels.AdminWeb.Nav;

namespace Service.PermissionSystem
{
    public class MenuService : EfRepository<Menu>, IMenuService
    {
        private readonly IApplicationService _applicationService;
        private readonly ILogger _logger;

        public MenuService(PermissionSystemContext context, IApplicationService applicationService,
            ILoggerFactory loggerFactory) : base(context)
        {
            _applicationService = applicationService;
            _logger = loggerFactory.CreateLogger<MenuService>();
        }

        public Task<IPagedList<MenuViewModel>> PageMenuViewModel(int pageSize, int pageIndex, string queryString)
        {
            return Task.Run<IPagedList<MenuViewModel>>(() =>
            {
                IQueryable<Menu> queryable = _dbSet;
                if (!string.IsNullOrEmpty(queryString))
                {
                    queryable = _dbSet.Where(n => n.Name.StartsWith(queryString) || n.PyCode.StartsWith(queryString));
                }

                var query = from n in queryable
                    select new MenuViewModel
                    {
                        ID = n.ID,
                        ApplicationName = n.ApplicationID_Model.Name,
                        ActionName = n.ActionName,
                        ApplicationID = n.ApplicationID,
                        Code = n.Code,
                        ControllerName = n.ControllerName,
                        CreateDate = n.CreateDate,
                        Description = n.Description,
                        IconCss = n.IconCss,
                        IsNav = n.IsNav,
                        Name = n.Name,
                        ShowIndex = n.ShowIndex,
                        ParentID = n.ParentID,
                        ParentMenuName = n.ParentID == default(Guid) ? "--根目录--" : _dbSet.FirstOrDefault(a=>a.ID == n.ParentID).Name
                    };

                return query.ToPagedList(pageSize, pageIndex);
            });
        }
    }
}