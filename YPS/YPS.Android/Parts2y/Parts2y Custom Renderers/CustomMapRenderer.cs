using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using System.Globalization;
using System.Collections.ObjectModel;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Droid.Parts2y.Parts2y_Custom_Renderers;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace YPS.Droid.Parts2y.Parts2y_Custom_Renderers
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {

        List<CustomPin> customPins;

        protected async override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            CustomPin cpin = (CustomPin)pin;
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            Android.Graphics.Drawables.Drawable icon;
            Android.Graphics.Bitmap anImage;
            if (cpin.ColorNumber == 0)
            {
              icon = Resources.GetDrawable(Resource.Drawable.marker);
                anImage = ((Android.Graphics.Drawables.BitmapDrawable)icon).Bitmap;
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(anImage));
            }
            else if (cpin.ColorNumber == 120)
            {
                icon = Resources.GetDrawable(Resource.Drawable.green_marker);
                anImage = ((Android.Graphics.Drawables.BitmapDrawable)icon).Bitmap;
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(anImage));
            }
            else if (cpin.ColorNumber == 50)
            {
                icon = Resources.GetDrawable(Resource.Drawable.yellow_marker);
                anImage = ((Android.Graphics.Drawables.BitmapDrawable)icon).Bitmap;
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(anImage));
            }
            else
            {

            }
            #region Added by Sindhu when we add pin in middle of time after map loading 
            //if (customPins != null)
            //{
            //    if (customPins.Contains(cpin))
            //    {

            //    }
            //    else
            //    {
            //        customPins.Add(cpin);
            //    }
            //}
            //else
            //{
                
            //}
            #endregion

            return marker;
        }

        string GetColourName(string htmlString)
        {
            System.Drawing.Color color = (System.Drawing.Color)new System.Drawing.ColorConverter().ConvertFromString(htmlString);
            System.Drawing.KnownColor knownColor = color.ToKnownColor();

            string name = knownColor.ToString();
            return name.Equals("0") ? "Unknown" : name;
        }
        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                //throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);


            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
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
}