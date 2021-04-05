using System;
using System.Collections.Generic;
using System.Text;

namespace Doppelkopf.Core.App.Helper
{
    public class Watch<T>
    {
        public event Action OnChange;

        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    OnChange?.Invoke();
                }
            }
        }
        public Watch(T value)
        {
            _value = value;
        }

        //public static implicit operator Watch<T>(T value)
        //{
        //    return new Watch<T>(value);
        //}

        public static implicit operator T(Watch<T> w) => w.Value;

        public override string ToString()
        {
            return Value.ToString();
        }

        public Watch(T value, Action onChange) : this(value)
        {
            OnChange += OnChange;
        }
    }
}
