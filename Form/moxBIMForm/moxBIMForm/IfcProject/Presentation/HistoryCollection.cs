// Funcion Based in IfcGroupsViewModel //
// https://github.com/xBimTeam/XbimWindowsUI/blob/master/Xbim.Presentation/HistoryCollection.cs
// End;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoxMain
{
    public class HistoryCollection<T>
    {
        private readonly List<T> _items = new List<T>();

        public int Size { get; }

        public HistoryCollection(int size)
        {
            Size = size;
        }

        public bool Any()
        {
            return _items.Any();
        }

        public void Push(T item)
        {
            _items.Add(item);
            while (_items.Count > Size)
            {
                _items.RemoveAt(0);
            }
        }
        public T Pop()
        {
            if (!_items.Any())
                return default(T);
            var temp = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            return temp;
        }

        public void Remove(int itemAtPosition)
        {
            _items.RemoveAt(itemAtPosition);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
