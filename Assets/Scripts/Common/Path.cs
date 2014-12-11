using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoEngine1
{
    public class Path : LinkedList<Vector2Int>
    {
        public Vector2Int PopFirst()
        {
            var r = First.Value;
            this.RemoveFirst();
            return r;
        }

        public bool IsEmpty()
        {
            return this.Count == 0;
        }
    }
}