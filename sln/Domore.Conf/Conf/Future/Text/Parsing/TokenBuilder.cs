namespace Domore.Conf.Future.Text.Parsing {
    internal abstract class TokenBuilder : Token {
        public abstract Token Add(string s, ref int i);
    }
}
