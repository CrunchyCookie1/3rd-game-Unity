using UnityEngine;
using UnityEngine.Events;

public class FillLiquid : MonoBehaviour
{
    RotatePipes rotatePipes;

    public GameObject[] liquid1;
    public GameObject[] liquid2;

    public GameObject[] rotatePipesValue1;
    public int[] correctValuesForPipes1;

    public GameObject[] rotatePipesValue2;
    public int[] correctValuesForPipes2;

    public UnityEvent onTargetReached;
    public UnityEvent onFail;

    private void Start()
    {
        if (rotatePipesValue1 != null && correctValuesForPipes1 != null)
        {
            if (rotatePipesValue1.Length != correctValuesForPipes1.Length)
            {
                Debug.LogWarning("rotatePipesValue1 and correctValuesForPipes1 arrays have different lengths!");
            }
        }

        if (rotatePipesValue2 != null && correctValuesForPipes2 != null)
        {
            if (rotatePipesValue2.Length != correctValuesForPipes2.Length)
            {
                Debug.LogWarning("rotatePipesValue2 and correctValuesForPipes2 arrays have different lengths!");
            }
        }

        if (rotatePipesValue1 != null && rotatePipesValue1.Length > 0)
        {
            rotatePipes = rotatePipesValue1[0].GetComponent<RotatePipes>();
        }
    }

    public void FillPipe()
    {
        if (rotatePipesValue1 == null || rotatePipesValue1.Length == 0)
        {
            Debug.LogWarning("No pipes assigned to rotatePipesValue1!");
            return;
        }

        if (correctValuesForPipes1 == null || correctValuesForPipes1.Length == 0)
        {
            Debug.LogWarning("No correct values assigned for pipes!");
            return;
        }

        if (rotatePipesValue1.Length != correctValuesForPipes1.Length)
        {
            Debug.LogError($"Array mismatch! rotatePipesValue1 has {rotatePipesValue1.Length} items but correctValuesForPipes1 has {correctValuesForPipes1.Length} items!");
            return;
        }

        bool allMatch = true;

        for (int i = 0; i < rotatePipesValue1.Length; i++)
        {
            GameObject pipeObj = rotatePipesValue1[i];
            int correctValue = correctValuesForPipes1[i];

            if (pipeObj == null)
            {
                Debug.LogWarning($"Pipe at index {i} is null!");
                allMatch = false;
                break;
            }

            RotatePipes pipeScript = pipeObj.GetComponent<RotatePipes>();
            if (pipeScript == null)
            {
                Debug.LogWarning($"Pipe {pipeObj.name} at index {i} doesn't have RotatePipes component!");
                allMatch = false;
                break;
            }
            if (pipeScript.currentStage != correctValue)
            {
                allMatch = false;
                Debug.LogWarning($"Pipe {pipeObj.name} has stage {pipeScript.currentStage}, but correct value is {correctValue}");
                break;
            }
        }

        if (allMatch)
        {
            ActivateLiquid(liquid1);
            Debug.LogWarning("Pipe 1 is correct! Liquid 1 activated.");
        }
        else
        {
            DeactivateLiquid(liquid1);
            Debug.LogWarning("Pipe 1 doesn't match - liquid 1 deactivated");
        }
    }

    public void FillPipe2()
    {
        if (rotatePipesValue2 == null || rotatePipesValue2.Length == 0)
        {
            Debug.LogWarning("No pipes assigned to rotatePipesValue2!");
            return;
        }

        if (correctValuesForPipes2 == null || correctValuesForPipes2.Length == 0)
        {
            Debug.LogWarning("No correct values assigned for pipes 2!");
            return;
        }

        if (rotatePipesValue2.Length != correctValuesForPipes2.Length)
        {
            Debug.LogError($"Array mismatch! rotatePipesValue2 has {rotatePipesValue2.Length} items but correctValuesForPipes2 has {correctValuesForPipes2.Length} items!");
            return;
        }

        bool allMatch = true;

        for (int i = 0; i < rotatePipesValue2.Length; i++)
        {
            GameObject pipeObj = rotatePipesValue2[i];
            int correctValue = correctValuesForPipes2[i];

            if (pipeObj == null)
            {
                Debug.LogWarning($"Pipe 2 at index {i} is null!");
                allMatch = false;
                break;
            }

            RotatePipes pipeScript = pipeObj.GetComponent<RotatePipes>();
            if (pipeScript == null)
            {
                Debug.LogWarning($"Pipe 2 {pipeObj.name} at index {i} doesn't have RotatePipes component!");
                allMatch = false;
                break;
            }

            if (pipeScript.currentStage != correctValue)
            {
                allMatch = false;
                Debug.LogWarning($"Pipe 2 {pipeObj.name} has stage {pipeScript.currentStage}, but correct value is {correctValue}");
                break;
            }
        }

        if (allMatch)
        {
            ActivateLiquid(liquid2);
            Debug.LogWarning("Pipe 2 is correct! Liquid 2 activated.");
        }
        else
        {
            DeactivateLiquid(liquid2);
            Debug.LogWarning("Pipe 2 doesn't match - liquid 2 deactivated");
        }
    }

