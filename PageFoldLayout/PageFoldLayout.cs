using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageFoldLayout
{
    public class PageFoldLayout : FrameLayout
    {
        private int mWidth, mHeight;
        private int HEIGHT_LIMIT;
        private float mPointX;
        private float mPointY;
        private float mDegree;
        private float mPercent = 0.0f;
        private bool isShort;
        //private Paint mPaint;
        private Path mPath;
        private Path mPathFoldandNext;

        public PageFoldLayout(Context context) : base(context) { }

        public PageFoldLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public PageFoldLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }

        private float BezierP(float p1, float p2, float p3, float t)
        {
            return (1 - t) * (1 - t) * p1 + 2 * (1 - t) * t * p2 + t * t * p3;
        }

        public void UpdatePoint(float percent)
        {
            mPercent = percent;
            if (percent < 0)
            {
                percent += 1;
            }
            mPointX = BezierP(0f, mWidth, (mWidth << 1), percent);
            mPointY = BezierP(mHeight, HEIGHT_LIMIT, mHeight, percent);
            //Log.d("kanna", "update " + mPointX + " " + mPointY + " " + percent + " " + angle);
            Invalidate();
        }

        private void InitPaint()
        {
            /*
            mPaint = new Paint(Paint.ANTI_ALIAS_FLAG | Paint.DITHER_FLAG);
            mPaint.setStyle(Paint.Style.STROKE);
            mPaint.setStrokeWidth(10);*/

            mPath = new Path();
            mPathFoldandNext = new Path();
        }

        protected void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            InitPaint();
            mWidth = w;
            mHeight = h;
            HEIGHT_LIMIT = h >> 1;
        }

        private void DrawView(Canvas canvas)
        {
            if (mPercent > 0)
            {
                //upper region
                canvas.Save();
                canvas.ClipRect(0, 0, mWidth, mHeight);
                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    canvas.ClipPath(mPathFoldandNext, Region.Op.Difference);
                }
                else
                {
                    canvas.ClipOutPath(mPathFoldandNext);
                }
                //canvas.drawColor(Color.WHITE);
                base.DispatchDraw(canvas);
                canvas.Restore();


                //fold region
                canvas.Save();
                canvas.ClipPath(mPath);
                //canvas.clipPath(mPathFoldandNext);

                //cos@ = sizeShort - mPointX / sizeShort
                if (isShort)
                {
                    canvas.Translate(mPointX, mPointY);
                    canvas.Rotate(mDegree);
                    canvas.Scale(1, -1);
                    canvas.Translate(0, -mHeight);
                }
                //sin@ = mHeight - mPointY / sizeShort
                else
                {
                    canvas.Translate(mPointX, mPointY);
                    canvas.Rotate(-mDegree);
                    canvas.Scale(-1, 1);
                    canvas.Translate(0, -mHeight);
                }


                base.DispatchDraw(canvas);
                //canvas.drawColor(Color.WHITE);
                canvas.Restore();
            }
            else
            {
                canvas.Save();
                canvas.ClipPath(mPathFoldandNext);
                //canvas.clipRegion(regionFold, Region.Op.DIFFERENCE);
                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    canvas.ClipPath(mPath, Region.Op.Difference);
                }
                else
                {
                    canvas.ClipOutPath(mPath);
                }
                base.DispatchDraw(canvas);
                canvas.Restore();
            }
        }

        private void DispatchDrawUsingBitmap(Canvas canvas)
        {
            mPath.Reset();
            mPathFoldandNext.Reset();
            canvas.DrawColor(Color.Transparent);

            float TX = mHeight - mPointY;
            float OX = mPointX;
            float temp = (float)(Math.Pow(TX, 2) + Math.Pow(OX, 2));

            float sizeShort = temp / (2 * OX);
            float sizeLong = temp / (2 * TX);

            if (sizeShort > sizeLong)
            {
                isShort = true;
                float sin = (sizeShort - mPointX) / sizeShort;
                mDegree = (float)(Math.Acos(sin) / Math.PI * 180);
                //Log.d("kanna","get "+ mDegree);
            }
            else
            {
                isShort = false;
                float sin = (mHeight - mPointY) / sizeShort;
                mDegree = (float)(Math.Asin(sin) / Math.PI * 180);
            }


            mPath.MoveTo(mPointX, mPointY);
            mPathFoldandNext.MoveTo(0, mHeight);

            if (sizeLong > mHeight)
            {
                float an = sizeLong - mHeight;
                float nm = an / (sizeLong - TX) * OX;
                float nq = an / sizeLong * sizeShort;
                mPath.LineTo(nm, 0);
                mPath.LineTo(nq, 0);
                mPath.LineTo(sizeShort, mHeight);
                mPath.Close();

                mPathFoldandNext.LineTo(0, 0);
                mPathFoldandNext.LineTo(nm, 0);
                mPathFoldandNext.LineTo(mPointX, mPointY);
                mPathFoldandNext.LineTo(sizeShort, mHeight);
                mPathFoldandNext.Close();


            }
            else
            {
                mPath.LineTo(0, mHeight - sizeLong);
                mPath.LineTo(sizeShort, mHeight);
                mPath.Close();


                mPathFoldandNext.LineTo(0, mHeight - sizeLong);
                mPathFoldandNext.LineTo(mPointX, mPointY);
                mPathFoldandNext.LineTo(sizeShort, mHeight);
                mPathFoldandNext.Close();
            }
            DrawView(canvas);
            /*
            mPaint.setColor(Color.BLACK);
            canvas.drawPath(mPath, mPaint);
            mPaint.setColor(Color.RED);
            canvas.drawPath(mPathFoldandNext, mPaint);*/
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            if (Width <= 0 || Height <= 0 || mPercent == 0)
            {
                base.DispatchDraw(canvas);
            }
            else
            {
                DispatchDrawUsingBitmap(canvas);
            }
            
        }
    }
}
