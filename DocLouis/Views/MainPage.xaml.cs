using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DocLouis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		private MobileServiceCollection<TrainingItem, TrainingItem> _items;
		private IMobileServiceTable<TrainingItem> _trainingItemsTable = App.MobileService.GetTable<TrainingItem>();
		Frame rootFrame = Window.Current.Content as Frame;

		public MainPage()
        {
            this.InitializeComponent();
        }

		private async Task RefreshTodoItems() {
			MobileServiceInvalidOperationException exception = null;
			try {
				// This code refreshes the entries in the list view by querying the TodoItems table.
				// The query excludes completed TodoItems
				_items = await _trainingItemsTable.ToCollectionAsync();
			} catch (MobileServiceInvalidOperationException e) {
				exception = e;
			}

			if (exception != null) {
				await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
			} else {
				fetchedTrainingsList.ItemsSource = _items;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			TrainingItem trainingItem = new TrainingItem { ProgramName = trainingNameInput.Text };
			rootFrame.Navigate(typeof(Program), new MainScreenMessage(true, trainingItem, _items, _trainingItemsTable));
		}

		private async void ButtonRefresh_Click(object sender, RoutedEventArgs e) {
			buttonRefreshList.IsEnabled = false;

			//await SyncAsync(); // offline sync
			await RefreshTodoItems();

			buttonRefreshList.IsEnabled = true;
		}

		private void CheckBoxLoad_Checked(object sender, RoutedEventArgs e) {
			CheckBox cb = (CheckBox) sender;
			TrainingItem item = cb.DataContext as TrainingItem;
			rootFrame.Navigate(typeof(Program), new MainScreenMessage(false, item, _items, _trainingItemsTable));
		}
	}
}
