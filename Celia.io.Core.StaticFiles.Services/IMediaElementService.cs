using Celia.io.Core.StaticFiles.Entities;

namespace Celia.io.Core.StaticFiles.Services
{
    public interface IMediaElementService
    {
        string GetDownloadUrl(IMediaElement element);
        string GetPublishUrl(IMediaElement element);

        ActionResult Publish(IMediaElement element);
        ActionResult RevokePublish(IMediaElement element);
    }
}