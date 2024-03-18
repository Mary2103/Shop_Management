using System.Globalization;

namespace SV20T1020285.Web
{
    public static class Converter
    {
        /// <summary>
        /// Chuyển một chuỗi s sang giá trị kiểu DateTime theo các formats được quy định
        /// (hàm trả về null nếu chuyển không thành công)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);

            }
            catch
            {
                return null;
            }
        }
    }
}
