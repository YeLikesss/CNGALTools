
namespace BKEUnpake
{
    public class AilgnmentManagar
    {
        /// <summary>
        /// 获取对齐后的大小
        /// </summary>
        /// <param name="value">对齐前数值</param>
        /// <param name="align">对齐因子</param>
        /// <returns>对齐后大小</returns>
        public static uint GetAlignment(uint value, uint align)
        {
            return ((value + align - 1) / align) * align;
        }

        /// <summary>
        /// 获取对齐后的大小
        /// </summary>
        /// <param name="value">对齐前数值</param>
        /// <param name="align">对齐因子</param>
        /// <returns>对齐后大小</returns>
        public static ulong GetAlignment(ulong value, ulong align)
        {
            return ((value + align - 1) / align) * align;
        }
    }
}
