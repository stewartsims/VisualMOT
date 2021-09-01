using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace VisualMOT
{
    class ToastHelper
    {
        public static ToastOptions GetInfoToastOptions(string message)
        {
            return GetToastOptions(message, Color.Black);
        }

        public static ToastOptions GetWarningToastOptions(string message)
        {
            return GetToastOptions(message, Color.Orange);
        }

        public static ToastOptions GetSuccessToastOptions(string message)
        {
            return GetToastOptions(message, Color.LightGreen);
        }

        public static ToastOptions GetToastOptions(string message, Color backgroundColor)
        {
            var messageOptions = new MessageOptions
            {
                Message = message,
                Foreground = Color.White,
                Font = Font.SystemFontOfSize(16),
                Padding = new Thickness(20)
            };

            var options = new ToastOptions
            {
                MessageOptions = messageOptions,
                CornerRadius = new Thickness(40, 40, 40, 40),
                BackgroundColor = backgroundColor
            };
            return options;
        }
    }
}
