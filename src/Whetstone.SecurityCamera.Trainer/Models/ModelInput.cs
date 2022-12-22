using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whetstone.SecurityCamera.Trainer.Models;

public class ModelInput
{
    /// <summary>
    /// Binary representation of the image. The model expects image data to be of this type for training.
    /// </summary>
    public byte[] Image { get; set; }


    /// <summary>
    /// Numerical representation of the Label
    /// </summary>
    public UInt32 LabelAsKey { get; set; }

    /// <summary>
    /// Fully qualified path where the image is stored.
    /// </summary>
    public string ImagePath { get; set; }


    /// <summary>
    /// Category the image belongs to. This is the value to predict.
    /// </summary>
    public string Label { get; set; }

}
