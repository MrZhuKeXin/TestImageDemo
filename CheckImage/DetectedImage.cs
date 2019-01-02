using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckImage {
     class DetectedImage {
        public BitmapImage bitmapImage;
        public bool IsNormal = true;

        public DetectedImage(BitmapImage bitmap) {
            bitmapImage = bitmap;
        }
    }
}
