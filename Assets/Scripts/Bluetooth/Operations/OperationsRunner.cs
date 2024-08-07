using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostLib;
using GoDice.Bluetooth.Bridge;
using UnityEngine;

namespace GoDice.Bluetooth.Operations
{
    /// Runner is using a stack to schedule operations instead of a queue. 
    /// When we send a bulk of the same commands to all dice (like battery request),
    /// it doesn't matter in what order we schedule them, since they all scheduled almost
    /// the same frame.
    /// 
    /// Yet when we establish connection, it's better to go one by one. E.g. connect to the die,
    /// then execute all initialization for that die, then proceed to connection to the next die.
    /// When we scan, we get all connected dice at once, so we schedule all connections at once.
    /// 
    /// Stack allows us to avoid initialization delay and make it seem smooth.
    /// Also, when disconnection happens unintentionally, we want to execute reconnection operation
    /// to the die immediately. Stack allows it without reorganizing the collection.
    internal partial class OperationsRunner
    {
        // BLE plugin can process only one operation in a time.
        // If another operation is scheduled - current operation might be cancelled. 
        // And you will get "Fail to write characteristic" message.
        private static readonly OperationType[] ExecutionCriticalOperationTypes =
        {
            OperationType.Connection,
            OperationType.ColorRequest,
            OperationType.Initialization
        };

        private readonly BluetoothBridge _bridge;
        private readonly RoutineRunner _routineRunner;
        private readonly ExpireContainer _timeoutAwaiter;

        private Stack<IOperation> _collection = new();
        private bool _routineRunning;
        private IOperation _currentOperation;
        private bool _pruneCurrentOperation;

        public OperationsRunner(BluetoothBridge bridge, RoutineRunner routineRunner, int timeout)
        {
            _bridge = bridge;
            _routineRunner = routineRunner;
            _timeoutAwaiter = new ExpireContainer(timeout);
        }

        public void Schedule(IOperation op)
        {
            if (_collection.Any(o => o.Equals(op)))
            {
                LogOperationDiscarded(op);
                return;
            }

            _collection.Push(op);
            LogOperationScheduled(op);

            if (!_routineRunning)
                _routineRunner.StartRoutine(Run());
        }

        private IEnumerator Run()
        {
            _routineRunning = true;

            while (_collection.Count > 0)
            {
                _currentOperation = _collection.Pop();
                _timeoutAwaiter.Refresh();
                LogPerformStart();
                SafePerform(_currentOperation);

                var isCriticalToWait = ExecutionCriticalOperationTypes.Contains(_currentOperation.Type);
                if (isCriticalToWait)
                    while (!_currentOperation.IsDone)
                    {
                        if (_timeoutAwaiter.IsExpired)
                        {
                            LogOperationTimeouted();
                        }
                        else if (_pruneCurrentOperation)
                        {
                            LogCurrentOperationPruned();
                        }
                        else
                        {
                            yield return null;

                            continue;
                        }

                        _currentOperation.Abort();
                        break;
                    }

                LogIfOperationIsDone(_currentOperation, isCriticalToWait);

                _currentOperation = null;
                _pruneCurrentOperation = false;
            }

            _routineRunning = false;
        }

        private void SafePerform(IOperation op)
        {
            try
            {
                op.Perform(_bridge);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void PruneOperationsByAddress(string address)
        {
            if (_currentOperation != null && _currentOperation.CanBePruned(address))
                _pruneCurrentOperation = true;

            if (_collection.Count == 0)
                return;

            var toPrune = _collection.Where(o => !o.Equals(_currentOperation) && o.CanBePruned(address))
                .ToArray();

            if (toPrune.Length == 0)
                return;

            var list = _collection.ToList();
            list.RemoveAll(o => toPrune.Contains(o));
            list.Reverse();
            _collection = new Stack<IOperation>(list);

            LogPrunedOperations(toPrune);
        }
    }
}