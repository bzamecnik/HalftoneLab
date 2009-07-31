// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Text;

namespace HalftoneLab
{
    /// <summary>
    /// Abstract tileable matrix.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Elements of the matrix are accessed through coordinates modulo
    /// matrix width (or height). The purpose of this class is to provide
    /// convenience functions to its descendants with concrete element
    /// types.
    /// </para>
    /// <para>
    /// The matrix as it was defined is stored for future editing and
    /// acessible via DefinitionMatrix property. In case the matrix has to be
    /// modified somehow for run-time use (eg. normalized) there is the
    /// WorkingMatrix property (which acts as a cache) and the
    /// computeWorkingMatrix() function which transforms the definition matrix
    /// (possibly with additional parameters) into the working matrix.
    /// </para>
    /// <para>
    /// DefinitionMatrix and WorkingMatrix need not to be of the same type,
    /// thus there are two type parameters: DefinitionType and WorkingType.
    /// </para>
    /// <para>
    /// WorkingMatrix, computeWorkingMatrix() and Clone() have to be
    /// implemented in a descendant class.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Matrix element type</typeparam>
    [Serializable]
    public abstract class Matrix<DefinitionType, WorkingType> : Module
    {
        private DefinitionType[,] _definitionMatrix;
        /// <summary>
        /// Definition matrix.
        /// </summary>
        /// <remarks>
        /// Before being stored it is cloned and the working matrix is
        /// computed.
        /// </remarks>
        public DefinitionType[,] DefinitionMatrix {
            get { return _definitionMatrix; }
            set {
                if (value != null) {
                    _definitionMatrix = (DefinitionType[,])value.Clone();
                    computeWorkingMatrix();
                }
            }
        }

        /// <summary>
        /// Working matrix - cache of the definition matrix, possibly
        /// transformed. It is recomputed by computeWorkingMatrix() function.
        /// </summary>
        protected abstract WorkingType[,] WorkingMatrix {
            get;
            set;
        }

        /// <summary>
        /// Access working matrix elements using coordinates modulo matrix
        /// width resp. height.
        /// </summary>
        /// <param name="y">Y coordinate (zero based)</param>
        /// <param name="x">X coordinate (zero based)</param>
        /// <returns></returns>
        public WorkingType this[int y, int x] {
            get {
                return WorkingMatrix[y % Height, x % Width];
            }
            set {
                WorkingMatrix[y % Height, x % Width] = value;
            }
        }

        /// <summary>
        /// Definition matrix height (= max Y coordinate + 1).
        /// </summary>
        public int Height {
            get { return DefinitionMatrix.GetLength(0); }
        }

        /// <summary>
        /// Definition matrix width (= max X coordinate + 1).
        /// </summary>
        public int Width {
            get { return DefinitionMatrix.GetLength(1); }
        }

        /// <summary>
        /// Transform definition matrix into working matrix, which act as a
        /// cache. It has to be called during module initialization.
        /// </summary>
        protected abstract void computeWorkingMatrix();

        /// <summary>
        /// Clone the matrix. Make a shallow copy.
        /// </summary>
        /// <returns>Cloned matrix.</returns>
        public abstract Matrix<DefinitionType, WorkingType> Clone();

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            computeWorkingMatrix();
        }

        /// <summary>
        /// Convert the matrix to a human readable string.
        /// </summary>
        /// <returns>String representaion of the matrix.</returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    sb.AppendFormat("{0} ", this[y, x]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
