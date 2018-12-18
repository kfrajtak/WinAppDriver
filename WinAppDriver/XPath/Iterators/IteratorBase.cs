using System.Collections;
using System.Collections.Generic;

namespace WinAppDriver.XPath.Iterators
{
    public class IteratorBase<T> : IEnumerable<T>
    {
        public virtual IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public class Enumerator : IEnumerator<T>
        {
            public T Current => throw new System.NotImplementedException();

            object IEnumerator.Current => throw new System.NotImplementedException();

            public Enumerator(IteratorBase<T> iterator)
            {

            }

            public bool MoveNext()
            {
                throw new System.NotImplementedException();
            }

            public void Reset()
            {
                throw new System.NotImplementedException();
            }

            public void Dispose()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
