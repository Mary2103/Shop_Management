﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom2.Web;
using SV20T1020285.BusinessLayers;
using SV20T1020285.DomainModels;
using SV20T1020285.Web.Models;
using System.Buffers;

namespace SV20T1020285.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung loại hàng";
        const string CATEGORY_SEARCH = "category_search";
        public IActionResult Index()
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
            if(input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);

        }
        public IActionResult Search(PaginationSearchInput input) 
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            var model = new Category()
            {
                CategoryID = 0
            };
            return View("Edit",model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật loại hàng";
            var model = CommonDataService.GetCategory(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Save(Category model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
                ModelState.AddModelError(nameof(model.CategoryName), "Tên không được để trống");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.CategoryID == 0 ? CREATE_TITLE : "Cập nhật thông tin loại hàng";
                return View("Edit", model);
            }

            if (model.CategoryID == 0)
            {
                int id = CommonDataService.AddCategory(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("CategoryName", "Tên loại hàng bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCategory(model);
                {
                    ModelState.AddModelError("Error", "Không cập nhật được loại hàng. Có thể loại hàng bị trùng");
                    ViewBag.Title = "Cập nhật thông tin loại hàng";
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if(Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCategory(id); 
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
    }
}
