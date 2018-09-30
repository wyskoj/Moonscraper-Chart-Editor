﻿// Copyright (c) 2016-2017 Alexander Ong
// See LICENSE in project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GameplayManager : MonoBehaviour {
    public AudioClip comboBreak;
    public CameraShake camShake;

    HitWindowFeeder hitWindowFeeder;
    AudioSource audioSource;
    OneShotSampleStream sample;

    public GameObject statsPanel;
    public UnityEngine.UI.Text noteStreakText;
    public UnityEngine.UI.Text percentHitText;
    public UnityEngine.UI.Text totalHitText;

    ChartEditor editor;
    float initSize;
    bool initialised = false;

    GuitarGameplayRulestate guitarGameplayRulestate;
    DrumsGameplayRulestate drumsGameplayRulestate;

    void Start()
    {
        sample = AudioManager.LoadSampleStream(comboBreak, 1);
        sample.onlyPlayIfStopped = true;

        editor = GameObject.FindGameObjectWithTag("Editor").GetComponent<ChartEditor>();

        initSize = transform.localScale.y;
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);

        hitWindowFeeder = gameObject.AddComponent<HitWindowFeeder>();
        guitarGameplayRulestate = new GuitarGameplayRulestate(KickMissFeedback);
        drumsGameplayRulestate = new DrumsGameplayRulestate(KickMissFeedback);

        initialised = true;
        statsPanel.SetActive(false);

        EventsManager.onApplicationModeChangedEventList.Add(OnApplicationModeChanged);
        EventsManager.onChartReloadEventList.Add(OnChartReloaded);
    }

    void Update()
    {
        // Configure collisions and choose to update the hit window or not
        if (Globals.applicationMode == Globals.ApplicationMode.Playing && !GameSettings.bot)
        {
            transform.localScale = new Vector3(transform.localScale.x, initSize, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
        }

        // Gameplay
        if (Globals.applicationMode == Globals.ApplicationMode.Playing && !GameSettings.bot)
        {
            float currentTime = editor.currentVisibleTime;
            GamepadInput gamepad = editor.inputManager.mainGamepad;

            if (editor.currentChart.gameMode == Chart.GameMode.Guitar)
            {
                guitarGameplayRulestate.Update(currentTime, hitWindowFeeder.guitarHitWindow, gamepad);
                UpdateUIStats(guitarGameplayRulestate);
            }
            else if (editor.currentChart.gameMode == Chart.GameMode.Drums)
            {
                drumsGameplayRulestate.Update(currentTime, hitWindowFeeder.drumsHitWindow, gamepad);
                UpdateUIStats(drumsGameplayRulestate);
            }
            else
            {
                Debug.LogError("Gameplay currently does not support this gamemode.");
            }
        }
    }

    void UpdateUIStats(BaseGameplayRulestate currentRulestate)
    {
        BaseGameplayRulestate.NoteStats stats = currentRulestate.stats;
        uint noteStreak = stats.noteStreak;
        uint totalNotes = stats.totalNotes;
        uint notesHit = stats.notesHit;

        noteStreakText.text = noteStreak.ToString();
        if (totalNotes > 0)
            percentHitText.text = ((float)notesHit / (float)totalNotes * 100).Round(2).ToString() + "%";
        else
            percentHitText.text = "0.00%";

        totalHitText.text = notesHit.ToString() + " / " + totalNotes.ToString();
    }

    void KickMissFeedback()
    {       
        if (sample.Play())
        {
            camShake.ShakeCamera();
        }
    }

    void Reset()
    {
        noteStreakText.text = "0";
        percentHitText.text = "0%";
        totalHitText.text = "0/0";

        if (initialised)
        {
            hitWindowFeeder.Reset();
            guitarGameplayRulestate.Reset();
            drumsGameplayRulestate.Reset();
        }
    }

    void OnApplicationModeChanged(Globals.ApplicationMode applicationMode)
    {
        Reset();
        statsPanel.SetActive(Globals.applicationMode == Globals.ApplicationMode.Playing && !GameSettings.bot);
    }

    void OnChartReloaded()
    {
        Reset();
    }
}
