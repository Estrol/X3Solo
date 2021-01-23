using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Estrol.X3Solo.Server.Data;

namespace Estrol.X3Solo.Modules {
    public class Parser {
        public static ChartMeta Parse(string filePath, int difficulty) {
            OJN ojn = OJNDecoder.Decode(AppDomain.CurrentDomain.BaseDirectory + $"\\{filePath}");

            var dif = "Hx";
            switch (difficulty) {
                case 0:
                    dif = "Ex";
                    break;

                case 1:
                    dif = "Nx";
                    break;

                default: break;
            }

            var Level = (short)ojn.GetType().GetProperty($"Level{dif}").GetValue(ojn, null);
            var Duration = (int)ojn.GetType().GetProperty($"Duration{dif}").GetValue(ojn, null);

            return new() {
                MusicId = ojn.Id,
                Title = ojn.TitleString,
                Artist = ojn.ArtistString,
                DifficultyText = dif,
                Difficulty = difficulty,
                Level = Level,
                Duration = Duration
            };
        }
    }
}
