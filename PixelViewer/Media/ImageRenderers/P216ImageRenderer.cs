﻿using System;

namespace Carina.PixelViewer.Media.ImageRenderers
{
    /// <summary>
    /// <see cref="IImageRenderer"/> which supports rendering image with 16-bit YUV422p based format.
    /// </summary>
    class P216ImageRenderer : BaseYuv422p16ImageRenderer
    {
        public P216ImageRenderer() : base(new ImageFormat(ImageFormatCategory.YUV, "P216", "P216 (16-bit YUV422p)", true, new ImagePlaneDescriptor[] {
            new ImagePlaneDescriptor(2),
            new ImagePlaneDescriptor(2),
            new ImagePlaneDescriptor(2),
        }), 16)
        { }


        // Select UV component.
        protected override void SelectUV(byte uv1, byte uv2, out byte u, out byte v)
        {
            u = uv1;
            v = uv2;
        }
    }
}