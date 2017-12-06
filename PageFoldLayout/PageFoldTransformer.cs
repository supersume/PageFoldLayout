using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;

namespace Murmur.Pagefoldlayout
{
    public class PageFoldTransformer : Java.Lang.Object, ViewPager.IPageTransformer
    {
        public void TransformPage(View view, float position)
        {
            int width = view.Width;
            if (position < -1)
            { // [-Infinity,-1)
              // This page is way off-screen to the left.
                view.TranslationX = 0;

            }
            else if (position <= 0)
            { // [-1,0]
              //Log.d("kanna",view.getTag().toString() + " position " + position);
                if (position != -1)
                {
                    ((PageFoldLayout)view).UpdatePoint(position);
                    view.TranslationX = (-position * width);
                }
                else
                {
                    view.TranslationX = 0;
                }
            }
            else if (position <= 1)
            { // (0,1]
              //Log.d("kanna",view.getTag().toString()+ " position " + position);
                if (position != 1)
                {
                    ((PageFoldLayout)view).UpdatePoint(position);
                    view.TranslationX = (-position * width);
                }
                else
                {
                    view.TranslationX = (0);
                }
            }
            else
            { // (1,+Infinity]
              // This page is way off-screen to the right.
                view.TranslationX = 0;
            }
        }
    }
}