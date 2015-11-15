using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DocLouis {
	public sealed partial class LoginPage : Page {
		private MobileServiceUser _user;
		Frame rootFrame = Window.Current.Content as Frame;

		public LoginPage() {
			this.InitializeComponent();
		}

		private async System.Threading.Tasks.Task AuthenticateAsync(String service) {
			string message;
			try {
				if (service.Equals("facebook")) {
					_user = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Facebook);
				} else if (service.Equals("google")) {
					_user = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Google);
				} else {
					_user = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
				}
				message = string.Format("You are now signed in - {0}", _user.UserId);
			} catch (InvalidOperationException) {
				message = "You must log in. Login Required";
			}

			var dialog = new MessageDialog(message);
			dialog.Commands.Add(new UICommand("OK"));
			await dialog.ShowAsync();
		}

		private async void ButtonLogin_Click(object sender, RoutedEventArgs e) {
			// Login the user and then load data from the mobile service.
			Button signupButton = (Button)sender;
			String service;
			if (signupButton.Name.Equals("ButtonLoginFacebook")) {
				service = "facebook";
			} else if (signupButton.Name.Equals("ButtonLoginGoogle")) {
				service = "google";
			} else {
				service = "twitter";
			}

			await AuthenticateAsync(service);

			if (_user != null) {
				// Hide the login button and load items from the mobile service.
				this.ButtonLoginFacebook.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				this.ButtonLoginGoogle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				this.ButtonLoginTwitter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				rootFrame.Navigate(typeof(MainPage), null);
			}
		}

	}
}
