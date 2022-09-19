namespace Domore.Conf.Future.Text.Parsing {
    internal abstract class TokenBuilder : Token {
        public char Sep { get; }

        public TokenBuilder(char sep) {
            Sep = sep;
        }

        protected char? Next(string s, ref int i) {
            for (; i < s.Length; i++) {
                var c = s[i];
                if (c == Sep) {
                    return c;
                }
                if (char.IsWhiteSpace(c) == false) {
                    return c;
                }
            }
            return null;
        }

        public abstract Token Add(string s, ref int i);
    }
}
