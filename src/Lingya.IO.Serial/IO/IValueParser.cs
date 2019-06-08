namespace Lingya.IO {
    public interface IValueParser {
        /// <summary>
        /// 解析数值
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        string Parse(string line);
    }
}