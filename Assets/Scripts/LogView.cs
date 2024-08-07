using TMPro;
using UnityEngine;

namespace GoDice
{
    internal class LogView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _output;

        private void Awake() => Application.logMessageReceived += OnLog;

        private void OnDestroy() => Application.logMessageReceived -= OnLog;

        private void OnLog(string condition, string stacktrace, LogType type)
        {
            if (type != LogType.Log)
                return;

            _output.text = $"[{Time.realtimeSinceStartup:N5}] {condition}\n{_output.text}";
        }

        public void Clear() => _output.text = string.Empty;
    }
}