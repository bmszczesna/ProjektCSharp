using System;
using System.Collections.ObjectModel;
using Model;
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
        }

        #endregion ctor

        #region public API

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));

            // Pobieranie liczby kul od użytkownika
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ile kul chcesz stworzyć?", "Wprowadź liczbę kul", "10");

           
            if (int.TryParse(input, out int numberOfBallsInput))
            {
               
                if (numberOfBallsInput < 1 || numberOfBallsInput > 15)
                {
  
                    System.Windows.MessageBox.Show("Liczba kul musi wynosić od 1 do 15.", "Błąd", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                else
                {
                 
                    ModelLayer.Start(numberOfBallsInput);
                }
            }
            else
            {
               
                System.Windows.MessageBox.Show("Wprowadź poprawną liczbę.", "Błąd", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            Observer.Dispose();
        }



        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
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

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}
