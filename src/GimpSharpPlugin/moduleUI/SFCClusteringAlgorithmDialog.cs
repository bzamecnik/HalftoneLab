using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class SFCClusteringAlgorithmDialog : ConfigDialog
    {
        private SFCClusteringAlgorithm module;
        private Table table;

        public SFCClusteringAlgorithmDialog()
            : this(new SFCClusteringAlgorithm()) { }

        public SFCClusteringAlgorithmDialog(
            SFCClusteringAlgorithm existingModule)
            : base(existingModule)
        {
        }
    }
}
