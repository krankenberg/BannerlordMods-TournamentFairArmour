using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TournamentFairArmour
{
    public class OverrideSpawnArmourMissionListener : IMissionListener
    {
        private readonly Equipment _tournamentEquipment;

        public OverrideSpawnArmourMissionListener(Equipment tournamentEquipment)
        {
            _tournamentEquipment = tournamentEquipment;
        }

        public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
        {
            if (agent.IsHuman)
            {
                foreach (var overriddenEquipmentIndex in SubModule.OverriddenEquipmentIndices)
                {
                    agent.SpawnEquipment[overriddenEquipmentIndex] = _tournamentEquipment[overriddenEquipmentIndex];
                }

                agent.AgentDrivenProperties.ArmorEncumbrance = agent.SpawnEquipment.GetTotalWeightOfArmor(agent.IsHuman);
                agent.AgentDrivenProperties.ArmorHead = agent.SpawnEquipment.GetHeadArmorSum();
                agent.AgentDrivenProperties.ArmorTorso = agent.SpawnEquipment.GetHumanBodyArmorSum();
                agent.AgentDrivenProperties.ArmorLegs = agent.SpawnEquipment.GetLegArmorSum();
                agent.AgentDrivenProperties.ArmorArms = agent.SpawnEquipment.GetArmArmorSum();
            }
        }

        public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
        {
        }

        public void OnEndMission()
        {
        }

        public void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
        {
        }

        public void OnConversationCharacterChanged()
        {
        }

        public void OnResetMission()
        {
        }
    }
}
