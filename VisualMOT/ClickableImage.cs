using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VisualMOT
{
    public class ClickableImage : Xamarin.Forms.Image
    {
        public static BindableProperty ClickedProperty =
            BindableProperty.Create("Clicked", typeof(Command), typeof(ClickableImage));

        public Command Clicked
        {
            get { return (Command)GetValue(ClickedProperty); }
            set { SetValue(ClickedProperty, value); }
        }

        public static BindableProperty ClickParameterProperty =
            BindableProperty.Create("ClickParameter", typeof(object), typeof(ClickableImage));

        public object ClickParameter
        {
            get { return (object)GetValue(ClickParameterProperty); }
            set { SetValue(ClickParameterProperty, value); }
        }

        public ClickableImage()
        {
            GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(DisTap), CommandParameter = this });
        }

        private void DisTap(object sender)
        {
            if (Clicked != null)
            {
                Clicked.Execute(sender);
            }
        }

    }
}
