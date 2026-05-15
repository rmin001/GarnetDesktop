using CommunityToolkit.Mvvm.ComponentModel;
using GarnetDesktop.Core.Models;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GarnetDesktop.ViewModels;

public partial class NetworkConfigViewModel : ObservableObject
{
    public ObservableCollection<BindAddressItem> BindAddresses { get; } = [];

    [ObservableProperty]
    private BindAddressItem? selectedBindAddress;

    [ObservableProperty]
    private int port = 3278;

    public string BindAddress => SelectedBindAddress?.Address ?? "127.0.0.1";
    public string NicName => SelectedBindAddress?.InterfaceName ?? "Loopback";

    public NetworkConfigViewModel()
    {
        LoadBindAddresses();
    }

    private void LoadBindAddresses()
    {
        BindAddresses.Clear();

        AddSpecialAddresses();
        AddNetworkInterfaces();

        SelectedBindAddress =
            BindAddresses.FirstOrDefault(x => x.Address == "127.0.0.1")
            ?? BindAddresses.FirstOrDefault();
    }

    private void AddSpecialAddresses()
    {
        BindAddresses.Add(new BindAddressItem("Localhost", "127.0.0.1", "IPv4", true, false));
        BindAddresses.Add(new BindAddressItem("Any IPv4", "0.0.0.0", "IPv4", false, true));
        BindAddresses.Add(new BindAddressItem("Any IPv6", "::", "IPv6", false, true));
    }

    private void AddNetworkInterfaces()
    {
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (!IsUsableNic(nic))
                continue;

            var ipProps = nic.GetIPProperties();

            foreach (var addr in ipProps.UnicastAddresses)
            {
                if (addr.Address.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                if (IPAddress.IsLoopback(addr.Address))
                    continue;

                var ip = addr.Address.ToString();

                BindAddresses.Add(new BindAddressItem(
                    nic.Name,
                    ip,
                    "IPv4",
                    IsLoopback: false,
                    IsPublic: IsPublicIp(ip)
                ));
            }
        }
    }

    private static bool IsUsableNic(NetworkInterface nic)
    {
        if (nic.OperationalStatus != OperationalStatus.Up)
            return false;

        if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            return false;

        var text = $"{nic.Name} {nic.Description}".ToLowerInvariant();

        string[] excluded =
        [
            "hyper-v", "virtual", "vmware", "docker",
            "tailscale", "vethernet", "wsl", "vpn", "bluetooth"
        ];

        return excluded.All(x => !text.Contains(x));
    }

    private static bool IsPublicIp(string ip)
    {
        if (!IPAddress.TryParse(ip, out var address))
            return false;

        var b = address.GetAddressBytes();

        return !(b[0] == 10 ||
                 (b[0] == 172 && b[1] >= 16 && b[1] <= 31) ||
                 (b[0] == 192 && b[1] == 168) ||
                 b[0] == 127);
    }

}