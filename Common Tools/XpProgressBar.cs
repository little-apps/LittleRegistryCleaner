/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.littleapps.co.cc/)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

// Copyright 2004-2005 Marcos Meli - www.MarcosMeli.com.ar

namespace Common_Tools
{

    #region "  Gradient Mode  "

    public enum GradientMode
    {
        Vertical,
        VerticalCenter,
        Horizontal,
        HorizontalCenter,
        Diagonal
    } ;

    #endregion

    public class XpProgressBar : Control
    {
        #region "  Constructor  "

        private const string CategoryName = "Xp ProgressBar";

        public XpProgressBar()
        { }

        #endregion

        #region "  Private Fields  "

        private Color mColor1 = Color.FromArgb(170, 240, 170);

        private Color mColor2 = Color.FromArgb(10, 150, 10);

        private Color mColorBackGround = Color.White;

        private Color mColorText = Color.Black;

        private Image mDobleBack = null;

        private GradientMode mGradientStyle = GradientMode.VerticalCenter;

        private int mMax = 100;

        private int mMin = 0;

        private int mPosition = 0;

        private byte mSteepDistance = 2;

        private byte mSteepWidth = 6;

        #endregion

        #region "  Dispose  "

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (mDobleBack != null)
                {
                    mDobleBack.Dispose();
                }
                if (mBrush1 != null)
                {
                    mBrush1.Dispose();
                }

