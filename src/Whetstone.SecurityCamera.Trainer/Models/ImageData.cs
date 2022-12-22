using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whetstone.SecurityCamera.Trainer.Models;

public class ImageData
{

    /// <summary>
    /// Fully qualified path where the image is stored.
    /// </summary>
    public string ImagePath { get; set; }

    /// <summary>
    /// Category the image belongs to. This is the value to predict.
    /// </summary>
    public string Label { get; set; }
}
