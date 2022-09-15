using System;
using System.Collections.Generic;

namespace Domore.Conf.Future.Text.Parsing {
    using Tokens;

    internal class MultiLineParser {
        public IEnumerable<ConfPair> Parse(string text) {
            if (null == text) throw new ArgumentNullException(nameof(text));
            var token = default(Token);
            for (var i = 0; i < text.Length; i++) {
                if (token == null) {
                    token = new KeyBuilder();
                }
                if (token is Invalid) {
                    for (; i < text.Length; i++) {
                        if (text[i] == '\n') {
                            token = null;
                            break;
                        }
                    }
                }
                if (token is TokenBuilder builder) {
                    token = builder.Add(text, ref i);
                }
                if (token is Complete complete) {
                    yield return complete.Pair();
                }
            }
        }
    }
}
