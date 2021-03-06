﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public interface ITaskController : IDisposable
    {
        /// <summary>
        /// The error event.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Get the flag indicating if this controller is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Get the controller <see cref="Task"/>.
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// Initiate the controller action.
        /// </summary>
        /// <param name="action">optional.</param>
        /// <param name="onError">optional.</param>
        void Begin(Func<CancellationToken, Task> action = null, Action<Exception> onError = null);

        /// <summary>
        /// Abort the controller action.
        /// </summary>
        void Abort();

        /// <summary>
        /// Abort and wait for <see cref="Task"/> to complete.
        /// </summary>
        /// <returns></returns>
        Task CancelAsync();

        /// <summary>
        /// Restart the controller.
        /// </summary>
        /// <returns></returns>
        Task RestartAsync();
    }
}
