using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Products3.States
{
    public class ObservableProperty<T>
    {
        private readonly BehaviorSubject<T> _behaviorSubject;

        public ObservableProperty(T initialValue)
        {
            _behaviorSubject = new(initialValue);
        }

        public void Set(T value)
        {
            _behaviorSubject.OnNext(value);
        }

        public T Get()
        {
            return _behaviorSubject.Value;
        }

        public IObservable<T> AsObservable()
        {
            return _behaviorSubject.AsObservable();
        }
    }
}
