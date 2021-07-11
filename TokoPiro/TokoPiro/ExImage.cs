using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TokoPiro
{
    // 押下イベント取得のためにImageクラスを拡張
    public class ExImage : Image
    {
        public event EventHandler Down;
        public bool OnDown()
        {
            Down?.Invoke(this, new EventArgs());
            return true;
        }
    }
}
