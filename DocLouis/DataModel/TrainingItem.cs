using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DocLouis {
	class TrainingItem {
		public string Id { get; set; }

		[JsonProperty(PropertyName = "programname")]
		public string ProgramName { get; set; }

		[JsonProperty(PropertyName = "exercisetype")]
		public string ExerciseType { get; set; }

		[JsonProperty(PropertyName = "exercisename")]
		public string ExerciseName { get; set; }

		[JsonProperty(PropertyName = "duration")]
		public int Duration { get; set; }

		[JsonProperty(PropertyName = "repetitions")]
		public int Repetitions { get; set; }

		[JsonProperty(PropertyName = "series")]
		public int Series { get; set; }

		[JsonProperty(PropertyName = "breakduration")]
		public int BreakDuration { get; set; }
	}
}
