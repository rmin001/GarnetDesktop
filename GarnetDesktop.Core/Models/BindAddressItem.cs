namespace GarnetDesktop.Core.Models;

public record BindAddressItem(
    string InterfaceName,
    string Address,
    string Type,
    bool IsLoopback,
    bool IsPublic)
{
    public string Display => $"{InterfaceName} ({Address})";
}