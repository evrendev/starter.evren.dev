using System.ComponentModel;

namespace EvrenDev.Domain.Common.Enums;

public enum FileType
{
    [Description(".jpg,.png,.jpeg")] Image,
    [Description(".mp4,.mov,.webm")] Video
}
