// DynamicTresholdFilter.cs created with MonoDevelop
// User: bohous at 15:30 26.3.2009
//

using System;
using System.Collections.Generic;
using System.Linq;
using Gimp;

namespace Halftone
{
	public class DynamicTresholdFilter : TresholdFilter
	{
        class TresholdTableRecord : IComparable<TresholdTableRecord> {
            public int intensityRangeStart;
            public double noiseAmplitude; // could be int
            public TresholdMatrix matrix;

            public int CompareTo(TresholdTableRecord other) {
                return intensityRangeStart.CompareTo(other.intensityRangeStart);
            }
        }
        SortedList<int, TresholdTableRecord> tresholdTable;
        public bool NoiseEnabled { get; set; }
        Random randomGenerator = null;
        public Random RandomGenerator {
            get {
                if (randomGenerator == null) {
                    randomGenerator = new Random();
                }
                return randomGenerator;
            }
        }
        
		public DynamicTresholdFilter()
		{
            tresholdTable = new SortedList<int, TresholdTableRecord>();
            NoiseEnabled = false;
		}
		
		protected override int treshold(Pixel pixel)
		{
            TresholdTableRecord record = getTresholdRecord(pixel[0]);
            TresholdMatrix matrix = record.matrix;
            int treshold = matrix[pixel.Y, pixel.X];
            if (NoiseEnabled) {
                // add noise from interval [-amplitude;amplitude)
                treshold += (int)((RandomGenerator.NextDouble() - 0.5) * record.noiseAmplitude);
            }
            return treshold;
		}

        TresholdTableRecord getTresholdRecord(int intensity) {
            // http://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-in-c-on-a-sortedlist
            return tresholdTable.FirstOrDefault(x => x.Key >= intensity).Value;
        }

        // TODO: functions to modify records in tresholdTable
        // Note: such an interface should be available in a Prototype which then
        // creates instances of this class
	}
}
