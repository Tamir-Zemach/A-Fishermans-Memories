using System;
using System.Collections.Generic;
using System.Linq;
using DissolveExample;
using UnityEngine;
using UnityEngine.Serialization;

namespace Walls
{
    public class ColorReactiveWall : MonoBehaviour
    {
        [SerializeField] private ColorNeededToExist _colorNeeded;
        
        private Collider _col;
        private ParticleSystem _particles;
        private Dissolve _dissolveScript;

        private readonly List<LanternColor> _colorsHittingNow = new(); 
        private LanternColor[] _colorTag;
        private Color _previousColor;

        private bool _isExisting;
        public bool ObjectInCollider;

        private void Awake()
        {
            _dissolveScript = gameObject.GetComponent<Dissolve>();
            _col = GetComponent<BoxCollider>();
            _particles = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            _colorTag = _colorNeeded switch
            {
                ColorNeededToExist.None => Array.Empty<LanternColor>(),
                ColorNeededToExist.Red => new[] { LanternColor.Red },
                ColorNeededToExist.Yellow => new[] { LanternColor.Yellow },
                ColorNeededToExist.Blue => new[] { LanternColor.Blue },
                ColorNeededToExist.Purple => new[] { LanternColor.Red, LanternColor.Blue },
                ColorNeededToExist.Green => new[] { LanternColor.Blue, LanternColor.Yellow },
                ColorNeededToExist.Orange => new[] { LanternColor.Red, LanternColor.Yellow },
                ColorNeededToExist.White => new[]
                {
                    LanternColor.Red, LanternColor.Yellow, LanternColor.Blue
                },
                _ => _colorTag
            };
        }

        private void Update()
        {
            CheckIfShouldExist();
            HandleParticleColor();
        }

        private void Exist()
        {
            if (_isExisting || ObjectInCollider) return;
            _dissolveScript.UnDissolve();
            _col.isTrigger = false;
            _isExisting = true;
        }

        private void DontExist()
        {
            if (!_isExisting) return;
            _dissolveScript._Dissolve();
            _col.isTrigger = true;
            _isExisting = false;

        }

        public void AddColorToList(LanternColor colorToAdd)
        {
            if (!_colorsHittingNow.Contains(colorToAdd))
            {
                _colorsHittingNow.Add(colorToAdd);
            }
        }

        public void RemoveColorFromList(LanternColor colorToRemove)
        {
            if (_colorsHittingNow.Contains(colorToRemove))
            {
                _colorsHittingNow.Remove(colorToRemove);
            }
        }

        private bool CompareListToColorTag(List<LanternColor> list, LanternColor[] lanternColorArray)
        {
        
            if (list.Count != lanternColorArray.Length)
            {
                return false;
            }

            return lanternColorArray.All(list.Contains);
        }

        private void CheckIfShouldExist()
        {
            if (_colorsHittingNow == null)
            {
                Debug.LogError("colorsHittingnow is Null");
            }

            if (!CompareListToColorTag(_colorsHittingNow, _colorTag))
            {
                Exist();
            }
            else
            {
                DontExist();
            }
        }
    
        private void HandleParticleColor()
        {
            var main = _particles.main;
            var newColor = // Default to current color
                main.startColor.color;

            switch (_colorsHittingNow)
            {
                case var _ when _colorsHittingNow.Count == 0 && !ObjectInCollider:
                    _particles.Stop();
                    _previousColor = Color.clear; // Track no color situation
                    return;

                case var _ when _colorsHittingNow.Contains(LanternColor.Red) && _colorsHittingNow.Count == 1:
                    newColor = Color.red;
                    break;

                case var _ when _colorsHittingNow.Contains(LanternColor.Blue) && _colorsHittingNow.Count == 1:
                    newColor = Color.blue;
                    break;

                case var _ when _colorsHittingNow.Contains(LanternColor.Yellow) && _colorsHittingNow.Count == 1:
                    newColor = Color.yellow;
                    break;

                case var _ when _colorsHittingNow.Contains(LanternColor.Yellow) && _colorsHittingNow.Contains(LanternColor.Red) && _colorsHittingNow.Count == 2:
                    newColor = new Color(1f, 0.4447487f, 0, 1f); // Orange
                    break;

                case var _ when _colorsHittingNow.Contains(LanternColor.Blue) && _colorsHittingNow.Contains(LanternColor.Red) && _colorsHittingNow.Count == 2:
                    newColor = new Color(0.7490196f, 0.2509804f, 0.7490196f, 1f); // Purple
                    break;

                case var _ when _colorsHittingNow.Contains(LanternColor.Yellow) && _colorsHittingNow.Contains(LanternColor.Blue) && _colorsHittingNow.Count == 2:
                    newColor = Color.green;
                    break;

                case var _ when _colorsHittingNow.Count == 3: // All colors hitting
                    newColor = Color.white;
                    break;
            }

            // Check if the color has changed
            if (newColor == _previousColor) return;
            _particles.Clear(); // Remove old particles
            main.startColor = newColor;
            _particles.Play();
            _previousColor = newColor; // Update previous color
        }

    }
}
