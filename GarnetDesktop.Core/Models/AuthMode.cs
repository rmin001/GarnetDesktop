using System.ComponentModel;

namespace GarnetDesktop.Core.Models;

public enum AuthMode
{
    [Description("No authentication")]
    None,

    [Description("Password based")]
    Password,

    //[Description("ACL")]
    //ACL,

    //[Description("Aad")]
    //Aad,

}
