using Data;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Test Bootstrap for initializing a save in the JSON Save System
    /// </summary>
    public class TestBootstrap : MonoBehaviour
    {
        private readonly PlayerSaveData _testSave = new()
        {
            username = "CaptainPookie",
            campaignStarted = false,
            missionsCompleted = 0,
        };

        private void Awake()
        {
            SaveDataManager.Save(_testSave);
        }
    }
}