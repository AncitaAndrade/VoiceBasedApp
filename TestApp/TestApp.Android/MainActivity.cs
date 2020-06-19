using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.Permissions;
using Autofac;
//using VoiceToCommandLib.Droid;
using VoiceToCommandLib.Android;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
//using VoiceToCommand.Core;
using VoiceToCommandLibrary;


namespace TestApp.Droid
{
    [Activity(Label = "TestApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        //private static IContainer Container { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            ContainerBuilder cb = new ContainerBuilder();
            // cb.RegisterType<VoiceToCommandServiceAndroid>().As<IVoiceToCommandService>().SingleInstance();
            cb.RegisterType<SpeechToTextService>().As<IVoiceToCommandService>().SingleInstance();
            IContainer container = cb.Build();
           AutofacServiceLocator serviceLocator = new AutofacServiceLocator(container);
           ServiceLocator.SetLocatorProvider(() => serviceLocator);

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}