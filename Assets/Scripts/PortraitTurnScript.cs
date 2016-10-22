using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PortraitTurnScript : MonoBehaviour
    {

        private CameraScript _cam;
        private GameObject _portrait;
        // Drag images to portraits in editor
        // 0 - player
        // 1 - Skeleton
        public Texture[] _textures;
        public Sprite targetSquare;
        public List<GameObject> _portraits;
        private GameObject _previousTarget;

        void Start ()
        {
            _cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
            _portrait = Resources.Load("UnitPortrait", typeof(GameObject)) as GameObject;
            _portraits = new List<GameObject>();
        }
	
        void Update () {
            // Re-create portraits if amount of characters change
            if (_cam.characterList.Count != _portraits.Count) 
            {
                foreach (GameObject port in _portraits)
                {
                    Destroy(port);
                }

                _portraits.RemoveAll(x => x);

                if (_cam.characterList.Count > _cam.pHandler.playerList.Count) { 
                    var i = 250;
                    foreach (var character in _cam.characterList)
                    {
                        GameObject portrait = Instantiate(_portrait, new Vector3(i, -50, 0), Quaternion.identity) as GameObject;
                        portrait.transform.SetParent(gameObject.transform, false);
                        _portraits.Add(portrait);

                        int texture = 0;

                        switch (character.tag)
                        {
                            case "Player":
                                texture = 0;
                            break;
                            case "Enemy":
                                texture = 1;
                            break;
                        }
                        portrait.GetComponent<RawImage>().texture = _textures[texture];
                        portrait.GetComponent<CharacterHolder>().Character = character;
                        i += 70;
                    }
                    SetPortraitFocus(true);
                }
            }

            SetPortraitFocus();
        }

        void SetPortraitFocus(bool force = false)
        {
            if (_cam.CurrentUnit != _previousTarget || force)
            {
                if (_previousTarget != null)
                    _portraits[_cam.characterList.IndexOf(_previousTarget)].GetComponentInChildren<Image>().enabled = false;

                if (_portraits.Count-1 >= _cam.characterList.IndexOf(_cam.CurrentUnit))
                    _portraits[_cam.characterList.IndexOf(_cam.CurrentUnit)].GetComponentInChildren<Image>().enabled = true;

                /*foreach (var portrait in _portraits)
                {
                    portrait.GetComponentInChildren<Image>().enabled = portrait.GetComponent<CharacterHolder>().Character.GetComponent<ICharacter>().IsMyTurn();
                    Debug.Log("Portrait:" + portrait.GetComponent<CharacterHolder>().Character.tag + " isturn: " + portrait.GetComponent<CharacterHolder>().Character.GetComponent<ICharacter>().IsMyTurn());
                }*/

                _previousTarget = _cam.CurrentUnit;
            }
        }
    }
}
