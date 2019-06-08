using System;
using System.IO.Ports;

namespace Lingya.IO {
   internal static class PortSettingConst {
        /// <summary>
        /// 波特率列表
        /// </summary>
        public static readonly int[] BaudrateList = {
            110, 300, 1200, 2400, 4800, 9600, 19200, 38400,57600,115200
        };

        /// <summary>
        /// 数据位列表
        /// </summary>
        public static readonly int[] Databitses = { 5, 6, 7, 8 };

        /// <summary>
        /// 校验位列表
        /// </summary>
        public static readonly Parity[] Paritys = (Parity[])Enum.GetValues(typeof(Parity));

        /// <summary>
        /// 停止位列表,不包括 <see cref="StopBits.None"/>
        /// </summary>
        public static readonly StopBits[] StopBitses = new StopBits[] {StopBits.One,StopBits.Two,StopBits.OnePointFive}; 

        /// <summary>
        /// 流控制列表
        /// </summary>
        public static Handshake[] Handshakes = (Handshake[])Enum.GetValues(typeof(Handshake));

        /// <summary>
        /// 端口列表
        /// </summary>
        public static string[] PortNames {
            get { return SerialPort.GetPortNames(); }
        }
    }
}