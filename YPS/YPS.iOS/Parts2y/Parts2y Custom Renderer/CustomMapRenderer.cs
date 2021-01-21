using CoreGraphics;
//using Rg.Plugins.Popup.Extensions;

using Foundation;
using MapKit;
using ObjCRuntime;
using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using YPS.iOS.Parts2y.Parts2y_Custom_Renderer;
using YPS.Parts2y.Parts2y_Common_Classes;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace YPS.iOS.Parts2y.Parts2y_Custom_Renderer
{
    public class CustomMapRenderer : MapRenderer
    {
        //ResourceService resourceService;
        //CLLocationManager
        object _lastTouchedView;
        UIView customPinView;
        List<CustomPin> customPins;
        MKMapView nativeMap;
        public event EventHandler<CustomPin> mapClickEvent;
        public event EventHandler<NavigationPage> NavigationToNewPage;
        public INavigation nav;
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {

            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.GetViewForAnnotation = null;
                nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                customPins = formsMap.CustomPins;
                nav = formsMap.Navigation;
                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;

            }
        }


        MKPointAnnotation annotate = null;
        MKAnnotationView annotationView = null;
        MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {


            if (annotation is MKUserLocation)
                return null;

            var customPin = GetCustomPin(annotation as MKPointAnnotation);

            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            annotate = new MKPointAnnotation();
            //annotate.Title = string.Empty;
            annotate.Subtitle = string.Empty;
            annotate.Coordinate = annotation.Coordinate;



            UILabel Label1 = new UILabel();
            Label1.Text = customPin.Label.ToString().Trim();
            Label1.LineBreakMode = UILineBreakMode.WordWrap;
            Label1.AdjustsFontSizeToFitWidth = true;
            Label1.AdjustsLetterSpacingToFitWidth = true;
            var width = NSLayoutConstraint.Create(Label1, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, null, NSLayoutAttribute.NoAttribute, 1, 200);
            Label1.AddConstraint(width);
            var height = NSLayoutConstraint.Create(Label1, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 40);
            Label1.AddConstraint(height);
            Label1.Lines = 2;
            annotationView = mapView.DequeueReusableAnnotation(customPin.Label);
            mapView.Annotations[0] = annotate;
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.Id);
                annotationView.Image = UIImage.FromFile("marker.png");
                //if (customPin.hexStrin == "green")
                //    annotationView.Image = UIImage.FromFile("marker.png");
                //else if (customPin.hexStrin == "red")
                //    annotationView.Image = UIImage.FromFile("red_marker.png");
                //else
                //    annotationView.Image = UIImage.FromFile("yellow_marker.png");
                annotationView.CalloutOffset = new CGPoint(0, 0);
                //annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("red_marker.png")); //new UIImageView(UIImageFromUrl(customPin.ScanCentreImage));
                annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                annotationView.SizeToFit();
                annotationView.DetailCalloutAccessoryView = Label1;
                ((CustomMKAnnotationView)annotationView).Id = customPin.Id;
                ((CustomMKAnnotationView)annotationView).Url = customPin.Url;
                annotationView.CanShowCallout = true;
            }
            AttachGestureToPin(annotationView, annotation);
            return annotationView;
        }
        //Each Radiology center icon
        //public static UIImage UIImageFromUrl(string uri)
        //{
        //    using (var url = new NSUrl(uri))
        //    using (var data = NSData.FromUrl(url))
        //        return UIImage.LoadFromData(data);
        //}

        protected void AttachGestureToPin(MKAnnotationView mapPin, IMKAnnotation annotation)
        {
            var recognizers = mapPin.GestureRecognizers;

            if (recognizers != null)
            {
                foreach (var r in recognizers)
                {
                    mapPin.RemoveGestureRecognizer(r);
                }
            }


            var recognizer = new UITapGestureRecognizer(g => OnClick(annotation, g))
            {
                ShouldReceiveTouch = (gestureRecognizer, touch) =>
                {
                    _lastTouchedView = touch.View;
                    return true;
                }
            };
            mapPin.AddGestureRecognizer(recognizer);
        }

        void OnClick(object annotationObject, UITapGestureRecognizer recognizer)
        {
            // https://bugzilla.xamarin.com/show_bug.cgi?id=26416
            NSObject annotation = Runtime.GetNSObject(((IMKAnnotation)annotationObject).Handle);
            if (annotation == null)
                return;

            // lookup pin
            Pin targetPin = null;
            foreach (Pin pin in ((Map)Element).Pins)
            {
                object target = pin.Id;
                if (target != annotation)
                    continue;

                targetPin = pin;
                break;
            }

            // pin not found. Must have been activated outside of forms
            if (targetPin == null)
                return;

            // if the tap happened on the annotation view itself, skip because this is what happens when the callout is showing
            // when the callout is already visible the tap comes in on a different view
            if (_lastTouchedView is MKAnnotationView)
                return;

            targetPin.SendTap();
        }
        async void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            try
            {

                //var customView = e.View as CustomMKAnnotationView;
                //customView.Annotation = annotate;
                //var coordinates = e.View.Annotation.Coordinate;
                //List<CustomPin> obj = (from s in customPins where s.Position.Latitude == coordinates.Latitude && s.Position.Longitude == coordinates.Longitude select s).ToList();
                //CustomPin objCustomPin = obj[0];
                //// var action = await App.Current.MainPage.DisplayActionSheet("You have selected " + objCustomPin.Label + ", click on confirm to proceed further", "Close", null, "Confirm");
                //vmAppointmentCenters ovmAppointmentCenters = objCustomPin.BindingContext as vmAppointmentCenters;
                //vmPatientAppointments gvmPatientAppointments = objCustomPin.gvmPatientAppointments;
                //gvmPatientAppointments.AppointmentRoomId = Convert.ToInt32(ovmAppointmentCenters.RoomId);
                //gvmPatientAppointments.Duration = Convert.ToInt32(ovmAppointmentCenters.Duration);
                //gvmPatientAppointments.AppointmentScanCentreId = Convert.ToInt32(ovmAppointmentCenters.CentreId);
                //gvmPatientAppointments.ScanPrice = Convert.ToDecimal(ovmAppointmentCenters.Price);
                //await nav.PushPopupAsync(new AlertPage(gvmPatientAppointments, objCustomPin, nav), true);
            }
            catch (Exception ex)
            {
                //string Errormessage = "Page Name:-CustomMapRender :-" + "UserId:" + Settings.UserId + ",Error Message:-" + Grab_scan.Helpers.ExceptionFormat.GetExceptionMessage(ex);
                //var result =await resourceService.Handleexception(Errormessage);
                //ex.Message.ToString();
            }
        }

        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            //var customView = e.View as CustomMKAnnotationView;
            //customPinView = new UIView();
            //customView.Annotation = annotate;
            //var coordinates = e.View.Annotation.Coordinate;
            //var obj = (from s in customPins where s.Position.Latitude == coordinates.Latitude && s.Position.Longitude == coordinates.Longitude select s);


            //var customView = e.View as CustomMKAnnotationView;
            //customPinView = new UIView();
            //customPinView.Frame = new CGRect(0, 0, 200, 84);
            //var image = new UIImageView(new CGRect(0, 0, 200, 84));
            //image.Image = UIImage.FromFile("icon.png");
            //customPinView.AddSubview(image);
            //customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
            //e.View.AddSubview(customPinView);


        }

        async void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            try
            {
                if (!e.View.Selected)
                {
                    if (customPinView != null)
                    {
                        customPinView.RemoveFromSuperview();
                        customPinView.Dispose();
                        customPinView = null;
                    }
                }
            }
            catch (Exception ex)
            {
                //string Errormessage = "Page Name:-CustomMapRender :-" + "UserId:" + Settings.UserId + ",Error Message:-" + Grab_scan.Helpers.ExceptionFormat.GetExceptionMessage(ex);
                //var result = await resourceService.Handleexception(Errormessage);
                // throw;
            }

        }

        CustomPin GetCustomPin(MKPointAnnotation annotation)
        {
            annotation.Title = string.Empty;
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }

    public class CustomMKAnnotationView : MKAnnotationView
    {
        public string Id { get; set; }

        public string Url { get; set; }
        public int ColorNumber { get; set; }
        public string hexStrin { get; set; }

        public CustomMKAnnotationView(IMKAnnotation annotation, string id)
            : base(annotation, id)
        {

        }
    }
}