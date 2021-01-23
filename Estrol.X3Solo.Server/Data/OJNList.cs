using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Solo.Server.Data {
    public class OJNList {
        private Dictionary<int, OJN> headers;

        public OJNFileFormat Version { get; set; }

        public bool Modified { get; private set; } = false;

        public int Count => headers.Count;

        public OJN this[int id] => headers[id];

        public OJNList() {
            headers = new Dictionary<int, OJN>();
        }

        public bool Add(OJN header) {
            if (!headers.ContainsKey(header.Id)) {
                headers.Add(header.Id, header);
                return Modified = true;
            }

            return false;
        }

        public bool Remove(OJN header) {
            return Modified = headers.Remove(header.Id);
        }

        public bool Remove(int id) {
            return Modified = headers.Remove(id);
        }

        public bool Update(int id, OJN header) {
            if (headers.ContainsKey(header.Id)) {
                headers[id] = header;
                return Modified = true;
            }

            return false;
        }

        public bool Contains(int id) {
            return headers.ContainsKey(id);
        }

        public OJN[] GetHeaders() {
            Modified = false;
            return headers.Values.ToArray();
        }

        public OJN[] GetHeaders(Func<OJN, bool> predicate) {
            Modified = false;
            return headers.Values.Where(predicate).ToArray();
        }

        public int Optimize() {
            int count = headers.Count;
            var data = headers.Values.GroupBy(h => h.TitleString).Select(h => h.First()).ToArray();

            headers.Clear();
            foreach (var header in data) {
                headers.Add(header.Id, header);
            }

            return count - headers.Count;
        }

        public void SetCharacterEncoding(Encoding encoding) {
            foreach (var header in headers.Values) {
                header.CharacterEncoding = encoding;
            }
        }
    }
}
