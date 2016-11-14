﻿using UnityEngine;
using System.Collections;

public abstract class MovementController : MonoBehaviour {
    [HideInInspector]
    public ApplicationMode applicationMode = ApplicationMode.Editor;

    public ChartEditor editor;

    protected Vector3 initPos;
    protected float scrollDelta = 0;

    // Program options
    protected float mouseScrollSensitivity = 0.5f;

    // Jump to a chart position
    public abstract void SetPosition(uint chartPosition);

    protected void Start()
    {
        initPos = transform.position;
    }

    public void PlayingMovement()
    {
        // Auto scroll camera- Run in FixedUpdate
        float speed = Globals.hyperspeed;
        Vector3 pos = transform.position;
        pos.y += (speed * Time.fixedDeltaTime);
        transform.position = pos;
    }

    void OnGUI()
    {
        if (UnityEngine.Event.current.type == EventType.ScrollWheel)
        {
            scrollDelta = -UnityEngine.Event.current.delta.y;
        }
        else
        {
            scrollDelta = 0;
        }
    }

    public enum ApplicationMode
    {
        Editor, Playing
    }
}
