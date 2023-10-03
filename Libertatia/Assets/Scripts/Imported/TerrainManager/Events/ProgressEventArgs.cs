using System;

public class ProgressEventArgs : EventArgs
{
    public string Caption { get; private set; }
    public string Content { get; private set; }
    public float Progress { get; private set; }

    public ProgressEventArgs(string caption, string content, float progress)
    {
        Caption = caption;
        Content = content;
        Progress = progress;
    }
}
