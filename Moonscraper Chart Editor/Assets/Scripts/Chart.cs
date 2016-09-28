﻿using UnityEngine;
using System.Collections.Generic;

public class Chart  {
    public const int NOTFOUND = -1;

    public List<Note> notes;

    public Note this[int i]
    {
        get { return notes[i]; }
        set { notes[i] = value; }
    }

    public int Length { get { return notes.Count; } }

    public Chart ()
    {
        notes = new List<Note>();
    }

    public void Add (Note note)
    {
        notes.Add(note);
    }

    public Note[] ToArray()
    {
        return notes.ToArray();
    }

    public int BinarySearchChartClosestNote(Note searchItem)
    {
        int lowerBound = 0;
        int upperBound = notes.Count;
        int index = NOTFOUND;

        int midPoint = NOTFOUND;

        while (lowerBound <= upperBound)
        {
            midPoint = lowerBound + (upperBound - lowerBound) / 2;

            if (notes[midPoint] == searchItem)
            {
                index = midPoint;

                break;
            }
            else
            {
                if (notes[midPoint] < searchItem)
                {
                    // data is in upper half
                    lowerBound = midPoint + 1;
                }
                else
                {
                    // data is in lower half 
                    upperBound = midPoint - 1;
                }
            }
        }

        index = midPoint;

        return index;
    }

    public int BinarySearchChartExactNote(Note searchItem)
    {
        int pos = BinarySearchChartClosestNote(searchItem);

        if (pos != NOTFOUND)
        {
            if (notes[pos] != searchItem)
                pos = NOTFOUND;
        }

        return pos;
    }

    // Returns all the notes found at the specified position, i.e. chords
    public Note[] GetNotes(int pos)
    {
        
        return new Note[0];
    }
}
