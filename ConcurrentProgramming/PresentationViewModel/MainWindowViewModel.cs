using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FW_LJ_CP.Presentation.Model;
using FW_LJ_CP.Presentation.ViewModel.MVVMLight;
using ModelIBall = FW_LJ_CP.Presentation.Model.IBall;

namespace FW_LJ_CP.Presentation.ViewModel
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

            _startCommand = new RelayCommand(
                execute: () => Start(NumberOfBalls),
                canExecute: () => NumberOfBalls > 0 && NumberOfBalls < 17 && !Disposed);
        }

        #endregion ctor

        #region public API

        public int NumberOfBalls
        {
            get => _numberOfBalls;
            set
            {
                _numberOfBalls = value;
                RaisePropertyChanged();
                _startCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand StartCommand => _startCommand;

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Balls.Clear();
            ModelLayer.Start(numberOfBalls);
            Observer?.Dispose();
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
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
                    _startCommand.RaiseCanExecuteChanged();
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

        private int _numberOfBalls;
        private readonly RelayCommand _startCommand;
        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}