                if (mBrush2 != null)
                {
                    mBrush2.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        #endregion

        #region "  Colors   "

        [Category(CategoryName)]
        [Description("The Back Color of the Progress Bar")]
        public Color ColorBackGround
        {
            get { return mColorBackGround; }
            set
            {
                mColorBackGround = value;
                this.InvalidateBuffer(true);
            }
        }

        [Category(CategoryName)]
        [Description("The Border Color of the gradient in the Progress Bar")]
        public Color ColorBarBorder
        {
            get { return mColor1; }
            set
            {
                mColor1 = value;
                this.InvalidateBuffer(true);
            }
        }

        [Category(CategoryName)]
        [Description("The Center Color of the gradient in the Progress Bar")]
        public Color ColorBarCenter
        {
            get { return mColor2; }
            set
            {
                mColor2 = value;
                this.InvalidateBuffer(true);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Set to TRUE to reset all colors like the Windows XP Progress Bar ®")]
        [Category(CategoryName)]
        [DefaultValue(false)]
        public bool ColorsXP
        {
            get { return false; }
            set
            {
                ColorBarBorder = Color.FromArgb(170, 240, 170);
                ColorBarCenter = Color.FromArgb(10, 150, 10);
                ColorBackGround = Color.White;
            }
        }

        [Category(CategoryName)]
        [Description("The Color of the text displayed in the Progress Bar")]
        public Color ColorText
        {
            get { return mColorText; }
            set
            {
                mColorText = value;

                if (this.Text != String.Empty)
                {
                    this.Invalidate();
                }
            }
        }

        #endregion

        #region "  Position   "

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        [Description("The Current Position of the Progress Bar")]
        public int Position
        {
            get { return mPosition; }
            set
            {
                if (value > mMax)
                {
                    mPosition = mMax;
                }
                else if (value < mMin)
                {
                    mPosition = mMin;
                }
                else
                {
                    mPosition = value;
                }
                this.Invalidate();
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        [Description("The Max Position of the Progress Bar")]
        public int PositionMax
        {
            get { return mMax; }
            set
            {
                if (value > mMin)
                {
                    mMax = value;

                    if (mPosition > mMax)
                    {
                        Position = mMax;
                    }

                    this.InvalidateBuffer(true);
                }
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        [Description("The Min Position of the Progress Bar")]
        public int PositionMin
        {
            get { return mMin; }
            set
            {
                if (value < mMax)
                {
                    mMin = value;

                    if (mPosition < mMin)
                    {
                        Position = mMin;
                    }
                    this.InvalidateBuffer(true);
                }
            }
        }

        [Category(CategoryName)]
        [Description("The number of Pixels between two Steeps in Progress Bar")]
        [DefaultValue((byte)2)]
        public byte SteepDistance
        {
            get { return mSteepDistance; }
            set
            {
                if (value >= 0)
                {
                    mSteepDistance = value;
                    this.InvalidateBuffer(true);
                }
            }
        }

        #endregion

        #region  "  Progress Style   "

        [Category(CategoryName)]
        [Description("The Style of the gradient bar in Progress Bar")]
        [DefaultValue(GradientMode.VerticalCenter)]
        public GradientMode GradientStyle
        {
            get { return mGradientStyle; }
            set
            {
                if (mGradientStyle != value)
                {
                    mGradientStyle = value;
                    CreatePaintElements();
                    this.Invalidate();
                }
            }
        }

        [Category(CategoryName)]
        [Description("The number of Pixels of the Steeps in Progress Bar")]
        [DefaultValue((byte)6)]
        public byte SteepWidth
        {
            get { return mSteepWidth; }
            set
            {
                if (value > 0)
                {
                    mSteepWidth = value;
                    this.InvalidateBuffer(true);
                }
            }
        }

        #endregion

        #region "  BackImage  "

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set
            {
                base.BackgroundImage = value;
                InvalidateBuffer();
            }
        }

        #endregion

        #region "  Text Override  "

        [Category(CategoryName)]
        [Description("The Text displayed in the Progress Bar")]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    this.Invalidate();
                }
            }
        }

        #endregion

        #region "  Text Shadow  "

        private bool mTextShadow = true;

        [Category(CategoryName)]
        [Description("Set the Text shadow in the Progress Bar")]
        [DefaultValue(true)]
        public bool TextShadow
        {
            get { return mTextShadow; }
            set
            {
                mTextShadow = value;
                this.Invalidate();
            }
        }

        #endregion

        #region "  Text Shadow Alpha  "

        private byte mTextShadowAlpha = 150;

        [Category(CategoryName)]
        [Description("Set the Alpha Channel of the Text shadow in the Progress Bar")]
        [DefaultValue((byte)150)]
        public byte TextShadowAlpha
        {
            get { return mTextShadowAlpha; }
            set
            {
                if (mTextShadowAlpha != value)
                {
                    mTextShadowAlpha = value;
                    this.TextShadow = true;
                }
            }
        }

        #endregion

        #region "  Paint Methods  "

        #region "  OnPaint  "

        protected override void OnPaint(PaintEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Paint " + this.Name + "  Pos: "+this.Position.ToString());
            try
            {
                int mSteepTotal = mSteepWidth + mSteepDistance;
                float mUtilWidth = this.Width - 6 + mSteepDistance;

                if (mDobleBack == null)
                {
                    mUtilWidth = this.Width - 6 + mSteepDistance;
                    int mMaxSteeps = (int)(mUtilWidth / mSteepTotal);
                    this.Width = 6 + mSteepTotal * mMaxSteeps;

                    mDobleBack = new Bitmap(this.Width, this.Height);

                    Graphics g2 = Graphics.FromImage(mDobleBack);

                    CreatePaintElements();

                    g2.Clear(mColorBackGround);

                    if (this.BackgroundImage != null)
                    {
                        TextureBrush textuBrush = new TextureBrush(this.BackgroundImage, WrapMode.Tile);
                        g2.FillRectangle(textuBrush, 0, 0, this.Width, this.Height);
                        textuBrush.Dispose();
                    }
                    //				g2.DrawImage()

                    g2.DrawRectangle(mPenOut2, outnnerRect2);
                    g2.DrawRectangle(mPenOut, outnnerRect);
                    g2.DrawRectangle(mPenIn, innerRect);
                    g2.Dispose();

                }

                Image ima = new Bitmap(mDobleBack);

                Graphics gtemp = Graphics.FromImage(ima);

                int mCantSteeps = (int)((((float)mPosition - mMin) / (mMax - mMin)) * mUtilWidth / mSteepTotal);

                for (int i = 0; i < mCantSteeps; i++)
                {
                    DrawSteep(gtemp, i);
                }

                if (this.Text != String.Empty)
                {
                    gtemp.TextRenderingHint = TextRenderingHint.AntiAlias;
                    DrawCenterString(gtemp, this.ClientRectangle);
                }

                e.Graphics.DrawImage(ima, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle, GraphicsUnit.Pixel);
                ima.Dispose();
                gtemp.Dispose();
            }
            catch (ArgumentNullException)
            {
                return;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        #endregion

        #region "  OnSizeChange  "

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!this.IsDisposed)
            {
                if (this.Height < 12)
                {
                    this.Height = 12;
                }

                base.OnSizeChanged(e);
                this.InvalidateBuffer(true);
            }

        }

        protected override Size DefaultSize
        {
            get { return new Size(100, 29); }
        }


        #endregion

        #region "  More Draw Methods  "

        private void DrawSteep(Graphics g, int number)
        {
            g.FillRectangle(mBrush1, 4 + number * (mSteepDistance + mSteepWidth), mSteepRect1.Y + 1, mSteepWidth, mSteepRect1.Height);
            g.FillRectangle(mBrush2, 4 + number * (mSteepDistance + mSteepWidth), mSteepRect2.Y + 1, mSteepWidth, mSteepRect2.Height - 1);
        }

        private void InvalidateBuffer()
        {
            InvalidateBuffer(false);
        }

        private void InvalidateBuffer(bool InvalidateControl)
        {
            if (mDobleBack != null)
            {
                mDobleBack.Dispose();
                mDobleBack = null;
            }

            if (InvalidateControl)
            {
                this.Invalidate();
            }
        }

        private void DisposeBrushes()
        {
            if (mBrush1 != null)
            {
                mBrush1.Dispose();
                mBrush1 = null;
            }

            if (mBrush2 != null)
            {
                mBrush2.Dispose();
                mBrush2 = null;
            }

        }

        private void DrawCenterString(Graphics gfx, Rectangle box)
        {
            SizeF ss = gfx.MeasureString(this.Text, this.Font);

            float left = box.X + (box.Width - ss.Width) / 2;
            float top = box.Y + (box.Height - ss.Height) / 2;

            if (mTextShadow)
            {
                SolidBrush mShadowBrush = new SolidBrush(Color.FromArgb(mTextShadowAlpha, Color.Black));
                gfx.DrawString(this.Text, this.Font, mShadowBrush, left + 1, top + 1);
                mShadowBrush.Dispose();
            }
            SolidBrush mTextBrush = new SolidBrush(mColorText);
            gfx.DrawString(this.Text, this.Font, mTextBrush, left, top);
            mTextBrush.Dispose();

        }

        #endregion

        #region "  CreatePaintElements   "

        private Rectangle innerRect;
        private LinearGradientBrush mBrush1;
        private LinearGradientBrush mBrush2;
        private Pen mPenIn = new Pen(Color.FromArgb(239, 239, 239));

        private Pen mPenOut = new Pen(Color.FromArgb(104, 104, 104));
        private Pen mPenOut2 = new Pen(Color.FromArgb(190, 190, 190));

        private Rectangle mSteepRect1;
        private Rectangle mSteepRect2;
        private Rectangle outnnerRect;
        private Rectangle outnnerRect2;

        private void CreatePaintElements()
        {
            DisposeBrushes();

            switch (mGradientStyle)
            {
                case GradientMode.VerticalCenter:

                    mSteepRect1 = new Rectangle(
                        0,
                        2,
                        mSteepWidth,
                        this.Height / 2 + (int)(this.Height * 0.05));
                    mBrush1 = new LinearGradientBrush(mSteepRect1, mColor1, mColor2, LinearGradientMode.Vertical);

                    mSteepRect2 = new Rectangle(
                        0,
                        mSteepRect1.Bottom - 1,
                        mSteepWidth,
                        this.Height - mSteepRect1.Height - 4);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, mColor2, mColor1, LinearGradientMode.Vertical);
                    break;

                case GradientMode.Vertical:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        this.Height - 7);
                    mBrush1 = new LinearGradientBrush(mSteepRect1, mColor1, mColor2, LinearGradientMode.Vertical);
                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, mColor2, mColor1, LinearGradientMode.Horizontal);
                    break;


                case GradientMode.Horizontal:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        this.Height - 7);

                    //					mBrush1 = new LinearGradientBrush(rTemp, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mBrush1 = new LinearGradientBrush(this.ClientRectangle, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, Color.Red, Color.Red, LinearGradientMode.Horizontal);
                    break;


                case GradientMode.HorizontalCenter:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        this.Height - 7);
                    //					mBrush1 = new LinearGradientBrush(rTemp, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mBrush1 = new LinearGradientBrush(this.ClientRectangle, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mBrush1.SetBlendTriangularShape(0.5f);

                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, Color.Red, Color.Red, LinearGradientMode.Horizontal);
                    break;


