using System;

namespace FrostLib
{
    public class Signal
    {
        private event Action Subs = delegate { };
        private event Action OnceSubs = delegate { };

        public void Subscribe(Action cb) => Subs += cb;

        public void SubscribeOnce(Action cb) => OnceSubs += cb;

        public void Unsubscribe(Action cb)
        {
            Subs -= cb;
            OnceSubs -= cb;
        }

        public void Dispatch()
        {
            Subs();
            OnceSubs();
            OnceSubs = delegate { };
        }

        public void Reset()
        {
            Subs = delegate { };
            OnceSubs = delegate { };
        }
    }

    public class Signal<T>
    {
        private event Action<T> Subs = delegate { };
        private event Action<T> OnceSubs = delegate { };

        public void Subscribe(Action<T> cb) => Subs += cb;

        public void SubscribeOnce(Action<T> cb) => OnceSubs += cb;

        public void Unsubscribe(Action<T> cb)
        {
            Subs -= cb;
            OnceSubs -= cb;
        }

        public void Dispatch(T arg)
        {
            Subs(arg);
            OnceSubs(arg);
            OnceSubs = delegate { };
        }

        public void Reset()
        {
            Subs = delegate { };
            OnceSubs = delegate { };
        }
    }

    public class Signal<T, U>
    {
        private event Action<T, U> Subs = delegate { };
        private event Action<T, U> OnceSubs = delegate { };

        public void Subscribe(Action<T, U> cb) => Subs += cb;

        public void SubscribeOnce(Action<T, U> cb) => OnceSubs += cb;

        public void Unsubscribe(Action<T, U> cb)
        {
            Subs -= cb;
            OnceSubs -= cb;
        }

        public void Dispatch(T arg1, U arg2)
        {
            Subs(arg1, arg2);
            OnceSubs(arg1, arg2);
            OnceSubs = delegate { };
        }

        public void Reset()
        {
            Subs = delegate { };
            OnceSubs = delegate { };
        }
    }
}