using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace TokoPiro
{
    public partial class MainPage : ContentPage
    {
        SKImageInfo info;
        SKSurface surface;
        SKCanvas canvas;
        private int Goal;
        private int Hosu;

        public MainPage()
        {
            InitializeComponent();

            //ショップボタン
            ExImage ImageShop = new ExImage {
                Source = ImageSource.FromResource("TokoPiro.Images.s_shop.png"),
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutFlags(ImageShop, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(ImageShop, new Rectangle(0, 1, 0.27, 0.16));
            absoluteLayout.Children.Add(ImageShop);
            ImageShop.Down += (sender, a) => { 
            };

            // メニューボタン
            ExImage ImageMenu = new ExImage {
                Source = ImageSource.FromResource("TokoPiro.Images.s_menu.png"),
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutFlags(ImageMenu, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(ImageMenu, new Rectangle(0.5, 1, 0.46, 0.1));
            absoluteLayout.Children.Add(ImageMenu);
            ImageMenu.Down += (sender, a) => {
                DispHosu();
            };

            // 着替えボタン
            ExImage ImageWear = new ExImage {
                Source = ImageSource.FromResource("TokoPiro.Images.s_wear.png"),
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutFlags(ImageWear, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(ImageWear, new Rectangle(1, 1, 0.27, 0.16));
            absoluteLayout.Children.Add(ImageWear);
            ImageWear.Down += (sender, a) => {
            };

            // ピロ助
            ImagePiro.Source = ImageSource.FromResource("TokoPiro.Images.piro_l.png");


            // 以下デバッグ用
            Goal = 1200;
            Hosu = 694;
        }

        // 歩数描画領域（円）の初期化
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            info = args.Info;
            surface = args.Surface;
            canvas = surface.Canvas;
            canvas.Clear();

            // 円の描画
            SKRect rect = new SKRect(50, 50, info.Width - 50, info.Height - 50);
            float startAngle = (float)270;
            float sweepAngle;
            sweepAngle = (float)(360 * 694 / Goal);

            SKPaint outlinePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Orange.ToSKColor(),
            };

            SKPaint arcPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.SkyBlue.ToSKColor(),
                StrokeWidth = 30
            };

            canvas.DrawOval(rect, outlinePaint);
            using (SKPath path = new SKPath()) {
                path.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path, arcPaint);
            }

        }

        // 歩数表示
        void DispHosu()
        {
            // ラベル表示
            LabelGoal.Text = "目標：" + Goal.ToString() + "歩";
            LabelHosu.Text = Hosu.ToString() + "歩";

            // 円の描画
            SKRect rect = new SKRect(50, 50, info.Width - 50, info.Height - 50);
            float startAngle = (float)270;
            float sweepAngle;
            if (Hosu >= Goal) {
                sweepAngle = (float)360;
            } else {
                sweepAngle = (float)(360 * Hosu / Goal);
            }

            SKPaint outlinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.GreenYellow.ToSKColor(),
                StrokeWidth = 10
            };
            SKPaint arcPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Orange.ToSKColor(),
                StrokeWidth = 30
            };

            canvas.DrawOval(rect, outlinePaint);
            using (SKPath path = new SKPath()) {
              path.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path, arcPaint);
            }
        }
    }
}
