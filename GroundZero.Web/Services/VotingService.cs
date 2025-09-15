using System.Threading.Channels;

namespace GroundZero.Web.Services;

public static class VotingService
{
    public static Channel<Guid> SomethingHappenedInHackathonChannel = Channel.CreateUnbounded<Guid>(
        new UnboundedChannelOptions
        {
            SingleReader = false
        });
}