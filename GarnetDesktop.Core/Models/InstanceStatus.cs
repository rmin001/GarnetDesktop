namespace GarnetDesktop.Core.Models;

public enum InstanceStatus
{
    Unknown = 0,

    NotInstalled = 1,
    Stopped = 2,
    StartPending = 3,
    StopPending = 4,
    Running = 5,
    PausePending = 6,
    Paused = 7,
    ContinuePending = 8
}
