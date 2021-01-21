using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using static YPS.Parts2y.Parts2y_Models.PhotoModel;
using YPS.Parts2y.Parts2y_Services;
using YPS.Parts2y.Parts2y_SQLITE;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class EPODPhotoUplodeViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand select_pic { set; get; }
        public ICommand CrossCommand { set; get; }
        public ICommand upload_pic { set; get; }
        int photoCounts = 0;
        string extension = "";
        public Stream picStream;
        public string fileName;
        public string filepath;
        string Mediafile;

        ObservableCollection<CustomPhotoModel> addedlist = new ObservableCollection<CustomPhotoModel>();

        public EPODPhotoUplodeViewModel(INavigation _Navigation)
        {
            try
            {
                Navigation = _Navigation;
                BgColor = Settings.Bar_Background;
                select_pic = new Command(async () => await SelectPic());
                upload_pic = new Command(async () => await uploadPic());
                CrossCommand = new Command(async () => await CrossCommand_click());

                if (Settings.ScanCompleted == "1" && !string.IsNullOrEmpty(Settings.PDICompleted))
                {
                    CameraEnable = false;
                    Camera_Opacity = 0.2;
                }
                else
                {
                    CameraEnable = true;
                    Camera_Opacity = 1;
                }

                GetUploadedImages();
            }
            catch (Exception ex)
            {

            }

        }

        public EPODPhotoUplodeViewModel()
        {
            GetUploadedImages();
        }

        public void GetUploadedImages()
        {
            try
            {
                IndicatorVisibility = true;
                SupTransportDB veDetailsDB = new SupTransportDB("image");
                photolist = new ObservableCollection<CustomPhotoModel>(veDetailsDB.GetImage());
                secondStack = true;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        private async Task CrossCommand_click()
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task SelectPic()
        {
            try
            {
                string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery");
                if (action == "Camera")
                {
                    IndicatorVisibility = true;
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        btnenable = false;

                        var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        var statusiOS = resultIOS[Permission.Camera];
                        if (statusiOS == PermissionStatus.Denied)
                        {
                            var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the camera to take photos.", null, null, "Maybe Later", "Settings");
                            switch (checkSelect)
                            {
                                case "Maybe Later":
                                    break;
                                case "Settings":
                                    CrossPermissions.Current.OpenAppSettings();
                                    break;
                            }
                        }
                        else if (statusiOS == PermissionStatus.Granted)
                        {
                            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                            {
                                PhotoSize = PhotoSize.Custom,
                                CustomPhotoSize = 35,
                                CompressionQuality = 100
                            });
                            if (file != null)
                            {
                                IndicatorVisibility = false;
                                btnenable = true;
                                CaptchaImage1 = ImageSource.FromStream(() =>
                                {
                                    return file.GetStreamWithImageRotatedForExternalStorage();
                                });
                                firstStack = true;
                                secondStack = false;
                                NoPhotos_Visibility = false;

                                extension = System.IO.Path.GetExtension(file.Path);//. Substring(file.Path.Length - 3, 3);
                                Mediafile = file.Path;                                               // base64Picture = ReadImageFile(file.Path);
                                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                fileName = System.IO.Path.GetFileNameWithoutExtension(file.Path);
                                FirstMainStack = false;
                                SecondMainStack = true;
                            }
                        }

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                    }
                    btnenable = true;
                }
                else if (action == "Gallery")
                {
                    IndicatorVisibility = true;
                    var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                    var statusiOS = resultIOS[Permission.Photos];
                    if (statusiOS == PermissionStatus.Denied)
                    {
                        var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the gallery to take photos.", null, null, "Maybe Later", "Settings");
                        switch (checkSelect)
                        {
                            case "Maybe Later":
                                break;
                            case "Settings":
                                CrossPermissions.Current.OpenAppSettings();
                                break;
                        }
                    }
                    else if (statusiOS == PermissionStatus.Granted)
                    {
                        IndicatorVisibility = true;

                        #region new code
                        var fileOS = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                        {
                            PhotoSize = PhotoSize.Medium,
                            CustomPhotoSize = 50,
                            CompressionQuality = 100
                        });

                        if (fileOS != null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                CaptchaImage1 = ImageSource.FromFile(fileOS.Path);
                                firstStack = true;
                                secondStack = false;
                                FirstMainStack = false;
                                SecondMainStack = true;
                                NoPhotos_Visibility = false;
                                btnenable = true;

                                Mediafile = fileOS.Path;
                                extension = System.IO.Path.GetExtension(fileOS.Path);//.Substring(imagepaths.Length - 3, 3);
                                picStream = fileOS.GetStreamWithImageRotatedForExternalStorage(); //System.IO.File.OpenRead(fileOS.Path);
                                                                                                  // base64Picture = ReadImageFile(imagepaths);
                                fileName = System.IO.Path.GetFileNameWithoutExtension(fileOS.Path);
                            });
                        }

                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                btnenable = true;
                IndicatorVisibility = false;
            }
        }
        public async Task uploadPic()
        {
            try
            {
                IndicatorVisibility = true;

                FirstMainStack = true;
                SecondMainStack = firstStack = secondStack = false;

                #region api calling need
                await Photo_Upload();
                #endregion
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }
        public async Task Photo_Upload()
        {
            try
            {

                Device.BeginInvokeOnMainThread(async () =>
                {
                    // Write your code here
                    IndicatorVisibility = true;
                    try
                    {
                        if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".gif" || extension == ".bmp")
                        {
                            secondStack = true;
                            addedlist.Clear();
                            addedlist.Add(new CustomPhotoModel { PhotoURL = Mediafile, PhotoID = 1, vinno = Settings.VINNo, queseq = Settings.QNo_Sequence, email = Settings.UserMail });
                            photolist = addedlist;
                            SupTransportDB veDetailsDB = new SupTransportDB("image");
                            veDetailsDB.SaveImage(addedlist);
                            photolist = new ObservableCollection<CustomPhotoModel>(veDetailsDB.GetImage());
                        }
                        else
                        {
                            App.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .png, .jpg, .jpeg, .gif, .bmp only.", "ok");
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                    IndicatorVisibility = false;
                });

            }
            catch (Exception ex)
            {

            }
        }

        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<CustomPhotoModel> _photolist;
        public ObservableCollection<CustomPhotoModel> photolist
        {
            get { return _photolist; }
            set
            {
                _photolist = value;
                NotifyPropertyChanged("photolist");
            }
        }

        private Color _BgColor;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }


        private bool _btnenable = true;
        public bool btnenable
        {
            get { return _btnenable; }
            set
            {
                _btnenable = value;
                NotifyPropertyChanged();
            }
        }

        private bool _FirstMainStack = true;
        public bool FirstMainStack
        {
            get { return _FirstMainStack; }
            set
            {
                _FirstMainStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _SecondMainStack = false;
        public bool SecondMainStack
        {
            get { return _SecondMainStack; }
            set
            {
                _SecondMainStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _firstStack = false;
        public bool firstStack
        {
            get { return _firstStack; }
            set
            {
                _firstStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _secondStack = false;
        public bool secondStack
        {
            get { return _secondStack; }
            set
            {
                _secondStack = value;
                NotifyPropertyChanged();
            }
        }

        public bool _NoPhotos_Visibility = false;
        public bool NoPhotos_Visibility
        {
            get { return _NoPhotos_Visibility; }
            set
            {
                _NoPhotos_Visibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool _listStack = true;
        public bool listStack
        {
            get { return _listStack; }
            set
            {
                _listStack = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource _captchaImage1;
        public ImageSource CaptchaImage1
        {
            get { return _captchaImage1; }
            set
            {
                _captchaImage1 = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource _captchaImage2;
        public ImageSource CaptchaImage2
        {
            get { return _captchaImage2; }
            set
            {
                _captchaImage2 = value;
                NotifyPropertyChanged();
            }
        }
        public bool _CameraEnable = true;
        public bool CameraEnable
        {
            get { return _CameraEnable; }
            set
            {
                _CameraEnable = value;
                NotifyPropertyChanged();
            }
        }
        public double _Camera_Opacity = 1.0;
        public double Camera_Opacity
        {
            get { return _Camera_Opacity; }
            set
            {
                _Camera_Opacity = value;
                NotifyPropertyChanged();
            }
        }


    }
}
