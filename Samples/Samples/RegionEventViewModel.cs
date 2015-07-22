using System;


namespace Samples {

    public class RegionEventViewModel {

        public string Description { get; }
        public string DateText { get; }


        public RegionEventViewModel(RegionEvent @event) {
            this.Description = $"{@event.Identifer} ({@event.Status})";
            this.DateText = @event.DateCreated.ToString("g");
        }
    }
}
