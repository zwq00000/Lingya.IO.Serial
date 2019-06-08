using System;

namespace Lingya.IO {
    /// <summary>
    /// 数据事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueEventArgs<T> : EventArgs {

        public ValueEventArgs(T value) {
            this.Value = value;
            this.RawValue = value.ToString();
        }

        public ValueEventArgs(string rawValue, T value) {
            this.RawValue = rawValue;
            this.Value = value;
        }

        /// <summary>
        /// 原始值
        /// </summary>
        public string RawValue { get;  }

        /// <summary>
        /// 测量值
        /// </summary>
        public T Value { get; }
    }
}