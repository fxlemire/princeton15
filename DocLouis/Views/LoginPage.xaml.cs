using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
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

		private async System.Threading.Tasks.Task AuthenticateAsync(String provider) {
			string message;

			// Use the PasswordVault to securely store and access credentials.
			PasswordVault vault = new PasswordVault();
			PasswordCredential credential = null;

			while (credential == null) {
				try {
					// Try to get an existing credential from the vault.
					credential = vault.FindAllByResource(provider).FirstOrDefault();
				} catch (Exception) {
					// When there is no matching resource an error occurs, which we ignore.
				}

				if (credential != null) {
					// Create a user from the stored credentials.
					_user = new MobileServiceUser(credential.UserName);
					credential.RetrievePassword();
					_user.MobileServiceAuthenticationToken = credential.Password;

					// Set the user from the stored credentials.
					App.MobileService.CurrentUser = _user;

					try {
						// Try to return an item now to determine if the cached credential has expired.
						await App.MobileService.GetTable<TrainingItem>().Take(1).ToListAsync();
					} catch (MobileServiceInvalidOperationException ex) {
						if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
							// Remove the credential with the expired token.
							vault.Remove(credential);
							credential = null;
							continue;
						}
					}
				} else {
					try {
						// Login with the identity provider.
						_user = await App.MobileService.LoginAsync(provider);

						// Create and store the user credentials.
						credential = new PasswordCredential(provider, _user.UserId, _user.MobileServiceAuthenticationToken);
						vault.Add(credential);
					} catch (MobileServiceInvalidOperationException ex) {
						message = "You must log in. Login Required";
					}
				}

				message = string.Format("You are now logged in - {0}", _user.UserId);
				var dialog = new MessageDialog(message);
				dialog.Commands.Add(new UICommand("OK"));
				await dialog.ShowAsync();
			}
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
			try {
				await AuthenticateAsync(service);
			} catch (System.InvalidOperationException) {
				//user cancelled login
			}

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
