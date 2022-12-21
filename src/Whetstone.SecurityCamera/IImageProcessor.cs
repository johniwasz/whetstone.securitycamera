using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whetstone.CameraMonitor
{
    public interface IImageProcessor
    {
        Task<bool> GetTagsAsync(Stream imageStream, IEnumerable<KeyValuePair<string, double>> tagMinimums);

    }
}
