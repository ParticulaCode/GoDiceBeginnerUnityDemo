using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoDice.Bluetooth.Operations
{
    internal partial class OperationsRunner
    {
        private void LogOperationScheduled(IOperation op) =>
            Log($"{op} scheduled to perform. Queue size: {_collection.Count}");

        private static void LogOperationDiscarded(IOperation op) =>
            Log($"{op} discarded. Same operation is already scheduled.");

        private void LogIfOperationIsDone(IOperation op, bool criticalToWait)
        {
            if (!op.IsDone && criticalToWait)
                return;

            Log($"{op} performed. Queue size: {_collection.Count}.");
        }

        private void LogPerformStart() =>
            Log($"Performing {_currentOperation}. Queue size: {_collection.Count}.");

        private void LogCurrentOperationPruned() =>
            Log($"{_currentOperation} aborted by prune request.");

        private void LogOperationTimeouted() =>
            Log($"{_currentOperation} aborted by timeout [{_timeoutAwaiter.TTL} sec].");

        private static void LogPrunedOperations(IReadOnlyCollection<IOperation> toPrune)
        {
            var stringedOperations = string.Join(",", toPrune.Select(o => o.ToString()));
            Log($"Operations pruned: {toPrune.Count}. {stringedOperations}");
        }

        private static void Log(string message) => Debug.Log($"[OperationsRunner] {message}");
    }
}