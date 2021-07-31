﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TokoPiro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KigaePage : ContentPage
    {
        public KigaePage()
        {
            InitializeComponent();
        }

        // 戻るボタン
        void OnBtnClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}