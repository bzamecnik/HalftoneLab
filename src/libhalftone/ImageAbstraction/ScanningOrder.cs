using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    public interface ScanningOrder : Module
    {
        void init(int width, int height);
        void getValue(out int x, out int y);
        void next(out int x, out int y);
        bool hasNext();
        IEnumerable<Coordinate<int>> getCoordsEnumerator(int width, int height);
    }

    public class ScanlineScanningOrder : ScanningOrder
    {
        public IEnumerable<Coordinate<int>> getCoordsEnumerator(int width, int height) {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    yield return new Coordinate<int>(x, y);
        }

        public void init(int width, int height) {
            this.width = width;
            this.height = height;
            x = y = 0;
        }

        public void getValue(out int x, out int y) {
            x = this.x;
            y = this.y;
        }

        public void next(out int x, out int y) {
            this.x = (this.x + 1) % width;
            if (this.x == 0) {
                this.y++;
            }
            x = this.x;
            y = this.y;
        }

        public bool hasNext() {
            return (x < width) && (y < height);
        }

        int x, y; // group2 coordinates (from 0)
        int width, height;
    }

    public class SerpentineScanningOrder : ScanningOrder
    {
        public IEnumerable<Coordinate<int>> getCoordsEnumerator(int width, int height) {
            // zig-zag order: odd lines go left to right, even lines right to left
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++)
                    yield return new Coordinate<int>((y % 2 == 1) ? x : width - 1 - x, y);
            }
        }

        public void init(int width, int height) {
            this.width = width;
            this.height = height;
            x = y = 0;
            xDiff = 1;
        }

        public void getValue(out int x, out int y) {
            x = this.x;
            y = this.y;
        }

        public void next(out int x, out int y) {
            this.x = (this.x + xDiff) % width;
            if (this.x == (width - 1)) {
                // end of even line
                this.y++;
                xDiff = -1;
            } else if (this.x == 0) {
                // end of odd line
                this.y++;
                xDiff = 1;
            }
            x = this.x;
            y = this.y;
        }

        public bool hasNext() {
            return (x < width) && (y < height);
        }

        int x, y; // group2 coordinates (from 0)
        int width, height;
        int xDiff; // movement: 1: --->, -1: <---
    }

    public abstract class SFCScanningOrder : ScanningOrder
    {
        // stub

        public IEnumerable<Coordinate<int>> getCoordsEnumerator(int width, int height) {
            yield break;
        }

        public void init(int width, int height) {
        }

        public void getValue(out int x, out int y) {
            x = 0;
            y = 0;
        }

        public void next(out int x, out int y) {
            x = 0;
            y = 0;
        }

        public bool hasNext() {
            return false;
        }
    }
}
