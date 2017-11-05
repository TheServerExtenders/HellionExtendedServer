using System.ComponentModel;
using ZeroGravity;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using HellionExtendedServer;

namespace HellionExtendedServer.GUI.Objects
{
    public class MyPlayer : BaseObject
    {
        public MyPlayer(Player player) : base(player)
        {
            CurrentPlayer = player;
            Parent = player.Parent;
        }

        [Browsable(false)]
        public Player CurrentPlayer { get; private set; }

        [Browsable(false)]
        public SpaceObject Parent { get; private set; }

        [ReadOnly(true)]
        [Category("Player Stats")]
        public override string Name => CurrentPlayer.Name;

        [ReadOnly(true)]
        [Category("Player Stats")]
        public string SteamID => CurrentPlayer.SteamId;

        [ReadOnly(true)]
        [Category("Player Stats")]
        public Gender Gender => CurrentPlayer.Gender;

        [ReadOnly(true)]
        [Category("Player Stats")]
        public bool IsAlive => CurrentPlayer.IsAlive;

        [Category("Player Stats")]
        public bool GodMode
        {
            get => CurrentPlayer.Stats.GodMode;
            set => CurrentPlayer.Stats.GodMode = value;
        }

        [Category("Player Stats")]
        public float Health
        {
            get
            {
                return CurrentPlayer.Health;
            }
            set
            {
                var currentHealth = CurrentPlayer.Health;

                if (value < currentHealth)
                {
                    CurrentPlayer.Stats.TakeDammage(currentHealth - value);
                }
                else if (value > currentHealth)
                {
                    CurrentPlayer.Stats.Heal((value + currentHealth));
                }
            }
        }

        [Category("Space Object")]
        public override string Position
        {
            get { return base.Position; }
            set
            {
                CurrentPlayer.ModifyLocalPositionAndRotation(StringToVector(value), CurrentPlayer.LocalRotation);           
            }
        }

        [Category("Space Object")]
        public string Rotation
        {
            get => CurrentPlayer.LocalRotation.ToString();
            set
            {
                CurrentPlayer.ModifyLocalPositionAndRotation(SpaceObject.Position, StringToQuat(value));
            }

        }

        public QuaternionD StringToQuat(string _value)
        {
            var args = _value.Split(',');

            if (args.Length < 0)
                return CurrentPlayer.LocalRotation;

            if (!double.TryParse(args[0], out double x))
                return CurrentPlayer.LocalRotation;

            if (!double.TryParse(args[1], out double y))
                return CurrentPlayer.LocalRotation;

            if (!double.TryParse(args[2], out double z))
                return CurrentPlayer.LocalRotation;

            if (!double.TryParse(args[3], out double w))
                return CurrentPlayer.LocalRotation;


            return new QuaternionD(x, y, z, w);
        }
    }
}