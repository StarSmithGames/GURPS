using Sirenix.OdinInspector;

using System;
using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public sealed class Stats
	{
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
            Strength = new StrengthStat(settigns.strength);
            Dexterity = new DexterityStat(settigns.dexterity);
            Intelligence = new IntelligenceStat(settigns.intelligence);
            Health = new HealthStat(settigns.health);

            HitPoints = new HitPointsStat(settigns.HitPoints, 0, settigns.HitPoints);
            FatiguePoints = new FatiguePointsStat(settigns.FatiguePoints, 0, settigns.FatiguePoints);
            Move = new MoveStat(settigns.Move, 0, settigns.Move);
            Speed = new SpeedStat(settigns.Speed, 0, settigns.Speed);
            Will = new WillStat(settigns.Will, 0, settigns.Will);
            Perception = new PerceptionStat(settigns.Perception, 0, settigns.Perception);

            ActionPoints = new ActionPointsStat(1, 0, 1);

            Lift = new LiftStat(0, 0, settigns.LiftMax);
        }

        public Stats(Data data)
		{
            Strength = new StrengthStat(data.strength);
            Dexterity = new DexterityStat(data.dexterity);
            Intelligence = new IntelligenceStat(data.intelligence);
            Health = new HealthStat(data.health);

            HitPoints = new HitPointsStat(data.hitPoints, 0, 10);
            FatiguePoints = new FatiguePointsStat(data.fatiguePoints, 0, 10);
            Move = new MoveStat(data.move, 0, 10);
            Speed = new SpeedStat(data.speed, 0, 10);
            Will = new WillStat(data.will, 0, 10);
            Perception = new PerceptionStat(data.perception, 0, 10);

            Lift = new LiftStat(data.lift, 0, 10);
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

    [HideLabel]
	[System.Serializable]
    public class StatsSettigns
    {
        [HorizontalGroup("Stats", LabelWidth = 100)]
        [VerticalGroup("Stats/Primary")]
        [RangeStep(0, 20, 1)]
        public float strength = 10;
        [VerticalGroup("Stats/Primary")]
        [RangeStep(0, 20, 1)]
        public float dexterity = 10;
        [VerticalGroup("Stats/Primary")]
        [RangeStep(0, 20, 1)]
        public float intelligence = 10;
        [VerticalGroup("Stats/Primary")]
        [RangeStep(0, 20, 1)]
        public float health = 10;

        [VerticalGroup("Stats/Secondary")]
        [RangeStep(-50, 50, 1)]
        public float addHitPoints = 0;
        [VerticalGroup("Stats/Secondary")]
        [RangeStep(-50, 50, 1)]
        public float addFatiguePoints = 0;
        [VerticalGroup("Stats/Secondary")]
        [RangeStep(-50, 50, 1)]
        public float addMove = 0;
        [VerticalGroup("Stats/Secondary")]
        [RangeStep(-10, 10, 0.25f)]
        public float addSpeed = 0;
        [VerticalGroup("Stats/Secondary")]
        [RangeStep(-50, 50, 1)]
        public float addWill = 0;
        [VerticalGroup("Stats/Secondary")]
        [RangeStep(-50, 50, 1)]
        public float addPerception = 0;

        [ShowInInspector]
        [GUIColor("HitPointsColor")]
        [FoldoutGroup("Read Only Properties")]
        public float HitPoints => strength + addHitPoints;

        [ShowInInspector]
        [GUIColor("FatiguePointsColor")]
        [FoldoutGroup("Read Only Properties")]
        public float FatiguePoints => health + addFatiguePoints;

        [ShowInInspector]
        [GUIColor("MoveColor")]
        [FoldoutGroup("Read Only Properties")]
        public float Move => (int)((health + dexterity) / 4f + addMove);

        [ShowInInspector]
        [GUIColor("SpeedColor")]
        [FoldoutGroup("Read Only Properties")]
        public float Speed => (int)((health + dexterity) / 4f + addSpeed);

        [ShowInInspector]
        [GUIColor("WillColor")]
        [FoldoutGroup("Read Only Properties")]
        public float Will => intelligence + addWill;

        [ShowInInspector]
        [GUIColor("PerceptionColor")]
        [FoldoutGroup("Read Only Properties")]
        public float Perception => intelligence + addPerception;

        [ShowInInspector]
        [GUIColor("PerceptionColor")]
        [LabelText("Lift Min Max kg")]
        [FoldoutGroup("Read Only Properties")]
        public Vector2 Lift => new Vector2(LiftMin, LiftMax);

        public float LiftMin => (float)Math.Round((strength * strength) / 5f);
        public float LiftMax => (float)Math.Round(strength * strength);


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

        private Color HitPointsColor => (((strength * 0.7f) <= HitPoints) && (HitPoints <= Math.Ceiling(strength * 1.3f))) ? Color.white : Color.red;
        private Color FatiguePointsColor => (((health * 0.7f) <= FatiguePoints) && (FatiguePoints <= Math.Ceiling(health * 1.3f))) ? Color.white : Color.red;
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