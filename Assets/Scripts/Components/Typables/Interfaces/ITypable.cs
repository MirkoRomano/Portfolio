using System;

namespace Portfolio.Shared
{
    public interface ITypable
    {
        string Text { set; get; }
        int TypedErrorCount { get; }
        int TypedLength { get; }
        ReadOnlySpan<char> TypedText { get; }
        bool IsTextCompletelyTyped { get; }
        bool Enabled { get; }
        char PreviousTypedChar { get; }
        char CharToType { get; }
        void ResetText();
    }
}
