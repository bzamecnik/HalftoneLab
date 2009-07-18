using System;

namespace Halftone
{
    /// <summary>
    /// A halftone algorithm which processes the pixels in continuous groups
    /// (cells), performs a computation on the whole cell and then ouputs
    /// its resulting pixels.
    /// </summary>
    /// <remarks>
    /// This is to differentiate it from other
    /// algorithms where the computation on pixels is performed separately.
    /// </remarks>
    [Serializable]
    public abstract class CellHalftoneMethod : HalftoneMethod
    {
    }

    // TODO:
    // - make a new algorithm:
    //   - traverse the image on an SFC using cells
    //   - compute average or median of each cell -> adaptive threshold
    //   - make thresholding for pixels in that cell
    // - cell sizes could be varied adaptively using image gradient
}
