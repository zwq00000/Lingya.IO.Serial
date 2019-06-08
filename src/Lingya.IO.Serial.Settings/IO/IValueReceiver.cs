using System;

namespace Lingya.IO {
    public interface IValueReceiver {
        /// <summary>
        /// 接收到数据
        /// </summary>
        event EventHandler<ValueEventArgs<double>> ReceivedValue;
    }
}