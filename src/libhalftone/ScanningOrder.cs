using System;
using System.Collections.Generic;

namespace Halftone
{
    public interface ScanningOrder
    {
        IEnumerable<Coords> getCoordsIterator(int width, int height);
    }

    public class ScanlineScanningOrder : ScanningOrder
    {
        public IEnumerable<Coords> getCoordsIterator(int width, int height) {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    yield return new Coords(x, y);
        }
    }

    public class SerpentineScanningOrder : ScanningOrder
    {
        public IEnumerable<Coords> getCoordsIterator(int width, int height) {
            // zig-zag order: odd lines go left to right, even lines right to left
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++)
                    yield return new Coords((y % 2 == 1) ? x : width - 1 - x, y);
            }
        }
    }

    public abstract class SFCScanningOrder : ScanningOrder
    {
        public IEnumerable<Coords> getCoordsIterator(int width, int height) {
            yield break;
        }
    }
}
