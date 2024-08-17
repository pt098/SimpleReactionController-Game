using SimpleReactionMachine;
using System.Data;

namespace SimpleReactionMachine
{
    public class  SimpleReactionController : IController
    {
        private IState state;
        private IGui Gui { get; set; }
        private IRandom Random { get; set; }
        private int Ticks { get; set; }

        public void CoinInserted()
        {
            state.CoinInserted(this);
        }

        public void Connect(IGui gui, IRandom random)
        {
            Gui = gui;
            Random = random;
            Init();
        }

        public void GoStopPressed()
        {
            state.GoStopPressed(this);
        }

        public void Init()
        {
            state = new StandbyState();
            state.Enter(this);
        }

        public void Tick()
        {
            state.Tick(this);
        }

        private void SetState(IState _state)
        {
            state = _state;
            state.Enter(this);
        }

        private interface IState
        {
            void Enter(SimpleReactionController controller);
            void CoinInserted(SimpleReactionController controller);
            void GoStopPressed(SimpleReactionController controller);
            void Tick(SimpleReactionController controller);
        }

        private class StandbyState : IState
        {
            public void Enter(SimpleReactionController controller)
            {
                controller.Gui.SetDisplay("Insert Coin");
                controller.Ticks = 0;
            }

            public void CoinInserted(SimpleReactionController controller)
            {
                controller.SetState(new ReadyToStartState());
            }

            public void GoStopPressed(SimpleReactionController controller) { }

            public void Tick(SimpleReactionController controller) { }
        }

        private class ReadyToStartState : IState
        {
            public void Enter(SimpleReactionController controller)
            {
                controller.Gui.SetDisplay("Press Go!");
                controller.Ticks = 0;
            }

            public void CoinInserted(SimpleReactionController controller) { }
 
            public void GoStopPressed(SimpleReactionController controller)
            {
                controller.SetState(new WaitState());
            }

            public void Tick(SimpleReactionController controller) { }
        }

        private class WaitState : IState
        {
            private int waitTiming;

            public void Enter(SimpleReactionController controller)
            {
                controller.Gui.SetDisplay("Wait..."); 
                controller.Ticks = 0;
                waitTiming = controller.Random.GetRandom(100, 250);
            }

            public void CoinInserted(SimpleReactionController controller) { }

            public void GoStopPressed(SimpleReactionController controller)
            {
                controller.SetState(new StandbyState());
            }

            public void Tick(SimpleReactionController controller)
            {
                controller.Ticks++; 
                if (controller.Ticks == waitTiming)
                {
                    controller.SetState(new RunningState());
                }
            }
        }

        private class RunningState : IState
        {
            public void Enter(SimpleReactionController controller)
            {
                controller.Gui.SetDisplay("0.00");
                controller.Ticks = 0;
            }

            public void CoinInserted(SimpleReactionController controller) { }

            public void GoStopPressed(SimpleReactionController controller)
            {
                controller.SetState(new GameOverState());
            }

            public void Tick(SimpleReactionController controller)
            {
                controller.Ticks++;
                controller.Gui.SetDisplay((controller.Ticks / 100.0).ToString("0.00"));
                if (controller.Ticks == 400)
                {
                    controller.SetState(new GameOverState());
                } 
            }
        }

        private class GameOverState : IState
        {
            public void Enter(SimpleReactionController controller)
            {
                controller.Ticks = 0;
            }

            public void CoinInserted(SimpleReactionController controller) { }

            public void GoStopPressed(SimpleReactionController controller)
            {
                controller.SetState(new StandbyState());
            }

            public void Tick(SimpleReactionController controller)
            {
                controller.Ticks++;
                if (controller.Ticks == 400)
                {
                    controller.SetState(new StandbyState());
                }
            }
        }                                                             
    }
}
