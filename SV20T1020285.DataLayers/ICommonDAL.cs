namespace SV20T1020285.DataLayers
{
    /// <summary>
    /// Mô tả các phép xử lý dữ liệu "chung chung" (generic)
    /// </summary>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu T dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang (bằng 0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng nếu lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");

        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm được
        /// </summary>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng nếu lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue = "");

        /// <summary>
        /// Lấy một bản ghi/dòng dữ liệu trên mã (id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);

        /// <summary>
        /// Bổ sung dữ liệu data thuộc kiểu T vào trong CSDL. Hàm trả về ID của dữ liệu mà được bổ sung
        /// (Trả về giá trị nhỏ hơn hoặc bằng 0 nếu lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);

        /// <summary>
        /// Xóa một bản ghi dữ liệu dựa vào id
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// Kiểm tra xem một bản ghi dữ liệu có mã id hiện đang có được sử dụng bởi các bảng khác hay không
        /// có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsUsed(int id);    
    }
}
