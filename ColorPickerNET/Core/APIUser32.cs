
//-----------------------------------------------------------------------
// <copyright file="APIUser32.cs" company="Lifeprojects.de">
//     Class: APIUser32
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
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public class APIUser32
    {
        private delegate int GetCursorPosDelegate(out System.Drawing.Point lpPoint);
        private static GetCursorPosDelegate internalGetCursorPos;

        static APIUser32()
        {
            try
            {
                IntPtr hWnd;
                internalGetCursorPos = DLLFunctionLoader.LoadFunction<GetCursorPosDelegate>("user32.dll", "GetCursorPos", out hWnd);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }
        }

        public static int GetCursorPos(out System.Drawing.Point lpPoint)
        {
            int result = -1;

            try
            {
                result = internalGetCursorPos(out lpPoint);
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
