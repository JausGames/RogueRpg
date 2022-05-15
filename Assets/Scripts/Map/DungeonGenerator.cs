using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] StageGenerator stageGenerator;
    [SerializeField] int currentStage = 0;
    [SerializeField] List<StageGeneratorSettings> settingsList = new List<StageGeneratorSettings>()
    {
        new StageGeneratorSettings(4, 8),
        new StageGeneratorSettings(7, 12),
        new StageGeneratorSettings(10, 16),
        new StageGeneratorSettings(12, 20)
    };
    private Stage stage = null;

    private void Awake()
    {
        CoroutineWithData cd = new CoroutineWithData(this, stageGenerator.CreateStage(settingsList[0]));
        Debug.Log("DungeonGenerator, Awake");
        StartCoroutine(WaitForStage(cd));
    }
    public void GenerateNextStage()
    {
        stage.DeleteStage();
        if (currentStage <= settingsList.Count - 2)
        {
            CoroutineWithData cd = new CoroutineWithData(this, stageGenerator.CreateStage(settingsList[currentStage++]));
            StartCoroutine(WaitForStage(cd));
        }

    }
    IEnumerator WaitForStage(CoroutineWithData corout)
    {

        while (!(corout.result is Stage) || corout.result == null)
        {
            Debug.Log("EditorUI, WaitForLogin : data is null");
            yield return false;
        }
        if (corout.result is Stage) stage = (Stage) corout.result;

    }
}
public class StageGeneratorSettings
{
    [SerializeField] public int pathLenght;
    [SerializeField] public int minRoom;

    public StageGeneratorSettings(int pathLenght, int minRoom)
    {
        this.pathLenght = pathLenght;
        this.minRoom = minRoom;
    }
}
