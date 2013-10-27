namespace System
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public class QRCodeGenerator
    {
        private int[,] gDataWords;
        private int[,] gECCWords;
        private int[,] gExtraDataWords;
        private int[,] gRSBlockCount;
        private byte[] lDeployOrderX;
        private byte[] lDeployOrderY;
        private byte[] lDeployTableX;
        private byte[] lDeployTableY;
        private byte[] lFormatFirstX;
        private byte[] lFormatFirstY;
        private byte[] lFormatSecondX;
        private byte[] lFormatSecondY;
        private Color lQRBackColorBigMark1;
        private Color lQRBackColorBigMark2;
        private Color lQRBackColorBigMark3;
        private Color lQRBackColorFormatInformation;
        private Color lQRBackColorSmallMark;
        private Color lQRBackColorTimingPattern;
        private Color lQRBackColorVersionInformation;
        private Color lQRColorBigMark1;
        private Color lQRColorBigMark2;
        private Color lQRColorBigMark3;
        private Color lQRColorBigMarkBorder1;
        private Color lQRColorBigMarkBorder2;
        private Color lQRColorBigMarkBorder3;
        private Color lQRColorFormatInformation;
        private Color lQRColorQuitZone;
        private Color lQRColorSmallMark;
        private Color lQRColorTimingPattern;
        private Color lQRColorVersionInformation;
        private int lQRInquireAlphaNumericBits;
        private int lQRInquireBinaryBits;
        private int lQRInquireKanjiBits;
        private int lQRInquireModuleSize;
        private int lQRInquireNumericBits;
        private QRTextTypes lQRInquireTextType;
        private QRVersions lQRInquireVersion;
        private int lQRQuitZone;
        private QRECCRates lQRSetECCRate;
        private QRTextTypes lQRSetTextType;
        private QRVersions lQRSetVersion;
        private Color myBarColor;
        private Color myBackGroundColor;
        private byte[] lRSBlock;
        private Bitmap myQRBackGroundBitmap;
        private string myQRBackGroundFileName;
        private float myQRRotate;
        private Rotates myRotate;

        public QRCodeGenerator()
        {
            this.myBarColor = Color.Black;
            this.myBackGroundColor = Color.Black;
            this.myRotate = Rotates.Rotate0;
            this.myQRRotate = 0f;
            this.myQRBackGroundFileName = "";
            this.lFormatFirstX = new byte[] { 0, 1, 2, 3, 4, 5, 7, 8, 8, 8, 8, 8, 8, 8, 8 };
            this.lFormatFirstY = new byte[] { 8, 8, 8, 8, 8, 8, 8, 8, 7, 5, 4, 3, 2, 1, 0 };
            this.lFormatSecondX = new byte[15];
            this.lFormatSecondY = new byte[15];
            this.lQRColorBigMark1 = Color.Black;
            this.lQRBackColorBigMark1 = Color.White;
            this.lQRColorBigMark2 = Color.Black;
            this.lQRBackColorBigMark2 = Color.White;
            this.lQRColorBigMark3 = Color.Black;
            this.lQRBackColorBigMark3 = Color.White;
            this.lQRColorSmallMark = Color.Black;
            this.lQRBackColorSmallMark = Color.White;
            this.lQRColorBigMarkBorder1 = Color.White;
            this.lQRColorBigMarkBorder2 = Color.White;
            this.lQRColorBigMarkBorder3 = Color.White;
            this.lQRColorTimingPattern = Color.Black;
            this.lQRBackColorTimingPattern = Color.White;
            this.lQRColorVersionInformation = Color.Black;
            this.lQRBackColorVersionInformation = Color.White;
            this.lQRColorFormatInformation = Color.Black;
            this.lQRBackColorFormatInformation = Color.White;
            this.lQRColorQuitZone = Color.White;
            this.lQRQuitZone = 4;
            this.Initialize();
        }



        private bool CheckTextCharAlphaNumeric(string QRTextChar)
        {
            return (((Operators.CompareString(QRTextChar, "0", false) >= 0) & (Operators.CompareString(QRTextChar, "9", false) <= 0)) || (((Operators.CompareString(QRTextChar, "A", false) >= 0) & (Operators.CompareString(QRTextChar, "Z", false) <= 0)) || (((((((((QRTextChar == " ") | (QRTextChar == "$")) | (QRTextChar == "%")) | (QRTextChar == "*")) | (QRTextChar == "+")) | (QRTextChar == "-")) | (QRTextChar == ".")) | (QRTextChar == "/")) | (QRTextChar == ":"))));
        }

        private bool CheckTextCharKanji(string QRTextChar)
        {
            byte[] bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(QRTextChar);
            return ((((bytes[0] >= Convert.ToInt32("81", 0x10)) & (bytes[0] <= Convert.ToInt32("9F", 0x10))) && ((bytes[1] >= Convert.ToInt32("40", 0x10)) & (bytes[1] <= Convert.ToInt32("FF", 0x10)))) || ((((bytes[0] >= Convert.ToInt32("E0", 0x10)) & (bytes[0] <= Convert.ToInt32("EA", 0x10))) && ((bytes[1] >= Convert.ToInt32("40", 0x10)) & (bytes[1] <= Convert.ToInt32("FF", 0x10)))) || (((bytes[0] >= Convert.ToInt32("EB", 0x10)) & (bytes[0] <= Convert.ToInt32("EB", 0x10))) && ((bytes[1] >= Convert.ToInt32("40", 0x10)) & (bytes[1] <= Convert.ToInt32("BF", 0x10))))));
        }

        private bool CheckTextCharNumeric(string QRTextChar)
        {
            return ((Operators.CompareString(QRTextChar, "0", false) >= 0) & (Operators.CompareString(QRTextChar, "9", false) <= 0));
        }

        private void ClearBarcodeArea(float X, float Y, float X2, float Y2, Graphics ev)
        {
            SolidBrush brush = new SolidBrush(Color.White);
            brush.Color = this.myBackGroundColor;
            ev.FillRectangle(brush, X, Y, X2, Y2);
        }


        public void Generate(string code, float X, float Y, int pQuitZoneSize, Graphics ev)
        {
            int num2 = 1;
            bool flag = false;
            bool flag2 = false;
            if (code.Length > 0)
            {
                byte[,] buffer = this.QRGetData(code);

                float length = buffer.GetLength(0);
                if (length > 0f)
                {
                    num2 = pQuitZoneSize;
                    if (num2 > 0)
                    {
                        float num3 = (float) (((num2 * (length + (this.lQRQuitZone * 2f))) * ((Math.Abs(Math.Sin(((this.QRRotate / 360f) * 2f) * 3.1415926535897931)) + Math.Abs(Math.Cos(((this.QRRotate / 360f) * 2f) * 3.1415926535897931))) - 1.0)) / 2.0);
                        X += num3;
                        Y += num3;
                        SolidBrush brush = new SolidBrush(this.BackGroundColor);
                        ev.FillRectangle(brush, (float) (X - num3), (float) (Y - num3), (float) ((2f * num3) + (num2 * (length + (this.lQRQuitZone * 2)))), (float) ((2f * num3) + (num2 * (length + (this.lQRQuitZone * 2)))));
                        if (this.QRRotate == 0f)
                        {
                            if (this.Rotate == Rotates.Rotate90)
                            {
                                ev.TranslateTransform(X, Y);
                                ev.RotateTransform(90f);
                                ev.TranslateTransform(-X, -Y - (num2 * (length + (this.lQRQuitZone * 2))));
                            }
                            if (this.Rotate == Rotates.Rotate180)
                            {
                                ev.TranslateTransform(X, Y);
                                ev.RotateTransform(180f);
                                ev.TranslateTransform(-X - (num2 * (length + (this.lQRQuitZone * 2))), -Y - (num2 * (length + (this.lQRQuitZone * 2))));
                            }
                            if (this.Rotate == Rotates.Rotate270)
                            {
                                ev.TranslateTransform(X, Y);
                                ev.RotateTransform(270f);
                                ev.TranslateTransform(-X - (num2 * (length + (this.lQRQuitZone * 2))), -Y);
                            }
                        }
                        else
                        {
                            float num4 = (num2 * (length + (this.lQRQuitZone * 2))) / 2f;
                            ev.TranslateTransform(X + num4, Y + num4);
                            ev.RotateTransform(this.QRRotate);
                            ev.TranslateTransform(-X - num4, -Y - num4);
                        }
                        brush.Color = this.QRColorQuitZone;
                        if (this.QRBackGroundFileName != "")
                        {
                            ev.DrawImage(this.myQRBackGroundBitmap, X, Y, (float) (num2 * (length + (this.lQRQuitZone * 2))), (float) (num2 * (length + (this.lQRQuitZone * 2))));
                            flag = true;
                        }
                        else
                        {
                            ev.FillRectangle(brush, X, Y, num2 * (length + (this.lQRQuitZone * 2)), num2 * (length + (this.lQRQuitZone * 2)));
                            flag = false;
                        }
                        int num8 = buffer.GetLength(0) - 1;
                        for (int i = 0; i <= num8; i++)
                        {
                            int num9 = buffer.GetLength(1) - 1;
                            for (int j = 0; j <= num9; j++)
                            {
                                byte num7 = buffer[i, j];
                                flag2 = false;
                                if ((num7 & 1) == 1)
                                {
                                    brush.Color = this.BarColor;
                                }
                                else
                                {
                                    brush.Color = this.BackGroundColor;
                                    flag2 = true;
                                }
                                if ((num7 & 2) == 2)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        if ((i < (length / 2f)) & (j < (length / 2f)))
                                        {
                                            brush.Color = this.QRColorBigMark1;
                                        }
                                        else if ((i >= (length / 2f)) & (j < (length / 2f)))
                                        {
                                            brush.Color = this.QRColorBigMark3;
                                        }
                                        else
                                        {
                                            brush.Color = this.QRColorBigMark2;
                                        }
                                    }
                                    else if ((i < (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRBackColorBigMark1;
                                    }
                                    else if ((i >= (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRBackColorBigMark3;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorBigMark2;
                                    }
                                }
                                if ((num7 & 0x10) == 0x10)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorTimingPattern;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorTimingPattern;
                                    }
                                }
                                if ((num7 & 4) == 4)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorSmallMark;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorSmallMark;
                                    }
                                }
                                if (num7 == 8)
                                {
                                    if ((i < (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRColorBigMarkBorder1;
                                    }
                                    else if ((i >= (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRColorBigMarkBorder3;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRColorBigMarkBorder2;
                                    }
                                }
                                if ((num7 & 0x20) == 0x20)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorVersionInformation;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorVersionInformation;
                                    }
                                }
                                if ((num7 & 0x40) == 0x40)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorFormatInformation;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorFormatInformation;
                                    }
                                }
                                if (!flag2 | !flag)
                                {
                                    ev.FillRectangle(brush, X + ((this.lQRQuitZone + i) * num2), Y + ((this.lQRQuitZone + j) * num2), (float) num2, (float) num2);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Generate(byte[] data, float X, float Y, int pQuitZoneSize, Graphics ev)
        {
            int num2 = 1;
            bool flag = false;
            bool flag2 = false;
            if (data.Length > 0)
            {
                byte[,] buffer = this.QRGetData(data);

                float length = buffer.GetLength(0);
                if (length > 0f)
                {
                    num2 = pQuitZoneSize;
                    if (num2 > 0)
                    {
                        float num3 = (float)(((num2 * (length + (this.lQRQuitZone * 2f))) * ((Math.Abs(Math.Sin(((this.QRRotate / 360f) * 2f) * 3.1415926535897931)) + Math.Abs(Math.Cos(((this.QRRotate / 360f) * 2f) * 3.1415926535897931))) - 1.0)) / 2.0);
                        X += num3;
                        Y += num3;
                        SolidBrush brush = new SolidBrush(this.BackGroundColor);
                        ev.FillRectangle(brush, (float)(X - num3), (float)(Y - num3), (float)((2f * num3) + (num2 * (length + (this.lQRQuitZone * 2)))), (float)((2f * num3) + (num2 * (length + (this.lQRQuitZone * 2)))));
                        if (this.QRRotate == 0f)
                        {
                            if (this.Rotate == Rotates.Rotate90)
                            {
                                ev.TranslateTransform(X, Y);
                                ev.RotateTransform(90f);
                                ev.TranslateTransform(-X, -Y - (num2 * (length + (this.lQRQuitZone * 2))));
                            }
                            if (this.Rotate == Rotates.Rotate180)
                            {
                                ev.TranslateTransform(X, Y);
                                ev.RotateTransform(180f);
                                ev.TranslateTransform(-X - (num2 * (length + (this.lQRQuitZone * 2))), -Y - (num2 * (length + (this.lQRQuitZone * 2))));
                            }
                            if (this.Rotate == Rotates.Rotate270)
                            {
                                ev.TranslateTransform(X, Y);
                                ev.RotateTransform(270f);
                                ev.TranslateTransform(-X - (num2 * (length + (this.lQRQuitZone * 2))), -Y);
                            }
                        }
                        else
                        {
                            float num4 = (num2 * (length + (this.lQRQuitZone * 2))) / 2f;
                            ev.TranslateTransform(X + num4, Y + num4);
                            ev.RotateTransform(this.QRRotate);
                            ev.TranslateTransform(-X - num4, -Y - num4);
                        }
                        brush.Color = this.QRColorQuitZone;
                        if (this.QRBackGroundFileName != "")
                        {
                            ev.DrawImage(this.myQRBackGroundBitmap, X, Y, (float)(num2 * (length + (this.lQRQuitZone * 2))), (float)(num2 * (length + (this.lQRQuitZone * 2))));
                            flag = true;
                        }
                        else
                        {
                            ev.FillRectangle(brush, X, Y, num2 * (length + (this.lQRQuitZone * 2)), num2 * (length + (this.lQRQuitZone * 2)));
                            flag = false;
                        }
                        int num8 = buffer.GetLength(0) - 1;
                        for (int i = 0; i <= num8; i++)
                        {
                            int num9 = buffer.GetLength(1) - 1;
                            for (int j = 0; j <= num9; j++)
                            {
                                byte num7 = buffer[i, j];
                                flag2 = false;
                                if ((num7 & 1) == 1)
                                {
                                    brush.Color = this.BarColor;
                                }
                                else
                                {
                                    brush.Color = this.BackGroundColor;
                                    flag2 = true;
                                }
                                if ((num7 & 2) == 2)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        if ((i < (length / 2f)) & (j < (length / 2f)))
                                        {
                                            brush.Color = this.QRColorBigMark1;
                                        }
                                        else if ((i >= (length / 2f)) & (j < (length / 2f)))
                                        {
                                            brush.Color = this.QRColorBigMark3;
                                        }
                                        else
                                        {
                                            brush.Color = this.QRColorBigMark2;
                                        }
                                    }
                                    else if ((i < (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRBackColorBigMark1;
                                    }
                                    else if ((i >= (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRBackColorBigMark3;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorBigMark2;
                                    }
                                }
                                if ((num7 & 0x10) == 0x10)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorTimingPattern;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorTimingPattern;
                                    }
                                }
                                if ((num7 & 4) == 4)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorSmallMark;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorSmallMark;
                                    }
                                }
                                if (num7 == 8)
                                {
                                    if ((i < (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRColorBigMarkBorder1;
                                    }
                                    else if ((i >= (length / 2f)) & (j < (length / 2f)))
                                    {
                                        brush.Color = this.QRColorBigMarkBorder3;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRColorBigMarkBorder2;
                                    }
                                }
                                if ((num7 & 0x20) == 0x20)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorVersionInformation;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorVersionInformation;
                                    }
                                }
                                if ((num7 & 0x40) == 0x40)
                                {
                                    if ((num7 & 1) == 1)
                                    {
                                        brush.Color = this.QRColorFormatInformation;
                                    }
                                    else
                                    {
                                        brush.Color = this.QRBackColorFormatInformation;
                                    }
                                }
                                if (!flag2 | !flag)
                                {
                                    ev.FillRectangle(brush, X + ((this.lQRQuitZone + i) * num2), Y + ((this.lQRQuitZone + j) * num2), (float)num2, (float)num2);
                                }
                            }
                        }
                    }
                }
            }
        }



        private int QRCodeCalcPixel(float pDimension, float pXsize, float pYsize)
        {
            float num2;
            if ((pXsize <= 0f) | (pYsize <= 0f))
            {
                return 0;
            }
            if (pXsize > pYsize)
            {
                num2 = pYsize;
            }
            else
            {
                num2 = pXsize;
            }
            int num = (int) Math.Round((double) ((((double) (num2 / (pDimension + (this.lQRQuitZone * 2f)))) / (Math.Abs(Math.Sin(((this.QRRotate / 360f) * 2f) * 3.1415926535897931)) + Math.Abs(Math.Cos(((this.QRRotate / 360f) * 2f) * 3.1415926535897931)))) - 0.5));
            if (num <= 0)
            {
                return 0;
            }
            return num;
        }

        private byte QRCodeChooseMaskNumber(byte[,] pModuleData, int pExtraBits)
        {
            int length = pModuleData.GetLength(0);
            int[] numArray2 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] numArray3 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] numArray4 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] numArray6 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] numArray5 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] numArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int num = 0;
            int num2 = 0;
            int[] numArray8 = new int[9];
            int[] numArray9 = new int[9];
            bool[] flagArray = new bool[9];
            bool[] flagArray2 = new bool[9];
            int num13 = length - 1;
            for (int i = 0; i <= num13; i++)
            {
                int num8 = 0;
                do
                {
                    numArray8[num8] = 0;
                    numArray9[num8] = 0;
                    flagArray[num8] = false;
                    flagArray2[num8] = false;
                    num8++;
                }
                while (num8 <= 7);
                int num14 = length - 1;
                for (int j = 0; j <= num14; j++)
                {
                    if ((j > 0) & (i > 0))
                    {
                        num = ((pModuleData[j, i] & pModuleData[j - 1, i]) & pModuleData[j, i - 1]) & pModuleData[j - 1, i - 1];
                        num2 = ((pModuleData[j, i] | pModuleData[j - 1, i]) | pModuleData[j, i - 1]) | pModuleData[j - 1, i - 1];
                    }
                    int num10 = 0;
                    do
                    {
                        int num15;
                        numArray8[num10] = ((numArray8[num10] & 0x3f) << 1) | (((byte) (pModuleData[j, i] >> (num10 & 7))) & 1);
                        numArray9[num10] = ((numArray9[num10] & 0x3f) << 1) | (((byte) (pModuleData[i, j] >> (num10 & 7))) & 1);
                        if ((pModuleData[j, i] & (((int) 1) << num10)) != 0)
                        {
                            num15 = num10;
                            numArray6[num15]++;
                        }
                        if (numArray8[num10] == Convert.ToInt32("1011101", 2))
                        {
                            num15 = num10;
                            numArray4[num15] += 40;
                        }
                        if (numArray9[num10] == Convert.ToInt32("1011101", 2))
                        {
                            num15 = num10;
                            numArray4[num15] += 40;
                        }
                        if ((j > 0) & (i > 0))
                        {
                            if (((num & 1) != 0) | ((num2 & 1) == 0))
                            {
                                num15 = num10;
                                numArray3[num15] += 3;
                            }
                            num = num >> 1;
                            num2 = num2 >> 1;
                        }
                        if (((numArray8[num10] & 0x1f) == 0) | ((numArray8[num10] & 0x1f) == 0x1f))
                        {
                            if (j > 3)
                            {
                                if (flagArray[num10])
                                {
                                    num15 = num10;
                                    numArray2[num15]++;
                                }
                                else
                                {
                                    num15 = num10;
                                    numArray2[num15] += 3;
                                    flagArray[num10] = true;
                                }
                            }
                        }
                        else
                        {
                            flagArray[num10] = false;
                        }
                        if (((numArray9[num10] & 0x1f) == 0) | ((numArray9[num10] & 0x1f) == 0x1f))
                        {
                            if (j > 3)
                            {
                                if (flagArray2[num10])
                                {
                                    num15 = num10;
                                    numArray2[num15]++;
                                }
                                else
                                {
                                    num15 = num10;
                                    numArray2[num15] += 3;
                                    flagArray2[num10] = true;
                                }
                            }
                        }
                        else
                        {
                            flagArray2[num10] = false;
                        }
                        num10++;
                    }
                    while (num10 <= 7);
                }
            }
            int num3 = 0;
            byte num5 = 0;
            int[] numArray7 = new int[] { 
                90, 80, 70, 60, 50, 40, 30, 20, 10, 0, 0, 10, 20, 30, 40, 50, 
                60, 70, 80, 90, 90
             };
            int index = 0;
            do
            {
                numArray5[index] = numArray7[(int) Math.Round(Conversion.Int((double) (((double) (20 * numArray6[index])) / ((double) pExtraBits))))];
                numArray[index] = ((numArray2[index] + numArray3[index]) + numArray4[index]) + numArray5[index];
                index++;
            }
            while (index <= 7);
            int num12 = 0;
            do
            {
                if ((numArray[num12] < num3) | (num12 == 0))
                {
                    num5 = (byte) num12;
                    num3 = numArray[num12];
                }
                num12++;
            }
            while (num12 <= 7);
            return num5;
        }

        private void Initialize()
        {
            this.lQRSetECCRate = QRECCRates.Medium15Percent;
            this.lQRSetTextType = QRTextTypes.Automatic;
            this.lQRSetVersion = QRVersions.Automatic;
            this.gDataWords = new int[,] { { 
                0x13, 0x22, 0x37, 80, 0x6c, 0x44, 0x4e, 0x61, 0x74, 0x44, 0x51, 0x5c, 0x6b, 0x73, 0x57, 0x62, 
                0x6b, 120, 0x71, 0x6b, 0x74, 0x6f, 0x79, 0x75, 0x6a, 0x72, 0x7a, 0x75, 0x74, 0x73, 0x73, 0x73, 
                0x73, 0x73, 0x79, 0x79, 0x7a, 0x7a, 0x75, 0x76
             }, { 
                0x10, 0x1c, 0x2c, 0x20, 0x2b, 0x1b, 0x1f, 0x26, 0x24, 0x2b, 50, 0x24, 0x25, 40, 0x29, 0x2d, 
                0x2e, 0x2b, 0x2c, 0x29, 0x2a, 0x2e, 0x2f, 0x2d, 0x2f, 0x2e, 0x2d, 0x2d, 0x2d, 0x2f, 0x2e, 0x2e, 
                0x2e, 0x2e, 0x2f, 0x2f, 0x2e, 0x2e, 0x2f, 0x2f
             }, { 
                13, 0x16, 0x11, 0x18, 15, 0x13, 14, 0x12, 0x10, 0x13, 0x16, 20, 20, 0x10, 0x18, 0x13, 
                0x16, 0x16, 0x15, 0x18, 0x16, 0x18, 0x18, 0x18, 0x18, 0x16, 0x17, 0x18, 0x17, 0x18, 0x18, 0x18, 
                0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18
             }, { 
                9, 0x10, 13, 9, 11, 15, 13, 14, 12, 15, 12, 14, 11, 12, 12, 15, 
                14, 14, 13, 15, 0x10, 13, 15, 0x10, 15, 0x10, 15, 15, 15, 15, 15, 15, 
                15, 0x10, 15, 15, 15, 15, 15, 15
             } };
            this.gExtraDataWords = new int[,] { { 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 1, 1, 1, 
                5, 1, 4, 5, 4, 7, 5, 4, 4, 2, 4, 10, 7, 10, 3, 0, 
                1, 6, 7, 14, 4, 0x12, 4, 6
             }, { 
                0, 0, 0, 0, 0, 0, 0, 2, 2, 1, 4, 2, 1, 5, 5, 3, 
                1, 4, 11, 13, 0, 0, 14, 14, 13, 4, 3, 0x17, 7, 10, 0x1d, 0x17, 
                0x15, 0x17, 0x1a, 0x22, 14, 0x20, 7, 0x1f
             }, { 
                0, 0, 0, 0, 2, 0, 4, 2, 4, 2, 4, 6, 4, 5, 7, 2, 
                15, 1, 4, 5, 6, 0x10, 14, 0x10, 0x16, 6, 0x1a, 0x1f, 0x25, 0x19, 1, 0x23, 
                0x13, 7, 14, 10, 10, 14, 0x16, 0x22
             }, { 
                0, 0, 0, 0, 2, 0, 1, 2, 4, 2, 8, 4, 4, 5, 7, 13, 
                0x11, 0x13, 0x10, 10, 6, 0, 14, 2, 13, 4, 0x1c, 0x1f, 0x1a, 0x19, 0x1c, 0x23, 
                0x2e, 1, 0x29, 0x40, 0x2e, 0x20, 0x43, 0x3d
             } };
            this.gECCWords = new int[,] { { 
                7, 10, 15, 20, 0x1a, 0x12, 20, 0x18, 30, 0x12, 20, 0x18, 0x1a, 30, 0x16, 0x18, 
                0x1c, 30, 0x1c, 0x1c, 0x1c, 0x1c, 30, 30, 0x1a, 0x1c, 30, 30, 30, 30, 30, 30, 
                30, 30, 30, 30, 30, 30, 30, 30
             }, { 
                10, 0x10, 0x1a, 0x12, 0x18, 0x10, 0x12, 0x16, 0x16, 0x1a, 30, 0x16, 0x16, 0x18, 0x18, 0x1c, 
                0x1c, 0x1a, 0x1a, 0x1a, 0x1a, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 
                0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c, 0x1c
             }, { 
                13, 0x16, 0x12, 0x1a, 0x12, 0x18, 0x12, 0x16, 20, 0x18, 0x1c, 0x1a, 0x18, 20, 30, 0x18, 
                0x1c, 0x1c, 0x1a, 30, 0x1c, 30, 30, 30, 30, 0x1c, 30, 30, 30, 30, 30, 30, 
                30, 30, 30, 30, 30, 30, 30, 30
             }, { 
                0x11, 0x1c, 0x16, 0x10, 0x16, 0x1c, 0x1a, 0x1a, 0x18, 0x1c, 0x18, 0x1c, 0x16, 0x18, 0x18, 30, 
                0x1c, 0x1c, 0x1a, 0x1c, 30, 0x18, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 
                30, 30, 30, 30, 30, 30, 30, 30
             } };
            this.gRSBlockCount = new int[,] { { 
                1, 1, 1, 1, 1, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 
                6, 6, 7, 8, 8, 9, 9, 10, 12, 12, 12, 13, 14, 15, 0x10, 0x11, 
                0x12, 0x13, 0x13, 20, 0x15, 0x16, 0x18, 0x19
             }, { 
                1, 1, 1, 2, 2, 4, 4, 4, 5, 5, 5, 8, 9, 9, 10, 10, 
                11, 13, 14, 0x10, 0x11, 0x11, 0x12, 20, 0x15, 0x17, 0x19, 0x1a, 0x1c, 0x1d, 0x1f, 0x21, 
                0x23, 0x25, 0x26, 40, 0x2b, 0x2d, 0x2f, 0x31
             }, { 
                1, 1, 2, 2, 4, 4, 6, 6, 8, 8, 8, 10, 12, 0x10, 12, 0x11, 
                0x10, 0x12, 0x15, 20, 0x17, 0x17, 0x19, 0x1b, 0x1d, 0x22, 0x22, 0x23, 0x26, 40, 0x2b, 0x2d, 
                0x30, 0x33, 0x35, 0x38, 0x3b, 0x3e, 0x41, 0x44
             }, { 
                1, 1, 2, 4, 4, 4, 5, 6, 8, 8, 11, 11, 0x10, 0x10, 0x12, 0x10, 
                0x13, 0x15, 0x19, 0x19, 0x19, 0x22, 30, 0x20, 0x23, 0x25, 40, 0x2a, 0x2d, 0x30, 0x33, 0x36, 
                0x39, 60, 0x3f, 0x42, 70, 0x4a, 0x4d, 0x51
             } };
        }

        private int QRCodeGet1DimensionAddress(int pX, int pY, int pSymbolSize)
        {
            return (pX + (pY * (pSymbolSize + 1)));
        }

        private byte[] QRCodeGet8BitData(int[] pInputData, byte[] pInputDataBits, int pMaxBytes)
        {
            bool flag;
            byte[] buffer = new byte[(pMaxBytes - 1) + 1];
            int length = pInputDataBits.Length;
            int index = 0;
            int num3 = 8;
            int num2 = 0;
            int num10 = length - 1;
            for (int i = 0; i <= num10; i++)
            {
                num2 += pInputDataBits[i];
            }
            int num11 = (int) Math.Round((double) ((Conversion.Int((double) (((double) (num2 - 1)) / 8.0)) + 1.0) - 1.0));
            for (int j = 0; j <= num11; j++)
            {
                buffer[j] = 0;
            }
            int num12 = length - 1;
            for (int k = 0; k <= num12; k++)
            {
                int num5 = pInputData[k];
                int num6 = pInputDataBits[k];
                flag = true;
                if (num6 != 0)
                {
                    goto Label_00FE;
                }
                break;
            Label_009C:
                if (num3 > num6)
                {
                    buffer[index] = (byte) (((byte) (buffer[index] << (num6 & 7))) | num5);
                    num3 -= num6;
                    flag = false;
                }
                else
                {
                    num6 -= num3;
                    buffer[index] = (byte) (((byte) (buffer[index] << (num3 & 7))) | (num5 >> num6));
                    if (num6 == 0)
                    {
                        flag = false;
                    }
                    else
                    {
                        num5 &= (((int) 1) << num6) - 1;
                        flag = true;
                    }
                    index++;
                    num3 = 8;
                }
            Label_00FE:
                if (flag)
                {
                    goto Label_009C;
                }
            }
            if (num3 != 8)
            {
                buffer[index] = (byte) (buffer[index] << (num3 & 7));
            }
            else
            {
                index--;
            }
            if (index < (pMaxBytes - 1))
            {
                for (flag = true; index < (pMaxBytes - 1); flag = !flag)
                {
                    index++;
                    if (flag)
                    {
                        buffer[index] = 0xec;
                    }
                    else
                    {
                        buffer[index] = 0x11;
                    }
                }
            }
            return buffer;
        }

        private byte[] QRCodeGetByteData(byte[] pInput1, byte[] pInput2)
        {
            byte[] buffer2;
            byte[] buffer3;
            if (pInput1.Length > pInput2.Length)
            {
                buffer2 = (byte[]) pInput1.Clone();
                buffer3 = (byte[]) pInput2.Clone();
            }
            else
            {
                buffer2 = (byte[]) pInput2.Clone();
                buffer3 = (byte[]) pInput1.Clone();
            }
            int length = buffer2.Length;
            int num2 = buffer3.Length;
            byte[] buffer = new byte[length + 1];
            int num4 = length - 1;
            for (int i = 0; i <= num4; i++)
            {
                if (i < num2)
                {
                    buffer[i] = (byte) (buffer2[i] ^ buffer3[i]);
                }
                else
                {
                    buffer[i] = buffer2[i];
                }
            }
            return buffer;
        }

        private byte[] QRCodeGetFormatX(int pVersion)
        {
            byte[] buffer = new byte[0x10];
            int index = 0;
            do
            {
                buffer[index] = 8;
                index++;
            }
            while (index <= 6);
            int num2 = 0;
            do
            {
                buffer[num2 + 7] = (byte) (((4 * pVersion) + 9) + num2);
                num2++;
            }
            while (num2 <= 7);
            return buffer;
        }

        private byte[] QRCodeGetFormatY(int pVersion)
        {
            byte[] buffer = new byte[0x10];
            int num = 0;
            do
            {
                buffer[num + 7] = 8;
                num++;
            }
            while (num <= 7);
            int index = 0;
            do
            {
                buffer[index] = (byte) (((4 * pVersion) + 0x10) - index);
                index++;
            }
            while (index <= 6);
            return buffer;
        }

        private bool QRCodeGetMaskBit(int pMaskPatternNo, int pY, int pX)
        {
            switch (pMaskPatternNo)
            {
                case 0:
                    if (((pX + pY) % 2) != 0)
                    {
                        return false;
                    }
                    return true;

                case 1:
                    if ((pX % 2) != 0)
                    {
                        return false;
                    }
                    return true;

                case 2:
                    if ((pY % 3) != 0)
                    {
                        return false;
                    }
                    return true;

                case 3:
                    if (((pX + pY) % 3) != 0)
                    {
                        return false;
                    }
                    return true;

                case 4:
                    if (((Conversion.Int((double) (((double) pX) / 2.0)) + Conversion.Int((double) (((double) pY) / 3.0))) % 2.0) != 0.0)
                    {
                        return false;
                    }
                    return true;

                case 5:
                    if ((((pX * pY) % 2) + ((pX * pY) % 3)) != 0)
                    {
                        return false;
                    }
                    return true;

                case 6:
                    if (((((pX * pY) % 2) + ((pX * pY) % 3)) % 2) != 0)
                    {
                        return false;
                    }
                    return true;

                case 7:
                    if (((((pX * pY) % 3) + ((pX + pY) % 2)) % 2) != 0)
                    {
                        return false;
                    }
                    return true;
            }
            return false;
        }

        private int QRCodeGetMaskBitGroup(int pY, int pX)
        {
            int num = 0;
            if (this.QRCodeGetMaskBit(0, pY, pX))
            {
                num = (int) Math.Round((double) (num + 1.0));
            }
            if (this.QRCodeGetMaskBit(1, pY, pX))
            {
                num = (int) Math.Round((double) (num + 2.0));
            }
            if (this.QRCodeGetMaskBit(2, pY, pX))
            {
                num = (int) Math.Round((double) (num + 4.0));
            }
            if (this.QRCodeGetMaskBit(3, pY, pX))
            {
                num = (int) Math.Round((double) (num + 8.0));
            }
            if (this.QRCodeGetMaskBit(4, pY, pX))
            {
                num = (int) Math.Round((double) (num + 16.0));
            }
            if (this.QRCodeGetMaskBit(5, pY, pX))
            {
                num = (int) Math.Round((double) (num + 32.0));
            }
            if (this.QRCodeGetMaskBit(6, pY, pX))
            {
                num = (int) Math.Round((double) (num + 64.0));
            }
            if (this.QRCodeGetMaskBit(7, pY, pX))
            {
                num = (int) Math.Round((double) (num + 128.0));
            }
            return num;
        }

        private byte[] QRCodeGetSymbolData(int pVersion)
        {
            byte[] buffer;
            int num5=0;
            int num6=0;
            int pSymbolSize = (pVersion * 4) + 0x11;
            int num4 = pSymbolSize * pSymbolSize;
            byte[] buffer2 = new byte[] { 3, (byte) ((pSymbolSize - 1) - 3) };
            switch (pVersion)
            {
                case 1:
                    buffer = new byte[] { 6 };
                    break;

                case 2:
                    buffer = new byte[] { 6, 0x12 };
                    break;

                case 3:
                    buffer = new byte[] { 6, 0x16 };
                    break;

                case 4:
                    buffer = new byte[] { 6, 0x1a };
                    break;

                case 5:
                    buffer = new byte[] { 6, 30 };
                    break;

                case 6:
                    buffer = new byte[] { 6, 0x22 };
                    break;

                case 7:
                    buffer = new byte[] { 6, 0x16, 0x26 };
                    break;

                case 8:
                    buffer = new byte[] { 6, 0x18, 0x2a };
                    break;

                case 9:
                    buffer = new byte[] { 6, 0x1a, 0x2e };
                    break;

                case 10:
                    buffer = new byte[] { 6, 0x1c, 50 };
                    break;

                case 11:
                    buffer = new byte[] { 6, 30, 0x36 };
                    break;

                case 12:
                    buffer = new byte[] { 6, 0x20, 0x3a };
                    break;

                case 13:
                    buffer = new byte[] { 6, 0x22, 0x3e };
                    break;

                case 14:
                    buffer = new byte[] { 6, 0x1a, 0x2e, 0x42 };
                    break;

                case 15:
                    buffer = new byte[] { 6, 0x1a, 0x30, 70 };
                    break;

                case 0x10:
                    buffer = new byte[] { 6, 0x1a, 50, 0x4a };
                    break;

                case 0x11:
                    buffer = new byte[] { 6, 30, 0x36, 0x4e };
                    break;

                case 0x12:
                    buffer = new byte[] { 6, 30, 0x38, 0x52 };
                    break;

                case 0x13:
                    buffer = new byte[] { 6, 30, 0x3a, 0x56 };
                    break;

                case 20:
                    buffer = new byte[] { 6, 0x22, 0x3e, 90 };
                    break;

                case 0x15:
                    buffer = new byte[] { 6, 0x1c, 50, 0x48, 0x5e };
                    break;

                case 0x16:
                    buffer = new byte[] { 6, 0x1a, 50, 0x4a, 0x62 };
                    break;

                case 0x17:
                    buffer = new byte[] { 6, 30, 0x36, 0x4e, 0x66 };
                    break;

                case 0x18:
                    buffer = new byte[] { 6, 0x1c, 0x36, 80, 0x6a };
                    break;

                case 0x19:
                    buffer = new byte[] { 6, 0x20, 0x3a, 0x54, 110 };
                    break;

                case 0x1a:
                    buffer = new byte[] { 6, 30, 0x3a, 0x56, 0x72 };
                    break;

                case 0x1b:
                    buffer = new byte[] { 6, 0x22, 0x3e, 90, 0x76 };
                    break;

                case 0x1c:
                    buffer = new byte[] { 6, 0x1a, 50, 0x4a, 0x62, 0x7a };
                    break;

                case 0x1d:
                    buffer = new byte[] { 6, 30, 0x36, 0x4e, 0x66, 0x7e };
                    break;

                case 30:
                    buffer = new byte[] { 6, 0x1a, 0x34, 0x4e, 0x68, 130 };
                    break;

                case 0x1f:
                    buffer = new byte[] { 6, 30, 0x38, 0x52, 0x6c, 0x86 };
                    break;

                case 0x20:
                    buffer = new byte[] { 6, 0x22, 60, 0x56, 0x70, 0x8a };
                    break;

                case 0x21:
                    buffer = new byte[] { 6, 30, 0x3a, 0x56, 0x72, 0x8e };
                    break;

                case 0x22:
                    buffer = new byte[] { 6, 0x22, 0x3e, 90, 0x76, 0x92 };
                    break;

                case 0x23:
                    buffer = new byte[] { 6, 30, 0x36, 0x4e, 0x66, 0x7e, 150 };
                    break;

                case 0x24:
                    buffer = new byte[] { 6, 0x18, 50, 0x4c, 0x66, 0x80, 0x9a };
                    break;

                case 0x25:
                    buffer = new byte[] { 6, 0x1c, 0x36, 80, 0x6a, 0x84, 0x9e };
                    break;

                case 0x26:
                    buffer = new byte[] { 6, 0x20, 0x3a, 0x54, 110, 0x88, 0xa2 };
                    break;

                case 0x27:
                    buffer = new byte[] { 6, 0x1a, 0x36, 0x52, 110, 0x8a, 0xa6 };
                    break;

                case 40:
                    buffer = new byte[] { 6, 30, 0x3a, 0x56, 0x72, 0x8e, 170 };
                    break;

                default:
                    buffer = new byte[] { 0 };
                    break;
            }
            int num = pSymbolSize * (pSymbolSize + 1);
            byte[] buffer3 = new byte[(num - 1) + 1];
            int num29 = buffer3.Length - 1;
            for (int i = 0; i <= num29; i++)
            {
                buffer3[i] = 0;
            }
            int num2 = this.QRCodeGet1DimensionAddress(num5, num6, pSymbolSize);
            int num30 = buffer2.Length - 1;
            for (int j = 0; j <= num30; j++)
            {
                int num31 = buffer2.Length - 1;
                for (int n = 0; n <= num31; n++)
                {
                    int pX = buffer2[j];
                    int pY = buffer2[n];
                    if (!((j == (buffer2.Length - 1)) & (n == (buffer2.Length - 1))))
                    {
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 1, pY - 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX, pY - 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 1, pY - 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 1, pY, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX, pY, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 1, pY, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 1, pY + 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX, pY + 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 1, pY + 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 3, pY - 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 2, pY - 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 1, pY - 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX, pY - 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 1, pY - 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 2, pY - 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 3, pY - 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 3, pY + 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 2, pY + 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 1, pY + 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX, pY + 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 1, pY + 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 2, pY + 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 3, pY + 3, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 3, pY - 2, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 3, pY - 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 3, pY, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 3, pY + 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 3, pY + 2, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 3, pY - 2, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 3, pY - 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 3, pY, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 3, pY + 1, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 3, pY + 2, pSymbolSize)] = 3;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 2, pY - 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 1, pY - 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX, pY - 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 1, pY - 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 2, pY - 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 2, pY + 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 1, pY + 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX, pY + 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 1, pY + 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 2, pY + 2, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 2, pY - 1, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 2, pY, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX - 2, pY + 1, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 2, pY - 1, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 2, pY, pSymbolSize)] = 2;
                        buffer3[this.QRCodeGet1DimensionAddress(pX + 2, pY + 1, pSymbolSize)] = 2;
                        if ((j == 0) & (n == 0))
                        {
                            int num12 = -3;
                            do
                            {
                                buffer3[this.QRCodeGet1DimensionAddress(pX + 4, pY + num12, pSymbolSize)] = 8;
                                num12++;
                            }
                            while (num12 <= 4);
                            int num13 = -3;
                            do
                            {
                                buffer3[this.QRCodeGet1DimensionAddress(pX + num13, pY + 4, pSymbolSize)] = 8;
                                num13++;
                            }
                            while (num13 <= 4);
                        }
                        if ((j == 0) & (n == 1))
                        {
                            int num14 = -4;
                            do
                            {
                                buffer3[this.QRCodeGet1DimensionAddress(pX + 4, pY + num14, pSymbolSize)] = 8;
                                num14++;
                            }
                            while (num14 <= 3);
                            int num15 = -3;
                            do
                            {
                                buffer3[this.QRCodeGet1DimensionAddress(pX - num15, pY - 4, pSymbolSize)] = 8;
                                num15++;
                            }
                            while (num15 <= 4);
                        }
                        if ((j == 1) & (n == 0))
                        {
                            int num16 = -3;
                            do
                            {
                                buffer3[this.QRCodeGet1DimensionAddress(pX - 4, pY + num16, pSymbolSize)] = 8;
                                num16++;
                            }
                            while (num16 <= 4);
                            int num17 = -4;
                            do
                            {
                                buffer3[this.QRCodeGet1DimensionAddress(pX + num17, pY + 4, pSymbolSize)] = 8;
                                num17++;
                            }
                            while (num17 <= 3);
                        }
                    }
                }
            }
            int num32 = buffer.Length - 1;
            for (int k = 0; k <= num32; k++)
            {
                int num33 = buffer.Length - 1;
                for (int num21 = 0; num21 <= num33; num21++)
                {
                    int num19 = buffer[k];
                    int num20 = buffer[num21];
                    if (!((((k == 0) & (num21 == 0)) | ((k == 0) & (num21 == (buffer.Length - 1)))) | ((num21 == 0) & (k == (buffer.Length - 1)))))
                    {
                        buffer3[this.QRCodeGet1DimensionAddress(num19, num20, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 2, num20 - 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 1, num20 - 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19, num20 - 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 1, num20 - 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 2, num20 - 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 2, num20 + 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 1, num20 + 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19, num20 + 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 1, num20 + 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 2, num20 + 2, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 2, num20 - 1, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 2, num20, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 2, num20 + 1, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 2, num20 - 1, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 2, num20, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 2, num20 + 1, pSymbolSize)] = 5;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 1, num20 - 1, pSymbolSize)] = 4;
                        buffer3[this.QRCodeGet1DimensionAddress(num19, num20 - 1, pSymbolSize)] = 4;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 1, num20 - 1, pSymbolSize)] = 4;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 1, num20 + 1, pSymbolSize)] = 4;
                        buffer3[this.QRCodeGet1DimensionAddress(num19, num20 + 1, pSymbolSize)] = 4;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 1, num20 + 1, pSymbolSize)] = 4;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 - 1, num20, pSymbolSize)] = 4;
                        buffer3[this.QRCodeGet1DimensionAddress(num19 + 1, num20, pSymbolSize)] = 4;
                    }
                }
            }
            int num34 = pSymbolSize - 9;
            for (int m = 8; m <= num34; m += 2)
            {
                buffer3[this.QRCodeGet1DimensionAddress(m, 6, pSymbolSize)] = (byte) (0x11 | buffer3[this.QRCodeGet1DimensionAddress(m, 6, pSymbolSize)]);
                buffer3[this.QRCodeGet1DimensionAddress(6, m, pSymbolSize)] = (byte) (0x11 | buffer3[this.QRCodeGet1DimensionAddress(6, m, pSymbolSize)]);
                if (m != (pSymbolSize - 9))
                {
                    buffer3[this.QRCodeGet1DimensionAddress(m + 1, 6, pSymbolSize)] = (byte) (0x10 | buffer3[this.QRCodeGet1DimensionAddress(m + 1, 6, pSymbolSize)]);
                    buffer3[this.QRCodeGet1DimensionAddress(6, m + 1, pSymbolSize)] = (byte) (0x10 | buffer3[this.QRCodeGet1DimensionAddress(6, m + 1, pSymbolSize)]);
                }
            }
            buffer3[this.QRCodeGet1DimensionAddress(8, pSymbolSize - 8, pSymbolSize)] = 0x81;
            if (pVersion >= 7)
            {
                int[] numArray = new int[0x22];
                numArray = new int[] { 
                    0x7c94, 0x85bc, 0x9a99, 0xa4d3, 0xbbf6, 0xc762, 0xd847, 0xe60d, 0xf928, 0x10b78, 0x1145d, 0x12a17, 0x13532, 0x149a6, 0x15683, 0x168c9, 
                    0x177ec, 0x18ec4, 0x191e1, 0x1afab, 0x1b08e, 0x1cc1a, 0x1d33f, 0x1ed75, 0x1f250, 0x209d5, 0x216f0, 0x228ba, 0x2379f, 0x24b0b, 0x2542e, 0x26a64, 
                    0x27541, 0x28c69
                 };
                int num23 = numArray[pVersion - 7];
                int num24 = 0;
                do
                {
                    int num25 = 0;
                    do
                    {
                        int num26 = num23 & 1;
                        if (num26 == 1)
                        {
                            buffer3[this.QRCodeGet1DimensionAddress((pSymbolSize - 11) + num25, num24, pSymbolSize)] = (byte) (0x21 | buffer3[this.QRCodeGet1DimensionAddress((pSymbolSize - 11) + num25, num24, pSymbolSize)]);
                            buffer3[this.QRCodeGet1DimensionAddress(num24, (pSymbolSize - 11) + num25, pSymbolSize)] = (byte) (0x21 | buffer3[this.QRCodeGet1DimensionAddress(num24, (pSymbolSize - 11) + num25, pSymbolSize)]);
                        }
                        else
                        {
                            buffer3[this.QRCodeGet1DimensionAddress((pSymbolSize - 11) + num25, num24, pSymbolSize)] = (byte) (0x20 | buffer3[this.QRCodeGet1DimensionAddress((pSymbolSize - 11) + num25, num24, pSymbolSize)]);
                            buffer3[this.QRCodeGet1DimensionAddress(num24, (pSymbolSize - 11) + num25, pSymbolSize)] = (byte) (0x20 | buffer3[this.QRCodeGet1DimensionAddress(num24, (pSymbolSize - 11) + num25, pSymbolSize)]);
                        }
                        num23 = num23 >> 1;
                        num25++;
                    }
                    while (num25 <= 2);
                    num24++;
                }
                while (num24 <= 5);
            }
            this.lFormatSecondX = this.QRCodeGetFormatX((int) this.lQRInquireVersion);
            this.lFormatSecondY = this.QRCodeGetFormatY((int) this.lQRInquireVersion);
            int index = 0;
            do
            {
                buffer3[this.QRCodeGet1DimensionAddress(this.lFormatFirstX[index], this.lFormatFirstY[index], pSymbolSize)] = (byte) (0x40 | buffer3[this.QRCodeGet1DimensionAddress(this.lFormatFirstX[index], this.lFormatFirstY[index], pSymbolSize)]);
                buffer3[this.QRCodeGet1DimensionAddress(this.lFormatSecondX[index], this.lFormatSecondY[index], pSymbolSize)] = (byte) (0x40 | buffer3[this.QRCodeGet1DimensionAddress(this.lFormatSecondX[index], this.lFormatSecondY[index], pSymbolSize)]);
                index++;
            }
            while (index <= 14);
            return buffer3;
        }

        private byte[] QRCodeGetTotalData(byte[] pInputData, byte pEcc, int pData, int pTotal)
        {
            byte[,] buffer5 = new byte[0x100, (pEcc - 1) + 1];
            int num2 = this.gDataWords[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            int num4 = this.gExtraDataWords[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            float num3 = this.gECCWords[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            int num7 = this.gRSBlockCount[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            byte[] buffer = new byte[] { 
                1, 2, 4, 8, 0x10, 0x20, 0x40, 0x80, 0x1d, 0x3a, 0x74, 0xe8, 0xcd, 0x87, 0x13, 0x26, 
                0x4c, 0x98, 0x2d, 90, 180, 0x75, 0xea, 0xc9, 0x8f, 3, 6, 12, 0x18, 0x30, 0x60, 0xc0, 
                0x9d, 0x27, 0x4e, 0x9c, 0x25, 0x4a, 0x94, 0x35, 0x6a, 0xd4, 0xb5, 0x77, 0xee, 0xc1, 0x9f, 0x23, 
                70, 140, 5, 10, 20, 40, 80, 160, 0x5d, 0xba, 0x69, 210, 0xb9, 0x6f, 0xde, 0xa1, 
                0x5f, 190, 0x61, 0xc2, 0x99, 0x2f, 0x5e, 0xbc, 0x65, 0xca, 0x89, 15, 30, 60, 120, 240, 
                0xfd, 0xe7, 0xd3, 0xbb, 0x6b, 0xd6, 0xb1, 0x7f, 0xfe, 0xe1, 0xdf, 0xa3, 0x5b, 0xb6, 0x71, 0xe2, 
                0xd9, 0xaf, 0x43, 0x86, 0x11, 0x22, 0x44, 0x88, 13, 0x1a, 0x34, 0x68, 0xd0, 0xbd, 0x67, 0xce, 
                0x81, 0x1f, 0x3e, 0x7c, 0xf8, 0xed, 0xc7, 0x93, 0x3b, 0x76, 0xec, 0xc5, 0x97, 0x33, 0x66, 0xcc, 
                0x85, 0x17, 0x2e, 0x5c, 0xb8, 0x6d, 0xda, 0xa9, 0x4f, 0x9e, 0x21, 0x42, 0x84, 0x15, 0x2a, 0x54, 
                0xa8, 0x4d, 0x9a, 0x29, 0x52, 0xa4, 0x55, 170, 0x49, 0x92, 0x39, 0x72, 0xe4, 0xd5, 0xb7, 0x73, 
                230, 0xd1, 0xbf, 0x63, 0xc6, 0x91, 0x3f, 0x7e, 0xfc, 0xe5, 0xd7, 0xb3, 0x7b, 0xf6, 0xf1, 0xff, 
                0xe3, 0xdb, 0xab, 0x4b, 150, 0x31, 0x62, 0xc4, 0x95, 0x37, 110, 220, 0xa5, 0x57, 0xae, 0x41, 
                130, 0x19, 50, 100, 200, 0x8d, 7, 14, 0x1c, 0x38, 0x70, 0xe0, 0xdd, 0xa7, 0x53, 0xa6, 
                0x51, 0xa2, 0x59, 0xb2, 0x79, 0xf2, 0xf9, 0xef, 0xc3, 0x9b, 0x2b, 0x56, 0xac, 0x45, 0x8a, 9, 
                0x12, 0x24, 0x48, 0x90, 0x3d, 0x7a, 0xf4, 0xf5, 0xf7, 0xf3, 0xfb, 0xeb, 0xcb, 0x8b, 11, 0x16, 
                0x2c, 0x58, 0xb0, 0x7d, 250, 0xe9, 0xcf, 0x83, 0x1b, 0x36, 0x6c, 0xd8, 0xad, 0x47, 0x8e, 1
             };
            byte[] buffer2 = new byte[] { 
                0, 0, 1, 0x19, 2, 50, 0x1a, 0xc6, 3, 0xdf, 0x33, 0xee, 0x1b, 0x68, 0xc7, 0x4b, 
                4, 100, 0xe0, 14, 0x34, 0x8d, 0xef, 0x81, 0x1c, 0xc1, 0x69, 0xf8, 200, 8, 0x4c, 0x71, 
                5, 0x8a, 0x65, 0x2f, 0xe1, 0x24, 15, 0x21, 0x35, 0x93, 0x8e, 0xda, 240, 0x12, 130, 0x45, 
                0x1d, 0xb5, 0xc2, 0x7d, 0x6a, 0x27, 0xf9, 0xb9, 0xc9, 0x9a, 9, 120, 0x4d, 0xe4, 0x72, 0xa6, 
                6, 0xbf, 0x8b, 0x62, 0x66, 0xdd, 0x30, 0xfd, 0xe2, 0x98, 0x25, 0xb3, 0x10, 0x91, 0x22, 0x88, 
                0x36, 0xd0, 0x94, 0xce, 0x8f, 150, 0xdb, 0xbd, 0xf1, 210, 0x13, 0x5c, 0x83, 0x38, 70, 0x40, 
                30, 0x42, 0xb6, 0xa3, 0xc3, 0x48, 0x7e, 110, 0x6b, 0x3a, 40, 0x54, 250, 0x85, 0xba, 0x3d, 
                0xca, 0x5e, 0x9b, 0x9f, 10, 0x15, 0x79, 0x2b, 0x4e, 0xd4, 0xe5, 0xac, 0x73, 0xf3, 0xa7, 0x57, 
                7, 0x70, 0xc0, 0xf7, 140, 0x80, 0x63, 13, 0x67, 0x4a, 0xde, 0xed, 0x31, 0xc5, 0xfe, 0x18, 
                0xe3, 0xa5, 0x99, 0x77, 0x26, 0xb8, 180, 0x7c, 0x11, 0x44, 0x92, 0xd9, 0x23, 0x20, 0x89, 0x2e, 
                0x37, 0x3f, 0xd1, 0x5b, 0x95, 0xbc, 0xcf, 0xcd, 0x90, 0x87, 0x97, 0xb2, 220, 0xfc, 190, 0x61, 
                0xf2, 0x56, 0xd3, 0xab, 20, 0x2a, 0x5d, 0x9e, 0x84, 60, 0x39, 0x53, 0x47, 0x6d, 0x41, 0xa2, 
                0x1f, 0x2d, 0x43, 0xd8, 0xb7, 0x7b, 0xa4, 0x76, 0xc4, 0x17, 0x49, 0xec, 0x7f, 12, 0x6f, 0xf6, 
                0x6c, 0xa1, 0x3b, 0x52, 0x29, 0x9d, 0x55, 170, 0xfb, 0x60, 0x86, 0xb1, 0xbb, 0xcc, 0x3e, 90, 
                0xcb, 0x59, 0x5f, 0xb0, 0x9c, 0xa9, 160, 0x51, 11, 0xf5, 0x16, 0xeb, 0x7a, 0x75, 0x2c, 0xd7, 
                0x4f, 0xae, 0xd5, 0xe9, 230, 0xe7, 0xad, 0xe8, 0x74, 0xd6, 0xf4, 0xea, 0xa8, 80, 0x58, 0xaf
             };
            byte[] buffer3 = new byte[(pEcc - 1) + 1];
            switch (pEcc)
            {
                case 7:
                    buffer3 = new byte[] { 0x7f, 0x7a, 0x9a, 0xa4, 11, 0x44, 0x75 };
                    break;

                case 10:
                    buffer3 = new byte[] { 0xd8, 0xc2, 0x9f, 0x6f, 0xc7, 0x5e, 0x5f, 0x71, 0x9d, 0xc1 };
                    break;

                case 13:
                    buffer3 = new byte[] { 0x89, 0x49, 0xe3, 0x11, 0xb1, 0x11, 0x34, 13, 0x2e, 0x2b, 0x53, 0x84, 120 };
                    break;

                case 15:
                    buffer3 = new byte[] { 0x1d, 0xc4, 0x6f, 0xa3, 0x70, 0x4a, 10, 0x69, 0x69, 0x8b, 0x84, 0x97, 0x20, 0x86, 0x1a };
                    break;

                case 0x10:
                    buffer3 = new byte[] { 0x3b, 13, 0x68, 0xbd, 0x44, 0xd1, 30, 8, 0xa3, 0x41, 0x29, 0xe5, 0x62, 50, 0x24, 0x3b };
                    break;

                case 0x11:
                    buffer3 = new byte[] { 
                        0x77, 0x42, 0x53, 120, 0x77, 0x16, 0xc5, 0x53, 0xf9, 0x29, 0x8f, 0x86, 0x55, 0x35, 0x7d, 0x63, 
                        0x4f
                     };
                    break;

                case 0x12:
                    buffer3 = new byte[] { 
                        0xef, 0xfb, 0xb7, 0x71, 0x95, 0xaf, 0xc7, 0xd7, 240, 220, 0x49, 0x52, 0xad, 0x4b, 0x20, 0x43, 
                        0xd9, 0x92
                     };
                    break;

                case 20:
                    buffer3 = new byte[] { 
                        0x98, 0xb9, 240, 5, 0x6f, 0x63, 6, 220, 0x70, 150, 0x45, 0x24, 0xbb, 0x16, 0xe4, 0xc6, 
                        0x79, 0x79, 0xa5, 0xae
                     };
                    break;

                case 0x16:
                    buffer3 = new byte[] { 
                        0x59, 0xb3, 0x83, 0xb0, 0xb6, 0xf4, 0x13, 0xbd, 0x45, 40, 0x1c, 0x89, 0x1d, 0x7b, 0x43, 0xfd, 
                        0x56, 0xda, 230, 0x1a, 0x91, 0xf5
                     };
                    break;

                case 0x18:
                    buffer3 = new byte[] { 
                        0x7a, 0x76, 0xa9, 70, 0xb2, 0xed, 0xd8, 0x66, 0x73, 150, 0xe5, 0x49, 130, 0x48, 0x3d, 0x2b, 
                        0xce, 1, 0xed, 0xf7, 0x7f, 0xd9, 0x90, 0x75
                     };
                    break;

                case 0x1a:
                    buffer3 = new byte[] { 
                        0xf6, 0x33, 0xb7, 4, 0x88, 0x62, 0xc7, 0x98, 0x4d, 0x38, 0xce, 0x18, 0x91, 40, 0xd1, 0x75, 
                        0xe9, 0x2a, 0x87, 0x44, 70, 0x90, 0x92, 0x4d, 0x2b, 0x5e
                     };
                    break;

                case 0x1c:
                    buffer3 = new byte[] { 
                        0xfc, 9, 0x1c, 13, 0x12, 0xfb, 0xd0, 150, 0x67, 0xae, 100, 0x29, 0xa7, 12, 0xf7, 0x38, 
                        0x75, 0x77, 0xe9, 0x7f, 0xb5, 100, 0x79, 0x93, 0xb0, 0x4a, 0x3a, 0xc5
                     };
                    break;

                case 30:
                    buffer3 = new byte[] { 
                        0xd4, 0xf6, 0x4d, 0x49, 0xc3, 0xc0, 0x4b, 0x62, 5, 70, 0x67, 0xb1, 0x16, 0xd9, 0x8a, 0x33, 
                        0xb5, 0xf6, 0x48, 0x19, 0x12, 0x2e, 0xe4, 0x4a, 0xd8, 0xc3, 11, 0x6a, 130, 150
                     };
                    break;
            }
            int num20 = buffer3.Length - 1;
            for (int i = 0; i <= num20; i++)
            {
                int num9 = buffer3[i];
                num9 = buffer2[num9];
                int num10 = 0;
                do
                {
                    int num11 = (num9 + buffer2[num10]) % 0xff;
                    int num12 = buffer[num11];
                    buffer5[num10, i] = (byte) num12;
                    num10++;
                }
                while (num10 <= 0xff);
            }
            int index = 0;
            int num6 = 0;
            int num = 0;
            this.lRSBlock = new byte[(num7 - 1) + 1];
            int num21 = num7 - 1;
            for (int j = 0; j <= num21; j++)
            {
                if (j >= (num7 - num4))
                {
                    this.lRSBlock[j] = (byte) Math.Round((double) ((num2 + num3) + 1f));
                }
                else
                {
                    this.lRSBlock[j] = (byte) Math.Round((double) (num2 + num3));
                }
            }
            byte[][] bufferArray = new byte[(this.lRSBlock.Length - 1) + 1][];
            byte[] destinationArray = new byte[(pTotal - 1) + 1];
            Array.Copy(pInputData, 0, destinationArray, 0, pInputData.Length);
            for (index = 0; index < this.lRSBlock.Length; index++)
            {
                int num22 = this.lRSBlock.Length - 1;
                for (int k = 0; k <= num22; k++)
                {
                    bufferArray[k] = new byte[((byte) (this.lRSBlock[index] - pEcc)) + 1];
                }
            }
            for (index = 0; index < pData; index++)
            {
                bufferArray[num][num6] = pInputData[index];
                num6++;
                if (num6 >= ((byte) (this.lRSBlock[num] - pEcc)))
                {
                    num6 = 0;
                    num++;
                }
            }
            for (num = 0; num < this.lRSBlock.Length; num++)
            {
                byte[] sourceArray = (byte[]) bufferArray[num].Clone();
                int num15 = this.lRSBlock[num];
                int num16 = num15 - pEcc;
                for (num6 = num16; num6 > 0; num6--)
                {
                    byte num17 = sourceArray[0];
                    if (num17 != 0)
                    {
                        byte[] buffer8 = new byte[(sourceArray.Length - 1) + 1];
                        Array.Copy(sourceArray, 1, buffer8, 0, sourceArray.Length - 1);
                        byte[] buffer9 = new byte[buffer5.GetLength(1) + 1];
                        int num23 = buffer5.GetLength(1) - 1;
                        for (int m = 0; m <= num23; m++)
                        {
                            buffer9[m] = buffer5[num17, m];
                        }
                        sourceArray = this.QRCodeGetByteData(buffer8, buffer9);
                    }
                    else if (pEcc < sourceArray.Length)
                    {
                        byte[] buffer10 = new byte[((sourceArray.Length - 1) - 1) + 1];
                        Array.Copy(sourceArray, 1, buffer10, 0, sourceArray.Length - 1);
                        sourceArray = (byte[]) buffer10.Clone();
                    }
                    else
                    {
                        byte[] buffer11 = new byte[(pEcc - 1) + 1];
                        Array.Copy(sourceArray, 1, buffer11, 0, sourceArray.Length - 1);
                        buffer11[pEcc - 1] = 0;
                        sourceArray = (byte[]) buffer11.Clone();
                    }
                }
                Array.Copy(sourceArray, 0, destinationArray, pInputData.Length + (num * pEcc), pEcc);
            }
            return destinationArray;
        }

        private void QRCodeInquireBits(string QRText)
        {
            bool flag3 = true;
            bool flag = true;
            bool flag2 = true;
            int length = QRText.Length;
            int num3 = length - 1;
            for (int i = 0; i <= num3; i++)
            {
                if (!this.CheckTextCharNumeric(QRText.Substring(i, 1)))
                {
                    flag3 = false;
                }
                if (!this.CheckTextCharAlphaNumeric(QRText.Substring(i, 1)))
                {
                    flag = false;
                }
                if (!this.CheckTextCharKanji(QRText.Substring(i, 1)))
                {
                    flag2 = false;
                }
            }
            if (flag3)
            {
                switch ((length % 3))
                {
                    case 0:
                        this.lQRInquireNumericBits = (int) Math.Round((double) (Conversion.Int((double) (((double) length) / 3.0)) * 10.0));
                        goto Label_0114;

                    case 1:
                        this.lQRInquireNumericBits = (int) Math.Round((double) ((Conversion.Int((double) (((double) length) / 3.0)) * 10.0) + 4.0));
                        goto Label_0114;

                    case 2:
                        this.lQRInquireNumericBits = (int) Math.Round((double) ((Conversion.Int((double) (((double) length) / 3.0)) * 10.0) + 7.0));
                        goto Label_0114;
                }
            }
            else
            {
                this.lQRInquireNumericBits = 0;
            }
        Label_0114:
            if (flag)
            {
                switch ((length % 2))
                {
                    case 0:
                        this.lQRInquireAlphaNumericBits = (int) Math.Round((double) (Conversion.Int((double) (((double) length) / 2.0)) * 11.0));
                        goto Label_018C;

                    case 1:
                        this.lQRInquireAlphaNumericBits = (int) Math.Round((double) ((Conversion.Int((double) (((double) length) / 2.0)) * 11.0) + 6.0));
                        goto Label_018C;
                }
            }
            else
            {
                this.lQRInquireAlphaNumericBits = 0;
            }
        Label_018C:
            if (flag2)
            {
                this.lQRInquireKanjiBits = length * 13;
            }
            else
            {
                this.lQRInquireKanjiBits = 0;
            }
            //byte[] bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(QRText);
            //this.lQRInquireBinaryBits = bytes.Length * 8;
            this.lQRInquireBinaryBits = -1;
        }

        private byte[] QRCodeRestoreDeployDataX(byte[] pInputFlag, byte[] pInputData, int pInputBytes)
        {
            sbyte[,] numArray = new sbyte[,] { { 0, 1, 0, 1, 0, 1, 0, 1 }, { 0, 1, 0, 1, 0, 1, 2, 3 }, { 0, 1, 0, 1, 2, 3, 2, 3 }, { 0, 1, 0, 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0, 1, 0, 1 }, { 0, 1, 0, 1, 0, 1, 1, 1 }, { 0, 0, 0, 1, 0, 1, 0, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0 } };
            byte[] buffer = new byte[(pInputBytes - 1) + 1];
            int index = 0;
            int num2 = 0;
            int num7 = pInputFlag.Length - 1;
            for (int i = 0; i <= num7; i++)
            {
                int num5 = 0;
                int num6 = 0;
                int num4 = pInputFlag[i];
                switch (num4)
                {
                    case 15:
                    case 0:
                        goto Label_04B8;
                }
                num6 = (int) Math.Round(Conversion.Int((double) (((double) num4) / 16.0)));
                num5 = num4 - (num6 * 0x10);
                if (num5 == 15)
                {
                    buffer[num2] = pInputData[index];
                    buffer[num2 + 1] = pInputData[index + 1];
                    buffer[num2 + 2] = pInputData[index + 2];
                    buffer[num2 + 3] = pInputData[index + 3];
                    buffer[num2 + 4] = pInputData[index + 4];
                    buffer[num2 + 5] = pInputData[index + 5];
                    buffer[num2 + 6] = pInputData[index + 6];
                    buffer[num2 + 7] = pInputData[index + 7];
                    num2 += 8;
                    index += 8;
                }
                else
                {
                    buffer[num2] = pInputData[index];
                    if (num5 >= 8)
                    {
                        buffer[num2 + 1] = (byte) ((short) (pInputData[index] - numArray[num5 - 8, 1]));
                        buffer[num2 + 2] = (byte) ((short) (pInputData[index] - numArray[num5 - 8, 2]));
                        buffer[num2 + 3] = (byte) ((short) (pInputData[index] - numArray[num5 - 8, 3]));
                        buffer[num2 + 4] = (byte) ((short) (pInputData[index] - numArray[num5 - 8, 4]));
                        buffer[num2 + 5] = (byte) ((short) (pInputData[index] - numArray[num5 - 8, 5]));
                        buffer[num2 + 6] = (byte) ((short) (pInputData[index] - numArray[num5 - 8, 6]));
                        buffer[num2 + 7] = (byte) ((short) (pInputData[index] - numArray[num5 - 8, 7]));
                    }
                    else
                    {
                        buffer[num2 + 1] = (byte) ((short) (pInputData[index] + numArray[num5 - 1, 1]));
                        buffer[num2 + 2] = (byte) ((short) (pInputData[index] + numArray[num5 - 1, 2]));
                        buffer[num2 + 3] = (byte) ((short) (pInputData[index] + numArray[num5 - 1, 3]));
                        buffer[num2 + 4] = (byte) ((short) (pInputData[index] + numArray[num5 - 1, 4]));
                        buffer[num2 + 5] = (byte) ((short) (pInputData[index] + numArray[num5 - 1, 5]));
                        buffer[num2 + 6] = (byte) ((short) (pInputData[index] + numArray[num5 - 1, 6]));
                        buffer[num2 + 7] = (byte) ((short) (pInputData[index] + numArray[num5 - 1, 7]));
                    }
                    num2 += 8;
                    index++;
                }
            }
        Label_04B8:
            while (num2 < pInputBytes)
            {
                buffer[num2] = pInputData[index];
                num2++;
                index++;
            }
            return buffer;
        }

        private QRTextTypes QRCodeSetInquireTextType(string QRText)
        {
            int qRInquireBinaryBits = this.QRInquireBinaryBits;
            QRTextTypes binary = QRTextTypes.Binary;
            if ((this.QRInquireNumericBits > 0) && (qRInquireBinaryBits > this.QRInquireNumericBits))
            {
                qRInquireBinaryBits = this.QRInquireNumericBits;
                binary = QRTextTypes.Numeric;
            }
            if ((this.QRInquireAlphaNumericBits > 0) && (qRInquireBinaryBits > this.QRInquireAlphaNumericBits))
            {
                qRInquireBinaryBits = this.QRInquireAlphaNumericBits;
                binary = QRTextTypes.AlphaNumeric;
            }
            if ((this.QRInquireKanjiBits > 0) && (qRInquireBinaryBits > this.QRInquireKanjiBits))
            {
                qRInquireBinaryBits = this.QRInquireKanjiBits;
                binary = QRTextTypes.Kanji;
            }
            if ((this.lQRSetTextType == QRTextTypes.Numeric) && (this.QRInquireNumericBits > 0))
            {
                return QRTextTypes.Numeric;
            }
            if ((this.lQRSetTextType == QRTextTypes.AlphaNumeric) && (this.QRInquireAlphaNumericBits > 0))
            {
                return QRTextTypes.AlphaNumeric;
            }
            if ((this.lQRSetTextType == QRTextTypes.Kanji) && (this.QRInquireKanjiBits > 0))
            {
                return QRTextTypes.Kanji;
            }
            if ((this.lQRSetTextType == QRTextTypes.Binary) && (this.QRInquireBinaryBits > 0))
            {
                return QRTextTypes.Binary;
            }
            this.lQRInquireTextType = binary;
            return binary;
        }

        private byte[] QRCodeTransRSBlock(byte[] pDeployData)
        {
            int num2 = this.gDataWords[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            int num6 = this.gExtraDataWords[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            float num4 = this.gECCWords[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            int num8 = this.gRSBlockCount[(int) this.lQRSetECCRate, ((int) this.lQRInquireVersion) - 1];
            byte[] buffer = new byte[(pDeployData.Length - 1) + 1];
            int index = 0;
            int num7 = 0;
            int[] numArray = new int[0x81];
            int num5 = 0;
            int num18 = num2 - 1;
            for (int i = 0; i <= num18; i++)
            {
                int num19 = num8 - 1;
                for (int m = 0; m <= num19; m++)
                {
                    int num11;
                    if (m > (num8 - num6))
                    {
                        num11 = m - (num8 - num6);
                        if (i == 0)
                        {
                            numArray[num5] = ((i + (m * num2)) + num11) - 1;
                            num5++;
                        }
                    }
                    else
                    {
                        num11 = 0;
                    }
                    int num12 = 0;
                    do
                    {
                        index = (((i + (m * num2)) + num11) * 8) + num12;
                        buffer[index] = pDeployData[num7];
                        num7++;
                        num12++;
                    }
                    while (num12 <= 7);
                }
            }
            if (num6 > 0)
            {
                numArray[num5] = ((num2 * num8) + num6) - 1;
                num5++;
            }
            int num20 = num5 - 1;
            for (int j = 0; j <= num20; j++)
            {
                int num14 = 0;
                do
                {
                    index = (numArray[j] * 8) + num14;
                    buffer[index] = pDeployData[num7];
                    num7++;
                    num14++;
                }
                while (num14 <= 7);
            }
            int num = num7;
            int num21 = (int) Math.Round((double) (num4 - 1f));
            for (int k = 0; k <= num21; k++)
            {
                int num22 = num8 - 1;
                for (int n = 0; n <= num22; n++)
                {
                    int num17 = 0;
                    do
                    {
                        index = (int) Math.Round((double) ((num + ((k + (n * num4)) * 8f)) + num17));
                        buffer[index] = pDeployData[num7];
                        num7++;
                        num17++;
                    }
                    while (num17 <= 7);
                }
            }
            while (num7 <= (pDeployData.Length - 1))
            {
                buffer[num7] = pDeployData[num7];
                num7++;
            }
            return buffer;
        }

        public void QRCopyToClipboard(string code, int pPixelSize)
        {
            float qRInquireModuleSize = this.QRInquireModuleSize;
            if (qRInquireModuleSize > 0f)
            {
                int pQuitZoneSize = pPixelSize;
                if (pQuitZoneSize > 0)
                {
                    int num3 = (int) Math.Round((double) (pQuitZoneSize * (qRInquireModuleSize + (this.lQRQuitZone * 2))));
                    float num4 = (float) (num3 * (Math.Abs(Math.Sin(((this.QRRotate / 360f) * 2f) * 3.1415926535897931)) + Math.Abs(Math.Cos(((this.QRRotate / 360f) * 2f) * 3.1415926535897931))));
                    Bitmap image = new Bitmap((int) Math.Round((double) num4), (int) Math.Round((double) num4), PixelFormat.Format24bppRgb);
                    Graphics ev = Graphics.FromImage(image);
                    SolidBrush brush = new SolidBrush(Color.White);
                    ev.FillRectangle(brush, 0, 0, (int) Math.Round((double) num4), (int) Math.Round((double) num4));
                    this.Generate(code, 0f, 0f, pQuitZoneSize, ev);
                    Clipboard.SetDataObject(image);
                }
            }
        }
        public void QRCopyToClipboard(byte[] data, int pPixelSize)
        {
            float qRInquireModuleSize = this.QRInquireModuleSize;
            if (qRInquireModuleSize > 0f)
            {
                int pQuitZoneSize = pPixelSize;
                if (pQuitZoneSize > 0)
                {
                    int num3 = (int)Math.Round((double)(pQuitZoneSize * (qRInquireModuleSize + (this.lQRQuitZone * 2))));
                    float num4 = (float)(num3 * (Math.Abs(Math.Sin(((this.QRRotate / 360f) * 2f) * 3.1415926535897931)) + Math.Abs(Math.Cos(((this.QRRotate / 360f) * 2f) * 3.1415926535897931))));
                    Bitmap image = new Bitmap((int)Math.Round((double)num4), (int)Math.Round((double)num4), PixelFormat.Format24bppRgb);
                    Graphics ev = Graphics.FromImage(image);
                    SolidBrush brush = new SolidBrush(Color.White);
                    ev.FillRectangle(brush, 0, 0, (int)Math.Round((double)num4), (int)Math.Round((double)num4));
                    this.Generate(data, 0f, 0f, pQuitZoneSize, ev);
                    Clipboard.SetDataObject(image);
                }
            }
        }

        private byte QRGetAlphaNumericCode(string pInput)
        {
            if ((Conversions.ToDouble(pInput) >= 48.0) & (Conversions.ToDouble(pInput) < 58.0))
            {
                return (byte) Math.Round((double) (Conversions.ToDouble(pInput) - 48.0));
            }
            if ((Conversions.ToDouble(pInput) >= 65.0) & (Conversions.ToDouble(pInput) < 91.0))
            {
                return (byte) Math.Round((double) (Conversions.ToDouble(pInput) - 55.0));
            }
            string str = pInput;
            if (str == Conversions.ToString(0x20))
            {
                return 0x24;
            }
            if (str == Conversions.ToString(0x24))
            {
                return 0x25;
            }
            if (str == Conversions.ToString(0x25))
            {
                return 0x26;
            }
            if (str == Conversions.ToString(0x2a))
            {
                return 0x27;
            }
            if (str == Conversions.ToString(0x2b))
            {
                return 40;
            }
            if (str == Conversions.ToString(0x2d))
            {
                return 0x29;
            }
            if (str == Conversions.ToString(0x2e))
            {
                return 0x2a;
            }
            if (str == Conversions.ToString(0x2f))
            {
                return 0x2b;
            }
            if (str == Conversions.ToString(0x3a))
            {
                return 0x2c;
            }
            return 0;
        }

        int[] numArray4 = new int[] { 
                0, 0x1a, 0x2c, 70, 100, 0x86, 0xac, 0xc4, 0xf2, 0x124, 0x15a, 0x194, 0x1d2, 0x214, 0x245, 0x28f, 
                0x2dd, 0x32f, 0x385, 0x3df, 0x43d, 0x484, 0x4ea, 0x554, 0x5c2, 0x634, 0x6aa, 0x724, 0x781, 0x803, 0x889, 0x913, 
                0x9a1, 0xa33, 0xac9, 0xb3c, 0xbda, 0xc7c, 0xd22, 0xdcc, 0xe7a
             };
        int[] numArray2 = new int[] { 
                0, 0, 7, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 3, 3, 
                3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 
                3, 3, 3, 0, 0, 0, 0, 0, 0
             };
        int[,] numArray = new int[,] { { 
                0, 0x98, 0x110, 440, 640, 0x360, 0x440, 0x4e0, 0x610, 0x740, 0x890, 0xa20, 0xb90, 0xd60, 0xe68, 0x1058, 
                0x1268, 0x1438, 0x1688, 0x18d8, 0x1ae8, 0x1d20, 0x1f70, 0x2230, 0x24b0, 0x27e0, 0x2ad0, 0x2de0, 0x2fd8, 0x32f8, 0x3638, 0x3998, 
                0x3d18, 0x40b8, 0x4478, 0x4810, 0x4c10, 0x5030, 0x5470, 0x57e0, 0x5c60
             }, { 
                0, 0x80, 0xe0, 0x160, 0x200, 0x2b0, 0x360, 0x3e0, 0x4d0, 0x5b0, 0x6c0, 0x7f0, 0x910, 0xa70, 0xb68, 0xcf8, 
                0xe28, 0xfd8, 0x1198, 0x1398, 0x14e8, 0x1650, 0x1870, 0x1ae0, 0x1c90, 0x1f40, 0x2130, 0x2340, 0x2548, 0x2798, 0x2ae8, 0x2d78, 
                0x3028, 0x32f8, 0x35e8, 0x38a0, 0x3bd0, 0x3e40, 0x41b0, 0x4540, 0x48f0
             }, { 
                0, 0x68, 0xb0, 0x110, 0x180, 0x1f0, 0x260, 0x2c0, 880, 0x420, 0x4d0, 0x5a0, 0x670, 0x7a0, 0x828, 0x938, 
                0xa28, 0xb78, 0xc68, 0xde8, 0xf28, 0x1000, 0x11c0, 0x1330, 0x14c0, 0x1670, 0x1790, 0x1940, 0x1b38, 0x1c78, 0x1ec8, 0x2048, 
                0x22d8, 0x2498, 0x2678, 0x2830, 0x2a50, 0x2c90, 0x2ef0, 0x3170, 0x3410
             }, { 
                0, 0x48, 0x80, 0xd0, 0x120, 0x170, 480, 0x210, 0x2b0, 800, 0x3d0, 0x460, 0x4f0, 0x5a0, 0x628, 0x6f8, 
                0x7e8, 0x8d8, 0x9c8, 0xaa8, 0xc08, 0xcb0, 0xdd0, 0xe80, 0x1010, 0x10d0, 0x12a0, 0x13a0, 0x14a8, 0x15e8, 0x1748, 0x18c8, 
                0x1a68, 0x1c28, 0x1e08, 0x1ed0, 0x20f0, 0x2240, 0x23b0, 0x2630, 0x27e0
             } };
        string[] strArray = new string[] { 
                "111011111000100", "111001011110011", "111110110101010", "111100010011101", "110011000101111", "110001100011000", "110110001000001", "110100101110110", "101010000010010", "101000100100101", "101111001111100", "101101101001011", "100010111111001", "100000011001110", "100111110010111", "100101010100000", 
                "011010101011111", "011000001101000", "011111100110001", "011101000000110", "010010010110100", "010000110000011", "010111011011010", "010101111101101", "001011010001001", "001001110111110", "001110011100111", "001100111010000", "000011101100010", "000001001010101", "000110100001100", "000100000111011"
             };
        public byte[,] QRGetData(string pTextData)
        {

            byte[] bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(pTextData);
            int length = bytes.Length;
            this.QRCodeInquireBits(pTextData);
            this.lQRInquireBinaryBits = bytes.Length*8;
            this.lQRInquireTextType = this.QRCodeSetInquireTextType(pTextData);

            return getqrcoderaw(bytes, length);
        }
        public byte[,] QRGetData(byte[] data)
        {

            int length = data.Length;
            this.lQRInquireNumericBits = 0;
            this.lQRInquireKanjiBits = 0;
            this.lQRInquireAlphaNumericBits = 0;
            this.lQRInquireBinaryBits = data.Length * 8;
            this.lQRInquireTextType = QRTextTypes.Binary;

            return getqrcoderaw(data, length);
        }

        private byte[,] getqrcoderaw(byte[] bytes, int length)
        {

            int num6 = 0;
            int num9;
            int[] numArray3;
            byte[] buffer5;

            byte[] buffer = new byte[] { 0 };
            int num = 0;
            int num10 = 0;
            if (this.lQRInquireTextType == QRTextTypes.Kanji)
            {
                numArray3 = new int[((int)Math.Round((double)(((((double)length) / 2.0) + 32.0) - 1.0))) + 1];
                buffer5 = new byte[((int)Math.Round((double)(((((double)length) / 2.0) + 32.0) - 1.0))) + 1];
            }
            else
            {
                numArray3 = new int[((length + 0x20) - 1) + 1];
                buffer5 = new byte[((length + 0x20) - 1) + 1];
            }
            if (length <= 0)
            {
                return new byte[,] { { 0 } };
            }

            int index = 0;
            buffer5[index] = 4;
            switch (this.QRInquireTextType)
            {
                case QRTextTypes.Numeric:
                    {

                        buffer = new byte[] { 
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 
                        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 
                        4, 4, 4, 4, 4, 4, 4, 4, 4
                     };

                        num6 = index + 1;
                        index = makenumericdata(numArray3, buffer5, index, bytes, length);
                        break;
                    }
                case QRTextTypes.AlphaNumeric:
                    {
                        buffer = new byte[] { 
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 
                        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 
                        4, 4, 4, 4, 4, 4, 4, 4, 4
                     };
                        num6 = index + 1;
                        index = makealphanumericdata(numArray3, buffer5, bytes, length, index);
                        break;
                    }
                case QRTextTypes.Kanji:
                    {
                        buffer = new byte[] { 
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 
                        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 
                        4, 4, 4, 4, 4, 4, 4, 4, 4
                     };
                        num6 = index + 1;
                        index = makekanjidata(numArray3, buffer5, bytes, length, index);
                        break;
                    }
                case QRTextTypes.Binary:
                    {
                        buffer = new byte[] { 
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 8, 8, 8, 8, 8, 
                        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 
                        8, 8, 8, 8, 8, 8, 8, 8, 8
                     };
                        num6 = index + 1;
                        index = makebinarydata(numArray3, buffer5, bytes, length, index);
                        break;
                    }
            }
            num = 0;
            int num51 = index - 1;
            for (int i = 0; i <= num51; i++)
            {
                num += buffer5[i];
            }
            this.lQRInquireVersion = QRVersions.Ver01;
            int num27 = 1;
            do
            {
                if (numArray[(int)this.lQRSetECCRate, num27] >= (num + buffer[(int)this.lQRInquireVersion]))
                {
                    num10 = numArray[(int)this.lQRSetECCRate, num27];
                    break;
                }
                this.lQRInquireVersion += 1;
                num27++;
            }
            while (num27 <= 40);
            if (this.lQRInquireVersion > QRVersions.Ver40)
            {
                this.lQRInquireVersion = QRVersions.Automatic;
                return new byte[,] { { 0 } };
            }
            if (this.lQRInquireVersion != QRVersions.Automatic)
            {
                if (this.lQRInquireVersion > this.QRSetVersion)
                {
                    num10 = numArray[(int)this.lQRSetECCRate, (int)this.lQRInquireVersion];
                }
                else
                {
                    this.lQRInquireVersion = this.QRSetVersion;
                    num10 = numArray[(int)this.lQRSetECCRate, (int)this.lQRInquireVersion];
                }
            }
            num += buffer[(int)this.lQRInquireVersion];
            int num52 = num6;
            buffer5[num52] = (byte)(buffer5[num52] + buffer[(int)this.lQRInquireVersion]);
            int pTotal = numArray4[(int)this.lQRInquireVersion];
            int num2 = numArray2[(int)this.lQRInquireVersion] + (pTotal << 3);
            this.lFormatSecondX = this.QRCodeGetFormatX((int)this.lQRInquireVersion);
            this.lFormatSecondY = this.QRCodeGetFormatY((int)this.lQRInquireVersion);
            byte pEcc = (byte)this.gECCWords[(int)this.lQRSetECCRate, ((int)this.lQRInquireVersion) - 1];
            int pMaxBytes = (int)Math.Round((double)(((double)num10) / 8.0));
            int num15 = 4 * (int)(this.lQRInquireVersion) + 17;
            this.lQRInquireModuleSize = num15;
            int num16 = num15 * num15;
            byte[] buffer8 = new byte[((num16 + num15) - 1) + 1];
            buffer8 = this.QRCodeGetSymbolData((int)this.lQRInquireVersion);
            this.lDeployTableX = new byte[num2 + 1];
            this.lDeployTableY = new byte[num2 + 1];
            this.lDeployOrderX = new byte[(num2 - 1) + 1];
            this.lDeployOrderY = new byte[(num2 - 1) + 1];
            int num3 = 0;
            for (int j = num15 - 1; j >= 3; j += -4)
            {
                for (int num29 = num15 - 1; num29 >= 0; num29 += -1)
                {
                    int num31 = (num29 * (num15 + 1)) + j;
                    int num30 = buffer8[num31] & 0xfe;
                    if (num30 == 0)
                    {
                        this.lDeployTableX[num3] = (byte)j;
                        this.lDeployTableY[num3] = (byte)num29;
                        num3++;
                    }
                    num31 = ((num29 * (num15 + 1)) + j) - 1;
                    num30 = buffer8[num31] & 0xfe;
                    if (num30 == 0)
                    {
                        this.lDeployTableX[num3] = (byte)(j - 1);
                        this.lDeployTableY[num3] = (byte)num29;
                        num3++;
                    }
                }
                if (j == 8)
                {
                    j = 7;
                }
                int num53 = num15 - 1;
                for (int num32 = 0; num32 <= num53; num32++)
                {
                    int num34 = ((num32 * (num15 + 1)) + j) - 2;
                    int num33 = buffer8[num34] & 0xfe;
                    if (num33 == 0)
                    {
                        this.lDeployTableX[num3] = (byte)(j - 2);
                        this.lDeployTableY[num3] = (byte)num32;
                        num3++;
                    }
                    num34 = (((num32 * (num15 + 1)) + j) - 1) - 2;
                    num33 = buffer8[num34] & 0xfe;
                    if (num33 == 0)
                    {
                        this.lDeployTableX[num3] = (byte)((j - 1) - 2);
                        this.lDeployTableY[num3] = (byte)num32;
                        num3++;
                    }
                }
            }
            this.lDeployOrderX = this.QRCodeTransRSBlock(this.lDeployTableX);
            this.lDeployOrderY = this.QRCodeTransRSBlock(this.lDeployTableY);
            if (num <= (num10 - 4))
            {
                numArray3[index] = 0;
                buffer5[index] = 4;
            }
            else if (num < num10)
            {
                numArray3[index] = 0;
                buffer5[index] = (byte)(num10 - num);
            }
            byte[] pInputData = this.QRCodeGet8BitData(numArray3, buffer5, pMaxBytes);
            byte[] buffer6 = this.QRCodeGetTotalData(pInputData, pEcc, pMaxBytes, pTotal);
            byte[,] pModuleData = new byte[(num15 - 1) + 1, (num15 - 1) + 1];
            int num54 = num15 - 1;
            for (int k = 0; k <= num54; k++)
            {
                int num55 = num15 - 1;
                for (int num36 = 0; num36 <= num55; num36++)
                {
                    pModuleData[num36, k] = 0;
                }
            }
            int num56 = pTotal - 1;
            for (int m = 0; m <= num56; m++)
            {
                byte num37 = buffer6[m];
                int num39 = 7;
                do
                {
                    int num40 = (m * 8) + num39;
                    num9 = this.QRCodeGetMaskBitGroup(this.lDeployOrderX[num40], this.lDeployOrderY[num40]);
                    pModuleData[this.lDeployOrderX[num40], this.lDeployOrderY[num40]] = (byte)((0xff * (num37 & 1)) ^ num9);
                    num37 = (byte)(num37 >> 1);
                    num39 += -1;
                }
                while (num39 >= 0);
            }
            for (int n = numArray2[(int)this.lQRInquireVersion]; n >= 1; n += -1)
            {
                int num42 = (n + (pTotal * 8)) - 1;
                num9 = this.QRCodeGetMaskBitGroup(this.lDeployOrderX[num42], this.lDeployOrderY[num42]);
                pModuleData[this.lDeployOrderX[num42], this.lDeployOrderY[num42]] = (byte)(0xff ^ num9);
            }
            byte num8 = this.QRCodeChooseMaskNumber(pModuleData, numArray2[(int)this.lQRInquireVersion] + (pTotal * 8));
            byte num7 = (byte)Math.Round(Math.Pow(2.0, (double)num8));
            byte num5 = (byte)((num8) + ((byte)this.lQRSetECCRate * 8));
            int startIndex = 0;
            do
            {
                byte num43 = byte.Parse(strArray[num5].Substring(startIndex, 1));
                pModuleData[this.lFormatFirstX[startIndex], this.lFormatFirstY[startIndex]] = (byte)(num43 * 0xff);
                pModuleData[this.lFormatSecondX[startIndex], this.lFormatSecondY[startIndex]] = (byte)(num43 * 0xff);
                startIndex++;
            }
            while (startIndex <= 14);
            byte[,] buffer2 = new byte[(num15 - 1) + 1, (num15 - 1) + 1];
            int num12 = 0;
            int num57 = num15 - 1;
            for (int num45 = 0; num45 <= num57; num45++)
            {
                int num58 = num15 - 1;
                for (int num46 = 0; num46 <= num58; num46++)
                {
                    if (((pModuleData[num46, num45] & num7) != 0) | ((buffer8[num12] & 1) == 1))
                    {
                        buffer2[num46, num45] = (byte)(1 | (buffer8[num12] & 0xfe));
                    }
                    else
                    {
                        buffer2[num46, num45] = (byte)(0 | (buffer8[num12] & 0xfe));
                    }
                    num12++;
                }
                num12++;
            }
            return buffer2;
        }

        private static int makebinarydata(int[] numArray3, byte[] buffer5, byte[] bytes, int length, int index)
        {

            numArray3[index] = 4;
            index++;
            numArray3[index] = length;
            buffer5[index] = 8;
            index++;
            int num50 = length - 1;
            for (int num25 = 0; num25 <= num50; num25++)
            {
                numArray3[num25 + index] = bytes[num25] & 0xff;
                buffer5[num25 + index] = 8;
            }
            index += length;
            return index;
        }

        private static int makekanjidata(int[] numArray3, byte[] buffer5, byte[] bytes, int length, int index)
        {
            numArray3[index] = 8;
            index++;
            numArray3[index] = (int)Math.Round((double)(((double)length) / 2.0));
            buffer5[index] = 8;
            index++;
            int num49 = length - 1;
            for (int num24 = 0; num24 <= num49; num24++)
            {
                byte num22 = bytes[num24];
                if (num22 < 0xe0)
                {
                    num22 = (byte)(num22 - 0x81);
                }
                else
                {
                    num22 = (byte)(num22 - 0xc1);
                }
                byte num23 = bytes[num24 + 1];
                num23 = (byte)(num23 - 0x40);
                numArray3[index] = (num22 * 0xc0) + num23;
                buffer5[index] = 13;
                index++;
                num24++;
            }
            return index;
        }

        private int makealphanumericdata(int[] numArray3, byte[] buffer5, byte[] bytes, int length, int index)
        {
            numArray3[index] = 2;
            index++;
            numArray3[index] = length;
            buffer5[index] = 9;
            index++;
            int num48 = length - 1;
            for (int num20 = 0; num20 <= num48; num20++)
            {
                byte num21 = this.QRGetAlphaNumericCode(Conversions.ToString(bytes[num20]));
                if ((num20 % 2) == 0)
                {
                    numArray3[index] = num21;
                    buffer5[index] = 6;
                }
                else
                {
                    numArray3[index] = (numArray3[index] * 0x2d) + num21;
                    buffer5[index] = 11;
                    if (num20 < (length - 1))
                    {
                        index++;
                    }
                }
            }
            index++;
            return index;
        }

        private int makenumericdata(int[] numArray3, byte[] buffer5,  int index, byte[] bytes, int length)
        {
            numArray3[index] = 1;
            index++;
            numArray3[index] = length;
            buffer5[index] = 10;
            index++;
            int num47 = length - 1;
            for (int num18 = 0; num18 <= num47; num18++)
            {
                byte num19 = this.QRGetNumericCode(Conversions.ToString(bytes[num18]));
                if ((num18 % 3) == 0)
                {
                    numArray3[index] = num19;
                    buffer5[index] = 4;
                }
                else
                {
                    numArray3[index] = (numArray3[index] * 10) + num19;
                    if ((num18 % 3) == 1)
                    {
                        buffer5[index] = 7;
                    }
                    else
                    {
                        buffer5[index] = 10;
                        if (num18 < (length - 1))
                        {
                            index++;
                        }
                    }
                }
            }
            index++;
            return index;
        }

        private byte QRGetNumericCode(string pInput)
        {
            if ((Conversions.ToDouble(pInput) >= 48.0) & (Conversions.ToDouble(pInput) < 58.0))
            {
                return (byte) Math.Round((double) (Conversions.ToDouble(pInput) - 48.0));
            }
            return 0;
        }

        //public void QRSave(string code, string FileName, int pPixelSize)
        //{
        //    byte[,] buffer = this.QRGetData(code);
        //    float qRInquireModuleSize = this.QRInquireModuleSize;
        //    if (qRInquireModuleSize > 0f)
        //    {
        //        int pQuitZoneSize = pPixelSize;
        //        if (pQuitZoneSize > 0)
        //        {
        //            int num3 = (int) Math.Round((double) (pQuitZoneSize * (qRInquireModuleSize + (this.lQRQuitZone * 2))));
        //            float num4 = (float) (num3 * (Math.Abs(Math.Sin(((this.QRRotate / 360f) * 2f) * 3.1415926535897931)) + Math.Abs(Math.Cos(((this.QRRotate / 360f) * 2f) * 3.1415926535897931))));
        //            Bitmap image = new Bitmap((int) Math.Round((double) num4), (int) Math.Round((double) num4), PixelFormat.Format24bppRgb);
        //            Graphics ev = Graphics.FromImage(image);
        //            SolidBrush brush = new SolidBrush(Color.White);
        //            ev.FillRectangle(brush, 0, 0, (int) Math.Round((double) num4), (int) Math.Round((double) num4));
        //            this.Generate(code, 0f, 0f, pQuitZoneSize, ev);
        //            switch (this.SaveFileType)
        //            {
        //                case SaveFileTypes.BitMap:
        //                    if (Path.GetExtension(FileName) == "")
        //                    {
        //                        FileName = FileName + ".bmp";
        //                    }
        //                    image.Save(FileName, ImageFormat.Bmp);
        //                    break;

        //                case SaveFileTypes.Emf:
        //                    if (Path.GetExtension(FileName) == "")
        //                    {
        //                        FileName = FileName + ".emf";
        //                    }
        //                    image.Save(FileName, ImageFormat.Emf);
        //                    break;

        //                case SaveFileTypes.Gif:
        //                    if (Path.GetExtension(FileName) == "")
        //                    {
        //                        FileName = FileName + ".gif";
        //                    }
        //                    image.Save(FileName, ImageFormat.Gif);
        //                    break;

        //                case SaveFileTypes.Jpeg:
        //                    if (Path.GetExtension(FileName) == "")
        //                    {
        //                        FileName = FileName + ".jpg";
        //                    }
        //                    image.Save(FileName, ImageFormat.Jpeg);
        //                    break;

        //                case SaveFileTypes.Png:
        //                    if (Path.GetExtension(FileName) == "")
        //                    {
        //                        FileName = FileName + ".png";
        //                    }
        //                    image.Save(FileName, ImageFormat.Png);
        //                    break;

        //                case SaveFileTypes.Tiff:
        //                    if (Path.GetExtension(FileName) == "")
        //                    {
        //                        FileName = FileName + ".tiff";
        //                    }
        //                    image.Save(FileName, ImageFormat.Tiff);
        //                    break;

        //                case SaveFileTypes.Wmf:
        //                    if (Path.GetExtension(FileName) == "")
        //                    {
        //                        FileName = FileName + ".wmf";
        //                    }
        //                    image.Save(FileName, ImageFormat.Wmf);
        //                    break;
        //            }
        //        }
        //    }
        //}
        //public void Save(string code, string FileName, float Width, float High)
        //{
        //        float qRInquireModuleSize = this.QRInquireModuleSize;
        //        if (qRInquireModuleSize > 0f)
        //        {
        //            int pPixelSize = this.QRCodeCalcPixel(qRInquireModuleSize, Width, High);
        //            if (pPixelSize > 0)
        //            {
        //                this.QRSave(code, FileName, pPixelSize);
        //            }
        //        }
           
        //}

        //public void Generate(string code, float X, float Y, float X2, float Y2, Graphics ev)
        //{
        //            this.qrcode_write_barcode(code, X, Y, X2, Y2, ev);
        //}

        public Color BackGroundColor
        {
            get
            {
                return this.myBackGroundColor;
            }
            set
            {
                this.myBackGroundColor = value;
                this.QRBackColorBigMark1 = value;
                this.QRBackColorBigMark2 = value;
                this.QRBackColorBigMark3 = value;
                this.QRBackColorSmallMark = value;
                this.QRColorBigMarkBorder1 = value;
                this.QRColorBigMarkBorder2 = value;
                this.QRColorBigMarkBorder3 = value;
                this.QRBackColorTimingPattern = value;
                this.QRBackColorVersionInformation = value;
                this.QRBackColorFormatInformation = value;
                this.QRColorQuitZone = value;
            }
        }

        public Color BarColor
        {
            get
            {
                return this.myBarColor;
            }
            set
            {
                this.myBarColor = value;
                this.QRColorBigMark1 = value;
                this.QRColorBigMark2 = value;
                this.QRColorBigMark3 = value;
                this.QRColorSmallMark = value;
                this.QRColorTimingPattern = value;
                this.QRColorVersionInformation = value;
                this.QRColorFormatInformation = value;
            }
        }

        public Color QRBackColorBigMark
        {
            set
            {
                this.lQRBackColorBigMark1 = value;
                this.lQRBackColorBigMark2 = value;
                this.lQRBackColorBigMark3 = value;
            }
        }

        public Color QRBackColorBigMark1
        {
            get
            {
                return this.lQRBackColorBigMark1;
            }
            set
            {
                this.lQRBackColorBigMark1 = value;
            }
        }

        public Color QRBackColorBigMark2
        {
            get
            {
                return this.lQRBackColorBigMark2;
            }
            set
            {
                this.lQRBackColorBigMark2 = value;
            }
        }

        public Color QRBackColorBigMark3
        {
            get
            {
                return this.lQRBackColorBigMark3;
            }
            set
            {
                this.lQRBackColorBigMark3 = value;
            }
        }

        public Color QRBackColorFormatInformation
        {
            get
            {
                return this.lQRBackColorFormatInformation;
            }
            set
            {
                this.lQRBackColorFormatInformation = value;
            }
        }

        public Color QRBackColorSmallMark
        {
            get
            {
                return this.lQRBackColorSmallMark;
            }
            set
            {
                this.lQRBackColorSmallMark = value;
            }
        }

        public Color QRBackColorTimingPattern
        {
            get
            {
                return this.lQRBackColorTimingPattern;
            }
            set
            {
                this.lQRBackColorTimingPattern = value;
            }
        }

        public Color QRBackColorVersionInformation
        {
            get
            {
                return this.lQRBackColorVersionInformation;
            }
            set
            {
                this.lQRBackColorVersionInformation = value;
            }
        }

        public string QRBackGroundFileName
        {
            get
            {
                return this.myQRBackGroundFileName;
            }
            set
            {
                if (value != "")
                {
                    try
                    {
                        this.myQRBackGroundBitmap = new Bitmap(value);
                    }
                    catch (Exception exception1)
                    {
                        ProjectData.SetProjectError(exception1);
                        Exception exception = exception1;
                        value = "";
                        ProjectData.ClearProjectError();
                    }
                }
                this.myQRBackGroundFileName = value;
            }
        }

        public Color QRColorBigMark
        {
            set
            {
                this.lQRColorBigMark1 = value;
                this.lQRColorBigMark2 = value;
                this.lQRColorBigMark3 = value;
            }
        }

        public Color QRColorBigMark1
        {
            get
            {
                return this.lQRColorBigMark1;
            }
            set
            {
                this.lQRColorBigMark1 = value;
            }
        }

        public Color QRColorBigMark2
        {
            get
            {
                return this.lQRColorBigMark2;
            }
            set
            {
                this.lQRColorBigMark2 = value;
            }
        }

        public Color QRColorBigMark3
        {
            get
            {
                return this.lQRColorBigMark3;
            }
            set
            {
                this.lQRColorBigMark3 = value;
            }
        }

        public Color QRColorBigMarkBorder
        {
            set
            {
                this.lQRColorBigMarkBorder1 = value;
                this.lQRColorBigMarkBorder2 = value;
                this.lQRColorBigMarkBorder3 = value;
            }
        }

        public Color QRColorBigMarkBorder1
        {
            get
            {
                return this.lQRColorBigMarkBorder1;
            }
            set
            {
                this.lQRColorBigMarkBorder1 = value;
            }
        }

        public Color QRColorBigMarkBorder2
        {
            get
            {
                return this.lQRColorBigMarkBorder2;
            }
            set
            {
                this.lQRColorBigMarkBorder2 = value;
            }
        }

        public Color QRColorBigMarkBorder3
        {
            get
            {
                return this.lQRColorBigMarkBorder3;
            }
            set
            {
                this.lQRColorBigMarkBorder3 = value;
            }
        }

        public Color QRColorFormatInformation
        {
            get
            {
                return this.lQRColorFormatInformation;
            }
            set
            {
                this.lQRColorFormatInformation = value;
            }
        }

        public Color QRColorQuitZone
        {
            get
            {
                return this.lQRColorQuitZone;
            }
            set
            {
                this.lQRColorQuitZone = value;
            }
        }

        public Color QRColorSmallMark
        {
            get
            {
                return this.lQRColorSmallMark;
            }
            set
            {
                this.lQRColorSmallMark = value;
            }
        }

        public Color QRColorTimingPattern
        {
            get
            {
                return this.lQRColorTimingPattern;
            }
            set
            {
                this.lQRColorTimingPattern = value;
            }
        }

        public Color QRColorVersionInformation
        {
            get
            {
                return this.lQRColorVersionInformation;
            }
            set
            {
                this.lQRColorVersionInformation = value;
            }
        }

        public int QRInquireAlphaNumericBits
        {
            get
            {
                return this.lQRInquireAlphaNumericBits;
            }
        }

        public int QRInquireBinaryBits
        {
            get
            {
                return this.lQRInquireBinaryBits;
            }
        }

        public int QRInquireKanjiBits
        {
            get
            {
                return this.lQRInquireKanjiBits;
            }
        }

        public int QRInquireModuleSize
        {
            get
            {
                return this.lQRInquireModuleSize;
            }
        }

        public int QRInquireNumericBits
        {
            get
            {
                return this.lQRInquireNumericBits;
            }
        }

        public QRTextTypes QRInquireTextType
        {
            get
            {
                return this.lQRInquireTextType;
            }
        }

        public QRVersions QRInquireVersion
        {
            get
            {
                return this.lQRInquireVersion;
            }
        }

        public int QRQuitZone
        {
            get
            {
                return this.lQRQuitZone;
            }
            set
            {
                this.lQRQuitZone = value;
            }
        }

        public float QRRotate
        {
            get
            {
                return this.myQRRotate;
            }
            set
            {
                this.myQRRotate = value;
                if (this.Rotate != Rotates.Rotate0)
                {
                    this.Rotate = Rotates.Rotate0;
                }
            }
        }

        public QRECCRates QRSetECCRate
        {
            get
            {
                return this.lQRSetECCRate;
            }
            set
            {
                this.lQRSetECCRate = value;
            }
        }

        public QRTextTypes QRSetTextType
        {
            get
            {
                return this.lQRSetTextType;
            }
            set
            {
                this.lQRSetTextType = value;
            }
        }

        public QRVersions QRSetVersion
        {
            get
            {
                return this.lQRSetVersion;
            }
            set
            {
                if ((value >= QRVersions.Automatic) & (value <= QRVersions.Ver40))
                {
                    this.lQRSetVersion = value;
                }
            }
        }

        public Rotates Rotate
        {
            get
            {
                return this.myRotate;
            }
            set
            {
                this.myRotate = value;
                if (this.QRRotate != 0f)
                {
                    this.QRRotate = 0f;
                }
            }
        }

      


        public enum QRECCRates
        {
            Low7Percent,
            Medium15Percent,
            Quality25Percent,
            HighQuality30Percent
        }

        public enum QRTextTypes
        {
            Automatic,
            Numeric,
            AlphaNumeric,
            Kanji,
            Binary
        }

        public enum QRVersions
        {
            Automatic,
            Ver01,
            Ver02,
            Ver03,
            Ver04,
            Ver05,
            Ver06,
            Ver07,
            Ver08,
            Ver09,
            Ver10,
            Ver11,
            Ver12,
            Ver13,
            Ver14,
            Ver15,
            Ver16,
            Ver17,
            Ver18,
            Ver19,
            Ver20,
            Ver21,
            Ver22,
            Ver23,
            Ver24,
            Ver25,
            Ver26,
            Ver27,
            Ver28,
            Ver29,
            Ver30,
            Ver31,
            Ver32,
            Ver33,
            Ver34,
            Ver35,
            Ver36,
            Ver37,
            Ver38,
            Ver39,
            Ver40
        }

        public enum Rotates
        {
            Rotate0 = 0,
            Rotate180 = 180,
            Rotate270 = 270,
            Rotate90 = 90
        }


    }
}

