using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
using Nhom2.Web;
using SV20T1020285.BusinessLayers;
using SV20T1020285.DomainModels;
using SV20T1020285.Web.Models;
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
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", input.SupplierID, input.CategoryID);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
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
               ProductID = 0
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
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Product model, IFormFile uploadPhoto)
        {

            
            if (string.IsNullOrWhiteSpace(model.ProductName))
                ModelState.AddModelError(nameof(model.ProductName), "Tên không được để trống");

            if (model.CategoryID.Equals(null))
                ModelState.AddModelError(nameof(model.CategoryID), "Vui lòng chọn loại hàng");

            if (string.IsNullOrWhiteSpace(model.SupplierID.ToString()))
                ModelState.AddModelError(nameof(model.SupplierID), "Vui lòng chọn nhà cung cấp");

            if (string.IsNullOrWhiteSpace(model.Unit))
                ModelState.AddModelError(nameof(model.Unit), "Đơn vị tính không được để trống");

            if (model.Price.Equals(null))
                ModelState.AddModelError(nameof(model.Price), "Giá của mặt hàng không được để trống");

            if (!ModelState.IsValid)
            {
                if(model.ProductID == 0)
                    return View("Create",model);
                else
                    return View("Edit", model);

            }

            if (model.ProductID == 0 && uploadPhoto == null)
            {
                ModelState.AddModelError(nameof(model.Photo), "Vui lòng thêm ảnh");
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
            return View();
        }

        public IActionResult Photo(string method, int productID, int photoId = 0)
        {
            if(productID < 0) 
                return RedirectToAction("Index");
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var model = new ProductPhoto()
                    {
                        ProductID = productID,  
                        PhotoId = 0,
                        IsHidden = false
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Cập nhật ảnh của mặt hàng";
                    return View();
                case "delete":
                    //TODO: Xóa ảnh có mã photoID(Xóa trực tiếp, không cần phải xác nhận)
                    return RedirectToAction("Edit", new { productID = productID });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto model, IFormFile uploadPhoto)
        {
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

            if(model.PhotoId == 0)
            {
                long id = ProductDataService.AddPhoto(model);
            }


            return View(model);
        }
        public IActionResult Attribute(string id, string method, int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    return View();
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                    return View();
                case "delete":
                    //TODO: Xóa ảnh có mã photoID(Xóa trực tiếp, không cần phải xác nhận)
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}
