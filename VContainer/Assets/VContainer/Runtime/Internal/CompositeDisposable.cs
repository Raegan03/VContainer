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

            /// <summary>
            /// Gets a <see cref="CompositeDisposable"/> from the pool or creates a new instance of it.
            /// </summary>
            /// <returns>Instance of <see cref="CompositeDisposable"/>.</returns>
            internal static CompositeDisposable Get()
            {
                if (_pool.Count == 0)
                {
                    return new CompositeDisposable();
                }

                return _pool.Pop();
            }
            
            /// <summary>
            /// Disposes and releases instance of <see cref="CompositeDisposable"/> to the pool.
            /// Ref is used to automatically set the field to a null value to mitigate the chance of leakage.
            /// </summary>
            /// <param name="compositeDisposable">Reference to an instance of <see cref="CompositeDisposable"/>.</param>
            internal static void DisposeAndRelease(ref CompositeDisposable compositeDisposable)
            {
                compositeDisposable.Dispose();
                _pool.Push(compositeDisposable);

                compositeDisposable = null;
            }
        }
    }
}