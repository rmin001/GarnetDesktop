using CommunityToolkit.Mvvm.Messaging.Messages;
using GarnetDesktop.Core.Models;

namespace GarnetDesktop.Messages;

public class OpenEditMessage(GarnetInstance value)
        : ValueChangedMessage<GarnetInstance>(value)
{
}