// ClusteringDitherAlgorithm.cs created with MonoDevelop
// User: bohous at 15:07 26.3.2009
//

using System;

namespace Halftone
{
	public abstract class ClusteringDitherAlgorithm : DitherAlgorithm
	{
	}

    //public class SFCAdaptiveClusteringDitherAlgorithm : ClusteringDitherAlgorithm
    //{
    //    // error filter (optional)
    //    public ErrorFilter ErrorFilter {
    //        get;
    //        set;
    //    }

    //    public ScanningOrder ScanningOrder {
    //        get;
    //        set;
    //    }

    //    bool ErrorFilterEnabled {
    //        get {
    //            return ErrorFilter != null;
    //        }
    //    }

    //    public override void run(Image image) {
    //        if (ErrorFilterEnabled) {
    //            ErrorFilter.setImageDimensions(image.Height, image.Width);
    //        }
    //        // TODO
    //    }
    //}
}
