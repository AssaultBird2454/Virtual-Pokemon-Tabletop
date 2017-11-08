﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultBird2454.VPTU.BattleManager.Battle_Instance
{
    public delegate void Participant_Changed(EntityManager.Entity Entity);

    public class Instance
    {
        #region Constructors
        public Instance()
        {
            // Set List
            Participants = new List<EntityManager.Entity>();
        }
        public Instance(List<EntityManager.Entity> _Partisipants)
        {
            // Set List
            Participants = _Partisipants;

            // Reset Entitys

            // Link Effect Triggers
        }
        #endregion

        #region Events
        public event Participant_Changed Participant_Added_Event;
        //public event Participant_Changed Participant_Changed_Event;
        public event Participant_Changed Participant_Removed_Event;
        #endregion

        private List<EntityManager.Entity> Participants { get; set; }

        public IEnumerable<EntityManager.Entity> GetParticipants
        {
            get
            {
                foreach (var child in Participants) yield return child;
            }
        }

        public void AddPartisipant(EntityManager.Entity Entity)
        {
            Partisipants.Add(Entity);
            Participant_Added_Event?.Invoke(Entity);
        }

        //public void ChangeParticipant(EntityManager.Entity Entity)
        //{
        //    Participant_Changed_Event?.Invoke(Entity);
        //}

        public void RemoveParticipant(EntityManager.Entity Entity)
        {
            Partisipants.Remove(Entity);
            Participant_Removed_Event?.Invoke(Entity);
        }

        public void Reset_Entity_BattleStats(EntityManager.Entity Entity)
        {
            if (Entity is EntityManager.Pokemon.PokemonCharacter)
            {
                ((EntityManager.Pokemon.PokemonCharacter)Entity).Attack_CombatStage = 0;
                ((EntityManager.Pokemon.PokemonCharacter)Entity).Defence_CombatStage = 0;
                ((EntityManager.Pokemon.PokemonCharacter)Entity).SpAttack_CombatStage = 0;
                ((EntityManager.Pokemon.PokemonCharacter)Entity).SpDefence_CombatStage = 0;
                ((EntityManager.Pokemon.PokemonCharacter)Entity).Speed_CombatStage = 0;
            }
            else if (Entity is EntityManager.Trainer.TrainerCharacter)
            {

            }
            else
            {

            }
        }

        public void Begin()
        {

        }

        public void End()
        {

        }
    }
}