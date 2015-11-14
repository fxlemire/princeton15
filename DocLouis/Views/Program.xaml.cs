using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
	public sealed partial class Program : Page {
		public Program() {
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e) {
			base.OnNavigatedTo(e);
			trainingName.Text = (String) e.Parameter + "'s Training";
		}

		private void RefreshUI(object sender, SelectionChangedEventArgs e) {
			ComboBoxItem typeComboBox = (ComboBoxItem) type.SelectedItem;
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
			var voiceStream = await synthesizer.SynthesizeTextToStreamAsync(sentence);
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

		//		private void countdown(int duration) {
		//			SpeechSynthesizer synthesizer = new SpeechSynthesizer();
		//
		//			while (duration > 0) {
		//				synthesizer.SpeakAsync(Convert.ToString(duration));
		//				wait(1000);
		//				--duration;
		//			}
		//		}

		private string processSentence() {
			string sentence = name.Text;
			ComboBoxItem typeComboBox = (ComboBoxItem) type.SelectedItem;
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
	}
}