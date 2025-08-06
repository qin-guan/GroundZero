using System.Diagnostics;
using System.Threading.Channels;
using SqlSugar;

namespace GroundZero.Web.Services;

public static class VotingService
{
    public static Channel<Guid> SomethingHappenedInHackathonChannel = Channel.CreateUnbounded<Guid>(
        new UnboundedChannelOptions
        {
            SingleReader = false
        });
}