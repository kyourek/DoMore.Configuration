using System.Collections;
using System.Collections.Generic;

namespace Domore.Conf {
    public interface IConfCollection : IEnumerable {
        int Count { get; }
    }

    public interface IConfCollection<out T> : IConfCollection, IEnumerable<T> {
        T this[int index] { get; }
    }
}
