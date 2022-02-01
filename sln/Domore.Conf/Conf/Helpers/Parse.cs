using System;
using System.Collections.Generic;

namespace Domore.Conf.Helpers {
    internal static class Parse {
        public static IEnumerable<int> IntegerCollection(string input) {
            if (null == input) throw new ArgumentNullException(nameof(input));

            var csv = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in csv) {
                var str = string.Join("", v.Split()).Trim();
                if (str == "") {
                    continue;
                }

                if (int.TryParse(str, out var val)) {
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
