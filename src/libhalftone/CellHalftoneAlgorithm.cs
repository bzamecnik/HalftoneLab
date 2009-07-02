using System;

namespace Halftone
{
    /// <summary>
    /// A halftone algorithm which processes the pixels in continuous groups
    /// (called cells), performs a computation on the whole cell and then
    /// ouputs its resulting pixels. This is to differentiate it from other
    /// algorithms where the computation on pixels is performed separately.
    /// </summary>
    [Serializable]
	public abstract class CellHalftoneAlgorithm : HalfoneAlgorithm
	{
	}

    // TODO:
    // - make a new algorithm:
    //   - traverse the image on an SFC using cells
    //   - compute average or median of each cell -> adaptive threshold
    //   - make thresholding for pixels in that cell
    // - cell sizes could be varied adaptively using image gradient
}
