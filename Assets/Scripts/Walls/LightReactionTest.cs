using DissolveExample;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReactionTest : MonoBehaviour
{
    [SerializeField] ColorNeededToExist colorNeeded;
    


    private Collider col;
    private ParticleSystem particles;
    private Dissolve _dissolveScript;

    private List<LanternColor> colorsHittingNow = new List<LanternColor>(); 
    private LanternColor[] colorTag;

    private bool _isExisting;
    public bool _objectInCollider;

    private void Awake()
    {
        _dissolveScript = gameObject.GetComponent<Dissolve>();
        col = GetComponent<BoxCollider>();
        particles = GetComponentInChildren<ParticleSystem>();
        //HandleParticleSystemSize(col);
    }

    private void Start()
    {
        switch (colorNeeded)
        {

            case ColorNeededToExist.None:
                colorTag = new LanternColor[0];
                break;

            case ColorNeededToExist.Red:
                colorTag = new LanternColor[] { LanternColor.Red };
                break;

            case ColorNeededToExist.Yellow:
                colorTag = new LanternColor[] { LanternColor.Yellow };
                break;

            case ColorNeededToExist.Blue:
                colorTag = new LanternColor[] { LanternColor.Blue };
                break;

            case ColorNeededToExist.Purple:
                colorTag = new LanternColor[] { LanternColor.Red, LanternColor.Blue };
                break;

            case ColorNeededToExist.Green:
                colorTag = new LanternColor[] { LanternColor.Blue, LanternColor.Yellow };
                break;

            case ColorNeededToExist.Orange:
                colorTag = new LanternColor[] { LanternColor.Red, LanternColor.Yellow };
                break;

            case ColorNeededToExist.White:
                colorTag = new LanternColor[] { LanternColor.Red, LanternColor.Yellow, LanternColor.Blue };
                break;


        }
        
    }

    void Update()
    {
        CheckIfShouldExist();
        HandleParticleColor();
    }

    private void Exist()
    {
        if (!_isExisting && !_objectInCollider)
        {
            _dissolveScript.UnDissolve();
            col.isTrigger = false;
            _isExisting = true;
        }
    }

    private void DontExist()
    {
        if (_isExisting)
        {
            _dissolveScript._Dissolve();
            col.isTrigger = true;
            _isExisting = false;
        }

    }

    public void AddColorToList(LanternColor colorToAdd)
    {
        if (!colorsHittingNow.Contains(colorToAdd))
        {
            colorsHittingNow.Add(colorToAdd);
        }
    }

    public void RemoveColorFromList(LanternColor colorToRemove)
    {
        if (colorsHittingNow.Contains(colorToRemove))
        {
            colorsHittingNow.Remove(colorToRemove);
        }
    }

    private bool CompareListToColorTag(List<LanternColor> list, LanternColor[] lanternColorArray)
    {


        if (list.Count != lanternColorArray.Length)
        {
            return false;
        }

        foreach (LanternColor color in lanternColorArray)
        {
            if (!list.Contains(color)) return false;
        }

        return true;
    }

    public void CheckIfShouldExist()
    {
        if (colorsHittingNow == null)
        {
            Debug.LogError("colorsHittingnow is Null");
        }
            if (CompareListToColorTag(colorsHittingNow, colorTag))
            {
                DontExist();
            }
            else
            {
                Exist();
            }
    }

    private Color _previousColor;
    private void HandleParticleColor()
    {
        var main = particles.main;
        Color newColor = main.startColor.color; // Default to current color

        switch (colorsHittingNow)
        {
            case var _ when colorsHittingNow.Count == 0 && !_objectInCollider:
                particles.Stop();
                _previousColor = Color.clear; // Track no color situation
                return;

            case var _ when colorsHittingNow.Contains(LanternColor.Red) && colorsHittingNow.Count == 1:
                newColor = Color.red;
                break;

            case var _ when colorsHittingNow.Contains(LanternColor.Blue) && colorsHittingNow.Count == 1:
                newColor = Color.blue;
                break;

            case var _ when colorsHittingNow.Contains(LanternColor.Yellow) && colorsHittingNow.Count == 1:
                newColor = Color.yellow;
                break;

            case var _ when colorsHittingNow.Contains(LanternColor.Yellow) && colorsHittingNow.Contains(LanternColor.Red) && colorsHittingNow.Count == 2:
                newColor = new Color(1f, 0.4447487f, 0, 1f); // Orange
                break;

            case var _ when colorsHittingNow.Contains(LanternColor.Blue) && colorsHittingNow.Contains(LanternColor.Red) && colorsHittingNow.Count == 2:
                newColor = new Color(0.7490196f, 0.2509804f, 0.7490196f, 1f); // Purple
                break;

            case var _ when colorsHittingNow.Contains(LanternColor.Yellow) && colorsHittingNow.Contains(LanternColor.Blue) && colorsHittingNow.Count == 2:
                newColor = Color.green;
                break;

            case var _ when colorsHittingNow.Count == 3: // All colors hitting
                newColor = Color.white;
                break;
        }

        // Check if the color has changed
        if (newColor != _previousColor)
        {
            particles.Clear(); // Remove old particles
            main.startColor = newColor;
            particles.Play();
            _previousColor = newColor; // Update previous color
        }
    }

    private void HandleParticleSystemSize(Collider col)
    {
        var shape = particles.shape;

        shape.scale = col.bounds.size; 
    }

}
