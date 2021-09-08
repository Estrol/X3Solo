using Estrol.X3Solo.Library.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Estrol.X3Solo.Library.Packet {
    public class PacketHandler {
        private readonly Client client;
        private readonly string Version;

        public PacketHandler(Client handler, string version) {
            Version = version;

            client = handler;
            client.m_stateful = XORStateful.CreateStatefulPacketSession();
        }

        /**
         * 1.5 Packet Header
         * 
         * [2  bytes] packet length
         * [2  bytes] packet opcode
         * [x  bytes] packet payload
         * 
         * 1.8 Packet Header
         * Source: https://github.com/sebastian-heinz/Arrowgene.Baf/blob/master/Arrowgene.Baf.Server/Packet/PacketFactory.cs
         * 
         * [2  bytes] packet length
         * [16 bytes] key seed
         * [8  bytes] encrypted end block
         * [4  bytes] size of packet data without padding
         * [4  bytes] size of packet data including padding
         * [x  bytes] packet data + padding bytes (0x4D)
         */

        public IPacket[] CreatePacket(byte[] data) {
            using MemoryStream ms = new(data);
            using BinaryReader br = new(ms);

            List<IPacket> packets = new();

            while (ms.Position < ms.Length) {
                ushort length = br.ReadUInt16();
                _ = ms.Seek(ms.Position - 2, SeekOrigin.Begin);

                byte[] packetData = br.ReadBytes(length);
                if (Version == "1.8") {
                    byte[] xoredData = new byte[length - 2];
                    Buffer.BlockCopy(packetData, 2, xoredData, 0, length - 2);

                    client.m_stateful.StatefulXor(xoredData);
                    using MemoryStream stream = new(xoredData);
                    using BinaryReader reader = new(stream);

                    byte[] Password = reader.ReadBytes(16);
                    byte[] ExpectedEndBlock = reader.ReadBytes(8);
                    uint BodySize = reader.ReadUInt32();
                    uint expectedBodySizeWithPadding = reader.ReadUInt32();

                    int bodySizeWithPadding = (int)(stream.Length - stream.Position);
                    if (bodySizeWithPadding < 0) {
                        Log.Write("::CreatePacket -> Unexpected body negative length");
                        continue;
                    }

                    if (bodySizeWithPadding != expectedBodySizeWithPadding) {
                        Log.Write("::CreatePacket -> Body size does not match with expected length");
                        continue;
                    }

                    if (BodySize > bodySizeWithPadding) {
                        Log.Write("::CreatePacket -> Expected body size does not match with Body Size");
                        continue;
                    }

                    int EndBlockOffset = 32 + bodySizeWithPadding - ExpectedEndBlock.Length;

                    byte[] EndBlock = new byte[ExpectedEndBlock.Length];
                    Buffer.BlockCopy(xoredData, EndBlockOffset, EndBlock, 0, ExpectedEndBlock.Length);

                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(ExpectedEndBlock, EndBlock)) {
                        Log.Write("::CreatePacket -> \"EndBlock\" buffer not equal with \"ExpectedEndBlock\"");
                        continue;
                    }

                    long DataPayloadSize = BodySize - 2;
                    if (DataPayloadSize > ushort.MaxValue) {
                        Log.Write("::CreatePacket -> DataPayloadSize exceeded max unsigned short");
                        continue;
                    }

                    if (DataPayloadSize < 0) {
                        Log.Write("::CreatePacket -> DataPayloadSize has negative value");
                        continue;
                    }

                    Md5AndDes.DesKey key = Md5AndDes.DeriveKey(Password, 16);
                    byte[] encrypted = reader.ReadBytes(bodySizeWithPadding);
                    byte[] decrypted = Md5AndDes.Decrypt(encrypted, key);

                    ushort Opcode = BitConverter.ToUInt16(decrypted, 0);
                    byte[] Payload = new byte[DataPayloadSize];

                    if (DataPayloadSize > 0) {
                        Buffer.BlockCopy(decrypted, 2, Payload, 0, (int)DataPayloadSize);
                    }

                    byte[] dataWithoutPadding = BitConverter.GetBytes((ushort)(BodySize + 2))   // Length
                        .Concat(BitConverter.GetBytes(Opcode))                                  // Opcode
                        .Concat(Payload).ToArray();                                             // Payload body

                    Payload = BitConverter.GetBytes(Opcode)
                        .Concat(Payload).ToArray();

                    packets.Add(new((Opcodes)Opcode, Payload, dataWithoutPadding));
                } else if (Version == "1.5") {
                    ushort Length = BitConverter.ToUInt16(packetData, 0);
                    ushort Opcode = BitConverter.ToUInt16(packetData, 2);

                    byte[] Payload = new byte[packetData.Length - 2];
                    Buffer.BlockCopy(packetData, 2, Payload, 0, packetData.Length - 2);

                    packets.Add(new((Opcodes)Opcode, Payload, packetData));
                } else {
                    throw new NotSupportedException($"Version: {Version} is not supported yet");
                }
            }

            return packets.ToArray();
        }
    }
}
