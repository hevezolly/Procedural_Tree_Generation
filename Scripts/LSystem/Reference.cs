using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public class Reference<T>
    {
        public Reference(T value)
        {
            Value = value;
        }

        public Reference<T> Update(System.Func<T,T> update)
        {
            Value = update(Value);
            return this;
        }

        public T Value { get; set; }
    }
}
