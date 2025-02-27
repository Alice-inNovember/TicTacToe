﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Util
{
	public static class TaskExtensions
	{
		public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
		{
			TaskCompletionSource<bool> tcs = new();
			await using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
			{
				if (task != await Task.WhenAny(task, tcs.Task))
					throw new OperationCanceledException(cancellationToken);
			}
			await task;
		}

		public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
		{
			TaskCompletionSource<bool> tcs = new();
			await using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
			{
				if (task != await Task.WhenAny(task, tcs.Task))
					throw new OperationCanceledException(cancellationToken);
			}
			return await task;
		}
	}
}