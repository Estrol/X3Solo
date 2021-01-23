using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Solo.Modules {
    public class PatchException : Exception {
        private readonly string msg;

        public PatchException(string message) : base() {
            msg = message;
        }

        public override string Message {
            get {
                return msg;
            }
        }
    }
}
