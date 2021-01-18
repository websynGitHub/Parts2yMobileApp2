using Xamarin.Forms;

namespace YPS.CustomRender
{
    /// <summary>
    /// ExtendedLabel
    /// </summary>
    public class ExtendedLabel : Label
    {
        public static readonly BindableProperty MaxLinesProperty =  BindableProperty.Create(nameof(MaxLines), typeof(int), typeof(ExtendedLabel), default(int));

        public int MaxLines
        {
            get => (int)GetValue(MaxLinesProperty);
            set => SetValue(MaxLinesProperty, value);
        }
    }
}
