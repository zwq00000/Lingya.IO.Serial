using System.Collections.Generic;
using System.Linq;

namespace Lingya.IO {
    /// <summary>
    /// 数值内容解析器
    /// </summary>
    public class NumberValueParser : IValueParser {
        /// <summary>
        /// 是否包含符号
        /// </summary>
        public bool ExcludeSign { get; set; }

        private IEnumerable<string> FilterNumber(string text) {
            int start = 0;
            int len = 0;
            for (var i = 0; i < text.Length; i++) {
                var c = text[i];
                switch (c) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                        len++;
                        break;
                    case '+':
                    case '-':
                        if (!this.ExcludeSign) {
                            len++;
                        } else {
                            start = i + 1;
                        }
                        break;
                    default:
                        if (len > 0) {
                            yield return text.Substring(start, len);
                            len = 0;
                        }

                        start = i + 1;
                        break;
                }
            }

            if (len > 0) {
                yield return text.Substring(start, len);
            }
        }
        #region Implementation of IValueParser

        /// <summary>
        /// 解析数值
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string Parse(string line) {
            var numbers = this.FilterNumber(line).ToArray();
            return numbers.OrderByDescending(s => s.Length).FirstOrDefault();
        }

        #endregion
    }
}