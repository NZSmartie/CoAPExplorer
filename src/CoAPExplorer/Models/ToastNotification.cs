using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoAPExplorer.Models
{
    public enum ToastNotificationType
    {
        Debug,
        Information,
        Warning,
        Error,
    }

    public readonly struct ToastNotification
    {
        public readonly string Message;

        public readonly ToastNotificationType Type;

        public readonly IReadOnlyList<(string Label, ReactiveCommand Command)> Actions;

        public ToastNotification(string message, ToastNotificationType type = ToastNotificationType.Information, params (string Label, ReactiveCommand Command)[] actions)
        {
            Message = message;

            Type = type;

            Actions = actions;
        }

        public ToastNotification(string message, ToastNotificationType type = ToastNotificationType.Information, params Tuple<string, ReactiveCommand>[] actions)
        {
            Message = message;

            Type = type;

            Actions = actions.Select(t => (t.Item1, t.Item2)).ToList();
        }
    }
}
