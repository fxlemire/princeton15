using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocLouis {
	class MainScreenMessage {
		bool _isNewTraining;
		TrainingItem _trainingItem;
		private MobileServiceCollection<TrainingItem, TrainingItem> _items;
		private IMobileServiceTable<TrainingItem> _trainingItemsTable;

		public MainScreenMessage(bool isNew, TrainingItem trainingItem, MobileServiceCollection<TrainingItem, TrainingItem> items, IMobileServiceTable<TrainingItem> trainingItemsTable) {
			_isNewTraining = isNew;
			_trainingItem = trainingItem;
			_items = items;
			_trainingItemsTable = trainingItemsTable;
		}

		public bool getIsNewTraining() { return _isNewTraining; }

		public TrainingItem getTrainingItem() { return _trainingItem; }

		public MobileServiceCollection<TrainingItem, TrainingItem> getItems() { return _items; }

		public IMobileServiceTable<TrainingItem> getTrainingItemsTable() { return _trainingItemsTable; }
	}
}
