using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string axiom; // The Root Sentence

    [Range(0,10)]
    public int iterationLimit = 1;

    public bool randomIgnoreRuleModifier = false;
    [Range(0, 1)]
    public float chanceToIgnorerule = 0.25f;

    private void Start()
    {
        Debug.Log(GenerateSentence());
    }

    public string GenerateSentence(string word = null)
    {
        if (word == null)
            word = axiom;
        return GrowRecursive(word);
    }

    private string GrowRecursive(string word, int iterationIndex = 0)
    {
        if (iterationIndex >= iterationLimit)
            return word;
        StringBuilder newWord = new StringBuilder();

        foreach (var c in word)
        {
            newWord.Append(c);
            ProcessRulesRecursively(newWord, c, iterationIndex);
        }
        return newWord.ToString();
    }

    private void ProcessRulesRecursively(StringBuilder newWord, char c, int iterationIndex)
    {
        foreach (var rule in rules)
        {
            if(rule.letter == c.ToString())
            {
                if (randomIgnoreRuleModifier && iterationIndex > 1)
                {
                    if (UnityEngine.Random.value < chanceToIgnorerule)
                        return;
                }
                newWord.Append(GrowRecursive(rule.GetResult(), iterationIndex + 1));
            }       
        }
    }
}
