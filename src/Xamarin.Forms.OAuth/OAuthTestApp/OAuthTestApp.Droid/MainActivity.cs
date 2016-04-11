
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace OAuthTestApp.Droid
{
    [Activity(Label = "OAuthTestApp", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        public override void OnBackPressed()
        {
            if (!App.HandleBackButton())
                base.OnBackPressed();
        }
    }
}

