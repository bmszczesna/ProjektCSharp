using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Model;
using Presentation.ViewModel;
using ModelIBall = Model.IBall;

namespace ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            StartCommand = new RelayCommand(_ =>
            {
                if (!int.TryParse(BallCount, out int numberOfBalls))
                {
                    MessageBox.Show("Wprowadź poprawną liczbę całkowitą.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (numberOfBalls <= 0)
                {
                    MessageBox.Show("Liczba kulek musi być większa niż 0.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (numberOfBalls > 100)
                {
                    MessageBox.Show("Maksymalna liczba kulek to 100.", "Ograniczenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Start(numberOfBalls);
            });
        }

        #endregion ctor

        #region public API

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));

            Balls.Clear(); // Reset listy przy nowym starcie
            ModelLayer.Start(numberOfBalls);
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        public string BallCount
        {
            get => _ballCount;
            set
            {
                if (_ballCount != value)
                {
                    _ballCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand StartCommand { get; }

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null!;
        private ModelAbstractApi ModelLayer = null!;
        private bool Disposed = false;
        private string _ballCount = "10";

        #endregion private
    }
}
