using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;
using YPS.Helpers;
using YPS.iOS.Dependencies;

[assembly: Dependency(typeof(OpenSdcardiOS))]
namespace YPS.iOS.Dependencies
{
    public class OpenSdcardiOS : IOpenSdCard
    {
        public async void openSdCard()
        {
            FileData fileData1 = await CrossFilePicker.Current.PickFile();

            if (fileData1 == null)
                return; // user canceled file picking
        }
    }
}