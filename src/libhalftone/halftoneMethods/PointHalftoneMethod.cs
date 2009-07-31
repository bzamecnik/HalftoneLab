// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HalftoneLab
{
    // TODO: move the point processing code here

    /// <summary>
    /// Point-wise halftone method which processes the pixels separately.
    /// </summary>
    [Serializable]
    [Module(TypeName = "Point halftone method")]
    public abstract class PointHalftoneMethod : HalftoneMethod
    {
    }
}
