using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DocLouis {
	public sealed partial class TrainingProgram : Page {
		Frame rootFrame = Window.Current.Content as Frame;
		private MobileServiceCollection<TrainingItem, TrainingItem> _items;
		private IMobileServiceTable<TrainingItem> _trainingItemsTable = App.MobileService.GetTable<TrainingItem>();

		public TrainingProgram() {
			this.InitializeComponent();
		}

		private async Task InsertTrainingItem(TrainingItem trainingItem) {
			// This code inserts a new TodoItem into the database. When the operation completes
			// and Mobile Services has assigned an Id, the item is added to the CollectionView
			await _trainingItemsTable.InsertAsync(trainingItem);
			_items.Add(trainingItem);

			//await SyncAsync(); // offline sync
		}

		protected override void OnNavigatedTo(NavigationEventArgs e) {
			base.OnNavigatedTo(e);
			MainScreenMessage msg = (MainScreenMessage)e.Parameter;
			TrainingItem trainingItem = msg.getTrainingItem();
			_items = msg.getItems();
			_trainingItemsTable = msg.getTrainingItemsTable();

			if (!msg.getIsNewTraining()) {
				((ComboBoxItem)type.SelectedItem).Content = trainingItem.ExerciseType;
				name.Text = trainingItem.ExerciseName;
				duration_rep.Text = trainingItem.ExerciseType.Equals("duration") ? trainingItem.Duration.ToString() : trainingItem.Repetitions.ToString();
				series.Text = trainingItem.Series.ToString();
				break_duration.Text = trainingItem.BreakDuration.ToString();
			}

			trainingName.Text = trainingItem.ProgramName;
			if (!trainingName.Text.Contains("'s Training")) {
				trainingName.Text += "'s Training";
			}
		}

		private void RefreshUI(object sender, SelectionChangedEventArgs e) {
			ComboBoxItem typeComboBox = (ComboBoxItem)type.SelectedItem;
			if (duration_rep != null) {
				if (typeComboBox.Content.ToString().Equals("duration")) {
					duration_rep.PlaceholderText = "Duration";
				} else {
					duration_rep.PlaceholderText = "Repetitions";
				}
			}
		}

		private async void Start(object sender, RoutedEventArgs e) {
			string sentence = processSentence();
			SpeechSynthesizer synthesizer = new SpeechSynthesizer();
			SpeechSynthesisStream voiceStream = await synthesizer.SynthesizeTextToStreamAsync(sentence);
			mediaElement.SetSource(voiceStream, voiceStream.ContentType);
			mediaElement.Play();
			await Task.Delay(5000);
			voiceStream = await synthesizer.SynthesizeTextToStreamAsync("Let's Start in");
			mediaElement.SetSource(voiceStream, voiceStream.ContentType);
			mediaElement.Play();
			await countdown(3);
			voiceStream = await synthesizer.SynthesizeTextToStreamAsync("GO!");
			mediaElement.SetSource(voiceStream, voiceStream.ContentType);
			mediaElement.Play();
			await Task.Delay((convert(duration_rep.Text) - 6) * 1000);
			await countdown(5);
			voiceStream = await synthesizer.SynthesizeTextToStreamAsync("Done!");
			mediaElement.SetSource(voiceStream, voiceStream.ContentType);
			mediaElement.Play();
			//			wait(2000);
			//
			//			synthesizer.Speak("Let's start!");
			//
			//			countdown(3);
			//
			//			int loop = 1;
			//
			//			if (_type == Type.SERIES_REPETITIONS) {
			//				loop = _series;
			//			}
			//
			//			while (loop > 0) {
			//				synthesizer.SpeakAsync("GO!");
			//				wait(_duration * 1000);
			//				synthesizer.Speak("Break time!");
			//				wait(_breakDuration * 1000);
			//				--loop;
			//			}
		}

		private async Task countdown(int duration) {
			SpeechSynthesizer synthesizer = new SpeechSynthesizer();
			SpeechSynthesisStream voiceStream;

			while (duration > 0) {
				voiceStream = await synthesizer.SynthesizeTextToStreamAsync(duration.ToString());
				mediaElement.SetSource(voiceStream, voiceStream.ContentType);
				mediaElement.Play();
				await Task.Delay(1000);
				--duration;
			}
		}

		private string processSentence() {
			string sentence = name.Text;
			ComboBoxItem typeComboBox = (ComboBoxItem)type.SelectedItem;
			switch (typeComboBox.Content.ToString()) {
				case "duration":
					sentence += " for ";
					sentence += duration_rep.Text + " seconds.";
					break;
				case "repetitions":
					sentence += ". Do " + series.Text + " series of " + duration_rep.Text + " repetitions. ";
					sentence += "Take " + break_duration.Text + " seconds of break between series.";
					break;
				case "":
				default:
					break;
			}

			return sentence;
		}

		private async void SaveProgram(object sender, RoutedEventArgs e) {
			int durationRepetition = convert(duration_rep.Text);

			TrainingItem trainingItem = new TrainingItem {
				ProgramName = trainingName.Text,
				ExerciseType = ((ComboBoxItem)type.SelectedItem).Content.ToString(),
				ExerciseName = name.Text,
				Duration = durationRepetition,
				Repetitions = durationRepetition,
				Series = convert(series.Text),
				BreakDuration = convert(break_duration.Text)
			};

			await InsertTrainingItem(trainingItem);
		}

		private int convert(String element) {
			int number = 0;
			bool convert = int.TryParse(element, out number);
			if (!convert) {
				number = 0;
			}
			return number;
		}

		private void goBack(object sender, PointerRoutedEventArgs e) {
			rootFrame.Navigate(typeof(MainPage), null);
		}

		private void goBack(object sender, TappedRoutedEventArgs e) {
			rootFrame.Navigate(typeof(MainPage), null);
		}
	}
}