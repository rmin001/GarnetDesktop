using CommunityToolkit.Mvvm.Messaging.Messages;
using GarnetDesktop.Core.Models;

namespace GarnetDesktop.Messages;

public sealed class MonitoringUpdateMessage(List<GarnetInstance> value) : ValueChangedMessage<List<GarnetInstance>>(value)
{
}
