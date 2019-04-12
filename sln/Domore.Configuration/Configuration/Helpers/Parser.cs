using System;
using System.Collections.Generic;

namespace Domore.Configuration.Helpers {
    public class Parser {
        public IEnumerable<int> ParseIntegerCollection(string input) {
            if (null == input) throw new ArgumentNullException(nameof(input));

            var csv = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in csv) {
                var str = string.Join("", v.Split()).Trim();
                if (str == "") {
                    continue;
                }

                var val = default(int);
                if (int.TryParse(str, out val)) {
                    yield return val;
                    continue;
                }

                var arr = str.Split('-');
                var min = int.Parse(arr[0]);
                var max = int.Parse(arr[1]);
                while (min <= max) {
                    yield return min++;
                }
            }
        }
    }
}
