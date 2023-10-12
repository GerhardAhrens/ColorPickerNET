
//-----------------------------------------------------------------------
// <copyright file="APIGdi32.cs" company="Lifeprojects.de">
//     Class: APIGdi32
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>11.01.2022</date>
//
// <summary>
// Klasse zum aufrufen von Win API Funktionen
// </summary>
//-----------------------------------------------------------------------


namespace ColorPickerNET.Core
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;

    public class APIGdi32
    {
        private delegate int BitBltDelegate(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        private static BitBltDelegate internalBitBlt;

        static APIGdi32()
        {
            try
            {
                IntPtr hWnd;
                internalBitBlt = DLLFunctionLoader.LoadFunction<BitBltDelegate>("gdi32.dll", "BitBlt", out hWnd);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }
        }

        public static int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop)
        {
            int result = -1;

            try
            {
                result = internalBitBlt(hDC, x, y, nWidth, nHeight, hSrcDC, xSrc, ySrc, dwRop);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }

            return result;
        }
    }
}
