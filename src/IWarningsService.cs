namespace LostTech.App {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IWarningsService {
        void Warn(Exception cause, string userFriendlyMessage, IReadOnlyDictionary<string, object?>? properties = null);
    }

    public static class WarningsService {
        static IWarningsService? instance;
        public static IWarningsService Default {
            get => instance ?? throw new InvalidOperationException($"{nameof(WarningsService)} is not initialized");
            set {
                if (value is null) throw new ArgumentNullException(nameof(Default));
                if (Interlocked.CompareExchange(ref instance, value, null) != null)
                    throw new InvalidOperationException($"{nameof(WarningsService)} has already been initialized");
            }
        }
    }

    public static class WarningServiceExtensions {
        public static void ReportAsWarning(this IWarningsService warningsService, Exception exception, string prefix = "Warning: ") {
            if (warningsService == null) throw new ArgumentNullException(nameof(warningsService));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            string message = prefix + exception.Message;
            warningsService.Warn(exception, message, properties: new SortedDictionary<string, object?> {
                [nameof(exception.StackTrace)] = exception.StackTrace,
                [nameof(exception.Source)] = exception.Source,
                [nameof(exception.HResult)] = exception.HResult,
                [nameof(exception.InnerException)] = exception.InnerException?.ToString(),
            });
        }

        public static void ReportAsWarning(this IWarningsService warningsService, Task<Exception> potentiallyFailingTask, string prefix = "Warning: ") {
            if (warningsService == null) throw new ArgumentNullException(nameof(warningsService));
            if (potentiallyFailingTask == null)
                throw new ArgumentNullException(nameof(potentiallyFailingTask));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            potentiallyFailingTask.ContinueWith(t => {
                if (t.IsFaulted)
                    warningsService.ReportAsWarning(t.Exception, prefix);
                if (t.IsCompleted && t.Result is not null)
                    warningsService.ReportAsWarning(t.Result, prefix);
            }, TaskScheduler.Default);
        }

        public static void ReportAsWarning(this IWarningsService warningsService, Task potentiallyFailingTask, string prefix = "Warning: ") {
            if (warningsService == null) throw new ArgumentNullException(nameof(warningsService));
            if (potentiallyFailingTask == null)
                throw new ArgumentNullException(nameof(potentiallyFailingTask));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            potentiallyFailingTask.ContinueWith(t => {
                if (t.IsFaulted)
                    warningsService.ReportAsWarning(t.Exception, prefix);
            }, TaskScheduler.Default);
        }
    }
}
