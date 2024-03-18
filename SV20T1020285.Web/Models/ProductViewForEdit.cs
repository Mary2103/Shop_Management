using SV20T1020285.DomainModels;

namespace SV20T1020285.Web
{
    /// <summary>
    /// Mô hình chung cho phần sửa mặt hàng với ảnh và thuộc tính
    /// </summary>
    public class ProductViewForEdit
    {
        public Product? Product { get; set; }
        public List<ProductPhoto>? ProductPhotos { get; set; }
        public List<ProductAttribute>? ProductAttributes { get; set; }

    }
}
