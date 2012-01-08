namespace Frostbyte.Obstacles
{
    internal abstract class Obstacle : OnScreenObject
    {
        internal Obstacle(string name, Actor actor)
            : base(name, actor)
        {
        }
    }

    internal abstract class TargetableObstacle : Obstacle
    {
        internal TargetableObstacle(string name, Actor actor)
            : base(name, actor)
        {
        }
    }
}
