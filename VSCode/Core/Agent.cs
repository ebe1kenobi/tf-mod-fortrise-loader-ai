using System;
using System.Collections.Generic;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
    public abstract class Agent
    {
        protected Level level;
        protected int index;
        protected Input input;
        protected Random random;
        protected string type;
        protected List<InputState> shoot = new List<InputState>();

        public Agent(int index, string type, PlayerInput input)
        {
            this.index = index;
            this.input = (Input)input;
            this.type = type;
            random = new Random(index * 666);
        }

        public virtual void SetLevel(Level level)
        {
            this.level = level;
            shoot.Clear();
        }

        public virtual string getType()
        {
            return type;
        }

        public virtual string getName()
        {
            return type + " " + index;
        }

        public virtual Input getInput()
        {
            return input;
        }

        public virtual void Play() {
          if (level.Paused) return;
          if (level.Frozen) return;
          if (level.Ending) return;

          Move();
        }

        protected abstract void Move();
  }
}
