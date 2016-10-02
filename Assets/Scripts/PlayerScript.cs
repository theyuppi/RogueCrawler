using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [System.Serializable]
    public class PlayerScript : MonoBehaviour, ICharacter
    {
        //private Rigidbody2D rBody;
        private Coroutine playerMovement;
        private SpriteRenderer sRender;
        private Animator animaThor;
        public TileScript tScript;
        public GameObject tile;

        public const float stepDuration = 0.2f;
        public const float stepAttackDuration = 0.15f;
        public const float stepDoorDuration = 0.5f;
        public const float stepDoorDistance = 140;
        public const float stepPortalDistance = 100;
        public List<Node> currentPath = null;
        public ReadSpriteScript map;
        public int tileX = 0;
        public int tileY = 0;
        public int tileXmoved = 0;
        public int tileYmoved = 0;
        public bool isMoving = false;
        public bool isPerformingAttack = false;
        public bool lastNotWalkable = false;
        public bool _myTurn = false;
        public Text healthText;

        //Stats
        public string playerName = "";
        public int xp = 0;
        private int health = 100;
        private int maxHealth = 100;
        public int attackPower = 0;
        public int currActPts = 0;
        private int maxActPts = 20;
        public int skillPointsRemaining = 0;
        public int skillPointsPerLevel = 5;
        private int base_vitality = 5;
        private int base_strength = 5;
        private int base_defence = 5;
        private int base_speed = 5;
        private int base_agility = 5;


        public int skill_vitality = 0;
        public int skill_strength = 0;
        public int skill_defence = 0;
        public int skill_speed = 0;
        public int skill_agility = 0;

        private int _initiative = 1;
        private int vitality = 0;
        private int strength = 0;
        private int defence = 0;
        private int speed = 0;
        private int agility = 0;
        //private int recoveryRate = 0;
        //private int combatStartAP = 0; //Actionpoints at start of combat


        public InventoryScript inventory;
        public List<Text> statsLabels;

        //Gear
        Item mainhand_Slot = null;
        Item offhand_Slot = null;
        Item helm_Slot = null;
        Item armor_Slot = null;
        Item leggings_Slot = null;
        int bonusStr = 0;
        int bonusDef = 0;
        int bonusVit = 0;
        //States
        public bool stunned = false;
        private bool CombatMode { get; set; }
        public int charLVL = 1;
        public List<int> xpLevels = new List<int> { };

        public float myOffsetX = 0;
        public float myOffsetY = 20f;

        private enum direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private direction myDirection = direction.Right;
        private CameraScript _cameraScript;


        void Start()
        {
           // rBody = GetComponent<Rigidbody2D>();
            sRender = GetComponent<SpriteRenderer>();
            animaThor = GetComponent<Animator>();
            animaThor.SetInteger("State", 0);
            healthText = GetComponentInChildren<Text>();
            healthText.text = health.ToString();
            _cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();

            StringBuilder str = new StringBuilder();
            str.Append(health.ToString());
            str.Append("/");
            str.Append(maxHealth.ToString());
            statsLabels[9].text = str.ToString();

            ReceiveActPts();

            playerName = PlayerPrefs.GetString("pName");
            charLVL = PlayerPrefs.GetInt("pLevel");
            xp = PlayerPrefs.GetInt("pCurrXp");
            maxActPts = PlayerPrefs.GetInt("pMaxAP");
            maxHealth = PlayerPrefs.GetInt("pMaxHealth");
            base_vitality = PlayerPrefs.GetInt("pVitality");
            base_strength = PlayerPrefs.GetInt("pStrength");
            base_agility = PlayerPrefs.GetInt("pAgility");
            base_speed = PlayerPrefs.GetInt("pSpeed");
            base_defence = PlayerPrefs.GetInt("pDefence");
            skillPointsRemaining = PlayerPrefs.GetInt("pSkillPoints");

		

            if (PlayerPrefs.GetInt("PlayerShouldLoadItems") == 1)
            {
                if (map.currentLevel == 1)
                {
                    inventory.AddStartItems();
                }
                else
                {
                    LoadItemsFromSave();
                }
			
                PlayerPrefs.SetInt("PlayerShouldLoadItems", 0);
            }
            else
            {
                ResetPlayer();
                inventory.AddStartItems();
            }

            if (charLVL == 0)
            {
                charLVL = 1;
            }
        }

        private void FixedUpdate()
        {
            //Level up
            if (xp >= xpLevels[charLVL - 1])
            {
                LevelUp();
            }

            if (health <= 0)
            {
                StartCoroutine(FadeIn());
                map.PlayerDied();
            }

            if (_cameraScript.CombatMode != CombatMode)
            {
                CombatMode = _cameraScript.CombatMode;
                currActPts = maxActPts;
            }

            UpdateStats();
        }

        void Update()
        {
            CheckKeyDown();
        }

        private void LevelUp()
        {
            if (charLVL < xpLevels.Count) {
                xp = 0;
                charLVL++;
                health = maxHealth;
                healthText.text = health.ToString();
                skillPointsRemaining += (skillPointsPerLevel);
            }
        }

        private void CheckKeyDown()
        {
            if (Debug.isDebugBuild)
            {
                if (Input.GetKeyDown(KeyCode.U))
                    LevelUp();

                if (Input.GetKeyDown(KeyCode.Y))
                {
                    foreach (var ent in _cameraScript.characterList)
                    {
                        Debug.Log(ent);
                    }
                }

                if (Input.GetKeyDown(KeyCode.T))
                {
                    Debug.Log(_cameraScript);
                }

            }


            if (Input.GetKeyDown(KeyCode.F))
            {
                Time.timeScale = 2;
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                Time.timeScale = 1;
            }
        }

        public void UpdateStats()
        {
            bonusStr = 0;
            bonusDef = 0;
            bonusVit = 0;
            mainhand_Slot = null;
            offhand_Slot = null;
            helm_Slot = null;
            armor_Slot = null;
            leggings_Slot = null;

            maxHealth = 75 + vitality * 5;
            attackPower = strength * 2;

            foreach (Transform child in inventory.eqSlots[1].transform)
            {
                if (child.CompareTag("Item"))
                {
                    mainhand_Slot = child.GetComponent<ItemDataScript>().item;
                }
            }

            foreach (Transform child in inventory.eqSlots[2].transform)
            {
                if (child.CompareTag("Item"))
                {
                    offhand_Slot = child.GetComponent<ItemDataScript>().item;
                }
            }

            foreach (Transform child in inventory.eqSlots[0].transform)
            {
                if (child.CompareTag("Item"))
                {
                    helm_Slot = child.GetComponent<ItemDataScript>().item;
                }
            }

            foreach (Transform child in inventory.eqSlots[3].transform)
            {
                if (child.CompareTag("Item"))
                {
                    armor_Slot = child.GetComponent<ItemDataScript>().item;
                }
            }

            foreach (Transform child in inventory.eqSlots[4].transform)
            {
                if (child.CompareTag("Item"))
                {
                    leggings_Slot = child.GetComponent<ItemDataScript>().item;
                }
            }

            if (mainhand_Slot != null)
            {
                attackPower += mainhand_Slot.stats.power;
                bonusDef += mainhand_Slot.stats.defence;
                maxHealth += mainhand_Slot.stats.vitality;
            }
            if (offhand_Slot != null)
            {
                attackPower += offhand_Slot.stats.power;
                bonusDef += offhand_Slot.stats.defence;
                maxHealth += offhand_Slot.stats.vitality;
            }
            if (helm_Slot != null)
            {
                attackPower += helm_Slot.stats.power;
                bonusDef += helm_Slot.stats.defence;
                maxHealth += helm_Slot.stats.vitality;
            }
            if (armor_Slot != null)
            {
                attackPower += armor_Slot.stats.power;
                bonusDef += armor_Slot.stats.defence;
                maxHealth += armor_Slot.stats.vitality;
            }
            if (leggings_Slot != null)
            {
                attackPower += leggings_Slot.stats.power;
                bonusDef += leggings_Slot.stats.defence;
                maxHealth += leggings_Slot.stats.vitality;
            }

            strength = base_strength + bonusStr + skill_strength;
            defence = base_defence + bonusDef + skill_defence;
            vitality = base_vitality + bonusVit + skill_vitality;
            agility = base_agility + skill_agility;
            speed = base_speed + skill_speed;

            maxActPts = 15 + speed;

            statsLabels[0].text = playerName;
            statsLabels[1].text = charLVL.ToString();
            statsLabels[2].text = xp.ToString();
            statsLabels[3].text = maxActPts.ToString();
            statsLabels[4].text = vitality.ToString();
            statsLabels[5].text = strength.ToString();
            statsLabels[6].text = agility.ToString();
            statsLabels[7].text = speed.ToString();
            statsLabels[8].text = defence.ToString();
            statsLabels[10].text = attackPower.ToString();
            statsLabels[11].text = skillPointsRemaining.ToString();

            StringBuilder str = new StringBuilder();
            str.Append(health.ToString());
            str.Append("/");
            str.Append(maxHealth.ToString());
            statsLabels[9].text = str.ToString();


            //Magic formula(TM)
        }

        #region ManualMove
        private IEnumerator Move(Vector2 direction)
        {
            Vector2 startPosition = transform.position;
            Vector2 destinationPosition = new Vector2(transform.position.x, transform.position.y) + (direction * 0.8f);
            float t = 0.0f;

            while (t < 1.0f)
            {
                transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
                t += Time.deltaTime / stepDuration;
                yield return new WaitForEndOfFrame();
            }

            transform.position = destinationPosition;

            playerMovement = null;
        }
        #endregion

        public IEnumerator MakeAMove()
        {

            int currNode = 0;
            isMoving = true;
            while (isMoving && currentPath != null && currNode < currentPath.Count - 1)
            {
                if (stunned)
                {
                    yield return new WaitForSeconds(0.5f);
                    stunned = false;
                }
                if (currActPts > 0)
                {
                    /*Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
                                    new Vector3(0, 0, -1f);
                    Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
                                  new Vector3(0, 0, -1f);

                    Debug.DrawLine(start, end, Color.red);*/

                    currNode++;


                    StartCoroutine(MoveNextTile(currNode));

                    if (currentPath == null || currNode == currentPath.Count - 1)
                    {
                        isMoving = false;
                    }
                    yield return new WaitForSeconds(stepDuration);
                }
                else
                {
                    isMoving = false;
                    break;
                }
            }

            map.ClearOldPath();
            currentPath = null;
        }

        public IEnumerator MoveNextTile(int currNode)
        {
            if (currentPath == null)
                yield return new WaitForEndOfFrame();

            // Move us to the next tile in the sequence

            tileX = currentPath[currNode].x;
            tileY = currentPath[currNode].y;

            Vector2 startPosition = transform.position;

            Vector2 destinationPosition = map.TileCoordToWorldCoord(tileX, tileY);
            destinationPosition.y += myOffsetY;
            Vector2 dir = destinationPosition - startPosition;
            dir.Normalize();
            switch ((int)dir.x)
            {
                case 1:
                    sRender.flipX = true;
                    break;

                case -1:
                    sRender.flipX = false;
                    break;

                default:
                    break;
            }

            var tileScript = map.myTileArray[tileX, tileY].GetComponent<TileScript>();
            // Attacking enemy or walking in to wall
            if (tileScript.walkable == false)
            {
                if (tileScript.hasEnemy)
                {
                    Vector2 roundDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
                    if (roundDir == Vector2.up)
                    {
                        myDirection = direction.Up;
                    }
                    else if (roundDir == Vector2.down)
                    {
                        myDirection = direction.Down;
                    }
                    else if (roundDir == Vector2.left)
                    {
                        myDirection = direction.Left;
                    }
                    else if (roundDir == Vector2.right)
                    {
                        myDirection = direction.Right;
                    }
                    if (isPerformingAttack == false)
                    {
                        if (currActPts >= 3)
                        {
                            PerformAttack(myDirection);
                            currActPts -= 3;
                        }
                    }
                }
                // Walking in to door
                if (tileScript.isDoor)
                {
                    yield return new WaitForSeconds(0.05f);


                    int a1 = 0;
                    int direction = 0;

                    if (tileX < 2)
                    {
                        direction = 3; // west
                    }
                    else if (tileX > 36)
                    {
                        direction = 4; // east
                    }
                    else if (tileY < 2)
                    {
                        direction = 2; // south 
                    }
                    else if (tileY > 36)
                    {
                        direction = 1; // north 
                    }

                    for (int a = 0; a < map.roomNode.GetLength(0); a++)
                    {
                        if (map.roomNode[a, 0] == tileScript.owner)
                        {
                            a1 = a;
                        }
                    }

                    StartCoroutine(FadeInOutPortal(a1, direction));
                }

                // Walking in to portal
                if (tileScript.isEndPortal)
                {
                    map.teleported = true;
                    Vector2 roundDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
                    if (roundDir == Vector2.up)
                    {
                        myDirection = direction.Up;
                    }
                    else if (roundDir == Vector2.down)
                    {
                        myDirection = direction.Down;
                    }
                    else if (roundDir == Vector2.left)
                    {
                        myDirection = direction.Left;
                    }
                    else if (roundDir == Vector2.right)
                    {
                        myDirection = direction.Right;
                    }

                    StartCoroutine(EnterPortal((int)myDirection)); //Enter Next Level
                }


                else if (currentPath != null)
                {
                    tileX = currentPath[currNode - 1].x;
                    tileY = currentPath[currNode - 1].y;
                }

            }
            else
            {

                if (_cameraScript.CombatMode)
                {
                    currActPts--;
                }

                // Lerp to new position
                float t = 0.0f;
                while (t <= 1.1f)
                {
                    transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
                    t += Time.deltaTime / stepDuration;
                    yield return new WaitForEndOfFrame();
                }

                if (currentPath != null)
                {
                    tileXmoved = currentPath[currNode].x;
                    tileYmoved = currentPath[currNode].y;
                }
                try
                {
                    map.myTileArray[tileXmoved, tileYmoved].GetComponent<TileScript>().ResetColor();  //Får fel ibland för att man går igenom en dörr och den försöker ta bort grön färg på rutor i gamla rummet
                }
                catch (Exception e)
                {
                    Debug.Log("Exception: " + e);
                }

            }


            if (currentPath != null && currNode == currentPath.Count)
            {
                currentPath = null;
            }
        }

        public int GetGoalTileX()
        {
            if (currentPath != null)
            {
                return currentPath[currentPath.Count - 1].x;
            }
            return 0;
        }
        public int GetGoalTileY()
        {
            return currentPath[currentPath.Count - 1].y;
        }

        private void PerformAttack(direction dir)
        {
            isPerformingAttack = true;
            switch (dir)
            {
                case direction.Up:
                    StartCoroutine(PerformAttackMove(Vector2.up * 0.5f));
                    animaThor.SetInteger("State", 1);
                    break;
                case direction.Down:
                    StartCoroutine(PerformAttackMove(Vector2.down * 0.5f));
                    animaThor.SetInteger("State", 2);
                    break;
                case direction.Left:
                    StartCoroutine(PerformAttackMove(Vector2.left * 0.5f));
                    animaThor.SetInteger("State", 1);
                    break;
                case direction.Right:
                    StartCoroutine(PerformAttackMove(Vector2.right * 0.5f));
                    animaThor.SetInteger("State", 1);

                    break;
                default:
                    break;
            }
            map.myTileArray[tileX, tileY].GetComponent<TileScript>().CharOnTileGetHit(attackPower, false);
            map.ClearOldPath();
            StartCoroutine(SetAttackFalse());
        }

        private IEnumerator PerformAttackMove(Vector2 dir)
        {
            Vector2 startPosition = transform.position;
            Vector2 destinationPosition = startPosition + (dir * 60f);
            float t = 0.0f;
            while (t < 1.1f)
            {
                transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
                t += Time.deltaTime / stepAttackDuration;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.6f);
            animaThor.SetInteger("State", 0);
            t = 0.0f;
            while (t < 1.1f)
            {
                transform.position = Vector2.Lerp(destinationPosition, startPosition, t);
                t += Time.deltaTime / stepAttackDuration;
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator SetAttackFalse()
        {
            yield return new WaitForSeconds(1.0f);
            isPerformingAttack = false;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
            {
                other.GetComponent<TileScript>().walkable = false;
                other.GetComponent<TileScript>().occupant = this.gameObject;
                other.GetComponent<TileScript>().hasPlayer = true;
                tile = other.gameObject;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
            {
                other.GetComponent<TileScript>().walkable = true;
                other.GetComponent<TileScript>().occupant = null;
                other.GetComponent<TileScript>().hasPlayer = false;
            }
        }

        public IEnumerator GetHit(int damageAmount)
        {
            yield return new WaitForSeconds(0.5f);
            int damageTaken = damageAmount - defence;
            damageTaken = Mathf.Clamp(damageTaken, 0, 100000);
            health -= damageTaken;
            healthText.text = health.ToString();
        }

        public void ReceiveActPts()
        {
            currActPts = maxActPts;
        }

        public void GainXP(int gainedXP)
        {
            xp += gainedXP;
        }


        public void BumpMe(int dir)
        {
            if (dir == 1 || dir == 2)
            {
                if (tileY == 0)
                    tileY = 1;
                tileY = (map.gridSizeY) - tileY - 1;
                Vector2 pos = transform.position;
                if (dir == 2)
                    pos = new Vector2(pos.x, tileY * 100 + myOffsetY + stepDoorDistance);
                else
                    pos = new Vector2(pos.x, tileY * 100 + myOffsetY - stepDoorDistance);
                transform.position = pos;

            }
            else if (dir == 3 || dir == 4)
            {
                if (tileX == 0)
                    tileX = 1;
                tileX = (map.gridSizeX) - tileX - 1;
                Vector2 pos = transform.position;
                if (dir == 3)
                    pos = new Vector2(tileX * 100 + stepDoorDistance, pos.y);
                else
                    pos = new Vector2(tileX * 100 - stepDoorDistance, pos.y);
                transform.position = pos;
            }
        }

        private IEnumerator FadeInOutPortal(int a, int b)
        {
            if (b == 1)
                StartCoroutine(MoveToDoor(Vector2.up));
            else if (b == 2)
                StartCoroutine(MoveToDoor(Vector2.down));
            else if (b == 3)
                StartCoroutine(MoveToDoor(Vector2.left));
            else if (b == 4)
                StartCoroutine(MoveToDoor(Vector2.right));

            yield return new WaitForSeconds(1f);
            float fadeTime = GetComponent<Fading>().BeginFade(1);
            yield return new WaitForSeconds(fadeTime);
            map.MakeRoom(0, 0, map.roomNode[a, b]);
            BumpMe(b);
            //yield return new WaitForSeconds(fadeTime);

            GetComponent<Fading>().BeginFade(-1);
            yield return new WaitForSeconds(1f);

            if (b == 1)
                StartCoroutine(MoveToDoor(Vector2.up));
            else if (b == 2)
                StartCoroutine(MoveToDoor(Vector2.down));
            else if (b == 3)
                StartCoroutine(MoveToDoor(Vector2.left));
            else if (b == 4)
                StartCoroutine(MoveToDoor(Vector2.right));
        }

        private IEnumerator FadeIn()
        {
            yield return new WaitForSeconds(1f);
            float fadeTime = GetComponent<Fading>().BeginFade(1);
            yield return new WaitForSeconds(fadeTime);
        }

        private IEnumerator MoveToDoor(Vector2 dir)
        {
            Vector2 startPosition = transform.position;
            Vector2 destinationPosition = startPosition + (dir * stepDoorDistance);
            float t = 0.0f;
            while (t < 1.1f)
            {
                transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
                t += Time.deltaTime / stepDoorDuration;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(3f);
        }

        private IEnumerator MoveToPortal(Vector2 dir)
        {
            Vector2 startPosition = transform.position;
            Vector2 destinationPosition = startPosition + (dir * stepPortalDistance);
            float t = 0.0f;
            while (t < 1.1f)
            {
                transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
                t += Time.deltaTime / stepDoorDuration;
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator EnterPortal(int b)
        {
            if (b == 0)
                StartCoroutine(MoveToPortal(Vector2.up));
            else if (b == 1)
                StartCoroutine(MoveToPortal(Vector2.down));
            else if (b == 2)
                StartCoroutine(MoveToPortal(Vector2.left));
            else if (b == 3)
                StartCoroutine(MoveToPortal(Vector2.right));

            yield return new WaitForSeconds(1f);
            float fadeTime = GetComponent<Fading>().BeginFade(1);
            yield return new WaitForSeconds(fadeTime);
            //Debug.Log("currentLevel: " + map.currentLevel);
            if (map.currentLevel < 3)
            {
                if (PlayerPrefs.GetInt("PD") == 1)
                {
                    map.cycleLevel++;
                }
                else
                {
                    SavePlayer();
                }

                map.GoToLevel(map.currentLevel + 1);
                GetComponent<Fading>().BeginFade(-1);
            }
            else
            {
                SceneManager.LoadScene("YouWin");
                
                //Debug.Log("GG, you win!");
            }
        }

        public void SavePlayer()
        {
            PlayerPrefs.SetInt("pLevel", charLVL);
            PlayerPrefs.SetInt("pCurrXp", xp);
            PlayerPrefs.SetInt("pMaxAP", maxActPts);
            PlayerPrefs.SetInt("pMaxHealth", maxHealth);
            PlayerPrefs.SetInt("pVitality", vitality);
            PlayerPrefs.SetInt("pStrength", strength);
            PlayerPrefs.SetInt("pAgility", agility);
            PlayerPrefs.SetInt("pSpeed", speed);
            PlayerPrefs.SetInt("pDefence", defence);
            PlayerPrefs.SetInt("pSkillPoints", skillPointsRemaining);

            for (int i = 0; i < inventory.slots.Count; i++)
            {
                PlayerPrefs.SetInt("Slot" + i, -1);
                if (inventory.slots[i].transform.childCount > 0)
                {
                    if (i < 40)
                    {
                        //Debug.Log("item " + i + " is " + inventory.items[i].title);
                        //Debug.Log("Slot " + i + " has item: " + inventory.slots[i].transform.GetChild(0).GetComponent<ItemDataScript>().item.title);
                        PlayerPrefs.SetInt("Slot" + i, inventory.slots[i].transform.GetChild(0).GetComponent<ItemDataScript>().item.id);
                    }
                }
                if (i >= 40 && inventory.slots[i].transform.childCount > 1)
                {
                    //Debug.Log("item " + i + " is " + inventory.items[i].title);
                    //Debug.Log("Slot " + i + " has item: " + inventory.slots[i].transform.GetChild(1).GetComponent<ItemDataScript>().item.title);
                    PlayerPrefs.SetInt("Slot" + i, inventory.slots[i].transform.GetChild(1).GetComponent<ItemDataScript>().item.id);
                }
                //Debug.Log("Item id in Slot" + i + ": " + PlayerPrefs.GetInt("Slot" + i));
            }
        }

        public void ResetPlayer()
        {
            for (int i = 0; i < inventory.slots.Count; i++)
            {
                PlayerPrefs.SetInt("Slot" + i, -1);
            }
        }

        public void LoadItemsFromSave()
        {
            for (int i = 0; i < inventory.slots.Count; i++)
            {
                if (PlayerPrefs.GetInt("Slot" + i) != -1)
                {
                    inventory.AddItemFromSave(PlayerPrefs.GetInt("Slot" + i), i);
                }
            }
        }

        public void PermaDeathSpawn()
        {

        }

        public void Heal(int amount)
        {
            health += amount;

            if (health > maxHealth)
            {
                health = maxHealth;
            }
            healthText.text = health.ToString();
        }

        public int GetInitiative()
        {
            return _initiative;
        }


        public bool IsMyTurn()
        {
            return _myTurn;
        }

        public void IsMyTurn(bool isMyTurn)
        {
            _myTurn = isMyTurn;
        }
    }
}