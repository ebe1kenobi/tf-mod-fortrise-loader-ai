using MonoMod.Utils;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
    public class Input : KeyboardInput
    {
        public InputState prevInputState;
        public InputState inputState;

        public Input(int index) : base()
        {
            Config = KeyboardConfigs.Configs[index];
            //InitIcons();
            var dynData = DynamicData.For(this);
            dynData.Invoke("InitIcons");
            dynData.Dispose();
        }

        public Input(KeyboardConfig config, int id) : base(config, id) { }
        public InputState GetCopy(InputState inputState)
        {
            return new InputState
            {
                AimAxis = inputState.AimAxis,
                ArrowsPressed = inputState.ArrowsPressed,
                DodgeCheck = inputState.DodgeCheck,
                DodgePressed = inputState.DodgePressed,
                JumpCheck = inputState.JumpCheck,
                JumpPressed = inputState.JumpPressed,
                MoveX = inputState.MoveX,
                MoveY = inputState.MoveY,
                ShootCheck = inputState.ShootCheck,
                ShootPressed = inputState.ShootPressed
            };
        }

        public override InputState GetState()
        {
            prevInputState = GetCopy(inputState);
            return GetCopy(prevInputState);
        }
    }
}
