using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 具有方向的點
    /// </summary>
    [Serializable]
    internal class TowardPair : ITowardPair
    {
        private Pair mPosition = new Pair();

        private Angle mToward = new Angle();

        public TowardPair()
        {
        }

        public TowardPair(int x, int y, double toward)
        {
            Position.X = x;
            Position.Y = y;
            Toward.Theta = toward;
        }

        public TowardPair(double x, double y, double toward) : this((int)x, (int)y, toward)
        {
        }

        public TowardPair(ITowardPair towardPair) : this(towardPair.Position.X, towardPair.Position.Y, towardPair.Toward.Theta)
        {
        }

        public TowardPair(IPair position, double toward) : this(position.X, position.Y, toward)
        {
        }

        public TowardPair(IPair position, IAngle toward) : this(position.X, position.Y, toward.Theta)
        {
        }

        public TowardPair(int x, int y, IAngle toward) : this(x, y, toward.Theta)
        {
        }

        public TowardPair(double x, double y, IAngle toward) : this((int)x, (int)y, toward.Theta)
        {
        }

        /// <summary>
        /// 座標點
        /// </summary>
        public IPair Position { get { return mPosition; } set { mPosition.X = value.X; mPosition.Y = value.Y; } }

        /// <summary>
        /// 方向
        /// </summary>
        public IAngle Toward { get { return mToward; } set { mToward.Theta = value.Theta; } }
    }
}