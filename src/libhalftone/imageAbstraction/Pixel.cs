//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Halftone
//{
//    public class Pixel
//    {
//        private int _intensity; // 0-255 (black-white)
//        private int _x;
//        private int _y;

//        public int Intensity {
//            get { return _intensity; }
//            set { _intensity = value; }
//        }

//        public int X {
//            get { return _x; }
//            private set { _x = value; }
//        }

//        public int Y {
//            get { return _y; }
//            private set { _y = value; }
//        }

//        public Pixel(int intensity, int x, int y) {
//            Intensity = intensity;
//            X = x;
//            Y = y;
//        }

//        public Pixel(int intensity) : this(intensity, 0, 0) {}

//        public Pixel(Pixel pixel) {
//            this._intensity = pixel._intensity;
//            this._x = pixel._x;
//            this._y = pixel._y;
//        }

//        public Pixel add(Pixel pixel) {
//            _intensity += pixel._intensity;
//            return this;
//        }

//        public Pixel subtract(Pixel pixel)
//        {
//            _intensity -= pixel._intensity;
//            return this;
//        }
        
//        public static Pixel operator + (Pixel lhs, Pixel rhs) {
//            return (new Pixel(lhs)).add(rhs);
//        }

//        public static Pixel operator -(Pixel lhs, Pixel rhs)
//        {
//            return (new Pixel(lhs)).subtract(rhs);
//        }
//    }
//}
