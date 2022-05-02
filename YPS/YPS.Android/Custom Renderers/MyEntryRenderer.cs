using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.CommonClasses;
using YPS.CustomRender;
using YPS.Droid.Custom_Renderers;
using YPS.Helpers;
using YPS.Service;

[assembly: ExportRenderer(typeof(MyEntry), typeof(MyEntryRenderer))]
namespace YPS.Droid.Custom_Renderers
{
    public class MyEntryRenderer : EntryRenderer
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public MyEntryRenderer(Context context) : base(context) { }
        MyEntry myEntry;
        /// <summary>
        /// Gets called when any changes occur to the MyEditor.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (Control != null)
                {
                    var entry = Element as Entry;
                    myEntry = e.NewElement as MyEntry;
                    GradientDrawable gd = new GradientDrawable();
                    gd.SetColor(global::Android.Graphics.Color.Transparent);
                    this.Control.SetBackgroundDrawable(gd);

                    #region This is for restricting copy,paste etc. functions
                    //if (entry.ClassId != null && entry.ClassId.Trim().ToLower() == "login")// for restricting the Copy,Paste etc.
                    //{
                    //    Control.CustomSelectionActionModeCallback = new Callback();
                    //    Control.LongClickable = false;
                    //}
                    #endregion

                    if ((entry.ClassId == null) || (entry.ClassId != null && entry.ClassId.Trim().ToLower() != "login"))
                    {
                        Control.FocusChange += Control_FocusChange;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnElementChanged -> in Myentryrenderer.cs " + Settings.userLoginID);
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when MyEditor is in MyEntry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_FocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                myEntry?.NotifyKeyBoardAppear(sender, null);
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustResize);
            }
            else
            {
                myEntry?.NotifyKeyBoardDisappear(sender, null);
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustNothing);
            }
        }
    }

    /// <summary>
    /// Callback to check the action performed
    /// </summary>
    public class Callback : Java.Lang.Object, ActionMode.ICallback
    {
        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            return false;
        }
        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            return false;
        }
        public void OnDestroyActionMode(ActionMode mode) { }
        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            return false;
        }
    }
}