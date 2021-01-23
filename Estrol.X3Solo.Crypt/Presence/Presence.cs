using Estrol.X3Solo.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Solo.Modules {
    public class Presence {
        private readonly Process Proc;
        private readonly string Path;

        public Presence(Process GameProc, string music) {
            Proc = GameProc;
            Path = music;
        }

        public JamState Update() {
            if (Proc.Handle == IntPtr.Zero) {
                return new() {
                    IsPlaying = false,
                    OJNId = 0,
                };
            }

            JamState state = new() {
                IsPlaying = ReadMemory(GAME_PLAYING_FLAG_ADDRESS, true) != 0,
                OJNId = ReadMemory(GAME_MUSIC_ID_ADDRESS, true),
            };

            var gameinfo = ReadMemory(GAME_INFO_ADDRESS, true);

            if (state.IsPlaying) {
                int difficulty = ((int)gameinfo >> 8) & 0xFF;
                string OJNPath = $"{Path}/o2ma{state.OJNId}.ojn";
                state.Chart = Parser.Parse(OJNPath, difficulty);
            } else {
                state.Chart = new ChartMeta();
            }

            return state;
        }

        private uint ReadMemory(IntPtr address, bool flag) {
            if (Proc.Handle == IntPtr.Zero) {
                Log.Write("Attempt to read zero-like pointer");
                return 0;
            }

            if (flag) {
                address = Proc.Modules[0].BaseAddress + address.ToInt32();
            }

            var result = new byte[4];
            if (!ReadProcessMemory(Proc.Handle, address, result, sizeof(uint), out IntPtr _)) {
                Log.Write("Failed to read process memory: {0} in offset {1}", Marshal.GetLastWin32Error(), address);
                return 0;
            }

            return BitConverter.ToUInt32(result);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess,
            IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        private IntPtr GAME_MUSIC_ID_ADDRESS        = (IntPtr)0x001D84E8;
        private IntPtr GAME_INFO_ADDRESS            = (IntPtr)0x001D84F4;
        private IntPtr GAME_SLOT_AVAILABLE          = (IntPtr)0x001D8500;
        private IntPtr GAME_SCENE_INDEX_ADDRESS     = (IntPtr)0x001D84F8;
        private IntPtr GAME_PLAYING_FLAG_ADDRESS    = (IntPtr)0x001D8504;
    }
}
