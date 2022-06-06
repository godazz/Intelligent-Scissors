using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelligentScissors
{
    class PriorityQueue
    {

        private List<  Tuple < int, int , double > > Arr = new List< Tuple<int, int, double> >();


        public void Push(Tuple<int, int, double> Node)
        {
            Arr.Add(Node);
            PushUp(Arr.Count - 1);
        }

        private void PushUp (int index)
        {
            int parent = (index - 1) / 2;

            if (index == 0 || Arr[index].Item3 >= Arr[parent].Item3)
                return;


            if (Arr[index].Item3 < Arr[parent].Item3)
            {
                Tuple<int, int, double> temp = Arr[index];
                Arr[index] = Arr[parent];
                Arr[parent] = temp;
            }

            PushUp(parent);
        }


        public Tuple<int, int, double> Pop()
        {
            Tuple<int, int, double> element = Arr[0];
            Arr[0] = Arr[Arr.Count() - 1];
            Arr.RemoveAt(Arr.Count() - 1);

            PushDown(0);

            return element;
        }

        private void PushDown(int index)
        {
            int left = (index * 2) + 1;
            int right = (index * 2) + 2;

            if (Arr.Count <= left) // no childrens
            {
                return;
            }else if (left < Arr.Count && right >= Arr.Count && Arr[left].Item3 >= Arr[index].Item3){ // only has left child (in place)
                return;

            }else if (left < Arr.Count && right < Arr.Count && Arr[left].Item3 >= Arr[index].Item3 && Arr[right].Item3 >= Arr[index].Item3) // parent smaller than both children ( right place)
            {
                return;
            }
                  
            // find the smallet children and replace it with the current node.
            int smallest = right < Arr.Count() && Arr[right].Item3 <= Arr[left].Item3 ? right : left;

            Tuple<int, int, double> temp = Arr[smallest];
            Arr[smallest] = Arr[index];
            Arr[index] = temp;
            PushDown(smallest);
        }

        public Tuple<int, int, double> Top()
        {
            return Arr[0];
        }


        public int Size()
        {
            return Arr.Count();
        }

        public bool IsEmpty()
        {

            return (Arr.Count() == 0);
        }
    }
}