                case GradientMode.Diagonal:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        this.Height - 7);
                    //					mBrush1 = new LinearGradientBrush(rTemp, mColor1, mColor2, LinearGradientMode.ForwardDiagonal);
                    mBrush1 = new LinearGradientBrush(this.ClientRectangle, mColor1, mColor2, LinearGradientMode.ForwardDiagonal);
                    //					((LinearGradientBrush) mBrush1).SetBlendTriangularShape(0.5f);

                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, Color.Red, Color.Red, LinearGradientMode.Horizontal);
                    break;

                default:
                    mBrush1 = new LinearGradientBrush(mSteepRect1, mColor1, mColor2, LinearGradientMode.Vertical);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, mColor2, mColor1, LinearGradientMode.Vertical);
                    break;

            }

            innerRect = new Rectangle(
                this.ClientRectangle.X + 2,
                this.ClientRectangle.Y + 2,
                this.ClientRectangle.Width - 4,
                this.ClientRectangle.Height - 4);
            outnnerRect = new Rectangle(
                this.ClientRectangle.X,
                this.ClientRectangle.Y,
                this.ClientRectangle.Width - 1,
                this.ClientRectangle.Height - 1);
            outnnerRect2 = new Rectangle(
                this.ClientRectangle.X + 1,
                this.ClientRectangle.Y + 1,
                this.ClientRectangle.Width,
                this.ClientRectangle.Height);

        }

        #endregion

        #endregion
    }

}