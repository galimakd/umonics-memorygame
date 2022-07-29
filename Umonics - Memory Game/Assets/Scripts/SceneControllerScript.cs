using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControllerScript : MonoBehaviour
{
    //2 rows and 2 columns
    public const int gridRows = 2;
    public const int gridColumns = 2;
    public const float offsetX = 4f;
    public const float offsetY = 5f;

    [SerializeField] private MainCardScript originalCard;
    [SerializeField] private Sprite[] images;
    
    // Get reduce lives
    LivesScript _livesScript;
    [SerializeField] GameObject Lives;

    void Awake() {
        _livesScript = Lives.GetComponent<LivesScript>();
    }


    private void Start() {
        Vector3 startPos = originalCard.transform.position; // position of first card

        int[] numbers = {0,0,1,1};
        numbers = ShuffleArray(numbers);

        for(int i = 0; i < gridColumns; i++) {
            for(int j = 0; j < gridRows; j++) {
                MainCardScript card;
                // if not original card, instantiate a new card
                if(i == 0 && j == 0) {
                    card = originalCard;
                }
                else {
                    card = Instantiate(originalCard) as MainCardScript;
                }

                //Set image and id of cards
                int index = j * gridColumns + i;
                int id = numbers[index];
                card.ChangeSprite(id, images[id]);

                //Set positions of cards
                float posX = (offsetX * i) + startPos.x;
                float posY = (offsetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }
    //Shuffle front face of cards
    private int[] ShuffleArray(int[] numbers) {
        int[] newArray = numbers.Clone() as int[];
        for(int i = 0; i < newArray.Length; i++) {
            int tmp = newArray[i];
            int r = Random.Range(1, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    //Only allow 2 cards revealed at a time

    private MainCardScript _firstRevealed;
    private MainCardScript _secondRevealed;

    private int _score = 0;
    private int _hearts = 3;
    [SerializeField] private TextMesh scoreLabel;

    public bool canReveal {
        get { return _secondRevealed == null;}
    }

    public void CardRevealed(MainCardScript card) {
        if(_firstRevealed == null) {
            _firstRevealed = card;
        }
        else{
            _secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch(){
        if(_firstRevealed.id == _secondRevealed.id) {
            _score++;
            if(_score==2){
                Won();
            }
        }
        else {
            yield return new WaitForSeconds(0.5f);
            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
            _livesScript.ReduceLives();
            _hearts--;
            if(_hearts == 0) {
                GameOver();//Game over on 0 hearts
            }
        }
        //so we can reuse
        _firstRevealed = null;
        _secondRevealed = null;
    }
    
    public TimerScript _timer;

    public void Stop(){
        _timer.Stop();
    }

    //Game over script
    public GameOverScript _gameOver;
    
    public void GameOver() {
        
        _gameOver.RetryBackground();
        Stop();
        Debug.Log(_timer.TimerString);
    }

    public ThreeStarsScript _3stars;
    public TwoStarsScript _2stars;
    public OneStarScript _1star;
    
    private int _timeScore;
    
    public void Won() {
        Stop();
        int.TryParse(_timer.TimerString, out _timeScore);

        if(_timeScore <= 5) {
            _3stars.ThreeStars();
            Debug.Log(_timeScore);
        } else if(_timeScore <=10){
            _2stars.TwoStars();
        } else {
            _1star.OneStar();
        }
        
        
        Debug.Log(_timer.TimerString);
    }
    
    
}
