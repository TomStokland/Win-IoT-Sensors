using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Communications;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sensor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        LPS25H lps = new LPS25H();
        LSM9DS1 lsm = new LSM9DS1();
        VL6180X tof = new VL6180X();
        BME280 bme = new BME280();

        public MainPage()
        {
            this.InitializeComponent();
            //TestVL6180X();
        }

        private void TestVL6180X()
        {
            int range = 0;

            while(true)
                range = tof.Range();
        }
    }
}
