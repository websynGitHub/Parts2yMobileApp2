using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.Droid.Custom_Renderers;
using YPS.CustomRenders;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly: ResolutionGroupName("YPS")]
[assembly: ExportEffect(typeof(AndroidLongPressedEffect), "LongPressedEffect")]
namespace YPS.Droid.Custom_Renderers
{
    /// <summary>
    /// Android long pressed effect.
    /// </summary>
    public class AndroidLongPressedEffect : PlatformEffect
    {
        private bool _attached;

        /// <summary>
        /// Initializer to avoid linking out
        /// </summary>
        public static void Initialize() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Yukon.Application.AndroidComponents.Effects.AndroidLongPressedEffect"/> class.
        /// Empty constructor required for the odd Xamarin.Forms reflection constructor search
        /// </summary>
        public AndroidLongPressedEffect()
        {
        }

        /// <summary>
        /// Apply the handler
        /// </summary>
        protected override void OnAttached()
        {
            try
            {
                //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time.
                if (!_attached)
                {
                    if (Control != null)
                    {
                        Control.LongClickable = true;
                        Control.LongClick += Control_LongClick;
                        Control.Clickable = true;
                        Control.Click += Control_Click;
                    }
                    else
                    {
                        Container.LongClickable = true;
                        Container.LongClick += Control_LongClick;
                        Container.Clickable = true;
                        Container.Click += Control_Click;
                    }
                    _attached = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAttached method -> in AndroidLongPressedEffect.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {

            try
            {
                var command = LongPressedEffect.GetSlCommand(Element);
                command?.Execute(LongPressedEffect.GetSlCommandParameter(Element));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Control_Click method -> in AndroidLongPressedEffect.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Invoke the command if there is one
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Control_LongClick(object sender, global::Android.Views.View.LongClickEventArgs e)
        {
            try
            {
                var command = LongPressedEffect.GetCommand(Element);
                command?.Execute(LongPressedEffect.GetCommandParameter(Element));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Control_LongClick method -> in AndroidLongPressedEffect.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Clean the event handler on detach
        /// </summary>
        protected override void OnDetached()
        {
            try
            {
                if (_attached)
                {
                    if (Control != null)
                    {
                        Control.LongClickable = true;
                        Control.LongClick -= Control_LongClick;
                        Control.Clickable = true;
                        Control.Click -= Control_Click;
                    }
                    else
                    {
                        Container.LongClickable = true;
                        Container.LongClick -= Control_LongClick;
                        Control.Clickable = true;
                        Control.Click -= Control_Click;
                    }
                    _attached = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDetached method -> in AndroidLongPressedEffect.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }
    }
}