using Data;
using System.IO;
using UI.Core;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Debug Tool for modifying the Save Data in game by pressing F3
    /// </summary>
    
	public class DebugTool : MonoBehaviour
    {
        private PlayerSaveData _edit;
        private bool _visible;
        private string _status;
        private Vector2 _scroll;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F3)) return;
            if (_visible) Hide();
            else Show();
        }

        public static void Attach(MenuRoot root)
        {
            root.gameObject.AddComponent<DebugTool>();
        }

        private void Show()
        {
            _edit = new PlayerSaveData();
            var save = SaveDataManager.CurrentSave;
            if (save != null)
                CopyFromSave(save, _edit);
            _status = "";
            _visible = true;
        }

        private void Hide()
        {
            _visible = false;
        }

        private void OnGUI()
        {
            if (!_visible) return;

            var panelRect = new Rect(Screen.width - 420, 0, 420, Screen.height);
            GUILayout.BeginArea(panelRect, GUI.skin.box);
            _scroll = GUILayout.BeginScrollView(_scroll);

            GUILayout.Label("DEBUG TOOL");
            GUILayout.Space(4);

            var saveExists = SaveDataManager.SaveFileExists;
            GUILayout.Label(saveExists ? "Save File: EXISTS" : "Save File: NONE");
            var path = Application.persistentDataPath + "/playersavedata.json";
            GUILayout.Label($"Path: {path}");
            GUILayout.Space(8);

            GUILayout.Label("--- Player ---");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Campaign Started", GUILayout.Width(130));
            _edit.campaignStarted = GUILayout.Toggle(_edit.campaignStarted, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Missions Completed", GUILayout.Width(130));
            _edit.missionsCompleted = Mathf.RoundToInt(
                GUILayout.HorizontalSlider(_edit.missionsCompleted, 0, 15));
            GUILayout.Label(_edit.missionsCompleted.ToString(), GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) HandleSave();
            if (GUILayout.Button("Delete")) HandleDelete();
            if (GUILayout.Button("New Player")) HandleNewPlayer();
            if (GUILayout.Button("Close")) Hide();
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(_status))
                GUILayout.Label(_status);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static void CopyFromSave(PlayerSaveData source, PlayerSaveData target)
        {
            target.username = source.username;
            target.campaignStarted = source.campaignStarted;
            target.missionsCompleted = source.missionsCompleted;
        }

        private void HandleSave()
        {
            SaveDataManager.Save(_edit);
            _status = "Saved";
        }

        private void HandleDelete()
        {
            SaveDataManager.Delete();
            _edit = new PlayerSaveData();
            var save = SaveDataManager.CurrentSave;
            if (save != null)
                CopyFromSave(save, _edit);
            _status = "Deleted, defaults loaded";
        }

        private void HandleNewPlayer()
        {
            SaveDataManager.Delete();
            _edit = new PlayerSaveData();
            var save = SaveDataManager.CurrentSave;
            if (save != null)
                CopyFromSave(save, _edit);
            _status = "New player — save notice shown";

            var router = UIRouter.Instance;
            router?.OpenModal<UI.Views.SaveNoticeView>();
        }
    }
}
