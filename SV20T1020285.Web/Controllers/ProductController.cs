using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
using Nhom2.Web;
using SV20T1020285.BusinessLayers;
using SV20T1020285.DomainModels;
using SV20T1020285.Web.Models;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV20T1020285.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung mặt hàng";
        const string PRODUCT_SEARCH = "product_search";
        public IActionResult Index()
        {
            ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0
                };
            }
            return View(input);
        }

        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", input.CategoryID, input.SupplierID);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = input.CategoryID,
                SupplierID = input.SupplierID,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            ViewBag.IsEdit = false;

            var model = new Product()
            {
                ProductID = 0,
                Photo = "emptyphoto.png",
                IsSelling = true
            };

            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.IsEdit = true;

            Product? product = ProductDataService.GetProduct(id);
            List<ProductPhoto> Photos = ProductDataService.ListPhotos(id);
            List<ProductAttribute> Attributes = ProductDataService.ListAttributes(id);

            if (product == null || Photos == null || Attributes == null)
                return RedirectToAction("Index");

            var model = new ProductViewForEdit()
            {
                Product = product,
                ProductPhotos = Photos,
                ProductAttributes = Attributes
            };
            if (string.IsNullOrWhiteSpace(model.Product.Photo))
                model.Product.Photo = "emptyphoto.png";
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Product model, IFormFile uploadPhoto)
        {
            if (string.IsNullOrWhiteSpace(model.ProductName))
                ModelState.AddModelError(nameof(model.ProductName), "Tên không được để trống");

            if (model.CategoryID == 0)
                ModelState.AddModelError(nameof(model.CategoryID), "Danh mục hàng không được để trống");

            if (model.SupplierID == 0)
                ModelState.AddModelError(nameof(model.SupplierID), "Nhà cung cấp không được để trống");

            if (string.IsNullOrWhiteSpace(model.Unit))
                ModelState.AddModelError(nameof(model.Unit), "Đơn vị tính không được để trống");

            if (model.Price == 0)
                ModelState.AddModelError(nameof(model.Price), "Phải bổ sung giá tiền của mặt hàng");

            if (!ModelState.IsValid)
            {
                if (model.ProductID == 0)
                    return View("Create", model);
                else
                    return View("Edit", model);
            }

            //Xử lý upload ảnh: nếu có ảnh được upload thì lưu ảnh lên server
            if (uploadPhoto != null)
            {
                //Tên file lưu trên server
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";

                //Đường dẫn vật lý đến file sẽ lưu trên server
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                //Lưu file trên server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }

            if (model.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(model);
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(model);

            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public IActionResult Photo(string method, int id = 0, long photoId = 0)
        {

            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    ProductPhoto model = new ProductPhoto()
                    {
                        ProductID = id,
                        Photo = "emptyphoto.png",
                        PhotoID = 0
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Cập nhật ảnh của mặt hàng";
                    var data = ProductDataService.GetPhoto(photoId);
                    if (data == null)
                        return RedirectToAction("Edit", new { id = id });

                    return View(data);

                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto model, IFormFile? uploadPhoto = null)
        {

            if (string.IsNullOrWhiteSpace(model.DisplayOrder.ToString()))
                ModelState.AddModelError(nameof(model.DisplayOrder), "Thứ tự hiển thị không được để trống");
            

            List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(model.ProductID);

            bool displayOrder = false;
            foreach (ProductPhoto item in productPhotos)
            {
                if (item.DisplayOrder == model.DisplayOrder && model.PhotoID != item.PhotoID)
                {
                    displayOrder = true;
                    break;
                }

            }
            if (displayOrder)
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị đã được sử dụng.Vui lòng chọn thứ tự khác !");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Cập nhật ảnh của mặt hàng";
                return View("Photo", model);
            }


            //Xử lý upload ảnh: nếu có ảnh được upload thì lưu ảnh lên server
            if (uploadPhoto != null)
            {
                //Tên file lưu trên server
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";

                //Đường dẫn vật lý đến file sẽ lưu trên server
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                //Lưu file trên server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }

            if (model.PhotoID == 0)
            {
                ProductDataService.AddPhoto(model);
            }
            else
            {
                ProductDataService.UpdatePhoto(model);
            }

            return RedirectToAction("Edit", new { id = model.ProductID });
        }
        public IActionResult Attribute(string method, int id = 0, int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    ProductAttribute model = new ProductAttribute()
                    {
                        ProductID = id,
                        AttributeID = 0
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                    var data = ProductDataService.GetAttribute(attributeId);
                    if (data == null)
                        return RedirectToAction("Edit", new { id = id });
                    return View(data);
                case "delete":
                    //TODO: Xóa thuộc tính có mã photoID(Xóa trực tiếp, không cần phải xác nhận)

                    bool result = ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute model)
        {

            if (string.IsNullOrWhiteSpace(model.AttributeName))
                ModelState.AddModelError(nameof(model.AttributeName), "Tên thuộc tính không được để trống");

            if (string.IsNullOrWhiteSpace(model.AttributeValue))
                ModelState.AddModelError(nameof(model.AttributeValue), "Giá trị thuộc tính không được để trống");

            List<ProductAttribute> productAttributes = ProductDataService.ListAttributes(model.ProductID);

            bool displayOrder = false;
            foreach (ProductAttribute item in productAttributes)
            {
                if (item.DisplayOrder == model.DisplayOrder && model.AttributeID != item.AttributeID)
                {
                    displayOrder = true;
                    break;
                }

            }
            if (displayOrder)
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị đã được sử dụng.Vui lòng chọn thứ tự khác !");


            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.AttributeID == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                return View("Attribute", model);
            }
            if (model.AttributeID == 0)
            {
                ProductDataService.AddAttribute(model);

            }
            else
            {
                ProductDataService.UpdateAttribute(model);
            }
            
            return RedirectToAction("Edit", new { id = model.ProductID });
        }
    }
}
