using System.Collections.Generic;
using System.Linq;

namespace processManager.Tests
{
    internal class ProcessManager
    {
        private IPublisher Publisher { get;  }
        private List<int> Prices = new List<int>();
        private List<int> SevenSecondPrices = new List<int>();
        private List<int> ElevenSecondPrices = new List<int>();
        private int _currentThreshold;
        private bool _stopLossMessageSent;

        const int THRESHOLD = 10;

        public ProcessManager(IPublisher publisher)
        {
            Publisher = publisher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void ProcessMessage(PositionAcquiredMessage message)
        {
            StartPrice = message.Value;
            UpdateCurrentThreshold(StartPrice);
            Publisher.Publish(new ThresholdUpdatedMessage() { ThresholdPrice = _currentThreshold });
        }

        public void ProcessMessage(PriceUpdateMessage message)
        {
            Prices.Add(message.Value);
            SevenSecondPrices.Add(message.Value);
            ElevenSecondPrices.Add(message.Value);

            Publisher.Publish(new PublishMessage() { Interval = 7, Price = message.Value });
            Publisher.Publish(new PublishMessage() { Interval = 11, Price = message.Value });
        }

        public void ProcessMessage(RemoveFromSevenSecondMessage message)
        {
            int minimumPrice = (int)SevenSecondPrices.Min();
            if (minimumPrice > _currentThreshold)
            {
                UpdateCurrentThreshold(minimumPrice);
                Publisher.Publish(new ThresholdUpdatedMessage() { ThresholdPrice = _currentThreshold });
            }
            
            SevenSecondPrices.Remove(message.Value);
        }

        public void ProcessMessage(RemoveFromElevenSecondMessage message)
        {
            int maximumPrice = (int)ElevenSecondPrices.Max();

            if (maximumPrice < _currentThreshold && !_stopLossMessageSent)
            {
                Publisher.Publish(new StopLossHitMessage());
                _stopLossMessageSent = true;
            }

            ElevenSecondPrices.Remove(message.Value);
        }

        private void UpdateCurrentThreshold(int minimumPrice)
        {
            _currentThreshold = minimumPrice - THRESHOLD;
        }

        public int StartPrice { get; set; }
    }
}