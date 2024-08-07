﻿using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Parcel.Types;

namespace Parcel.Neo.Helpers
{
    internal static class ImageSourceHelper
    {
        public static ImageSource ConvertToBitmapImage(Types.Image image)
        {
            // Remark-cz: This is slightly hacky because at the moment we cannot find a reliable way to conver Bitmap directly into WPF recognizable ImageSource and honestly it's API is very sick and I don't want to bother.
            string tempPath = Image.GetTempImagePath();
            image.ConvertParcelImageToBitmap().Save(tempPath);
            return new BitmapImage(new Uri(tempPath));
        }
    }
}
