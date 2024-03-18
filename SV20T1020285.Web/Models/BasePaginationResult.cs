﻿using SV20T1020285.DomainModels;
namespace SV20T1020285.Web.Models
{
    /// <summary>
    /// Lớp cơ sở cho các lớp biểu diễn dữ liệu là kết quả của thao tác
    /// Tìm kiếm, phân trang
    /// </summary>
    public abstract class BasePaginationResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    c += 1;
                return c;
            }
        }
    }

    public class CustomerSearchResult : BasePaginationResult
    {
        public List<Customer> Data { get; set; } = new List<Customer>();
    }

    public class SupplierSearchResult : BasePaginationResult
    {
        public List<Supplier> Data { get; set; } = new List<Supplier>();
    }

    public class ShipperSearchResult : BasePaginationResult
    {
        public List<Shipper> Data { get; set; } = new List<Shipper>();
    }

    public class EmployeeSearchResult : BasePaginationResult
    {
        public List<Employee> Data { get; set; } = new List<Employee>();
    }

    public class CategorySearchResult : BasePaginationResult
    {
        public List<Category> Data { get; set; } = new List<Category>();
    }

    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> Data { get; set; } = new List<Product>();
    }
}