    public void CheckAllPipes()
    {
        bool pipe1Correct = CheckPipe1();
        bool pipe2Correct = CheckPipe2();

        if (pipe1Correct && pipe2Correct)
        {
            ActivateLiquid(liquid1);
            ActivateLiquid(liquid2);
            Debug.LogWarning("Both pipe systems are correct! All liquids activated.");
        }
        else if (pipe1Correct)
        {
            ActivateLiquid(liquid1);
            onTargetReached.Invoke();
            Debug.LogWarning("Pipe 1 is correct! Liquid 1 activated.");
        }
        else if (pipe2Correct)
        {
            ActivateLiquid(liquid2);
            onTargetReached.Invoke();
            Debug.LogWarning("Pipe 2 is correct! Liquid 2 activated.");
        }
        else
        {
            DeactivateLiquid(liquid1);
            DeactivateLiquid(liquid2);
            ResetPipes();
            Debug.LogWarning("Neither pipe system is correct! All liquids deactivated.");
        }
    }

    void ResetPipes()
    {
        onFail.Invoke();
        // Reset pipes from Pipe 1 system
        if (rotatePipesValue1 != null)
        {
            foreach (GameObject pipeObj in rotatePipesValue1)
            {
                if (pipeObj != null)
                {
                    RotatePipes pipe = pipeObj.GetComponent<RotatePipes>();
                    if (pipe != null)
                    {
                        pipe.ResetPipe();
                    }
                }
            }
        }

        // Reset pipes from Pipe 2 system
        if (rotatePipesValue2 != null)
        {
            foreach (GameObject pipeObj in rotatePipesValue2)
            {
                if (pipeObj != null)
                {
                    RotatePipes pipe = pipeObj.GetComponent<RotatePipes>();
                    if (pipe != null)
                    {
                        pipe.ResetPipe();
                    }
                }
            }
        }

        Debug.LogWarning("All pipes have been reset!");
    }

    private bool CheckPipe1()
    {
        if (rotatePipesValue1 == null || rotatePipesValue1.Length == 0)
            return false;

        if (correctValuesForPipes1 == null || correctValuesForPipes1.Length == 0)
            return false;

        if (rotatePipesValue1.Length != correctValuesForPipes1.Length)
            return false;

        for (int i = 0; i < rotatePipesValue1.Length; i++)
        {
            GameObject pipeObj = rotatePipesValue1[i];
            int correctValue = correctValuesForPipes1[i];

            if (pipeObj == null)
                return false;

            RotatePipes pipeScript = pipeObj.GetComponent<RotatePipes>();
            if (pipeScript == null)
                return false;

            if (pipeScript.currentStage != correctValue)
                return false;
        }

        return true;
    }

    private bool CheckPipe2()
    {
        if (rotatePipesValue2 == null || rotatePipesValue2.Length == 0)
            return false;

        if (correctValuesForPipes2 == null || correctValuesForPipes2.Length == 0)
            return false;

        if (rotatePipesValue2.Length != correctValuesForPipes2.Length)
            return false;

        for (int i = 0; i < rotatePipesValue2.Length; i++)
        {
            GameObject pipeObj = rotatePipesValue2[i];
            int correctValue = correctValuesForPipes2[i];

            if (pipeObj == null)
                return false;

            RotatePipes pipeScript = pipeObj.GetComponent<RotatePipes>();
            if (pipeScript == null)
                return false;

            if (pipeScript.currentStage != correctValue)
                return false;
        }

        return true;
    }

    private void ActivateLiquid(GameObject[] liquidArray)
    {
        if (liquidArray == null) return;

        foreach (GameObject liquid in liquidArray)
        {
            if (liquid != null)
            {
                liquid.SetActive(true);
            }
        }
    }

    private void DeactivateLiquid(GameObject[] liquidArray)
    {
        if (liquidArray == null) return;

        foreach (GameObject liquid in liquidArray)
        {
            if (liquid != null)
            {
                liquid.SetActive(false);
            }
        }
    }

    public void TestLiquid()
    {
        ActivateLiquid(liquid1);
    }
}