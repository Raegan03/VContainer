using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
    internal sealed class CompositeDisposable : IDisposable
    {
        readonly Stack<IDisposable> disposables = new Stack<IDisposable>();

        public void Dispose()
        {
            IDisposable disposable;
            do
            {
                lock (disposables)
                {
                    disposable = disposables.Count > 0
                        ? disposables.Pop()
                        : null;
                }
                disposable?.Dispose();
            } while (disposable != null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IDisposable disposable)
        {
            lock (disposables)
            {
                disposables.Push(disposable);
            }
        }

        internal static class Pool
        {
            static readonly Stack<CompositeDisposable> _pool = new Stack<CompositeDisposable>();

            internal static CompositeDisposable Get()
            {
                if (_pool.Count == 0)
                {
                    return new CompositeDisposable();
                }

                return _pool.Pop();
            }
        
            internal static void DisposeAndRelease(CompositeDisposable compositeDisposable)
            {
                compositeDisposable.Dispose();
                _pool.Push(compositeDisposable);
            }
        }
    }
}