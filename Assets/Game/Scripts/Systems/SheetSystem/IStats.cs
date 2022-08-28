using Sirenix.OdinInspector;

using System;
using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public interface IStats
	{
        IStat Level { get; }

        IStat Strength { get; }
        IStat Dexterity { get; }
        IStat Intelligence { get; }
        IStat Health { get; }

        IStatBar HitPoints { get; }
        IStatBar FatiguePoints { get; }
        IStatBar Move { get; }
        IStatBar Speed { get; }
        IStatBar Will { get; }
        IStatBar Perception { get; }

        IStatBar ActionPoints { get; }

        IStatBar Lift { get; }

        void RecoveMove();
    }

	public class Stats : IStats
	{
        public IStat Level { get; }

        public IStat Strength { get; }
		public IStat Dexterity { get; }
		public IStat Intelligence { get; }
		public IStat Health { get; }

		public IStatBar HitPoints { get; }
		public IStatBar FatiguePoints { get; }
		public IStatBar Move { get; }
		public IStatBar Speed { get; }
		public IStatBar Will { get; }
		public IStatBar Perception { get; }

        public IStatBar ActionPoints { get; }

        public IStatBar Lift { get; }

		public Stats(StatsSettigns settigns)
        {
            Level = new LevelStat(settigns.level);

            Strength = new StrengthStat(settigns.strength);
            Dexterity = new DexterityStat(settigns.dexterity);
            Intelligence = new IntelligenceStat(settigns.intelligence);
            Health = new HealthStat(settigns.health);

            HitPoints = new HitPointsStat(settigns.HitPoints, 10);
            FatiguePoints = new FatiguePointsStat(settigns.FatiguePoints, 10);
            Move = new MoveStat(settigns.Move, settigns.Move);
            Speed = new SpeedStat(settigns.Speed, 10);
            Will = new WillStat(settigns.Will, 10);
            Perception = new PerceptionStat(settigns.Perception, 10);

            ActionPoints = new ActionPointsStat(1, 1);

            Lift = new LiftStat(0, settigns.MaxLift);
        }

        public Stats(Data data)
		{
            Strength = new StrengthStat(data.strength);
            Dexterity = new DexterityStat(data.dexterity);
            Intelligence = new IntelligenceStat(data.intelligence);
            Health = new HealthStat(data.health);

            HitPoints = new HitPointsStat(data.hitPoints, 10);
            FatiguePoints = new FatiguePointsStat(data.fatiguePoints, 10);
            Move = new MoveStat(data.move, 10);
            Speed = new SpeedStat(data.speed, 10);
            Will = new WillStat(data.will, 10);
            Perception = new PerceptionStat(data.perception, 10);

            Lift = new LiftStat(data.lift, 10);
        }

        public void RecoveMove()
        {
            Move.CurrentValue = Move.MaxValue;
        }

        public Data GetData()
		{
            return new Data()
            {
                strength = Strength.CurrentValue,
                dexterity = Dexterity.CurrentValue,
                intelligence = Intelligence.CurrentValue,
                health = Health.CurrentValue,
                
                hitPoints = HitPoints.CurrentValue,
                fatiguePoints = FatiguePoints.CurrentValue,
                move = Move.CurrentValue,
                speed = Speed.CurrentValue,
                perception = Perception.CurrentValue,
                
                lift = Lift.CurrentValue,
            };
		}

		public class Data
		{
            public float strength;
            public float dexterity;
            public float intelligence;
            public float health;

            public float hitPoints;
            public float fatiguePoints;
            public float move;
            public float speed;
            public float will;
            public float perception;
           
            public float lift;
        }
    }

	[System.Serializable]
    public class StatsSettigns
    {
        [Min(1)]
        public int level = 1;
        [Header("Primary")]
        [RangeStep(0, 20, 1)]
        public float strength = 10;
        [RangeStep(0, 20, 1)]
        public float dexterity = 10;
        [RangeStep(0, 20, 1)]
        public float intelligence = 10;
        [RangeStep(0, 20, 1)]
        public float health = 10;

        [Header("Secondary")]
        [RangeStep(-50, 50, 1)]
        public float addHitPoints = 0;
        [RangeStep(-50, 50, 1)]
        public float addFatiguePoints = 0;
        [RangeStep(-50, 50, 1)]
        public float addMove = 0;
        [RangeStep(-10, 10, 0.25f)]
        public float addSpeed = 0;
        [RangeStep(-50, 50, 1)]
        public float addWill = 0;
        [RangeStep(-50, 50, 1)]
        public float addPerception = 0;

        [PropertySpace]
        [ReadOnly][ShowInInspector] public float BasicHitPoints => strength;
        [GUIColor("HitPointsColor")]
        [ReadOnly][ShowInInspector] public float HitPoints => BasicHitPoints + addHitPoints;

        [PropertySpace]
        [ReadOnly][ShowInInspector] public float BasicFatiguePoints => health;
        [GUIColor("FatiguePointsColor")]
        [ReadOnly][ShowInInspector] public float FatiguePoints => BasicFatiguePoints + addFatiguePoints;

        [PropertySpace]
        [ReadOnly][ShowInInspector] public float BasicMove => (health + dexterity) / 4f;
        [GUIColor("MoveColor")]
        [ReadOnly][ShowInInspector] public float Move => (int)(BasicMove + addMove);

        [PropertySpace]
        [ReadOnly][ShowInInspector] public float BasicSpeed => (health + dexterity) / 4f;
        [GUIColor("SpeedColor")]
        [ReadOnly][ShowInInspector] public float Speed => BasicSpeed + addSpeed;

        [PropertySpace]
        [ReadOnly][ShowInInspector] public float BasicWill => intelligence;
        [GUIColor("WillColor")]
        [ReadOnly][ShowInInspector] public float Will => BasicWill + addWill;

        [PropertySpace]
        [ReadOnly][ShowInInspector] public float BasicPerception => intelligence;
        [GUIColor("PerceptionColor")]
        [ReadOnly][ShowInInspector] public float Perception => BasicPerception + addPerception;

        [PropertySpace]
        [SuffixLabel("kg", true)]
        [ReadOnly][ShowInInspector] public float BasicLift => (float)Math.Round((strength * strength) / 5f);
        [SuffixLabel("kg", true)]
        [ReadOnly][ShowInInspector] public float MaxLift => (float)Math.Round(strength * strength);

        [PropertySpace]


        [Button]
        private void Reset()
        {
            strength = 10;
            dexterity = 10;
            intelligence = 10;
            health = 10;

            addHitPoints = 0;
            addFatiguePoints = 0;
            addMove = 0;
            addSpeed = 0;
            addWill = 0;
            addPerception = 0;
        }

        private Color HitPointsColor => (((BasicHitPoints * 0.7f) <= HitPoints) && (HitPoints <= Math.Ceiling(BasicHitPoints * 1.3f))) ? Color.white : Color.red;
        private Color FatiguePointsColor => (((BasicFatiguePoints * 0.7f) <= FatiguePoints) && (FatiguePoints <= Math.Ceiling(BasicFatiguePoints * 1.3f))) ? Color.white : Color.red;
        private Color MoveColor => Move <= 0 ? Color.red : Color.white;
        private Color SpeedColor => Speed <= 0 ? Color.red : Color.white;
        private Color WillColor => Will <= 4 || Will >= 20 ? Color.red : Color.white;
        private Color PerceptionColor => Perception <= 4 || Perception >= 20 ? Color.red : Color.white;

        public override string ToString()
        {
            return $"Primary\nST = {strength}\nDX = {dexterity}\nIQ = {intelligence}\nHT = {health}" +
                $"Secondary\nHP = {HitPoints}\nMove = {Move}\nSpeed = {Speed}\nWill = {Will}\nPer = {Perception}\nFP = {FatiguePoints}";
        }
    }
}