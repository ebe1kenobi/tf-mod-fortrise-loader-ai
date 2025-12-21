using Microsoft.Xna.Framework;
using TFModFortRiseAI.Abstractions;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
  public sealed class AgentAdapter : Agent
  {
    private readonly IAgentLogic logic;

    //const int X = 0;
    //const int Y = 1;
    //const int JUMP = 2;
    //const int DODGE = 3;
    //const int SHOOT = 4;
    //public const int ALT_SHOOT = 5;
    //public const int ARROW_PRESSED = 6;

    public AgentAdapter(
        IAgentLogic logic,
        int index,
        Input input
    ) : base(index, logic.Type, input)
    {
      //Logger.Info("AgentAdapter.ctor");
      this.logic = logic;
      logic.Initialize(index, this.input);
    }

    public override void SetLevel(Level level)
    {
      //Logger.Info("AgentAdapter.SetLevel");
      base.SetLevel(level);
      logic.SetLevel(level);
    }

    protected override void Move()
    {
      //Logger.Info("AgentAdapter.Move call logic.Update");
      System.Collections.Generic.List<int> actions = logic.Update();
      input.inputState = new InputState
      {
        AimAxis = new Vector2(actions[IAgentLogic.X], actions[IAgentLogic.Y]),
        //ArrowsPressed = false,
        DodgeCheck = actions[IAgentLogic.DODGE] == 1 ? true : false,
        DodgePressed = actions[IAgentLogic.DODGE] == 1 ? false : true,
        JumpCheck = actions[IAgentLogic.JUMP] == 1 ? true : false,
        JumpPressed = actions[IAgentLogic.JUMP] == 1 ? false : true,
        MoveX = actions[IAgentLogic.X],
        MoveY = actions[IAgentLogic.Y],
        ShootCheck = actions[IAgentLogic.SHOOT] == 1 ? true : false,
        ShootPressed = actions[IAgentLogic.SHOOT] == 1 ? false : true,

        AltShootCheck = actions[IAgentLogic.ALT_SHOOT] == 1 ? true : false,
        AltShootPressed = actions[IAgentLogic.ALT_SHOOT] == 1 ? false : true,
        ArrowsPressed = actions[IAgentLogic.ARROW_PRESSED] == 1 ? true : false,
      };
    }
  }
}
