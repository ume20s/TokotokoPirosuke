using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace TokoPiro
{
    // 歩数カウンタ用インターフェース
    public interface IStepCounter
    {
        int Steps { get; set; }
        void InitSensorService();
        void StopSensorService();
    }

    public partial class MainPage : ContentPage
    {
        // 歩数カウンタの実装
        readonly IStepCounter StepCounter = DependencyService.Get<IStepCounter>();

        // 歩数管理用変数
        private readonly string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "TokoPiroData.txt";
        private int Date = 0;
        private int Goal = 2000;
        private int HosuToday = 0;
        private int HosuBefor = 0;

        // ピロ助アニメ用変数
        private readonly string[] PiroPng = { "TokoPiro.Images.piro_l.png", "TokoPiro.Images.piro_r.png" };
        private ExImage[] _Pimage;
        private int PiroLeg = 0;

        // ピロ助トーク用変数
        private bool Talking;

        public MainPage()
        {
            // もとからある初期化
            InitializeComponent();

            // センサ権限チェックと取得
            Task<PermissionStatus> task = CheckAndRequestSensorPermission();

            // 歩数カウンタの初期化
            StepCounter.InitSensorService();

            // ファイルから諸データ読み込み
            if (System.IO.File.Exists(localAppData)) {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(localAppData)) {
                    Date = int.Parse(sr.ReadLine());
                    Goal = int.Parse(sr.ReadLine());
                    HosuToday = int.Parse(sr.ReadLine());
                    HosuBefor = int.Parse(sr.ReadLine());
                }
                StepCounter.Steps = HosuToday;
            }

            // 初期状態でトーキングはオフ
            Talking = false;

            // ショップボタン
            ExImage ImageShop = new ExImage {
                Source = ImageSource.FromResource("TokoPiro.Images.s_shop.png"),
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutFlags(ImageShop, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(ImageShop, new Rectangle(0, 1, 0.27, 0.16));
            absoluteLayout.Children.Add(ImageShop);
            ImageShop.Down += (sender, a) => {
                Navigation.PushModalAsync(new ShopPage());
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
                Navigation.PushModalAsync(new MenuPage());
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
                Navigation.PushModalAsync(new KigaePage());
            };

            // ピロ助
            _Pimage = new ExImage[2];
            for(int i = 0; i < 2; i++) {
                _Pimage[i] = new ExImage {
                    Source = ImageSource.FromResource(PiroPng[i]),
                    Aspect = Aspect.Fill
                };
                AbsoluteLayout.SetLayoutFlags(_Pimage[i], AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(_Pimage[i], new Rectangle(0.5, 0.84, 0.36, 0.37));
                absoluteLayout.Children.Add(_Pimage[i]);
                _Pimage[i].Down += (sender, a) => {
                    PiroTalk();
                };
            }
            _Pimage[0].IsVisible = true;
            _Pimage[1].IsVisible = false;

            // タイマー処理
            Device.StartTimer(TimeSpan.FromMilliseconds(800), () => {
                // 日付処理
                if (Date != DateTime.Now.Day) {
                    HosuBefor += HosuToday;
                    HosuToday = 0;
                    StepCounter.Steps = 0;
                    Date = DateTime.Now.Day;
                }

                // 歩数が増えていたら歩数更新してピロ助歩く
                if (HosuToday != StepCounter.Steps) {
                    if (PiroLeg == 0) {
                        _Pimage[0].IsVisible = true;
                        _Pimage[1].IsVisible = false;
                        PiroLeg = 1;
                    } else {
                        _Pimage[1].IsVisible = true;
                        _Pimage[0].IsVisible = false;
                        PiroLeg = 0;
                    }
                }
                HosuToday = StepCounter.Steps;
                DispHosu();
                return true;
            });
        }

        // 歩数表示
        void DispHosu()
        {
            // ラベル表示
            LabelTotal.Text = "トータル：" + (HosuBefor + HosuToday).ToString() + "歩";
            LabelGoal.Text = "目標：" + Goal.ToString() + "歩";
            LabelHosu.Text = HosuToday.ToString() + "歩";

            // 諸データの保存
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(localAppData)) {
                sw.WriteLine(Date.ToString());
                sw.WriteLine(Goal.ToString());
                sw.WriteLine(HosuToday.ToString());
                sw.WriteLine(HosuBefor.ToString());
            }

            // 円の描画（これでOnCanvasViewPaintSurface()が呼び出される）
            Circle.InvalidateSurface();
        }

        // 円の描画
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            // 円の描画領域（半径50）
            SKRect rect = new SKRect(50, 50, info.Width - 50, info.Height - 50);

            // オレンジの塗りつぶし円
            SKPaint outlinePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Orange.ToSKColor(),
            };
            canvas.DrawOval(rect, outlinePaint);

            // 水色の円弧
            float startAngle = (float)270; // 12時の位置は270度
            float sweepAngle;
            sweepAngle = (float)(360 * HosuToday / Goal);
            SKPaint arcPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.SkyBlue.ToSKColor(),
                StrokeWidth = 30
            };
            using (SKPath path = new SKPath()) {
                path.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path, arcPaint);
            }
        }

        // センサへのアクセス権限を得る
        public async Task<PermissionStatus> CheckAndRequestSensorPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Sensors>();

            // 既に権限が付与されていたらおしまい
             if (status == PermissionStatus.Granted)
                return status;

            // 権限をリクエスト
            if (Permissions.ShouldShowRationale<Permissions.Sensors>()) {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.Sensors>();
            return status;
        }

        // ピロ助タップしたらしゃべる
        private void PiroTalk()
        {
            Random r1 = new System.Random();
            string[] Words = { "がんばるッス！", 
                　　　　　　　 "ファイトッス！",
                               "地道に歩くッス！" };

            // しゃべってたらやめる
            if(Talking) {
                LabelTalkRight.IsVisible = false;
                LabelTalkLeft.IsVisible = false;
                Talking = false;
            // しゃべってなかったらしゃべる
            } else {
                if (r1.Next(0, 2) == 0) {
                    LabelTalkLeft.Text = Words[r1.Next(0, 3)];
                    LabelTalkLeft.IsVisible = true;
                } else {
                    LabelTalkRight.Text = Words[r1.Next(0, 3)];
                    LabelTalkRight.IsVisible = true;
                }
                Talking = true;
            }
        }
    }
}
