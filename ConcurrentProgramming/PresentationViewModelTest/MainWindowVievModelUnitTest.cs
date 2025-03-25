using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using FW_LJ_CP.Presentation.Model;
using ModelIBall = FW_LJ_CP.Presentation.Model.IBall;

namespace FW_LJ_CP.Presentation.ViewModel.Test
{
    public class MainWindowViewModelUnitTest
    {
        [Fact]
        public void ConstructorTest()
        {
            ModelNullFixture nullModelFixture = new();
            Assert.Equal<int>(0, nullModelFixture.Disposed);
            Assert.Equal<int>(0, nullModelFixture.Started);
            Assert.Equal<int>(0, nullModelFixture.Subscribed);
            using (MainWindowViewModel viewModel = new(nullModelFixture))
            {
                Random random = new Random();
                int numberOfBalls = random.Next(1, 10);
                viewModel.Start(numberOfBalls);
                Assert.NotNull(viewModel.Balls);
                Assert.Equal<int>(0, nullModelFixture.Disposed);
                Assert.Equal<int>(numberOfBalls, nullModelFixture.Started);
                Assert.Equal<int>(1, nullModelFixture.Subscribed);
            }
            Assert.Equal<int>(1, nullModelFixture.Disposed);
        }

        [Fact]
        public void BehaviorTestMethod()
        {
            ModelSimulatorFixture modelSimulator = new();
            MainWindowViewModel viewModel = new(modelSimulator);
            Assert.NotNull(viewModel.Balls);
            Assert.Empty(viewModel.Balls);
            Random random = new Random();
            int numberOfBalls = random.Next(1, 10);
            viewModel.Start(numberOfBalls);
            Assert.Equal<int>(numberOfBalls, viewModel.Balls.Count);
            viewModel.Dispose();
            Assert.True(modelSimulator.Disposed);
            Assert.Empty(viewModel.Balls);
        }

        #region testing infrastructure

        private class ModelNullFixture : ModelAbstractApi
        {
            #region Test

            internal int Disposed = 0;
            internal int Started = 0;
            internal int Subscribed = 0;

            #endregion Test

            #region ModelAbstractApi

            public override void Dispose()
            {
                Disposed++;
            }

            public override void Start(int numberOfBalls)
            {
                Started = numberOfBalls;
            }

            public override IDisposable Subscribe(IObserver<ModelIBall> observer)
            {
                Subscribed++;
                return new NullDisposable();
            }

            #endregion ModelAbstractApi

            #region private

            private class NullDisposable : IDisposable
            {
                public void Dispose()
                { }
            }

            #endregion private
        }

        private class ModelSimulatorFixture : ModelAbstractApi
        {
            #region Testing indicators

            internal bool Disposed = false;

            #endregion Testing indicators

            #region ctor

            public ModelSimulatorFixture()
            {
                eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
            }

            #endregion ctor

            #region ModelAbstractApi fixture

            public override IDisposable? Subscribe(IObserver<ModelIBall> observer)
            {
                return eventObservable?.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
            }

            public override void Start(int numberOfBalls)
            {
                for (int i = 0; i < numberOfBalls; i++)
                {
                    ModelBall newBall = new ModelBall(0, 0) { };
                    BallChanged?.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
                }
            }

            public override void Dispose()
            {
                Disposed = true;
            }

            #endregion ModelAbstractApi

            #region API

            public event EventHandler<BallChaneEventArgs> BallChanged;

            #endregion API

            #region private

            private IObservable<EventPattern<BallChaneEventArgs>>? eventObservable = null;

            private class ModelBall : ModelIBall
            {
                public ModelBall(double top, double left)
                { }

                #region IBall

                public double Diameter => throw new NotImplementedException();

                public double Top => throw new NotImplementedException();

                public double Left => throw new NotImplementedException();

                #region INotifyPropertyChanged

                public event PropertyChangedEventHandler? PropertyChanged;

                #endregion INotifyPropertyChanged

                #endregion IBall
            }

            #endregion private
        }

        #endregion testing infrastructure
    }
}