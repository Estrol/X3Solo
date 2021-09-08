using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Solo.Library {
    /// <summary>
    /// X3-JAM UTILITY: Memory Writer
    /// </summary>
    public class EBuffer : IDisposable {
        public MemoryStream Stream { get; }
        public BinaryWriter Writer { get; }

        /// <summary>
        /// Intialize Memory Buffer for 8kb
        /// </summary>
        public EBuffer() : this(8192) { }

        /// <summary>
        /// Intialize Memory Buffer for whatever size is!
        /// </summary>
        /// <param name="capacity"></param>
        public EBuffer(int capacity) {
            Stream = new MemoryStream(capacity);
            Writer = new BinaryWriter(Stream);
        }

        /// <summary>
        /// Convert Stream Buffer to Array bytes
        /// </summary>
        /// <returns>Byte Array contain the data</returns>
        public byte[] ToArray() {
            return Stream.ToArray();
        }

        /// <summary>
        /// Copy current Stream Buffer to another Stream
        /// </summary>
        /// <param name="stream"></param>
        public void CopyTo(Stream stream) {
            Stream.CopyTo(stream);
        }

        /// <summary>
        /// Set first 2 byte to current "Stream" Length
        /// </summary>
        public void SetLength() => SetLength((int)Stream.Length);

        /// <summary>
        /// Set first 2 byte to current "Stream" for given length
        /// </summary>
        /// <param name="length"></param>
        public void SetLength(int length) {
            Writer.Seek(0, SeekOrigin.Begin);
            Writer.Write((short)length);

            Writer.Seek((int)Stream.Length, SeekOrigin.End);
        }

        /// <summary>
        /// Write a signed byte to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte value) => Writer.Write(value);

        /// <summary>
        /// Write 2 signed byte integer to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(short value) => Writer.Write(value);

        /// <summary>
        /// Write 2 signed byte integer to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(ushort value) => Writer.Write(value);

        /// <summary>
        /// Write 4 signed byte integer to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value) => Writer.Write(value);

        /// <summary>
        /// Write 8 signed byte to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value) => Writer.Write(value);

        /// <summary>
        /// Write 8 signed byte of floating point value to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(double value) => Writer.Write(value);

        /// <summary>
        /// Write 4 signed byte of floating point value to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(float value) => Writer.Write(value);

        /// <summary>
        /// Write byte array to "Stream"
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte[] value) => Writer.Write(value);

        /// <summary>
        /// Write string null-terminated to "stream" with UTF-8 Encoding
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value) => Write(value, Encoding.UTF8);

        /// <summary>
        /// Write string null-terminated to "stream" with specific Encoding
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value, Encoding encoding) {
            value += char.MinValue;

            char[] data = value.ToCharArray();
            byte[] bData = encoding.GetBytes(data);
            Writer.Write(bData);
        }

        public void Dispose() {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Writer.Close();
            }
        }
    }
}
