using System.Collections;

namespace CSharpCourse.DesignPatterns.Assignments
{
    internal interface IBidirectionalList<T> : IEnumerable<T>
    {
        int Count { get; }

        void Add(T value);

        void AddRange(IEnumerable<T> values);

        IEnumerable<T> Backward();
    }

    internal class BidirectionalList<T> : IBidirectionalList<T>
    {
        private readonly LinkedList<T> _items = new();

        public int Count => _items.Count;

        public void Add(T value)
        {
            _items.AddLast(value);
        }

        public void AddRange(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                _items.AddLast(value);
            }
        }

        public IEnumerable<T> Backward()
        {
            var node = _items.Last;
            while (node != null)
            {
                yield return node.Value;
                node = node.Previous;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}