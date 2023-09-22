using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace WPFMidiApp.Behaviors
{
    public class TextBoxCaretIndexBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += TextBox_TextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= TextBox_TextChanged;

            base.OnDetaching();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
            AssociatedObject.ScrollToEnd();
        }
    }
}
