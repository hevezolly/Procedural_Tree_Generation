using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using LSystem;

public class LSystemTest : MonoBehaviour
{
    [SerializeField]
    private LSystemProvider LSys;
    [SerializeField]
    private bool FireEvent;
    [SerializeField]
    private bool debugTime;
    [SerializeField]
    private bool displayParameters;

    private ILSystem system;

    public LSystemVisualiseType visualiseType => LSys.Type;

    private Coroutine timer;
    private Coroutine flow;

    [SerializeField]
    private ModuleListEvent StepEvent;

    public List<IModule> GetCurrentState()
    {
        return system.CurrentState.ToList();
    }

    void Start()
    {
        system = LSys.Compile();
        system.Init();
        InvokeEvent(system.CurrentState.ToList());
        system.Display(displayParameters);
    }

    private void InvokeEvent(List<IModule> modules)
    {
        if (!FireEvent)
            return;
        StepEvent?.Invoke(modules);
    }

    private void Step()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        system.Step();
        watch.Stop();
        InvokeEvent(system.CurrentState.ToList());
        system.Display(displayParameters);
        if (debugTime)
            Debug.Log($"Milliseconds: {watch.ElapsedMilliseconds}");
    }

    private IEnumerator StartHoldCountDown()
    {
        yield return new WaitForSeconds(1);
        flow = StartCoroutine(StartFlow());
    }

    private IEnumerator StartFlow()
    {
        while (true)
        {
            Step();
            yield return new WaitForSeconds(0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Step();
            timer = StartCoroutine(StartHoldCountDown());
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (flow != null)
                StopCoroutine(flow);
            if (timer != null)
                StopCoroutine(timer);
            timer = null;
            flow = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (system != null && system.CurrentState.Count() > 0)
        {
            var leafs = system.CurrentState.First().GetParameterValue<LeafsPositions>("all_leafs", null);
            if (leafs == null)
                return;
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(leafs.Avg(), 0.2f);
        }
    }
}
