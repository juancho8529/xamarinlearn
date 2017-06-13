using Android.App;
using Android.Widget;
using Android.OS;
using System.Timers;
using System;
using Android.Graphics;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace CapturaPantalla
{
    [Activity(Label = "CapturaPantalla", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Timer timer = new Timer();
        Random rnd = new Random();
        TextView txtRev;
        TextView txtVel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            timer.Interval = 1000;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            timer.Start();

            txtRev = FindViewById<TextView>(Resource.Id.txtDatoUno);
            txtVel = FindViewById<TextView>(Resource.Id.txtDato2);

            Button buttonCapture = FindViewById<Button>(Resource.Id.button1);
            buttonCapture.Click += ButtonCapture_Click;
        }

        private async void ButtonCapture_Click(object sender, EventArgs e)
        {
            var view = this.Window.DecorView;
            view.DrawingCacheEnabled = true;

            Bitmap bitmap = view.GetDrawingCache(true);
            byte[] bitmapData;

            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                bitmapData = stream.ToArray();

                var container = GetContainer();
                string identifier = $"{Guid.NewGuid()}.jpg";
                var fileBlob = container.GetBlockBlobReference(identifier);

                await fileBlob.UploadFromByteArrayAsync(bitmapData, 0, bitmapData.Length);
                System.Threading.Thread.Sleep(2000);
                Toast.MakeText(this, "captura hecha", ToastLength.Long).Show();
            }
        }

        private CloudBlobContainer GetContainer()
        {
            var account = CloudStorageAccount.Parse("");
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference("logs");
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            string rev = string.Concat(rnd.Next(1001, 5000), "rpm");
            string vel = string.Concat(rnd.Next(0, 80), "km/h");
            RunOnUiThread(() =>
            {
                txtRev.SetText(rev, TextView.BufferType.Normal);
                txtVel.SetText(vel, TextView.BufferType.Normal);
            });
        }
    }
}

