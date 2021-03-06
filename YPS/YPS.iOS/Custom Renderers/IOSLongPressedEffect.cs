using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRenders;
using YPS.iOS.Custom_Renderers;

[assembly: ResolutionGroupName("YPS")]
[assembly: ExportEffect(typeof(iOSLongPressedEffect), "LongPressedEffect")]
namespace YPS.iOS.Custom_Renderers
{
    public class iOSLongPressedEffect : PlatformEffect
    {
        private bool _attached;
        private readonly UILongPressGestureRecognizer _longPressRecognizer;
        private readonly UITapGestureRecognizer _tapGestureRecognizer;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Yukon.Application.iOSComponents.Effects.iOSLongPressedEffect"/> class.
        /// </summary>
        public iOSLongPressedEffect()
        {
            _longPressRecognizer = new UILongPressGestureRecognizer(HandleLongClick);
            _tapGestureRecognizer = new UITapGestureRecognizer(SingleClick);
        }

        private void SingleClick()
        {
            var command = LongPressedEffect.GetSlCommand(Element);
            command?.Execute(LongPressedEffect.GetSlCommandParameter(Element));
        }

        /// <summary>
        /// Apply the handler
        /// </summary>
        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time
            if (!_attached)
            {
                Container.AddGestureRecognizer(_longPressRecognizer);
                Container.AddGestureRecognizer(_tapGestureRecognizer);
                _attached = true;
            }
        }

        /// <summary>
        /// Invoke the command if there is one
        /// </summary>
        private void HandleLongClick()
        {
            try
            {
                if (_longPressRecognizer.State == UIGestureRecognizerState.Began)
                {
                    var command = LongPressedEffect.GetCommand(Element);
                    command?.Execute(LongPressedEffect.GetCommandParameter(Element));
                }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Clean the event handler on detach
        /// </summary>
        protected override void OnDetached()
        {
            if (_attached)
            {
                Container.RemoveGestureRecognizer(_longPressRecognizer);
                Container.RemoveGestureRecognizer(_tapGestureRecognizer);
                _attached = false;
            }
        }
    }
}